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
        private DeadReckoner deadReckoner = new DeadReckoner(
            initialGridWidthMm: 5000.0,
            initialGridHeightMm: 5000.0,
            cellSizeMm: 1.0,
            expandMarginCells: 50);
        private EstimatorParameters estParams = new EstimatorParameters();
        private DateTime _lastEstimatorUpdate = DateTime.UtcNow;
        private DateTime _lastDiagLog = DateTime.MinValue;
        private long? _lastTpA = null;
        private long? _lastTpB = null;
        private int _loopCount = 0;
        private DateTime _lastRateCalc = DateTime.UtcNow;
        private System.Windows.Forms.Timer controlTimer;

        private double leftDir = 1.0;
        private double rightDir = -1.0; // flip this motor

        private bool _scanEnabled = false;
        private readonly string _settingsFilePath =
            System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ui_settings.json");
        private UiSettings _uiSettings = new UiSettings();

        public Form1()
        {
            InitializeComponent();

            // ensure form receives key events before controls so we can suppress them when input mode forbids keyboard
            this.KeyPreview = true;
            try
            {
                string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "innovair.ico");
                if (System.IO.File.Exists(iconPath))
                    this.Icon = new Icon(iconPath);
            }
            catch { }
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

            LoadUiSettings();
            ApplyUiSettingsToControls(_uiSettings);
            ApplyControlValuesToModels();

            this.FormClosing += Form1_FormClosing;
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
                // Combo items: 0 = Gamepad, 1 = Keyboard
                if (cmbInputMode.SelectedIndex == 0) currentInputMode = InputLayer.InputMode.Gamepad;
                else if (cmbInputMode.SelectedIndex == 1) currentInputMode = InputLayer.InputMode.Keyboard;
                // reflect in header label if present
                if (this.lblInputMode != null) this.lblInputMode.Text = "Input Mode: " + currentInputMode.ToString().ToUpper();
                // move focus to a non-text control so subsequent key presses don't change the combo
                try { if (this.btnConnect != null) this.btnConnect.Focus(); else { this.ActiveControl = null; this.Focus(); } } catch { }
            };
            // wire galil console send
            this.btnSendGalil.Click += (s, e) => SendGalilCommand();
            //this.txtGalilCmd.KeyDown += (s, e) => { if (e.KeyCode == System.Windows.Forms.Keys.Enter) { e.SuppressKeyPress = true; SendGalilCommand(); } };
            // reflect simulation checkbox and reset TP baseline when mode changes
            this.chkSimulation.CheckedChanged += (s, e) =>
            {
                controller.SimulationMode = this.chkSimulation.Checked;
                try
                {
                    _lastTpA = controller.QueryPosition('A');
                    _lastTpB = controller.QueryPosition('B');
                }
                catch { }
            };

            // reset pose button
            try
            {
                if (this.btnResetPose != null)
                {
                    this.btnResetPose.Click += (s, e) =>
                    {
                        deadReckoner.Reset();
                        _lastTpA = null;
                        _lastTpB = null;

                        try
                        {
                            this.coverageMapControl?.UpdateMap(
                                deadReckoner.Grid,
                                deadReckoner.GridXMinMm,
                                deadReckoner.GridYMinMm,
                                deadReckoner.CellSizeMm,
                                deadReckoner.CurrentPose,
                                estParams);
                        }
                        catch { }

                        AppendLog("Pose and coverage reset");
                    };
                }
            }
            catch { }
            // Responsive UI hooks (temporarily disabled to preserve designer layout).
            // These can be re-enabled after responsive logic is aligned with the designer.
            // this.Load += (s, e) => ApplyUiProfileIfNeeded();
            // this.SizeChanged += (s, e) => ApplyUiProfileIfNeeded();
            // this.DpiChanged += (s, e) => ApplyUiProfileIfNeeded();

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
            if (this.btnSaveSettings != null)
            {
                this.btnSaveSettings.Click += (s, e) =>
                {
                    SaveUiSettings();
                    AppendLog("Settings saved");
                    if (lblSettingsStatus != null) lblSettingsStatus.Text = "Settings: SAVED";
                };
            }
        }

        // Compute approximate encoder counts required for an in-place rotation
        // by given degrees. Uses estimator parameters (effective track width and counts per mm).
        private double RotationCountsForDegrees(double degrees)
        {
            if (this.chkUseCustomTurn90 != null && this.chkUseCustomTurn90.Checked && this.numTurn90Steps != null)
                return (double)this.numTurn90Steps.Value;

            double arcMm = Math.PI * estParams.EffectiveTrackWidthMm * (degrees / 360.0);
            double counts = arcMm * estParams.EstimatedCountsPerMm;
            return counts;
        }
        private void BtnAdvancedSettings_Click(object? sender, EventArgs e)
        {
            var workingCopy = CaptureUiSettings();

            using (var dlg = new AdvancedSettingsForm(workingCopy))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _uiSettings = dlg.Settings;
                    ApplyUiSettingsToControls(_uiSettings);
                    ApplyControlValuesToModels();
                    SaveUiSettings();
                }
            }
        }
        private void BtnTurn90Left_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!controller.SimulationMode && !controller.IsConnected)
                {
                    AppendLog("Pivot ignored: controller not connected");
                    return;
                }

                // compute counts required for 90° rotation using geometry parameters
                double counts = RotationCountsForDegrees(90.0);
                double desiredLeftCounts = -counts;   // left wheel backward for left pivot
                double desiredRightCounts = counts;   // right wheel forward

                double axisLeft = leftDir * desiredLeftCounts;
                double axisRight = rightDir * desiredRightCounts;

                MoveTracksRelative(axisLeft, axisRight, 0);
                AppendLog($"Pivot 90° left issued: axisLeft={axisLeft:0} axisRight={axisRight:0} counts={counts:0}");
            }
            catch (Exception ex)
            {
                AppendLog("Pivot failed: " + ex.Message);
            }
        }

        private void BtnTurn90Right_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!controller.SimulationMode && !controller.IsConnected)
                {
                    AppendLog("Pivot ignored: controller not connected");
                    return;
                }

                double counts = RotationCountsForDegrees(90.0);
                double desiredLeftCounts = counts;    // left forward
                double desiredRightCounts = -counts;  // right backward

                double axisLeft = leftDir * desiredLeftCounts;
                double axisRight = rightDir * desiredRightCounts;

                MoveTracksRelative(axisLeft, axisRight, 0);
                AppendLog($"Pivot 90° right issued: axisLeft={axisLeft:0} axisRight={axisRight:0} counts={counts:0}");
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
            try
            {
                var cmd = this.txtGalilCmd?.Text?.Trim();
                if (string.IsNullOrEmpty(cmd))
                {
                    // nothing to send
                    return;
                }

                if (!controller.IsConnected && !controller.SimulationMode)
                {
                    AppendLog("Cannot send: controller not connected");
                    return;
                }

                string resp = string.Empty;
                try
                {
                    resp = controller.SendRawCommand(cmd) ?? string.Empty;
                }
                catch (Exception ex)
                {
                    AppendLog("Send failed: " + ex.Message);
                    return;
                }

                if (this.lstGalilHistory != null)
                {
                    this.lstGalilHistory.Items.Add($"> {cmd}");
                    if (!string.IsNullOrEmpty(resp)) this.lstGalilHistory.Items.Add(resp.Trim());
                    if (this.lstGalilHistory.Items.Count > 2000) this.lstGalilHistory.Items.RemoveAt(0);
                    try { this.lstGalilHistory.TopIndex = Math.Max(0, this.lstGalilHistory.Items.Count - 1); } catch { }
                }

                AppendLog($"GALIL: {cmd} -> {resp}");
                if (this.txtGalilCmd != null) this.txtGalilCmd.Clear();
            }
            catch (Exception ex)
            {
                AppendLog("SendGalilCommand error: " + ex.Message);
            }
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

            this.btnApplySettings.Click += BtnApplySettings_Click;
            this.btnAdvancedSettings.Click += BtnAdvancedSettings_Click;

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
                MoveTracksRelative(steps, steps, 0);
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
            ApplyControlValuesToModels();

            controller.ApplySettings(settings);
            AppendLog("Drive settings updated");

            if (lblSettingsStatus != null)
                lblSettingsStatus.Text = "Settings: APPLIED";
        }

        private void MarkSettingsPending()
        {
            if (lblSettingsStatus != null) lblSettingsStatus.Text = "Settings: PENDING";
        }

        private void BtnEmergencyStop_Click(object? sender, EventArgs e)
        {
            inputLayer.ClearManualOverride();
            controller.StopAll();
            controller.ClearRelativeMoveFlag();
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
                    else if (cmbInputMode.SelectedIndex == 1)
                        currentInputMode = InputLayer.InputMode.Keyboard;

                    if (this.lblInputMode != null)
                        this.lblInputMode.Text = "Input Mode: " + currentInputMode.ToString().ToUpper();
                }

                var state = inputLayer.Update(currentInputMode);
                var outp = mixer.Mix(state.Forward, state.Turn);

                double leftSteps = leftDir * outp.Left * settings.MaxVelocityStepsPerSec;
                double rightSteps = rightDir * outp.Right * settings.MaxVelocityStepsPerSec;
                double probeSteps = state.Probe * 20000.0;

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
                // Update dead-reckoning pose using absolute TP encoder deltas (recommended)
                try
                {
                    var nowEst = DateTime.UtcNow;
                    double estDt = (nowEst - _lastEstimatorUpdate).TotalSeconds;
                    if (estDt <= 0) estDt = 0.02; // fallback

                    // Read absolute encoder positions (TP) for A/B. Allow swapping axes if hardware mapping differs.
                    long tpA = controller.QueryPosition(estParams.SwapAxes ? 'B' : 'A');
                    long tpB = controller.QueryPosition(estParams.SwapAxes ? 'A' : 'B');

                    // Compute deltas since last sample
                    if (_lastTpA == null) _lastTpA = tpA;
                    if (_lastTpB == null) _lastTpB = tpB;

                    long dA = tpA - _lastTpA.Value;
                    long dB = tpB - _lastTpB.Value;

                    // Convert counts to mm using per-side calibration
                    double dLeftMm = (leftDir * dA) * estParams.LeftMmPerCount;
                    double dRightMm = (rightDir * dB) * estParams.RightMmPerCount;

                    // Diagnostics logging of raw TP values (throttled)
                    if ((DateTime.UtcNow - _lastDiagLog).TotalMilliseconds > 250)
                    {
                        _lastDiagLog = DateTime.UtcNow;
                        AppendLog($"TP: A={tpA} dA={dA} B={tpB} dB={dB} dL={dLeftMm:0.###}mm dR={dRightMm:0.###}mm");
                    }

                    deadReckoner.IntegrateAndPaint(dLeftMm, dRightMm, estParams, _scanEnabled);
                    _lastEstimatorUpdate = nowEst;
                    _lastTpA = tpA;
                    _lastTpB = tpB;

                    if (this.lblPose != null)
                    {
                        double thetaDeg = deadReckoner.Theta * (180.0 / Math.PI);
                        this.lblPose.Text = $"Pose: X={deadReckoner.X:0.##} mm Y={deadReckoner.Y:0.##} mm Θ={thetaDeg:0.##}°";
                    }

                    try
                    {
                        this.coverageMapControl?.UpdateMap(
                            deadReckoner.Grid,
                            deadReckoner.GridXMinMm,
                            deadReckoner.GridYMinMm,
                            deadReckoner.CellSizeMm,
                            deadReckoner.CurrentPose,
                            estParams);
                    }
                    catch { }
                }
                catch { }
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
                // NOTE: removed forced compact rule for typical 1920 displays. Let preferred sizing decide.

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
                        // fallback: identify by known group boxes within tlpMain
                        foreach (Control c in this.tlpMain.Controls)
                        {
                            try
                            {
                                if (c.Controls.Contains(this.gbDrive) || c.Controls.Contains(this.gbConnection) || c.Controls.Contains(this.gbSettings))
                                    left = c;
                                if (c.Controls.Contains(this.gbTelemetry))
                                    right = c;
                            }
                            catch { }
                        }

                        // If right isn't in tlpMain, attempt to locate it on tlpRoot (some designer versions added it there)
                        if (right == null && this.tlpRoot != null)
                        {
                            try
                            {
                                for (int r = 0; r < this.tlpRoot.RowCount; r++)
                                {
                                    for (int c = 0; c < this.tlpRoot.ColumnCount; c++)
                                    {
                                        var ctrl = this.tlpRoot.GetControlFromPosition(c, r);
                                        if (ctrl != null && ctrl.Controls.Contains(this.gbTelemetry))
                                        {
                                            right = ctrl;
                                            break;
                                        }
                                    }
                                    if (right != null) break;
                                }
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
                        this.tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66F));
                        this.tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
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
        private void MoveTracksRelative(double leftSteps, double rightSteps, double probeSteps = 0)
        {
            controller.MoveRelative(leftSteps, rightSteps, probeSteps);

            _ = System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    long prevA = controller.QueryPosition('A');
                    long prevB = controller.QueryPosition('B');
                    int stable = 0;
                    var sw = System.Diagnostics.Stopwatch.StartNew();

                    while (true)
                    {
                        await System.Threading.Tasks.Task.Delay(100);

                        long a = controller.QueryPosition('A');
                        long b = controller.QueryPosition('B');

                        if (a == prevA && b == prevB)
                            stable++;
                        else
                            stable = 0;

                        prevA = a;
                        prevB = b;

                        if (stable >= 3 || sw.ElapsedMilliseconds > 30000)
                            break;
                    }
                }
                catch
                {
                    // ignore monitor errors; just ensure state is cleared
                }
                finally
                {
                    try
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            controller.ClearRelativeMoveFlag();

                            if (this.lblPose != null)
                            {
                                double thetaDeg = deadReckoner.Theta * (180.0 / Math.PI);
                                this.lblPose.Text = $"Pose: X={deadReckoner.X:0.##} mm Y={deadReckoner.Y:0.##} mm Θ={thetaDeg:0.##}°";
                            }

                            try
                            {
                                this.coverageMapControl?.UpdateMap(
                                    deadReckoner.Grid,
                                    deadReckoner.GridXMinMm,
                                    deadReckoner.GridYMinMm,
                                    deadReckoner.CellSizeMm,
                                    deadReckoner.CurrentPose,
                                    estParams);
                            }
                            catch { }
                        }));
                    }
                    catch { }
                }
            });
        }
        private void ApplyControlValuesToModels()
        {
            settings.MaxVelocityStepsPerSec = (double)numMaxSpeed.Value;
            settings.AccelStepsPerSec2 = (double)numAccel.Value;
            settings.DecelStepsPerSec2 = (double)numDecel.Value;
            settings.StepsPerMm = (double)numStepsPerMm.Value;

            estParams.GridResolutionMm = 1.0;
        }

        private UiSettings CaptureUiSettings()
        {
            return new UiSettings
            {
                IpAddress = this.txtIp?.Text ?? "192.168.0.101",
                SimulationMode = this.chkSimulation?.Checked ?? false,
                InputModeIndex = this.cmbInputMode?.SelectedIndex ?? 0,

                MaxVelocityStepsPerSec = (double)this.numMaxSpeed.Value,
                AccelStepsPerSec2 = (double)this.numAccel.Value,
                DecelStepsPerSec2 = (double)this.numDecel.Value,
                StepsPerMm = (double)this.numStepsPerMm.Value,
                JogSteps = (double)this.numJogSteps.Value,

                UseCustomTurn90Counts = this.chkUseCustomTurn90?.Checked ?? false,
                Turn90Counts = (double)this.numTurn90Steps.Value,

                LeftMmPerCount = estParams.LeftMmPerCount,
                RightMmPerCount = estParams.RightMmPerCount,
                EffectiveTrackWidthMm = estParams.EffectiveTrackWidthMm,
                SwapAxes = estParams.SwapAxes,

                RobotWidthMm = estParams.RobotWidthMm,
                RobotLengthMm = estParams.RobotLengthMm,
                ProbeWidthMm = estParams.ProbeWidthMm,
                ProbeForwardOffsetMm = estParams.ProbeForwardOffsetMm,
                ProbeLateralOffsetMm = estParams.ProbeLateralOffsetMm,
                GridResolutionMm = estParams.GridResolutionMm
            };
        }

        private void ApplyUiSettingsToControls(UiSettings? s = null)
        {
            s ??= new UiSettings();

            if (this.txtIp != null) this.txtIp.Text = s.IpAddress;
            if (this.chkSimulation != null) this.chkSimulation.Checked = s.SimulationMode;
            if (this.cmbInputMode != null)
            {
                int idx = Math.Max(0, Math.Min(this.cmbInputMode.Items.Count - 1, s.InputModeIndex));
                this.cmbInputMode.SelectedIndex = idx;
            }

            this.numMaxSpeed.Value = ClampDecimal((decimal)s.MaxVelocityStepsPerSec, this.numMaxSpeed.Minimum, this.numMaxSpeed.Maximum);
            this.numAccel.Value = ClampDecimal((decimal)s.AccelStepsPerSec2, this.numAccel.Minimum, this.numAccel.Maximum);
            this.numDecel.Value = ClampDecimal((decimal)s.DecelStepsPerSec2, this.numDecel.Minimum, this.numDecel.Maximum);
            this.numStepsPerMm.Value = ClampDecimal((decimal)s.StepsPerMm, this.numStepsPerMm.Minimum, this.numStepsPerMm.Maximum);
            this.numJogSteps.Value = ClampDecimal((decimal)s.JogSteps, this.numJogSteps.Minimum, this.numJogSteps.Maximum);

            if (this.chkUseCustomTurn90 != null) this.chkUseCustomTurn90.Checked = s.UseCustomTurn90Counts;
            if (this.numTurn90Steps != null) this.numTurn90Steps.Value = ClampDecimal((decimal)s.Turn90Counts, this.numTurn90Steps.Minimum, this.numTurn90Steps.Maximum);

            estParams.LeftMmPerCount = s.LeftMmPerCount;
            estParams.RightMmPerCount = s.RightMmPerCount;
            estParams.EffectiveTrackWidthMm = s.EffectiveTrackWidthMm;
            estParams.SwapAxes = s.SwapAxes;

            estParams.RobotWidthMm = s.RobotWidthMm;
            estParams.RobotLengthMm = s.RobotLengthMm;
            estParams.ProbeWidthMm = s.ProbeWidthMm;
            estParams.ProbeForwardOffsetMm = s.ProbeForwardOffsetMm;
            estParams.ProbeLateralOffsetMm = s.ProbeLateralOffsetMm;
            estParams.GridResolutionMm = s.GridResolutionMm;
        }

        private void LoadUiSettings()
        {
            try
            {
                if (!System.IO.File.Exists(_settingsFilePath))
                    return;

                string json = System.IO.File.ReadAllText(_settingsFilePath);
                var loaded = System.Text.Json.JsonSerializer.Deserialize<UiSettings>(json);
                if (loaded != null)
                    _uiSettings = loaded;
            }
            catch (Exception ex)
            {
                AppendLog("Load settings failed: " + ex.Message);
            }
        }

        private void SaveUiSettings()
        {
            try
            {
                _uiSettings = CaptureUiSettings();

                var json = System.Text.Json.JsonSerializer.Serialize(
                    _uiSettings,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                System.IO.File.WriteAllText(_settingsFilePath, json);
            }
            catch (Exception ex)
            {
                AppendLog("Save settings failed: " + ex.Message);
            }
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            SaveUiSettings();
        }

        private static decimal ClampDecimal(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
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