using System;
using System.Collections.Generic;
using System.Windows.Forms;
using gclib;

namespace Chainbox_controller
{
    public partial class Form1 : Form
    {
        // Responsive UI state
        private readonly Dictionary<Control, System.Drawing.Font> _originalFonts = new();
        private readonly Dictionary<Control, System.Windows.Forms.Padding> _originalPaddings = new();
        private readonly Dictionary<Control, System.Windows.Forms.Padding> _originalMargins = new();
        private readonly Dictionary<System.Windows.Forms.Control, System.Drawing.Size> _originalMinSizes = new();
        private bool _isCompact = false;

        private const int CompactWidthThreshold = 1280; // logical pixels at 96 DPI
        private InputLayer.InputMode currentInputMode = InputLayer.InputMode.Keyboard;
        private DateTime lastUiUpdate = DateTime.MinValue;
        private bool logAutoScroll = true;
        private const int MaxLogLines = 2000;
        private InputLayer inputLayer;
        private DriveMixer mixer;
        private ControllerInterface controller;
        private ControllerSettings settings;
        private int _loopCount = 0;
        private DateTime _lastRateCalc = DateTime.UtcNow;
        private System.Windows.Forms.Timer controlTimer;

        public Form1()
        {
            InitializeComponent();

            // ensure form receives key events before controls so we can suppress them when input mode forbids keyboard
            this.KeyPreview = true;
            this.Icon = new Icon("innovair.ico");
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

            // Disable focus on 90° turn buttons
            btnTurn90Left.TabStop = false;
            btnTurn90Right.TabStop = false;

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
                // move focus to a non-text control so subsequent key presses don't change the combo
                try { if (this.btnConnect != null) this.btnConnect.Focus(); else { this.ActiveControl = null; this.Focus(); } } catch { }
            };
            // wire galil console send
            this.btnSendGalil.Click += (s, e) => SendGalilCommand();
            //this.txtGalilCmd.KeyDown += (s, e) => { if (e.KeyCode == System.Windows.Forms.Keys.Enter) { e.SuppressKeyPress = true; SendGalilCommand(); } };
            // reflect simulation checkbox
            this.chkSimulation.CheckedChanged += (s, e) => controller.SimulationMode = this.chkSimulation.Checked;

            // Responsive UI: evaluate profile on load/size change/DPI change
            this.Load += (s, e) => ApplyUiProfileIfNeeded();
            this.SizeChanged += (s, e) => ApplyUiProfileIfNeeded();
            this.DpiChanged += (s, e) => ApplyUiProfileIfNeeded();

            // Ensure header status labels are visible on dark background
            try
            {
                if (this.lblControllerStatus != null) this.lblControllerStatus.ForeColor = System.Drawing.Color.White;
                if (this.lblGamepad != null) this.lblGamepad.ForeColor = System.Drawing.Color.White;
                if (this.lblLoopRate != null) this.lblLoopRate.ForeColor = System.Drawing.Color.White;
                if (this.lblInputMode != null) this.lblInputMode.ForeColor = System.Drawing.Color.White;
                if (this.lblMotorsStatus != null && controller != null)
                    this.lblMotorsStatus.ForeColor = controller.MotorsEnabled ? System.Drawing.Color.Green : System.Drawing.Color.Orange;
            }
            catch { }
        }
        private void BtnTurn90Left_Click(object? sender, EventArgs e)
        {
            // Turn by moving left motor negative, right motor positive for a 90-degree pivot
            try
            {
                double steps = (double)numJogSteps.Value * 5.0; // predefined larger pivot amount (tweak as needed)
                // left negative, right positive
                if (!controller.SimulationMode && !controller.IsConnected)
                {
                    AppendLog("Pivot ignored: controller not connected");
                    return;
                }
                controller.MoveRelative(-steps, steps, 0);
                AppendLog($"Pivot 90° left issued: {-steps:0} / {steps:0} steps");
            }
            catch (Exception ex)
            {
                AppendLog("Pivot failed: " + ex.Message);
            }
        }

        private void BtnTurn90Right_Click(object? sender, EventArgs e)
        {
            // Turn by moving left motor positive, right motor negative for a 90-degree pivot
            try
            {
                double steps = (double)numJogSteps.Value * 5.0; // predefined larger pivot amount (tweak as needed)
                if (!controller.SimulationMode && !controller.IsConnected)
                {
                    AppendLog("Pivot ignored: controller not connected");
                    return;
                }
                controller.MoveRelative(steps, -steps, 0);
                AppendLog($"Pivot 90° right issued: {steps:0} / {-steps:0} steps");
            }
            catch (Exception ex)
            {
                AppendLog("Pivot failed: " + ex.Message);
            }
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            // Only allow probe control via keyboard when in Keyboard input mode.
            if (currentInputMode != InputLayer.InputMode.Keyboard)
                return;

            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Up:
                    // move probe forward/up
                    inputLayer.SetManualOverride(new InputState() { Probe = 1.0 });
                    break;
                case System.Windows.Forms.Keys.Down:
                    // move probe reverse/down
                    inputLayer.SetManualOverride(new InputState() { Probe = -1.0 });
                    break;
                default:
                    // ignore other keys here; key handling for pivots/jogs is processed in ProcessCmdKey
                    break;
            }

            e.Handled = true;
        }
        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            if (currentInputMode != InputLayer.InputMode.Keyboard)
                return;

            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Up:
                case System.Windows.Forms.Keys.Down:
                    inputLayer.ClearManualOverride();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Intercept command keys to prevent keyboard navigation and map keys to drive/probe actions
        /// when in Keyboard input mode. Always swallow Tab to prevent focus traversal.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Always swallow Tab so the form cannot be navigated via keyboard
            var key = keyData & Keys.KeyCode;
            if (key == Keys.Tab)
                return true;

            // If keyboard input mode, map specific keys to pivot/jog and let probe keys be handled in KeyDown/KeyUp
            if (currentInputMode == InputLayer.InputMode.Keyboard)
            {
                try
                {
                    switch (key)
                    {
                        case Keys.D4:
                        case Keys.NumPad4:
                            BtnTurn90Left_Click(this, EventArgs.Empty);
                            return true;
                        case Keys.D6:
                        case Keys.NumPad6:
                            BtnTurn90Right_Click(this, EventArgs.Empty);
                            return true;
                        case Keys.D8:
                        case Keys.NumPad8:
                            BtnJogForward_Click(this, EventArgs.Empty);
                            return true;
                        case Keys.D5:
                        case Keys.NumPad5:
                            BtnJogReverse_Click(this, EventArgs.Empty);
                            return true;
                        // Swallow Enter/Escape/Space so they don't activate controls
                        case Keys.Enter:
                        case Keys.Escape:
                        case Keys.Space:
                            return true;
                        default:
                            // swallow other keys to prevent changing focus or interacting with controls
                            return true;
                    }
                }
                catch { }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SendGalilCommand()
        {
            if (!controller.IsConnected)
            {
                AppendLog("Cannot send: controller not connected");
                return;
            }

            //var cmd = this.txtGalilCmd.Text?.Trim();
            //if (string.IsNullOrEmpty(cmd)) return;
            //var resp = controller.SendRawCommand(cmd);
            //this.lstGalilHistory.Items.Add($"> {cmd}");
            //if (!string.IsNullOrEmpty(resp)) this.lstGalilHistory.Items.Add(resp);
            //if (this.lstGalilHistory.Items.Count > 2000) this.lstGalilHistory.Items.RemoveAt(0);
            //AppendLog($"GALIL: {cmd} -> {resp}");
            //this.txtGalilCmd.Clear();
            // keep auto-scroll
            //this.lstGalilHistory.TopIndex = Math.Max(0, this.lstGalilHistory.Items.Count - 1);
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
            this.btnConnect.Click += BtnConnect_Click;
            this.btnDisconnect.Click += BtnDisconnect_Click;
            this.btnEnableMotors.Click += BtnEnableMotors_Click;
            // this.btnDisableMotors.Click += BtnDisableMotors_Click;   // remove unless you add the button back

            this.btnApplySettings.Click += BtnApplySettings_Click;

            this.numMaxSpeed.ValueChanged += (s, e) => MarkSettingsPending();
            this.numAccel.ValueChanged += (s, e) => MarkSettingsPending();
            this.numDecel.ValueChanged += (s, e) => MarkSettingsPending();
            this.numStepsPerMm.ValueChanged += (s, e) => MarkSettingsPending();
            this.numJogSteps.ValueChanged += (s, e) => MarkSettingsPending();

            this.btnForward.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Forward = 1.0 });
            this.btnForward.MouseUp += (s, e) => inputLayer.ClearManualOverride();

            this.btnReverse.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Forward = -1.0 });
            this.btnReverse.MouseUp += (s, e) => inputLayer.ClearManualOverride();

            this.btnLeft.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Turn = -1.0 });
            this.btnLeft.MouseUp += (s, e) => inputLayer.ClearManualOverride();

            this.btnRight.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Turn = 1.0 });
            this.btnRight.MouseUp += (s, e) => inputLayer.ClearManualOverride();

            this.btnStop.Click += BtnEmergencyStop_Click;
            this.btnTurn90Left.Click += BtnTurn90Left_Click;
            this.btnTurn90Right.Click += BtnTurn90Right_Click;

            this.btnProbeLeft.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Probe = -1.0 });
            this.btnProbeLeft.MouseUp += (s, e) => inputLayer.ClearManualOverride();

            this.btnProbeRight.MouseDown += (s, e) => inputLayer.SetManualOverride(new InputState() { Probe = 1.0 });
            this.btnProbeRight.MouseUp += (s, e) => inputLayer.ClearManualOverride();

            this.btnProbeStop.Click += BtnProbeStop_Click;

            this.btnJogForward.Click += BtnJogForward_Click;
            this.btnJogReverse.Click += BtnJogReverse_Click;

            this.btnExportLog.Click += BtnExportLog_Click;
            this.btnCopyLog.Click += BtnCopyLog_Click;
            this.btnClearLog.Click += (s, e) => { this.txtLog.Clear(); };
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
        private void JogTracksBySteps(double steps)
        {
            AppendLog($"Jog button pressed: {steps:0} steps");

            if (!controller.SimulationMode && !controller.IsConnected)
            {
                AppendLog("Jog ignored: controller not connected");
                return;
            }

            try
            {
                inputLayer.ClearManualOverride();
                controller.MoveRelative(steps, steps, 0);
                AppendLog($"Jog move issued: {steps:0} steps");
            }
            catch (Exception ex)
            {
                AppendLog("Jog failed: " + ex.Message);
            }
        }
        private void BtnJogForward_Click(object? sender, EventArgs e)
        {
            JogTracksBySteps((double)numJogSteps.Value);
        }

        private void BtnJogReverse_Click(object? sender, EventArgs e)
        {
            JogTracksBySteps(-(double)numJogSteps.Value);
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
            try
            {
                var text = this.txtLog?.Text ?? string.Empty;
                if (string.IsNullOrEmpty(text))
                {
                    AppendLog("Copy skipped: log is empty");
                    return;
                }
                Clipboard.SetText(text);
                AppendLog("Log copied to clipboard");
            }
            catch (Exception ex)
            {
                AppendLog("Copy failed: " + ex.Message);
            }
        }

        private void ControlTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                // refresh currentInputMode from UI control if present
                if (this.cmbInputMode != null)
                {
                    if (cmbInputMode.SelectedIndex == 0)
                        currentInputMode = InputLayer.InputMode.Gamepad;
                    else
                        currentInputMode = InputLayer.InputMode.Keyboard;

                    if (this.lblInputMode != null)
                        this.lblInputMode.Text = "Input Mode: " + currentInputMode.ToString().ToUpper();
                }

                var state = inputLayer.Update(currentInputMode);
                var outp = mixer.Mix(state.Forward, state.Turn);

                double leftSteps = outp.Left * settings.MaxVelocityStepsPerSec;
                double rightSteps = outp.Right * settings.MaxVelocityStepsPerSec;
                double probeSteps = state.Probe * 20000.0;   // fixed probe speed for now

                if (!controller.RelativeMoveActive)
                {
                    controller.JogVelocity(leftSteps, rightSteps, probeSteps);
                }

                double leftMm = settings.StepsPerMm > 0 ? leftSteps / settings.StepsPerMm : 0;
                double rightMm = settings.StepsPerMm > 0 ? rightSteps / settings.StepsPerMm : 0;

                lblForwardInput.Text = $"Forward Input: {state.Forward:0.00}";
                lblTurnInput.Text = $"Turn Input: {state.Turn:0.00}";
                lblProbeInput.Text = $"Probe Input: {state.Probe:0.00}";
                lblLeftVel.Text = $"Left Track Velocity: {leftSteps:0} steps/s ({leftMm:0.##} mm/s)";
                lblRightVel.Text = $"Right Track Velocity: {rightSteps:0} steps/s ({rightMm:0.##} mm/s)";
                lblProbeVel.Text = $"Probe Velocity: {probeSteps:0} steps/s";

                lblGamepad.Text = inputLayer.GamepadConnected
                    ? $"Gamepad: CONNECTED (#{inputLayer.GamepadIndex})"
                    : "Gamepad: DISCONNECTED";

                lblControllerStatus.Text = controller.IsConnected ? "Controller: CONNECTED" : "Controller: DISCONNECTED";
                lblControllerStatus.ForeColor = controller.IsConnected ? System.Drawing.Color.Green : System.Drawing.Color.Red;

                lblMotorsStatus.Text = controller.MotorsEnabled ? "Motors: ENABLED" : "Motors: DISABLED";
                lblMotorsStatus.ForeColor = controller.MotorsEnabled ? System.Drawing.Color.Green : System.Drawing.Color.Orange;

                try
                {
                    double max = Math.Max(1.0, settings.MaxVelocityStepsPerSec);

                    int leftW = (int)((Math.Abs(leftSteps) / max) * (pnlLeftBarBg?.Width ?? 100));
                    int rightW = (int)((Math.Abs(rightSteps) / max) * (pnlRightBarBg?.Width ?? 100));

                    if (pnlLeftBarFill != null)
                        pnlLeftBarFill.Width = Math.Max(0, Math.Min((pnlLeftBarBg?.Width ?? 100), leftW));

                    if (pnlRightBarFill != null)
                        pnlRightBarFill.Width = Math.Max(0, Math.Min((pnlRightBarBg?.Width ?? 100), rightW));
                }
                catch { }

                _loopCount++;
                var now = DateTime.UtcNow;
                var dt = (now - _lastRateCalc).TotalSeconds;

                if (dt >= 0.5)
                {
                    double hz = _loopCount / dt;
                    lblLoopRate.Text = $"Loop Rate: {hz:0.0} Hz";
                    _loopCount = 0;
                    _lastRateCalc = now;
                }

                if ((DateTime.UtcNow - lastUiUpdate).TotalMilliseconds >= 100)
                    lastUiUpdate = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                controlTimer.Stop();
                AppendLog("Control loop crashed: " + ex.Message);
            }
        }

        private void ApplyUiProfileIfNeeded()
        {
            try
            {
                // Compute logical width at 96 DPI reference
                var dpi = this.DeviceDpi; // current DPI
                double logicalWidth = (this.ClientSize.Width * 96.0) / Math.Max(1, dpi);
                bool shouldCompact = logicalWidth < CompactWidthThreshold;

                // If the main content prefers more width than available, force compact/stacked layout
                try
                {
                    if (this.tlpMain != null)
                    {
                        var preferred = this.tlpMain.GetPreferredSize(new System.Drawing.Size(0, 0)).Width;
                        if (preferred > this.ClientSize.Width)
                            shouldCompact = true;
                    }
                }
                catch { }
                // If user target is a 1920-wide monitor at 100% scale, force compact stacked layout to guarantee fit
                try
                {
                    var screenWidth = System.Windows.Forms.Screen.PrimaryScreen?.Bounds.Width ?? 0;
                    if (screenWidth > 0 && screenWidth <= 1920 && dpi == 96)
                    {
                        shouldCompact = true;
                    }
                }
                catch { }

                SetUiProfile(shouldCompact);
            }
            catch { }
        }

        private void SetUiProfile(bool compact)
        {
            if (_isCompact == compact) return;

            // Capture originals on first use
            if (_originalFonts.Count == 0)
            {
                foreach (var c in GetAllControls(this))
                {
                    try
                    {
                        _originalFonts[c] = c.Font;
                        _originalPaddings[c] = c.Padding;
                        _originalMargins[c] = c.Margin;
                        if (c is Button || c is TextBox || c is NumericUpDown)
                        {
                            _originalMinSizes[c] = c.MinimumSize;
                        }
                    }
                    catch { }
                }
            }

            float fontScale = compact ? 0.75f : 1.0f;
            float paddingScale = compact ? 0.5f : 1.0f;
            float minSizeScale = compact ? 0.6f : 1.0f;

            foreach (var kv in _originalFonts)
            {
                var ctrl = kv.Key;
                var origFont = kv.Value;
                try
                {
                    if (compact)
                        ctrl.Font = new System.Drawing.Font(origFont.FontFamily, Math.Max(6F, origFont.Size * fontScale), origFont.Style);
                    else
                        ctrl.Font = origFont;
                }
                catch { }

                try
                {
                    var origPad = _originalPaddings[ctrl];
                    if (compact)
                        ctrl.Padding = new System.Windows.Forms.Padding((int)(origPad.Left * paddingScale), (int)(origPad.Top * paddingScale), (int)(origPad.Right * paddingScale), (int)(origPad.Bottom * paddingScale));
                    else
                        ctrl.Padding = origPad;
                }
                catch { }

                try
                {
                    var origMarg = _originalMargins[ctrl];
                    if (compact)
                        ctrl.Margin = new System.Windows.Forms.Padding((int)(origMarg.Left * paddingScale), (int)(origMarg.Top * paddingScale), (int)(origMarg.Right * paddingScale), (int)(origMarg.Bottom * paddingScale));
                    else
                        ctrl.Margin = origMarg;
                }
                catch { }

                try
                {
                    if (_originalMinSizes.ContainsKey(ctrl))
                    {
                        var origMin = _originalMinSizes[ctrl];
                        if (compact)
                            ctrl.MinimumSize = new System.Drawing.Size((int)(origMin.Width * minSizeScale), (int)(origMin.Height * minSizeScale));
                        else
                            ctrl.MinimumSize = origMin;
                    }
                }
                catch { }
            }

            // Slightly tighten main layout padding
            try
            {
                if (this.tlpMain != null)
                {
                    if (compact)
                        this.tlpMain.Padding = new Padding(4);
                    else
                        this.tlpMain.Padding = new Padding(8);
                }
                if (this.tlpRoot != null)
                {
                    if (compact)
                        this.tlpRoot.Padding = new Padding(4);
                    else
                        this.tlpRoot.Padding = new Padding(8);
                }
            }
            catch { }

            // Rearrange main columns: stack left/right when compact to avoid horizontal clipping
            try
            {
                if (this.tlpMain != null)
                {
                    Control left = null, right = null;
                    try
                    {
                        left = this.tlpMain.GetControlFromPosition(0, 0);
                        right = this.tlpMain.GetControlFromPosition(1, 0);
                    }
                    catch { }

                    if (left == null || right == null)
                    {
                        // fallback: identify by known group boxes
                        foreach (Control c in this.tlpMain.Controls)
                        {
                            try
                            {
                                if (c.Controls.Contains(this.gbDrive) || c.Controls.Contains(this.gbConnection) || c.Controls.Contains(this.gbSettings))
                                    left = c;
                                if (c.Controls.Contains(this.gbTelemetry) || c.Controls.Contains(this.gbDebug))
                                    right = c;
                            }
                            catch { }
                        }
                    }

                    if (compact)
                    {
                        // move to stacked layout
                        this.tlpMain.SuspendLayout();
                        var savedLeft = left;
                        var savedRight = right;
                        this.tlpMain.Controls.Clear();
                        this.tlpMain.ColumnStyles.Clear();
                        this.tlpMain.RowStyles.Clear();
                        this.tlpMain.ColumnCount = 1;
                        this.tlpMain.RowCount = 2;
                        this.tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 72F));
                        this.tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
                        if (savedLeft != null) { if (!this.tlpMain.Controls.Contains(savedLeft)) this.tlpMain.Controls.Add(savedLeft, 0, 0); savedLeft.Dock = DockStyle.Fill; }
                        if (savedRight != null) { if (!this.tlpMain.Controls.Contains(savedRight)) this.tlpMain.Controls.Add(savedRight, 0, 1); savedRight.Dock = DockStyle.Fill; }
                        this.tlpMain.ResumeLayout();
                    }
                    else
                    {
                        // restore two-column layout
                        this.tlpMain.SuspendLayout();
                        var savedLeft = left;
                        var savedRight = right;
                        this.tlpMain.Controls.Clear();
                        this.tlpMain.ColumnStyles.Clear();
                        this.tlpMain.RowStyles.Clear();
                        this.tlpMain.ColumnCount = 2;
                        this.tlpMain.RowCount = 1;
                        this.tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
                        this.tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
                        if (savedLeft != null) { if (!this.tlpMain.Controls.Contains(savedLeft)) this.tlpMain.Controls.Add(savedLeft, 0, 0); savedLeft.Dock = DockStyle.Fill; }
                        if (savedRight != null) { if (!this.tlpMain.Controls.Contains(savedRight)) this.tlpMain.Controls.Add(savedRight, 1, 0); savedRight.Dock = DockStyle.Fill; }
                        this.tlpMain.ResumeLayout();

                        // If the restored two-column layout still doesn't fit, keep stacked layout to avoid horizontal clipping
                        try
                        {
                            var preferred = this.tlpMain.GetPreferredSize(new System.Drawing.Size(0, 0)).Width;
                            if (preferred > this.ClientSize.Width)
                            {
                                // fallback to stacked
                                this.tlpMain.SuspendLayout();
                                this.tlpMain.Controls.Clear();
                                this.tlpMain.ColumnStyles.Clear();
                                this.tlpMain.RowStyles.Clear();
                                this.tlpMain.ColumnCount = 1;
                                this.tlpMain.RowCount = 2;
                                this.tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));
                                this.tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
                                if (savedLeft != null) { if (!this.tlpMain.Controls.Contains(savedLeft)) this.tlpMain.Controls.Add(savedLeft, 0, 0); savedLeft.Dock = DockStyle.Fill; }
                                if (savedRight != null) { if (!this.tlpMain.Controls.Contains(savedRight)) this.tlpMain.Controls.Add(savedRight, 0, 1); savedRight.Dock = DockStyle.Fill; }
                                this.tlpMain.ResumeLayout();
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }

            // Reduce minimum sizes for common container controls to improve fit
            try
            {
                foreach (Control c in GetAllControls(this))
                {
                    try
                    {
                        if (compact)
                        {
                            if (c is GroupBox || c is TableLayoutPanel || c is Panel)
                            {
                                c.MinimumSize = new System.Drawing.Size(20, 20);
                            }
                            // reduce jog button autosize to avoid wrapping
                            if (c == this.btnJogForward || c == this.btnJogReverse)
                            {
                                c.AutoSize = false;
                                c.MinimumSize = new System.Drawing.Size(40, 28);
                            }
                        }
                        else
                        {
                            // restore if we stored an original min size
                            if (_originalMinSizes.ContainsKey(c))
                                c.MinimumSize = _originalMinSizes[c];
                            else
                                c.MinimumSize = new System.Drawing.Size(0, 0);
                            if (c == this.btnJogForward || c == this.btnJogReverse)
                            {
                                c.AutoSize = true;
                                // restore min if available
                                if (_originalMinSizes.ContainsKey(c)) c.MinimumSize = _originalMinSizes[c];
                            }
                        }
                    }
                    catch { }
                }

                // Adjust root row fixed heights to free vertical space in compact mode
                if (this.tlpRoot != null && this.tlpRoot.RowStyles.Count >= 3)
                {
                    if (compact)
                    {
                        // reduce header height to prevent pushing status off-screen
                        this.tlpRoot.RowStyles[0].SizeType = SizeType.Absolute;
                        this.tlpRoot.RowStyles[0].Height = 56F;
                        // reduce diagnostics height to give more space for main/drive area
                        this.tlpRoot.RowStyles[2].SizeType = SizeType.Absolute;
                        this.tlpRoot.RowStyles[2].Height = 90F;
                    }
                    else
                    {
                        this.tlpRoot.RowStyles[0].SizeType = SizeType.Absolute;
                        this.tlpRoot.RowStyles[0].Height = 88F;
                        this.tlpRoot.RowStyles[2].SizeType = SizeType.Absolute;
                        this.tlpRoot.RowStyles[2].Height = 250F;
                    }
                }

                // Ensure form allows scrolling as a last resort so controls are never completely inaccessible
                this.AutoScroll = true;
            }
            catch { }

            _isCompact = compact;
        }

        private IEnumerable<Control> GetAllControls(Control root)
        {
            if (root == null) yield break;
            var stack = new Stack<Control>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                var c = stack.Pop();
                yield return c;
                foreach (Control child in c.Controls)
                    stack.Push(child);
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