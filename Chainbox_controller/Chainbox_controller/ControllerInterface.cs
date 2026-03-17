using System;
using System.Text;

namespace Chainbox_controller
{
    public class ControllerSettings
    {
        public double MaxVelocityStepsPerSec { get; set; } = 10000; // steps/sec
        public double AccelStepsPerSec2 { get; set; } = 1000; // steps/sec^2
        public double DecelStepsPerSec2 { get; set; } = 1000; // steps/sec^2
        public double StepsPerMm { get; set; } = 100; // conversion
    }
    public class ControllerInterface : IDisposable
    {
        private gclib.gclib g = new gclib.gclib();
        private bool connected = false;
        private bool motorsEnabled = false;

        public bool IsConnected => connected;
        public bool MotorsEnabled => motorsEnabled;

        public string LastLog { get; private set; } = "";

        // Event for log messages (command/response/errors)
        public event Action<string>? OnLog;
        public bool SimulationMode { get; set; } = false;

        // Track last logged velocities to avoid flooding log with identical jog commands
        private double lastLoggedLeft = double.NaN;
        private double lastLoggedRight = double.NaN;
        private double lastLoggedProbe = double.NaN;
        private const double JogLogTolerance = 1.0; // steps/sec

        public ControllerInterface()
        {
        }

        // Public logging helper so UI/simulation can append messages without calling private method.
        public void LogMessage(string s)
        {
            LogInternal(s);
        }

        public void Connect(string ip)
        {
            if (SimulationMode)
            {
                connected = true;
                LogInternal("Simulation: Connected to " + ip);
                return;
            }

            g.GOpen(ip);
            connected = true;
            var info = g.GInfo();
            LogInternal("Connected to " + ip + ": " + info);
        }

        public void Disconnect()
        {
            if (connected)
            {
                StopAll();
                if (!SimulationMode)
                {
                    g.GClose();
                }
                connected = false;
                LogInternal(SimulationMode ? "Simulation: Disconnected" : "Disconnected");
            }
        }

        public void ApplySettings(ControllerSettings s)
        {
            if (!connected)
            {
                LogInternal("Cannot apply settings: controller not connected");
                return;
            }

            // Use axis letters A (left), B (right), C (probe) for Galil commands.
            // Set acceleration (AC) per axis
            string cmd = string.Format("AC A{0};AC B{0};AC C{0}", (int)s.AccelStepsPerSec2);
            var resp = SimulationMode ? "(sim)" : g.GCommand(cmd);
            LogInternal(cmd + " -> " + resp);

            // Deceleration
            cmd = string.Format("DC A{0};DC B{0};DC C{0}", (int)s.DecelStepsPerSec2);
            resp = SimulationMode ? "(sim)" : g.GCommand(cmd);
            LogInternal(cmd + " -> " + resp);

            // Set maximum velocity (SP) per axis
            cmd = string.Format("SP A{0};SP B{0};SP C{0}", (int)s.MaxVelocityStepsPerSec);
            resp = SimulationMode ? "(sim)" : g.GCommand(cmd);
            LogInternal(cmd + " -> " + resp);
        }

        public void EnableMotors()
        {
            if (!connected)
            {
                LogInternal("Cannot enable motors: controller not connected");
                return;
            }

            // Energize motors on axes A/B/C
            string cmd = "MO A1;MO B1;MO C1";
            var resp = SimulationMode ? "(sim)" : g.GCommand(cmd);
            motorsEnabled = true;
            LogInternal(cmd + " -> " + resp);
        }

        public void DisableMotors()
        {
            if (!connected)
            {
                LogInternal("Cannot disable motors: controller not connected");
                return;
            }
            string cmd = "MO A0;MO B0;MO C0";
            var resp = SimulationMode ? "(sim)" : g.GCommand(cmd);
            motorsEnabled = false;
            LogInternal(cmd + " -> " + resp);
        }

        public void JogVelocity(double leftStepsPerSec, double rightStepsPerSec, double probeStepsPerSec)
        {
            if (!connected)
            {
                LogInternal("Jog ignored: controller not connected");
                return;
            }

            if (!motorsEnabled)
            {
                LogInternal("Jog ignored: motors not enabled");
                return;
            }

            // Set axis velocities using V command per axis and then issue BG to start.
            // Using Galil syntax: 'V' sets velocity for an axis: e.g., 'VA1200' sets axis A velocity to 1200
            // We'll use A for left, B for right, C for probe.
            string cmd = string.Format("VA{0};VB{1};VC{2}", (int)leftStepsPerSec, (int)rightStepsPerSec, (int)probeStepsPerSec);
            var resp = SimulationMode ? "(sim)" : g.GCommand(cmd);

            // Only log jog commands when velocity changed beyond tolerance to avoid flooding the UI
            bool shouldLog = double.IsNaN(lastLoggedLeft) || Math.Abs(leftStepsPerSec - lastLoggedLeft) >= JogLogTolerance
                             || double.IsNaN(lastLoggedRight) || Math.Abs(rightStepsPerSec - lastLoggedRight) >= JogLogTolerance
                             || double.IsNaN(lastLoggedProbe) || Math.Abs(probeStepsPerSec - lastLoggedProbe) >= JogLogTolerance;

            if (shouldLog)
            {
                LogInternal(cmd + " -> " + resp);
                lastLoggedLeft = leftStepsPerSec;
                lastLoggedRight = rightStepsPerSec;
                lastLoggedProbe = probeStepsPerSec;
            }

            resp = SimulationMode ? "(sim)" : g.GCommand("BG"); // begin motion
            if (shouldLog) LogInternal("BG -> " + resp);
        }

        public void StopAll()
        {
            if (!connected)
            {
                LogInternal("Stop ignored: controller not connected");
                return;
            }

            var resp = SimulationMode ? "(sim)" : g.GCommand("ST");
            LogInternal("ST -> " + resp);
        }

        // Allow advanced users to send raw Galil commands and return raw response.
        public string SendRawCommand(string cmd)
        {
            if (!connected)
            {
                var msg = "Send ignored: controller not connected";
                LogInternal(msg);
                return "";
            }

            var resp = SimulationMode ? "(sim)" : g.GCommand(cmd);
            LogInternal(cmd + " -> " + resp);
            return resp ?? string.Empty;
        }

        private readonly object logLock = new object();
        private const long MaxLogFileBytes = 256 * 1024; // 256 KB
        private readonly string logFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chainbox_log.txt");

        private void LogInternal(string s)
        {
            LastLog = s;
            OnLog?.Invoke(s);

            // Append to file with size limit
            try
            {
                lock (logLock)
                {
                    System.IO.File.AppendAllText(logFilePath, $"{DateTime.Now:HH:mm:ss} {s}{Environment.NewLine}");
                    var fi = new System.IO.FileInfo(logFilePath);
                    if (fi.Length > MaxLogFileBytes)
                    {
                        // Trim oldest lines - simple approach: keep last 2000 lines
                        var lines = System.IO.File.ReadAllLines(logFilePath);
                        int keep = Math.Min(2000, lines.Length);
                        var last = new string[keep];
                        System.Array.Copy(lines, lines.Length - keep, last, 0, keep);
                        System.IO.File.WriteAllLines(logFilePath, last);
                    }
                }
            }
            catch
            {
                // ignore logging failures
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
