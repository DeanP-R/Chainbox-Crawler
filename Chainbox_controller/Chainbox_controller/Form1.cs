using System;
using System.Windows.Forms;
using gclib;

namespace Chainbox_controller
{
    public partial class Form1 : Form
    {
        private InputLayer.InputMode currentInputMode = InputLayer.InputMode.Automatic;
        private DateTime lastUiUpdate = DateTime.MinValue;
        private bool logAutoScroll = true;
        private const int MaxLogLines = 2000;
        private InputLayer inputLayer;
        private DriveMixer mixer;
        private ControllerInterface controller;
        private ControllerSettings settings;

        private System.Windows.Forms.Timer controlTimer;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.TabStop = false;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            // Disable keyboard focus on drive buttons
            btnForward.TabStop = false;
            btnReverse.TabStop = false;
            btnLeft.TabStop = false;
            btnRight.TabStop = false;
            btnStop.TabStop = false;

            // Disable focus on probe buttons
            btnProbeLeft.TabStop = false;
            btnProbeRight.TabStop = false;
            btnProbeStop.TabStop = false;



            inputLayer = new InputLayer();
            mixer = new DriveMixer();
            controller = new ControllerInterface();
            settings = new ControllerSettings();

            // Controls created in Designer; wire runtime events
            WireUpEvents();

            // Setup timer ~50Hz
            controlTimer = new System.Windows.Forms.Timer();
            controlTimer.Interval = 20;
            controlTimer.Tick += ControlTimer_Tick;
            controlTimer.Start();

            AppendLog("Application started");
            controller.OnLog += Controller_OnLog;
            // log auto-scroll control
            this.btnResumeAutoScroll.Click += (s, e) => { logAutoScroll = true; };
            this.txtLog.MouseWheel += (s, e) => { logAutoScroll = false; };
            // pause scrolling checkbox (designer created)
            if (this.chkPauseScrolling != null) this.chkPauseScrolling.CheckedChanged += (s, e) => { logAutoScroll = !this.chkPauseScrolling.Checked; };
            // input mode selector
            if (this.cmbInputMode != null) this.cmbInputMode.SelectedIndexChanged += (s, e) =>
            {
                if (cmbInputMode.SelectedIndex == 0) currentInputMode = InputLayer.InputMode.Automatic;
                else if (cmbInputMode.SelectedIndex == 1) currentInputMode = InputLayer.InputMode.Keyboard;
                else currentInputMode = InputLayer.InputMode.Gamepad;
                // reflect in header label if present
                if (this.lblInputMode != null) this.lblInputMode.Text = "Input Mode: " + currentInputMode.ToString().ToUpper();
                // move focus away from combo so subsequent key presses don't re-select items in the combo
                try { this.ActiveControl = null; this.Focus(); } catch { }
            };
            // wire galil console send
            this.btnSendGalil.Click += (s, e) => SendGalilCommand();
            this.txtGalilCmd.KeyDown += (s, e) => { if (e.KeyCode == System.Windows.Forms.Keys.Enter) { e.SuppressKeyPress = true; SendGalilCommand(); } };
            // reflect simulation checkbox
            this.chkSimulation.CheckedChanged += (s, e) => controller.SimulationMode = this.chkSimulation.Checked;
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentInputMode != InputLayer.InputMode.Keyboard)
                return;

            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Up:
                    inputLayer.SetManualOverride(new InputState() { Forward = 1.0 });
                    break;

                case System.Windows.Forms.Keys.Down:
                    inputLayer.SetManualOverride(new InputState() { Forward = -1.0 });
                    break;

                case System.Windows.Forms.Keys.Left:
                    inputLayer.SetManualOverride(new InputState() { Turn = -1.0 });
                    break;

                case System.Windows.Forms.Keys.Right:
                    inputLayer.SetManualOverride(new InputState() { Turn = 1.0 });
                    break;

                case System.Windows.Forms.Keys.Space:
                    controller.StopAll();
                    break;
            }

            e.Handled = true;
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (currentInputMode != InputLayer.InputMode.Keyboard)
                return;

            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Up:
                case System.Windows.Forms.Keys.Down:
                case System.Windows.Forms.Keys.Left:
                case System.Windows.Forms.Keys.Right:
                    inputLayer.ClearManualOverride();
                    break;
            }
        }

        private void SendGalilCommand()
        {
            if (!controller.IsConnected)
            {
                AppendLog("Cannot send: controller not connected");
                return;
            }

            var cmd = this.txtGalilCmd.Text?.Trim();
            if (string.IsNullOrEmpty(cmd)) return;
            var resp = controller.SendRawCommand(cmd);
            this.lstGalilHistory.Items.Add($"> {cmd}");
            if (!string.IsNullOrEmpty(resp)) this.lstGalilHistory.Items.Add(resp);
            if (this.lstGalilHistory.Items.Count > 2000) this.lstGalilHistory.Items.RemoveAt(0);
            AppendLog($"GALIL: {cmd} -> {resp}");
            this.txtGalilCmd.Clear();
            // keep auto-scroll
            this.lstGalilHistory.TopIndex = Math.Max(0, this.lstGalilHistory.Items.Count - 1);
        }

        private void Controller_OnLog(string obj)
        {
            // show commands immediately
            // add to command stream listbox (limit size)
            if (this.lstCommandStream != null)
            {
                this.lstCommandStream.Items.Add(obj);
                if (this.lstCommandStream.Items.Count > 2000) this.lstCommandStream.Items.RemoveAt(0);
                // auto-scroll unless paused
                if (logAutoScroll && (chkPauseScrolling == null || !chkPauseScrolling.Checked))
                    this.lstCommandStream.TopIndex = Math.Max(0, this.lstCommandStream.Items.Count - 1);
            }

            // also append to diagnostics textbox (limited, auto-scroll behavior handled separately)
            AppendLog(obj);
        }

        private void WireUpEvents()
        {
            // Hook up button events created in designer
            this.btnConnect.Click += BtnConnect_Click;
            this.btnDisconnect.Click += BtnDisconnect_Click;
            this.btnEnableMotors.Click += BtnEnableMotors_Click;
            this.btnDisableMotors.Click += BtnDisableMotors_Click;
            this.btnApplySettings.Click += BtnApplySettings_Click;
            // when any numeric changes, mark settings pending
            this.numMaxSpeed.ValueChanged += (s, e) => MarkSettingsPending();
            this.numAccel.ValueChanged += (s, e) => MarkSettingsPending();
            this.numDecel.ValueChanged += (s, e) => MarkSettingsPending();
            this.numStepsPerMm.ValueChanged += (s, e) => MarkSettingsPending();
            this.btnForward.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Forward = 1.0 });
            this.btnForward.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            this.btnReverse.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Forward = -1.0 });
            this.btnReverse.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            this.btnLeft.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Turn = -1.0 });
            this.btnLeft.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            this.btnRight.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Turn = 1.0 });
            this.btnRight.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            this.btnStop.Click += BtnEmergencyStop_Click;
            this.btnProbeLeft.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Probe = -1.0 });
            this.btnProbeLeft.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            this.btnProbeRight.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Probe = 1.0 });
            this.btnProbeRight.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            this.btnProbeStop.Click += BtnProbeStop_Click;
            this.btnExportLog.Click += BtnExportLog_Click;
            this.btnCopyLog.Click += BtnCopyLog_Click;
            this.btnClearLog.Click += (s, e) => { this.txtLog.Clear(); };
        }

        private void BuildUi()
        {
            Text = "Chainbox Controller";
            Width = 1000;
            Height = 700;

            // Connection group
            gbConnection = new GroupBox() { Text = "Controller Connection", Left = 10, Top = 10, Width = 480, Height = 100 };
            txtIp = new TextBox() { Left = 10, Top = 24, Width = 180, Text = "192.168.0.42" };
            btnConnect = new Button() { Left = 200, Top = 22, Width = 80, Text = "Connect" };
            btnDisconnect = new Button() { Left = 290, Top = 22, Width = 80, Text = "Disconnect" };
            lblControllerStatus = new Label() { Left = 10, Top = 50, Width = 300, Text = "Controller: DISCONNECTED" };
            lblMotorsStatus = new Label() { Left = 320, Top = 50, Width = 140, Text = "Motors: DISABLED" };
            btnConnect.Click += BtnConnect_Click;
            btnDisconnect.Click += BtnDisconnect_Click;
            gbConnection.Controls.AddRange(new Control[] { txtIp, btnConnect, btnDisconnect, lblControllerStatus, lblMotorsStatus });
            Controls.Add(gbConnection);

            // Settings group
            gbSettings = new GroupBox() { Text = "Drive Settings", Left = 10, Top = 120, Width = 480, Height = 160 };
            numMaxSpeed = new NumericUpDown() { Left = 10, Top = 24, Width = 120, Maximum = 100000, Value = 10000 };
            numAccel = new NumericUpDown() { Left = 10, Top = 54, Width = 120, Maximum = 100000, Value = 1000 };
            numDecel = new NumericUpDown() { Left = 150, Top = 54, Width = 120, Maximum = 100000, Value = 1000 };
            numStepsPerMm = new NumericUpDown() { Left = 10, Top = 84, Width = 120, Maximum = 10000, Value = 100 };
            btnApplySettings = new Button() { Left = 10, Top = 114, Width = 120, Text = "Apply Settings" };
            btnApplySettings.Click += BtnApplySettings_Click;
            gbSettings.Controls.AddRange(new Control[] { new Label() { Left = 140, Top = 24, Text = "Max Speed" }, numMaxSpeed, new Label() { Left = 140, Top = 54, Text = "Accel / Decel" }, numAccel, numDecel, new Label() { Left = 140, Top = 84, Text = "Steps / mm" }, numStepsPerMm, btnApplySettings });
            Controls.Add(gbSettings);

            // Telemetry
            gbTelemetry = new GroupBox() { Text = "Live Motion Status", Left = 500, Top = 10, Width = 470, Height = 200 };
            lblForwardInput = new Label() { Left = 10, Top = 24, Width = 220, Text = "Forward Input: 0.00" };
            lblTurnInput = new Label() { Left = 10, Top = 48, Width = 220, Text = "Turn Input: 0.00" };
            lblProbeInput = new Label() { Left = 10, Top = 72, Width = 220, Text = "Probe Input: 0.00" };
            lblLeftVel = new Label() { Left = 10, Top = 100, Width = 300, Text = "Left Track Velocity: 0" };
            lblRightVel = new Label() { Left = 10, Top = 124, Width = 300, Text = "Right Track Velocity: 0" };
            gbTelemetry.Controls.AddRange(new Control[] { lblForwardInput, lblTurnInput, lblProbeInput, lblLeftVel, lblRightVel });
            Controls.Add(gbTelemetry);

            // Drive controls
            gbDrive = new GroupBox() { Text = "Manual Drive Controls", Left = 10, Top = 290, Width = 480, Height = 240 };
            btnForward = new Button() { Left = 180, Top = 20, Width = 120, Height = 60, Text = "FORWARD" };
            btnLeft = new Button() { Left = 20, Top = 90, Width = 120, Height = 60, Text = "TURN LEFT" };
            btnRight = new Button() { Left = 340, Top = 90, Width = 120, Height = 60, Text = "TURN RIGHT" };
            btnReverse = new Button() { Left = 180, Top = 160, Width = 120, Height = 60, Text = "REVERSE" };
            btnStop = new Button() { Left = 140, Top = 90, Width = 200, Height = 60, Text = "STOP", BackColor = System.Drawing.Color.Red, ForeColor = System.Drawing.Color.White };
            btnForward.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Forward = 1.0 });
            btnForward.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            btnReverse.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Forward = -1.0 });
            btnReverse.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            btnLeft.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Turn = -1.0 });
            btnLeft.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            btnRight.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Turn = 1.0 });
            btnRight.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            btnStop.Click += (s, e) => { inputLayer.ClearManualOverride(); controller.StopAll(); AppendLog("Emergency STOP"); };
            gbDrive.Controls.AddRange(new Control[] { btnForward, btnLeft, btnRight, btnReverse, btnStop });
            Controls.Add(gbDrive);

            // Probe controls
            gbProbe = new GroupBox() { Text = "Probe Control", Left = 500, Top = 220, Width = 470, Height = 160 };
            numProbeSpeed = new NumericUpDown() { Left = 10, Top = 24, Width = 120, Maximum = 100000, Value = 2000 };
            btnProbeLeft = new Button() { Left = 10, Top = 54, Width = 120, Text = "Move Left" };
            btnProbeRight = new Button() { Left = 140, Top = 54, Width = 120, Text = "Move Right" };
            btnProbeStop = new Button() { Left = 270, Top = 54, Width = 120, Text = "Stop Probe" };
            btnProbeLeft.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Probe = -1.0 });
            btnProbeLeft.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            btnProbeRight.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Probe = 1.0 });
            btnProbeRight.MouseUp += (s, e) => inputLayer.ClearManualOverride();
            btnProbeStop.Click += (s, e) => { inputLayer.ClearManualOverride(); controller.JogVelocity(0, 0, 0); };
            gbProbe.Controls.AddRange(new Control[] { new Label() { Left = 140, Top = 24, Text = "Probe Speed" }, numProbeSpeed, btnProbeLeft, btnProbeRight, btnProbeStop });
            Controls.Add(gbProbe);

            // Log box
            txtLog = new TextBox() { Left = 10, Top = 540, Width = 960, Height = 110, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical };
            Controls.Add(txtLog);
        }

        private void BtnConnect_Click(object? sender, EventArgs e)
        {
            try
            {
                controller.Connect(txtIp.Text);
                lblControllerStatus.Text = "Controller: CONNECTED";
                lblControllerStatus.ForeColor = System.Drawing.Color.Green;
                lblMotorsStatus.Text = controller.MotorsEnabled ? "Motors: ENABLED" : "Motors: DISABLED";
                AppendLog("Connected to controller");
            }
            catch (Exception ex)
            {
                AppendLog("Connect failed: " + ex.Message);
            }
        }

        private void BtnDisconnect_Click(object? sender, EventArgs e)
        {
            controller.Disconnect();
            lblControllerStatus.Text = "Controller: DISCONNECTED";
            lblControllerStatus.ForeColor = System.Drawing.Color.Red;
            lblMotorsStatus.Text = "Motors: DISABLED";
            AppendLog("Disconnected");
        }

        private void BtnEnableMotors_Click(object? sender, EventArgs e)
        {
            controller.EnableMotors();
            lblMotorsStatus.Text = controller.MotorsEnabled ? "Motors: ENABLED" : "Motors: DISABLED";
            if (controller.MotorsEnabled) AppendLog("Motors enabled");
        }

        private void BtnDisableMotors_Click(object? sender, EventArgs e)
        {
            controller.DisableMotors();
            lblMotorsStatus.Text = controller.MotorsEnabled ? "Motors: ENABLED" : "Motors: DISABLED";
            AppendLog("Motors disabled");
        }

        private void BtnApplySettings_Click(object? sender, EventArgs e)
        {
            settings.MaxVelocityStepsPerSec = (double)numMaxSpeed.Value;
            settings.AccelStepsPerSec2 = (double)numAccel.Value;
            settings.DecelStepsPerSec2 = (double)numDecel.Value;
            settings.StepsPerMm = (double)numStepsPerMm.Value;
            controller.ApplySettings(settings);
            AppendLog("Drive settings updated");
            if (lblSettingsStatus != null) lblSettingsStatus.Text = "Settings: APPLIED";
        }

        private void MarkSettingsPending()
        {
            if (lblSettingsStatus != null) lblSettingsStatus.Text = "Settings: PENDING";
        }

        private void BtnEmergencyStop_Click(object? sender, EventArgs e)
        {
            inputLayer.ClearManualOverride();
            controller.StopAll();
            AppendLog("Emergency STOP");
        }

        private void BtnProbeStop_Click(object? sender, EventArgs e)
        {
            inputLayer.ClearManualOverride();
            controller.JogVelocity(0, 0, 0);
            AppendLog("Probe stop issued");
        }

        private void BtnExportLog_Click(object? sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog() { Filter = "Text files|*.txt", FileName = "chainbox_log.txt" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(dlg.FileName, txtLog.Text);
                    AppendLog("Log exported to " + dlg.FileName);
                }
            }
        }

        private void BtnCopyLog_Click(object? sender, EventArgs e)
        {
            Clipboard.SetText(txtLog.Text);
            AppendLog("Log copied to clipboard");
        }

        private void ControlTimer_Tick(object? sender, EventArgs e)
        {
            var tickStart = DateTime.UtcNow;
            // refresh currentInputMode from UI control if present (keeps behavior in sync with designer combo)
            try
            {
                if (this.cmbInputMode != null)
                {
                    if (cmbInputMode.SelectedIndex == 0) currentInputMode = InputLayer.InputMode.Automatic;
                    else if (cmbInputMode.SelectedIndex == 1) currentInputMode = InputLayer.InputMode.Keyboard;
                    else currentInputMode = InputLayer.InputMode.Gamepad;
                    if (this.lblInputMode != null) this.lblInputMode.Text = "Input Mode: " + currentInputMode.ToString().ToUpper();
                }
            }
            catch { }

            var state = inputLayer.Update(currentInputMode);
            var outp = mixer.Mix(state.Forward, state.Turn);

            // scale to steps/sec
            double leftSteps = outp.Left * settings.MaxVelocityStepsPerSec;
            double rightSteps = outp.Right * settings.MaxVelocityStepsPerSec;
            double probeSteps = state.Probe * (double)numProbeSpeed.Value;

            // send or simulate
            if (!controller.SimulationMode)
                controller.JogVelocity(leftSteps, rightSteps, probeSteps);
            else
                controller.LogMessage($"SIM: VA{(int)leftSteps};VB{(int)rightSteps};VC{(int)probeSteps}");

            // update telemetry labels (with mm/s conversion)
            double leftMm = settings.StepsPerMm > 0 ? leftSteps / settings.StepsPerMm : 0;
            double rightMm = settings.StepsPerMm > 0 ? rightSteps / settings.StepsPerMm : 0;

            lblForwardInput.Text = $"Forward Input: {state.Forward:0.00}";
            lblTurnInput.Text = $"Turn Input: {state.Turn:0.00}";
            lblProbeInput.Text = $"Probe Input: {state.Probe:0.00}";
            lblLeftVel.Text = $"Left Track Velocity: {leftSteps:0} steps/s ({leftMm:0.##} mm/s)";
            lblRightVel.Text = $"Right Track Velocity: {rightSteps:0} steps/s ({rightMm:0.##} mm/s)";
            lblGamepad.Text = inputLayer.GamepadConnected ? $"Gamepad: CONNECTED (#{inputLayer.GamepadIndex})" : "Gamepad: DISCONNECTED";
            lblControllerStatus.Text = controller.IsConnected ? "Controller: CONNECTED" : "Controller: DISCONNECTED";
            lblControllerStatus.ForeColor = controller.IsConnected ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            lblMotorsStatus.Text = controller.MotorsEnabled ? "Motors: ENABLED" : "Motors: DISABLED";
            lblMotorsStatus.ForeColor = controller.MotorsEnabled ? System.Drawing.Color.Green : System.Drawing.Color.Orange;

            // update live bars (scale to 0..1 by max velocity)
            try
            {
                double max = Math.Max(1.0, settings.MaxVelocityStepsPerSec);
                int leftW = (int)( (Math.Abs(leftSteps) / max) * (pnlLeftBarBg?.Width ?? 100) );
                int rightW = (int)( (Math.Abs(rightSteps) / max) * (pnlRightBarBg?.Width ?? 100) );
                if (pnlLeftBarFill != null) pnlLeftBarFill.Width = Math.Max(0, Math.Min((pnlLeftBarBg?.Width ?? 100), leftW));
                if (pnlRightBarFill != null) pnlRightBarFill.Width = Math.Max(0, Math.Min((pnlRightBarBg?.Width ?? 100), rightW));
            }
            catch { }

            // debug panel - guard against missing controls to avoid timer crash
            try
            {
                if (lblDbgInputFwd != null) lblDbgInputFwd.Text = $"Input Forward: {state.Forward:0.000}";
                else AppendLog("Warning: lblDbgInputFwd is null");

                if (lblDbgInputTurn != null) lblDbgInputTurn.Text = $"Input Turn: {state.Turn:0.000}";
                else AppendLog("Warning: lblDbgInputTurn is null");

                if (lblDbgInputProbe != null) lblDbgInputProbe.Text = $"Input Probe: {state.Probe:0.000}";
                else AppendLog("Warning: lblDbgInputProbe is null");

                if (lblDbgMixerLeft != null) lblDbgMixerLeft.Text = $"Mixer Left: {outp.Left:0.000}";
                else AppendLog("Warning: lblDbgMixerLeft is null");

                if (lblDbgMixerRight != null) lblDbgMixerRight.Text = $"Mixer Right: {outp.Right:0.000}";
                else AppendLog("Warning: lblDbgMixerRight is null");

                if (lblDbgCmdLeft != null) lblDbgCmdLeft.Text = $"Cmd Left: {leftSteps:0}";
                else AppendLog("Warning: lblDbgCmdLeft is null");

                if (lblDbgCmdRight != null) lblDbgCmdRight.Text = $"Cmd Right: {rightSteps:0}";
                else AppendLog("Warning: lblDbgCmdRight is null");

                if (lblDbgCmdProbe != null) lblDbgCmdProbe.Text = $"Cmd Probe: {probeSteps:0}";
                else AppendLog("Warning: lblDbgCmdProbe is null");
            }
            catch (Exception ex)
            {
                AppendLog("Debug panel update failed: " + ex.Message);
            }

            // loop timing
            var tickEnd = DateTime.UtcNow;
            var elapsed = tickEnd - tickStart;
            double hz = elapsed.TotalSeconds > 0 ? 1.0 / elapsed.TotalSeconds : 0;
            lblLoopRate.Text = $"Loop Rate: {hz:0.0} Hz";

            // update log if controller produced output
            var l = controller.LastLog;
            if (!string.IsNullOrEmpty(l)) AppendLog(l);

            // throttle UI updates to ~10Hz for labels to keep UI readable
            if ((DateTime.UtcNow - lastUiUpdate).TotalMilliseconds >= 100)
            {
                lastUiUpdate = DateTime.UtcNow;
                // no-op here because labels updated above; this ensures rate control externally if needed
            }
        }

        private void AppendLog(string s)
        {
            if (txtLog.InvokeRequired)
            {
                BeginInvoke(new Action(() => AppendLog(s)));
                return;
            }

            var line = $"{DateTime.Now:HH:mm:ss} {s}" + Environment.NewLine;
            // append
            txtLog.AppendText(line);

            // limit lines
            try
            {
                var lines = txtLog.Lines;
                if (lines != null && lines.Length > MaxLogLines)
                {
                    int skip = lines.Length - MaxLogLines;
                    var keep = new string[MaxLogLines];
                    System.Array.Copy(lines, skip, keep, 0, MaxLogLines);
                    txtLog.Lines = keep;
                }
            }
            catch
            {
                // ignore trimming errors
            }

            // auto-scroll if enabled
            if (logAutoScroll)
            {
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
        }
    }
}