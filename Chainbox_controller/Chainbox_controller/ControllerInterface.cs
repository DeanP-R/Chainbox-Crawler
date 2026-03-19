using System;
using gclib;

namespace Chainbox_controller
{
    /// <summary>
    /// Wraps the Galil gclib API for the Chainbox crawler.
    ///
    /// Axis mapping (DMC-4103):
    ///   A = Left track
    ///   B = Right track
    ///   C = Probe
    ///
    /// Key Galil commands used (verified from DMC-4103 Command Reference Rev 1840):
    ///   SH  - Servo Here  : enable servo, latch current position as command position
    ///   MO  - Motor Off   : disable motor outputs
    ///   JG  - Jog         : set continuous jog velocity (counts/s, signed)
    ///   BG  - Begin       : start motion on specified axes
    ///   ST  - Stop        : decelerate axes to a stop
    ///   AB 1- Abort motion: instant stop, no deceleration, keeps program running
    ///   AC  - Acceleration: counts/s^2, rounds to nearest 1024 at TM=1000
    ///   DC  - Deceleration: counts/s^2, rounds to nearest 1024 at TM=1000
    ///   TC  - Tell Code   : last error code from controller
    ///   TV  - Tell Velocity: actual encoder velocity (counts/s)
    ///   TS  - Tell Switches: axis status byte (bit7=in motion, bit5=motor off)
    ///   MG _TVxx - Message operand: single-axis velocity query
    ///   MG _TSxx - Message operand: single-axis status query
    /// </summary>
    public class ControllerInterface
    {
        // ── gclib handle ──────────────────────────────────────────────────────
        private gclib.gclib? _galil;

        // ── State ─────────────────────────────────────────────────────────────
        public bool IsConnected { get; private set; }
        public bool MotorsEnabled { get; private set; }
        public bool SimulationMode { get; set; }

        // ── Logging ───────────────────────────────────────────────────────────
        /// <summary>Fired on every Galil command sent or response received.</summary>
        public event Action<string>? OnLog;

        /// <summary>
        /// Snapshot of the last log message (polled by the UI timer as a
        /// fallback when event subscription is not available).
        /// </summary>
        public string? LastLog { get; private set; }

        // ── Velocity tracking (used to suppress redundant JG commands) ────────
        private double _lastLeftSteps;
        private double _lastRightSteps;
        private double _lastProbeSteps;

        // ─────────────────────────────────────────────────────────────────────
        // Connection
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Opens a TCP connection to the DMC-4103 at the given IP address.
        ///
        /// gclib GOpen string format: "--address {ip} --port 23 --timeout {ms}"
        /// Port 23 is the Galil default for TCP command connections.
        /// </summary>
        public void Connect(string ipAddress)
        {
            Disconnect();

            try
            {
                _galil = new gclib.gclib();

                // Try simplest, most reliable form first
                _galil.GOpen(ipAddress);

                // Verify connection with a real command
                string response = _galil.GCommand("MG TIME");

                if (string.IsNullOrWhiteSpace(response))
                    throw new Exception("No response from controller");

                IsConnected = true;
                LogMessage($"Connected to {ipAddress}");

                // Optional: get ID
                try
                {
                    var id = _galil.GCommand("ID");
                    LogMessage($"Controller ID: {id.Trim()}");
                }
                catch { }
            }
            catch (Exception ex)
            {
                IsConnected = false;
                LogMessage($"Connection FAILED: {ex.Message}");
                throw; // important so UI catch triggers
            }
        }

        public void Disconnect()
        {
            if (_galil != null)
            {
                try
                {
                    // Best-effort safe stop before closing
                    if (IsConnected) GCommandNoReply("ST");
                }
                catch { }

                try { _galil.GClose(); } catch { }
                _galil = null;
            }

            IsConnected = false;
            MotorsEnabled = false;
            LogMessage("Disconnected");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Motor enable / disable
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// SH AABBCC — "Servo Here" on axes A, B, C.
        ///
        /// The controller latches the current motor position as the commanded
        /// position and enables closed-loop servo (or step-and-direction for
        /// stepper axes). After SH the axes accept JG/BG commands.
        ///
        /// Galil axis mask syntax: each axis letter is doubled.
        ///   SH AABBCC  → enable A, B and C
        ///   SH AA      → enable A only, leave B and C unchanged
        /// </summary>
        public void EnableMotors()
        {
            if (!IsConnected && !SimulationMode) return;

            GCommandNoReply("SH AABBCC");
            MotorsEnabled = true;
            LogMessage("Motors enabled (SH AABBCC)");
        }

        /// <summary>
        /// MO — Motor Off on all axes.
        ///
        /// Disables the motor command output and toggles the amplifier enable
        /// signal. The controller continues to monitor encoder position (TP).
        /// Use SH to re-enable.
        /// </summary>
        public void DisableMotors()
        {
            if (!IsConnected && !SimulationMode) return;

            GCommandNoReply("MO");
            MotorsEnabled = false;
            LogMessage("Motors disabled (MO)");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Motion settings
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sends AC and DC to all three axes.
        ///
        /// Galil implicit comma notation — values for A, B, C in order:
        ///   AC 10000,10000,10000
        ///   DC 10000,10000,10000
        ///
        /// Values are rounded DOWN to the nearest multiple of 1024 (the
        /// hardware resolution at TM=1000). Minimum is 1024.
        /// </summary>
        public void ApplySettings(ControllerSettings s)
        {
            if (!IsConnected && !SimulationMode) return;

            int accel = RoundToGalilResolution(s.AccelStepsPerSec2);
            int decel = RoundToGalilResolution(s.DecelStepsPerSec2);

            GCommandNoReply($"AC {accel},{accel},{accel}");
            GCommandNoReply($"DC {decel},{decel},{decel}");

            LogMessage($"Settings applied — AC={accel} DC={decel} (counts/s²)");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Continuous jogging
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sets jog velocity on A (left), B (right), C (probe) axes.
        ///
        /// Strategy:
        ///  - Only resends JG+BG for axes whose velocity has actually changed,
        ///    to avoid flooding the controller.
        ///  - For axes going to zero, issues ST {mask} for a decelerated stop.
        ///  - For axes getting a non-zero velocity, issues JG then BG.
        ///
        /// JG command reference:
        ///   JG n,n,n  — implicit notation, A/B/C in order
        ///   Sign encodes direction: positive = forward (A/B) or right (C)
        ///   Minimum non-zero: 2 counts/s.  Maximum: 3,000,000 counts/s (stepper)
        ///   JG does NOT start motion — you must issue BG afterward.
        ///
        /// BG command reference:
        ///   BG AABBCC — begin motion on A, B, C simultaneously
        ///   Axis mask letters are doubled per Galil convention.
        ///
        /// ST command reference:
        ///   ST AABB — decelerated stop on A and B
        ///   Uses the DC deceleration rate set by the DC command.
        /// </summary>
        public void JogVelocity(double leftSteps, double rightSteps, double probeSteps)
        {
            if (SimulationMode)
            {
                LogMessage($"SIM: JG {(int)leftSteps},{(int)rightSteps},{(int)probeSteps}");
                _lastLeftSteps = leftSteps;
                _lastRightSteps = rightSteps;
                _lastProbeSteps = probeSteps;
                return;
            }

            if (!IsConnected || !MotorsEnabled) return;

            int l = (int)leftSteps;
            int r = (int)rightSteps;
            int p = (int)probeSteps;

            bool leftChanged = l != (int)_lastLeftSteps;
            bool rightChanged = r != (int)_lastRightSteps;
            bool probeChanged = p != (int)_lastProbeSteps;

            if (!leftChanged && !rightChanged && !probeChanged) return;

            // Axes going to zero → decelerated ST stop
            string stopMask = BuildAxisMask(
                l == 0 && leftChanged,
                r == 0 && rightChanged,
                p == 0 && probeChanged);

            if (stopMask.Length > 0)
                GCommandNoReply($"ST {stopMask}");

            // Axes getting a new non-zero speed → JG then BG
            bool setL = leftChanged && l != 0;
            bool setR = rightChanged && r != 0;
            bool setP = probeChanged && p != 0;

            if (setL || setR || setP)
            {
                string jgCmd = BuildImplicitJgCommand(l, r, p, setL, setR, setP);
                string bgMask = BuildAxisMask(setL, setR, setP);

                GCommandNoReply(jgCmd);
                GCommandNoReply($"BG {bgMask}");
            }

            _lastLeftSteps = leftSteps;
            _lastRightSteps = rightSteps;
            _lastProbeSteps = probeSteps;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Stop / Abort
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// ST — decelerated stop on all axes (preferred for normal E-stop).
        /// Uses the DC deceleration rate. Safe for mechanical systems.
        /// </summary>
        public void StopAll()
        {
            if (SimulationMode) { LogMessage("SIM: ST"); return; }
            if (!IsConnected) return;

            GCommandNoReply("ST");
            _lastLeftSteps = _lastRightSteps = _lastProbeSteps = 0;
            LogMessage("ST — all axes stopping");
        }

        /// <summary>
        /// AB 1 — instant abort of motion, keeps embedded program running.
        /// No deceleration ramp. Use only when a decelerated stop is too slow.
        /// </summary>
        public void AbortMotion()
        {
            if (SimulationMode) { LogMessage("SIM: AB 1"); return; }
            if (!IsConnected) return;

            GCommandNoReply("AB 1");
            _lastLeftSteps = _lastRightSteps = _lastProbeSteps = 0;
            LogMessage("AB 1 — motion aborted");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Raw command console
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sends an arbitrary Galil command and returns the response string.
        /// Used by the Galil Command Console panel in the UI.
        /// </summary>
        public string SendRawCommand(string command)
        {
            if (SimulationMode)
            {
                string simResp = $"[SIM] {command}";
                LogMessage(simResp);
                return simResp;
            }

            if (!IsConnected) throw new InvalidOperationException("Not connected");
            return GCommand(command);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Telemetry queries
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Queries actual velocity on A, B, C using MG _TVxx operand syntax.
        ///
        /// TV operand note: TV uses a ~0.25 s averaging filter at TM=1000,
        /// so it lags instantaneous velocity slightly. Adequate for telemetry.
        ///
        /// Falls back to last commanded steps in simulation mode.
        /// </summary>
        public (double left, double right, double probe) QueryVelocity()
        {
            if (!IsConnected || SimulationMode)
                return (_lastLeftSteps, _lastRightSteps, _lastProbeSteps);

            try
            {
                double left = ParseDouble(GCommand("MG _TVAA"));
                double right = ParseDouble(GCommand("MG _TVBB"));
                double probe = ParseDouble(GCommand("MG _TVCC"));
                return (left, right, probe);
            }
            catch
            {
                return (0, 0, 0);
            }
        }

        /// <summary>
        /// Queries TS status byte for a single axis using MG _TSxx operand syntax.
        ///
        /// TS bit meanings:
        ///   Bit 7 — axis in motion
        ///   Bit 6 — position error exceeds limit
        ///   Bit 5 — motor off
        ///   Bit 3 — forward limit switch inactive (1 = OK)
        ///   Bit 2 — reverse limit switch inactive (1 = OK)
        /// </summary>
        public int QueryAxisStatus(char axis)
        {
            if (!IsConnected || SimulationMode) return 0;
            try
            {
                string resp = GCommand($"MG _TS{axis}{axis}");
                return (int)ParseDouble(resp);
            }
            catch { return 0; }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Sends command, logs it, returns response string.</summary>
        private string GCommand(string cmd)
        {
            LogMessage($"> {cmd}");
            string resp = _galil!.GCommand(cmd);
            if (!string.IsNullOrWhiteSpace(resp))
                LogMessage($"< {resp.Trim()}");
            return resp;
        }

        /// <summary>
        /// Sends command, discards response.
        /// Motion commands typically reply with ':' (the Galil prompt).
        /// </summary>
        private void GCommandNoReply(string cmd)
        {
            LogMessage($"> {cmd}");
            try { _galil!.GCommand(cmd); } catch { /* ':' prompt is not an error */ }
        }

        /// <summary>Fires OnLog event and updates LastLog.</summary>
        public void LogMessage(string msg)
        {
            LastLog = msg;
            OnLog?.Invoke(msg);
        }

        /// <summary>
        /// Rounds a value DOWN to the nearest multiple of 1024 (Galil AC/DC
        /// resolution at TM=1000). Enforces a minimum of 1024.
        /// </summary>
        private static int RoundToGalilResolution(double value, int resolution = 1024)
        {
            int v = (int)Math.Abs(value);
            return Math.Max(resolution, (v / resolution) * resolution);
        }

        /// <summary>
        /// Builds a Galil axis mask string.
        /// Galil convention doubles each axis letter: A→"AA", B→"BB", C→"CC".
        /// Example: BuildAxisMask(true, false, true) → "AACC"
        /// </summary>
        private static string BuildAxisMask(bool includeA, bool includeB, bool includeC)
        {
            var sb = new System.Text.StringBuilder();
            if (includeA) sb.Append("AA");
            if (includeB) sb.Append("BB");
            if (includeC) sb.Append("CC");
            return sb.ToString();
        }

        /// <summary>
        /// Builds a "JG a,b,c" command using Galil implicit comma notation.
        /// Axes we are not changing get an empty slot (position preserved).
        ///
        /// Examples:
        ///   setL=true, setR=true, setP=false → "JG 500,500"
        ///   setL=false, setR=false, setP=true → "JG ,,-100"  (two leading commas)
        ///   setL=true, setR=false, setP=true  → "JG 500,,-100"
        /// </summary>
        private static string BuildImplicitJgCommand(
            int l, int r, int p,
            bool setL, bool setR, bool setP)
        {
            string sa = setL ? l.ToString() : "";
            string sb = setR ? r.ToString() : "";
            string sc = setP ? p.ToString() : "";

            // Trim trailing empty fields for brevity
            if (!setP && !setR) return $"JG {sa}";
            if (!setP) return $"JG {sa},{sb}";
            return $"JG {sa},{sb},{sc}";
        }

        private static double ParseDouble(string s)
        {
            if (double.TryParse(s.Trim(),
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double v))
                return v;
            return 0;
        }
    }
}