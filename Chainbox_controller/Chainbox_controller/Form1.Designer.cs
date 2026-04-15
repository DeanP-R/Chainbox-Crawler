namespace Chainbox_controller
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            // subtle off-white form background for a cleaner dashboard look
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.Text = "Chainbox Crawler Manual Control";

            // Root layout - vertical: status (fixed), main (fill), diagnostics (fixed)
            this.tlpRoot = new System.Windows.Forms.TableLayoutPanel();

            // Use a two-column root so the app feels like a professional two-pane dashboard.
            tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            tlpRoot.RowCount = 2; tlpRoot.ColumnCount = 2;
            tlpRoot.RowStyles.Clear(); tlpRoot.ColumnStyles.Clear();
            // fixed header height, main row fills remaining space
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));      // header (taller to fit status)
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // main
            // Slightly adjusted proportions for a balanced dashboard
            tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            // use slightly larger outer padding for breathing room
            tlpRoot.Padding = new System.Windows.Forms.Padding(12);
            var tlpStatus = new System.Windows.Forms.TableLayoutPanel();

            // Status bar (dark header)
            var header = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(0x2b, 0x2b, 0x2b),
                Padding = new Padding(4, 0, 4, 0),
                Margin = new Padding(0)
            };

            var lblTitle = new Label()
            {
                Text = "CHAINBOX CRAWLER MANUAL CONTROL",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };

            this.headerLayout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };

            headerLayout.RowStyles.Clear();
            // align title/status to match header height (64px)
            headerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));  // title
            headerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));  // status row

            headerLayout.Controls.Add(lblTitle, 0, 0);
            headerLayout.Controls.Add(tlpStatus, 0, 1);

            header.Controls.Clear();
            header.Controls.Add(headerLayout); 


            tlpStatus.Dock = System.Windows.Forms.DockStyle.Fill; 
            tlpStatus.Padding = new Padding(0);
            tlpStatus.Margin = new Padding(0);

            tlpStatus.RowCount = 1;
            tlpStatus.ColumnCount = 4;
            tlpStatus.ColumnStyles.Clear();
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Controls - keep names used elsewhere to avoid larger refactor
            var headerFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblControllerStatus = new System.Windows.Forms.Label() { Text = "Controller: DISCONNECTED", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ForeColor = System.Drawing.Color.White, Font = headerFont };
            this.lblMotorsStatus = new System.Windows.Forms.Label() { Text = "Motors: DISABLED", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ForeColor = System.Drawing.Color.Orange, Font = headerFont };
            this.lblGamepad = new System.Windows.Forms.Label() { Text = "Gamepad: DISCONNECTED", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ForeColor = System.Drawing.Color.White, Font = headerFont };
            this.lblLoopRate = new System.Windows.Forms.Label() { Text = "Loop Rate: 0.0 Hz", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ForeColor = System.Drawing.Color.White, Font = headerFont };
            this.lblInputMode = new System.Windows.Forms.Label() { Text = "Input Mode: AUTOMATIC", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleRight, ForeColor = System.Drawing.Color.White, Font = headerFont };

            if (this.lblControllerStatus != null) tlpStatus.Controls.Add(this.lblControllerStatus, 0, 0);
            if (this.lblMotorsStatus != null) tlpStatus.Controls.Add(this.lblMotorsStatus, 1, 0);
            if (this.lblGamepad != null) tlpStatus.Controls.Add(this.lblGamepad, 2, 0);

            // Right side horizontal layout
            var tlpRightHdr = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            tlpRightHdr.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpRightHdr.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            tlpRightHdr.Controls.Add(this.lblInputMode, 0, 0);
            tlpRightHdr.Controls.Add(this.lblLoopRate, 1, 0);

            // Align properly
            this.lblInputMode.TextAlign = ContentAlignment.MiddleRight;
            this.lblLoopRate.TextAlign = ContentAlignment.MiddleRight;

            tlpStatus.Controls.Add(tlpRightHdr, 3, 0);

            //header.Controls.Add(tlpStatus);

            // Main two-column panel
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            tlpMain = this.tlpMain;
            tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tlpMain.ColumnCount = 2;
            tlpMain.RowCount = 1;
            tlpMain.RowStyles.Clear();
            tlpMain.ColumnStyles.Clear();
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            // left/right proportions
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            tlpMain.Padding = new Padding(8);
            tlpMain.Margin = new Padding(0);
            tlpMain.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            // Standardized fonts for drive buttons
            var driveButtonFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            var auxButtonFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            var stopButtonFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);

            // Drive controls - full 3x3 grid
            this.gbDrive = new System.Windows.Forms.GroupBox()
            {
                Text = "Drive Controls",
                Dock = System.Windows.Forms.DockStyle.Fill,
                Padding = new System.Windows.Forms.Padding(12)
            };
            // group boxes use a consistent margin and bolder title font
            this.gbDrive.Margin = new Padding(8);
            this.gbDrive.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

            var tlpDrive = new System.Windows.Forms.TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 3,
                Margin = new Padding(0),
                Padding = new Padding(0),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            tlpDrive.RowStyles.Clear();
            // give middle row (stop & turn) a bit more room
            tlpDrive.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tlpDrive.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tlpDrive.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));

            tlpDrive.ColumnStyles.Clear();
            tlpDrive.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpDrive.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpDrive.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            // Slightly reduce drive button sizes for 1080p; use AutoScale on font
            this.btnForward = new System.Windows.Forms.Button()
            {
                Text = "FORWARD",
                Dock = DockStyle.Fill,
                Font = driveButtonFont,
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnTurn90Left = new System.Windows.Forms.Button()
            {
                Text = "90° LEFT",
                Dock = DockStyle.Fill,
                Font = driveButtonFont,
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnLeft = new System.Windows.Forms.Button()
            {
                Text = "TURN LEFT",
                Dock = DockStyle.Fill,
                Font = driveButtonFont,
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnStop = new System.Windows.Forms.Button()
            {
                Text = "STOP",
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Red,
                ForeColor = System.Drawing.Color.White,
                Font = stopButtonFont,
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(64, 52)
            };

            this.btnRight = new System.Windows.Forms.Button()
            {
                Text = "TURN RIGHT",
                Dock = DockStyle.Fill,
                Font = driveButtonFont,
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnTurn90Right = new System.Windows.Forms.Button()
            {
                Text = "90° RIGHT",
                Dock = DockStyle.Fill,
                Font = driveButtonFont,
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnReverse = new System.Windows.Forms.Button()
            {
                Text = "REVERSE",
                Dock = DockStyle.Fill,
                Font = driveButtonFont,
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnJogForward = new System.Windows.Forms.Button()
            {
                Text = "JOG FORWARD",
                Dock = DockStyle.Fill,
                Font = driveButtonFont,
                Margin = new System.Windows.Forms.Padding(8),
                MinimumSize = new System.Drawing.Size(80, 60)
            };

            this.btnJogReverse = new System.Windows.Forms.Button()
            {
                Text = "JOG REVERSE",
                Dock = DockStyle.Fill,
                Font = driveButtonFont,
                Margin = new System.Windows.Forms.Padding(8),
                MinimumSize = new System.Drawing.Size(80, 60)
            };

            // top row
            // [empty] [FORWARD] [empty]
            tlpDrive.Controls.Add(this.btnTurn90Left, 0, 0);
            tlpDrive.Controls.Add(this.btnForward, 1, 0);
            tlpDrive.Controls.Add(this.btnTurn90Right, 2, 0);

            // middle row
            // [LEFT] [STOP] [RIGHT]
            tlpDrive.Controls.Add(this.btnLeft, 0, 1); // Adding button for left control
            tlpDrive.Controls.Add(this.btnStop, 1, 1); // Adding button for stop control
            tlpDrive.Controls.Add(this.btnRight, 2, 1); // Adding button for right control

            // bottom row
            // [JOG FWD] [REVERSE] [JOG REV]
            tlpDrive.Controls.Add(this.btnJogForward, 0, 2); // Adding button for jog forward control
            tlpDrive.Controls.Add(this.btnReverse, 1, 2); // Adding button for reverse control
            tlpDrive.Controls.Add(this.btnJogReverse, 2, 2); // Adding button for jog reverse control

            // ensure jog buttons wrap properly on small widths
            this.btnJogForward.AutoSize = true; this.btnJogForward.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.btnJogReverse.AutoSize = true; this.btnJogReverse.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            this.gbDrive.Controls.Clear();
            this.gbDrive.Controls.Add(tlpDrive);

            // Probe controls
            this.gbProbe = new System.Windows.Forms.GroupBox()
            {
                Text = "Probe Controls",
                Dock = System.Windows.Forms.DockStyle.Fill,
                Padding = new System.Windows.Forms.Padding(12)
            };
            this.gbProbe.Margin = new Padding(8);
            this.gbProbe.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

            var tlpProbe = new System.Windows.Forms.TableLayoutPanel()
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };

            tlpProbe.ColumnStyles.Clear();
            // make probe controls compact on smaller screens
            tlpProbe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpProbe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpProbe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            this.btnProbeLeft = new System.Windows.Forms.Button() { Text = "Move Left", Dock = DockStyle.Fill };
            this.btnProbeStop = new System.Windows.Forms.Button() { Text = "Stop", Dock = DockStyle.Fill };
            this.btnProbeRight = new System.Windows.Forms.Button() { Text = "Move Right", Dock = DockStyle.Fill };

            tlpProbe.Controls.Add(this.btnProbeLeft, 0, 0);
            tlpProbe.Controls.Add(this.btnProbeStop, 1, 0);
            tlpProbe.Controls.Add(this.btnProbeRight, 2, 0);

            this.gbProbe.Controls.Add(tlpProbe);

            // Drive settings
            this.gbSettings = new System.Windows.Forms.GroupBox()
            {
                Text = "Drive Settings",
                Dock = System.Windows.Forms.DockStyle.Fill,
                Padding = new System.Windows.Forms.Padding(6)
            };
            // Slightly larger, consistent bold font for group titles
            this.gbSettings.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gbSettings.Margin = new Padding(8);

            var settingsScroll = new System.Windows.Forms.Panel()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var tlpSettings = new System.Windows.Forms.TableLayoutPanel()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 6,
                Dock = DockStyle.Top
            };

            tlpSettings.ColumnStyles.Clear();
            tlpSettings.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlpSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            this.numMaxSpeed = new System.Windows.Forms.NumericUpDown();
            this.numAccel = new System.Windows.Forms.NumericUpDown();
            this.numDecel = new System.Windows.Forms.NumericUpDown();
            this.numStepsPerMm = new System.Windows.Forms.NumericUpDown();
            this.numJogSteps = new System.Windows.Forms.NumericUpDown();

            ((System.ComponentModel.ISupportInitialize)(this.numMaxSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAccel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDecel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStepsPerMm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numJogSteps)).BeginInit();

            this.numMaxSpeed.Maximum = 640000m;
            this.numMaxSpeed.Value = 320000m;

            this.numAccel.Maximum = 640000m;
            this.numAccel.Value = 320000m;

            this.numDecel.Maximum = 640000m;
            this.numDecel.Value = 320000m;

            this.numStepsPerMm.Maximum = 1280000m;
            this.numStepsPerMm.Value = 100m;

            this.numJogSteps.Maximum = 5000000m;
            this.numJogSteps.Value = 640000m;

            ((System.ComponentModel.ISupportInitialize)(this.numMaxSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAccel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDecel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStepsPerMm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numJogSteps)).EndInit();

            tlpSettings.RowStyles.Clear();
            for (int i = 0; i < 6; i++)
                tlpSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            tlpSettings.Controls.Add(new System.Windows.Forms.Label() { Text = "Max Speed (steps/s)", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
            tlpSettings.Controls.Add(this.numMaxSpeed, 1, 0);

            tlpSettings.Controls.Add(new System.Windows.Forms.Label() { Text = "Accel (steps/s²)", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1);
            tlpSettings.Controls.Add(this.numAccel, 1, 1);

            tlpSettings.Controls.Add(new System.Windows.Forms.Label() { Text = "Decel (steps/s²)", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 2);
            tlpSettings.Controls.Add(this.numDecel, 1, 2);

            tlpSettings.Controls.Add(new System.Windows.Forms.Label() { Text = "Steps / mm", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 3);
            tlpSettings.Controls.Add(this.numStepsPerMm, 1, 3);

            tlpSettings.Controls.Add(new System.Windows.Forms.Label() { Text = "Jog steps", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 4);
            tlpSettings.Controls.Add(this.numJogSteps, 1, 4);

            this.btnApplySettings = new System.Windows.Forms.Button()
            {
                Text = "Apply",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            this.lblSettingsStatus = new System.Windows.Forms.Label()
            {
                Text = "Settings: APPLIED",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true
            };

            tlpSettings.Controls.Add(this.btnApplySettings, 0, 5);
            tlpSettings.Controls.Add(this.lblSettingsStatus, 1, 5);

            settingsScroll.Controls.Add(tlpSettings);
            this.gbSettings.Controls.Add(settingsScroll);

            // Simulation checkbox
            this.chkSimulation = new System.Windows.Forms.CheckBox()
            {
                Text = "Simulation Mode",
                Dock = DockStyle.Left,
                AutoSize = true,
                Margin = new Padding(8)
            };

            // Left column assembly
            var leftCol = new System.Windows.Forms.TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                RowCount = 6,
                ColumnCount = 1
            };

            leftCol.RowStyles.Clear();
            leftCol.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // connection
            leftCol.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));  // drive (slightly reduced)
            leftCol.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // probe
            leftCol.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));  // settings (increased)
            leftCol.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F)); // simulation (small fixed height)
            leftCol.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));  // spacer / flexible

            // Connection group at top of left column
            this.gbConnection = new System.Windows.Forms.GroupBox()
            {
                Text = "Controller Connection",
                Dock = DockStyle.Fill
            };

            this.gbConnection.Padding = new System.Windows.Forms.Padding(6);
            this.gbConnection.Margin = new System.Windows.Forms.Padding(4);
            this.gbConnection.AutoSize = true;
            this.gbConnection.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // Use a FlowLayoutPanel for connection row so controls wrap instead of clipping horizontally
            var tlpConn = new System.Windows.Forms.FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            this.txtIp = new System.Windows.Forms.TextBox()
            {
                Text = "192.168.0.101",
                Width = 220,
                Height = 24,
                Margin = new Padding(6)
            };

            // use smaller button fonts and minimum sizes so layout fits on 1080p
            this.btnConnect = new System.Windows.Forms.Button()
            {
                Text = "Connect",
                AutoSize = true,
                Width = 100,
                Height = 28,
                Margin = new Padding(4,6,4,6),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            this.btnDisconnect = new System.Windows.Forms.Button()
            {
                Text = "Disconnect",
                AutoSize = true,
                Width = 100,
                Height = 28,
                Margin = new Padding(4,6,4,6),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            this.btnEnableMotors = new System.Windows.Forms.Button()
            {
                Text = "Enable",
                AutoSize = true,
                Width = 100,
                Height = 28,
                Margin = new Padding(4,6,4,6),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };

            // Add simulation toggle inline with connection controls (compact)
            this.chkSimulation = new System.Windows.Forms.CheckBox()
            {
                Text = "Simulation",
                AutoSize = true,
                Margin = new Padding(6, 8, 6, 6)
            };

            var pnlInputMode = new System.Windows.Forms.FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            var lblInputModeSel = new System.Windows.Forms.Label()
            {
                Text = "Mode",
                AutoSize = true,
                Margin = new Padding(0, 8, 6, 0)
            };

            this.cmbInputMode = new System.Windows.Forms.ComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 95,
                Margin = new Padding(0, 3, 0, 0)
            };

            this.cmbInputMode.Items.AddRange(new object[] { "Gamepad", "Keyboard" });
            this.cmbInputMode.SelectedIndex = 0;

            pnlInputMode.Controls.Add(lblInputModeSel);
            pnlInputMode.Controls.Add(this.cmbInputMode);
            // Prevent key presses from changing the combo selection after user chooses mode
            this.cmbInputMode.KeyDown += (s, e) => { e.SuppressKeyPress = true; e.Handled = true; };

            // Add controls to flow panel (they will wrap instead of clipping)
            tlpConn.Controls.Add(this.txtIp);
            tlpConn.Controls.Add(this.btnConnect);
            tlpConn.Controls.Add(this.btnDisconnect);
            tlpConn.Controls.Add(this.btnEnableMotors);
            tlpConn.Controls.Add(this.chkSimulation);
            tlpConn.Controls.Add(pnlInputMode);

            this.gbConnection.Controls.Add(tlpConn);

            // Add left column controls
            leftCol.Controls.Add(this.gbConnection, 0, 0);
            leftCol.Controls.Add(this.gbDrive, 0, 1);
            leftCol.Controls.Add(this.gbProbe, 0, 2);
            leftCol.Controls.Add(this.gbSettings, 0, 3);
            // diagnostics/logs (gbCmd) are added later into row 4
            // NOTE: simulation checkbox moved to the right column (between telemetry and console)


            // Right: telemetry and debug
            var rightCol = new System.Windows.Forms.TableLayoutPanel();
            rightCol.Dock = System.Windows.Forms.DockStyle.Fill;
            rightCol.RowCount = 3; rightCol.ColumnCount = 1;
            rightCol.RowStyles.Clear();
            // telemetry (top), diagnostics/logs (middle), console (bottom)
            rightCol.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            rightCol.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            rightCol.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));

            this.gbTelemetry = new System.Windows.Forms.GroupBox() { Text = "Telemetry", Dock = System.Windows.Forms.DockStyle.Fill };
            this.gbTelemetry.Padding = new System.Windows.Forms.Padding(12);
            this.gbTelemetry.Margin = new System.Windows.Forms.Padding(8);
            var tlpTel = new System.Windows.Forms.TableLayoutPanel() { Dock = System.Windows.Forms.DockStyle.Fill, ColumnCount = 1, RowCount = 10 };
            tlpTel.RowStyles.Clear();
            for (int i = 0; i < 10; i++)
                tlpTel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));

            var mono = new System.Drawing.Font("Consolas", 10);
            this.lblForwardInput = new System.Windows.Forms.Label() { Text = "Forward Input: 0.00", Font = mono, Dock = System.Windows.Forms.DockStyle.Fill };
            this.lblTurnInput = new System.Windows.Forms.Label() { Text = "Turn Input: 0.00", Font = mono, Dock = System.Windows.Forms.DockStyle.Fill };
            this.lblProbeInput = new System.Windows.Forms.Label() { Text = "Probe Input: 0.00", Font = mono, Dock = System.Windows.Forms.DockStyle.Fill };
            this.lblMixerLeft = new System.Windows.Forms.Label() { Text = "Mixer Left: 0.00", Font = mono, Dock = System.Windows.Forms.DockStyle.Fill };
            this.lblMixerRight = new System.Windows.Forms.Label() { Text = "Mixer Right: 0.00", Font = mono, Dock = System.Windows.Forms.DockStyle.Fill };
            this.lblLeftVel = new System.Windows.Forms.Label() { Text = "Left Track Velocity: 0 steps/s (0 mm/s)", Font = mono, Dock = System.Windows.Forms.DockStyle.Fill };
            this.lblRightVel = new System.Windows.Forms.Label() { Text = "Right Track Velocity: 0 steps/s (0 mm/s)", Font = mono, Dock = System.Windows.Forms.DockStyle.Fill };
            this.lblProbeVel = new System.Windows.Forms.Label() { Text = "Probe Velocity: 0 steps/s", Font = mono, Dock = System.Windows.Forms.DockStyle.Fill };

            // Live velocity bars
            this.pnlLeftBarBg = new System.Windows.Forms.Panel() { Dock = System.Windows.Forms.DockStyle.Fill, BackColor = System.Drawing.Color.LightGray, Margin = new System.Windows.Forms.Padding(4), Padding = new System.Windows.Forms.Padding(0) };
            this.pnlLeftBarFill = new System.Windows.Forms.Panel() { BackColor = System.Drawing.Color.FromArgb(0x1b, 0xa1, 0x1b), Width = 0, Dock = System.Windows.Forms.DockStyle.Left, Margin = new System.Windows.Forms.Padding(0) };
            this.pnlLeftBarBg.Controls.Add(this.pnlLeftBarFill);

            this.pnlRightBarBg = new System.Windows.Forms.Panel() { Dock = System.Windows.Forms.DockStyle.Fill, BackColor = System.Drawing.Color.LightGray, Margin = new System.Windows.Forms.Padding(4), Padding = new System.Windows.Forms.Padding(0) };
            this.pnlRightBarFill = new System.Windows.Forms.Panel() { BackColor = System.Drawing.Color.FromArgb(0x1b, 0xa1, 0x1b), Width = 0, Dock = System.Windows.Forms.DockStyle.Left, Margin = new System.Windows.Forms.Padding(0) };
            this.pnlRightBarBg.Controls.Add(this.pnlRightBarFill);

            tlpTel.Controls.Add(this.lblForwardInput);
            tlpTel.Controls.Add(this.lblTurnInput);
            tlpTel.Controls.Add(this.lblProbeInput);
            tlpTel.Controls.Add(this.lblMixerLeft);
            tlpTel.Controls.Add(this.lblMixerRight);
            tlpTel.Controls.Add(this.lblLeftVel);
            tlpTel.Controls.Add(this.pnlLeftBarBg);
            tlpTel.Controls.Add(this.lblRightVel);
            tlpTel.Controls.Add(this.pnlRightBarBg);
            tlpTel.Controls.Add(this.lblProbeVel);
            this.gbTelemetry.Controls.Add(tlpTel);

            // Right column: telemetry at top; console will be added below after it is created
            rightCol.Controls.Add(this.gbTelemetry, 0, 0);

            // Add left and right columns into tlpMain so tlpMain owns both panes
            tlpMain.Controls.Add(leftCol, 0, 0);
            tlpMain.Controls.Add(rightCol, 1, 0);

            // Command stream / diagnostics area
            var gbCmd = new System.Windows.Forms.GroupBox() { Text = "Controller Command Stream / Diagnostics", Dock = System.Windows.Forms.DockStyle.Fill };
            gbCmd.Margin = new System.Windows.Forms.Padding(8);
            gbCmd.Padding = new System.Windows.Forms.Padding(12);
            var tlpCmd = new System.Windows.Forms.TableLayoutPanel() { Dock = System.Windows.Forms.DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            tlpCmd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            tlpCmd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));

            this.lstCommandStream = new System.Windows.Forms.ListBox()
            {
                Visible = false
            };

            this.txtLog = new System.Windows.Forms.TextBox()
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                Font = mono,
                ScrollBars = System.Windows.Forms.ScrollBars.Vertical
            };

            tlpCmd.Controls.Add(this.txtLog, 0, 0);

            var tlpButtons = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1
            };

            for (int i = 0; i < 5; i++)
                tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));

            // create buttons (same as before)
            this.btnExportLog = new System.Windows.Forms.Button() { Text = "Export Log", Dock = DockStyle.Fill };
            this.btnCopyLog = new System.Windows.Forms.Button() { Text = "Copy Log", Dock = DockStyle.Fill };
            this.btnClearLog = new System.Windows.Forms.Button() { Text = "Clear Log", Dock = DockStyle.Fill };
            this.btnResumeAutoScroll = new System.Windows.Forms.Button() { Text = "Resume Auto-Scroll", Dock = DockStyle.Fill };
            this.chkPauseScrolling = new System.Windows.Forms.CheckBox() { Text = "Pause Scrolling", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };

            // add them to grid
            tlpButtons.Controls.Add(this.btnExportLog, 0, 0);
            tlpButtons.Controls.Add(this.btnCopyLog, 1, 0);
            tlpButtons.Controls.Add(this.btnClearLog, 2, 0);
            tlpButtons.Controls.Add(this.btnResumeAutoScroll, 3, 0);
            tlpButtons.Controls.Add(this.chkPauseScrolling, 4, 0);

            // plug into layout (same spot as before)
            tlpCmd.Controls.Add(tlpButtons, 0, 1);




            gbCmd.Controls.Add(tlpCmd);

            // Place the simulation checkbox inline is already in the connection flow
            // Place the command stream / diagnostics box into the right column (middle row)
            rightCol.Controls.Add(gbCmd, 0, 1);

            // Galil console (manual commands) - place into right column under telemetry
            var gbConsole = new GroupBox() { Dock = DockStyle.Fill };
            gbConsole.Padding = new System.Windows.Forms.Padding(12);
            gbConsole.Margin = new System.Windows.Forms.Padding(8);
            var tlpConsole = new System.Windows.Forms.TableLayoutPanel() { Dock = System.Windows.Forms.DockStyle.Fill, RowCount = 2, ColumnCount = 2 };
            tlpConsole.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            tlpConsole.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.txtGalilCmd = new System.Windows.Forms.TextBox() { Dock = System.Windows.Forms.DockStyle.Fill };
            this.btnSendGalil = new System.Windows.Forms.Button() { Text = "Send", Dock = System.Windows.Forms.DockStyle.Fill };
            this.lstGalilHistory = new System.Windows.Forms.ListBox() { Dock = System.Windows.Forms.DockStyle.Fill, Font = mono };
            tlpConsole.Controls.Add(this.txtGalilCmd, 0, 0);
            tlpConsole.Controls.Add(this.btnSendGalil, 1, 0);
            tlpConsole.Controls.Add(this.lstGalilHistory, 0, 1);
            gbConsole.Controls.Add(tlpConsole);

            // add console to right column under diagnostics (rightCol defined earlier)
            rightCol.Controls.Add(gbConsole, 0, 2);

            // Put controls into root: header across top, left content (tlpMain) in left column, rightCol in right column
            tlpRoot.Controls.Add(header, 0, 0);
            // header should span both columns
            tlpRoot.SetColumnSpan(header, 2);

            // left main column (tlpMain)
            tlpRoot.Controls.Add(tlpMain, 0, 1);
            this.Controls.Add(tlpRoot);
        }

        #endregion

        private GroupBox gbConnection;
        private TextBox txtIp;
        private Button btnConnect;
        private Button btnDisconnect;
        private Button btnEnableMotors;
        //private Button btnDisableMotors;
        private Label lblControllerStatus;
        private Label lblMotorsStatus;

        private GroupBox gbSettings;
        private NumericUpDown numMaxSpeed;
        private NumericUpDown numAccel;
        private NumericUpDown numDecel;
        private NumericUpDown numStepsPerMm;
        private Button btnApplySettings;
        private Label lblSettingsStatus;

        private GroupBox gbTelemetry;
        private Label lblForwardInput;
        private Label lblTurnInput;
        private Label lblProbeInput;
        private Label lblLeftVel;
        private Label lblRightVel;
        private Label lblMixerLeft;
        private Label lblMixerRight;
        private Label lblProbeVel;
        private Label lblLoopRate;
        private Label lblGamepad;
        private Label lblInputMode;

        private GroupBox gbDrive;
        private Button btnForward;
        private Button btnReverse;
        private Button btnLeft;
        private Button btnRight;
        private Button btnStop;

        private GroupBox gbProbe;
        private NumericUpDown numProbeSpeed;
        private Button btnProbeLeft;
        private Button btnProbeRight;
        private Button btnProbeStop;
        private GroupBox gbCommandStream;
        private System.Windows.Forms.ListBox lstCommandStream;

        private System.Windows.Forms.TextBox txtGalilCmd;
        private System.Windows.Forms.Button btnSendGalil;
        private System.Windows.Forms.ListBox lstGalilHistory;

        // debug signals removed - declarations removed

        private TextBox txtLog;
        private Button btnExportLog;
        private Button btnCopyLog;
        private Button btnClearLog;
        private Button btnResumeAutoScroll;
        private CheckBox chkPauseScrolling;
        private System.Windows.Forms.ComboBox cmbInputMode;
        private System.Windows.Forms.Panel pnlLeftBarBg;
        private System.Windows.Forms.Panel pnlLeftBarFill;
        private System.Windows.Forms.Panel pnlRightBarBg;
        private System.Windows.Forms.Panel pnlRightBarFill;
        private CheckBox chkSimulation;

        private NumericUpDown numJogSteps;
        private Button btnJogForward;
        private Button btnJogReverse;
        private Button btnTurn90Left;
        private Button btnTurn90Right;
        // Layout panels exposed for runtime responsive profile switching
        private System.Windows.Forms.TableLayoutPanel tlpRoot;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
    }
}
