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
            this.Text = "Chainbox Crawler Manual Control";

            // Root layout - vertical: status (fixed), main (fill), diagnostics (fixed)
            this.tlpRoot = new System.Windows.Forms.TableLayoutPanel();

            tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            tlpRoot.RowCount = 3; tlpRoot.ColumnCount = 1;
            tlpRoot.RowStyles.Clear();
            // fixed header height so status bar remains visible on different resolutions
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));      // header
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // main
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 250F)); // diagnostics
            tlpRoot.Padding = new System.Windows.Forms.Padding(8);
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
            // reduced title/status spacing
            headerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));  // title
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
            tlpMain.Dock = System.Windows.Forms.DockStyle.Fill; tlpMain.ColumnCount = 2; tlpMain.RowCount = 1;
            // give more horizontal room to control panels on smaller screens
            tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tlpMain.Padding = new System.Windows.Forms.Padding(8);
            tlpMain.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;

            // Drive controls - full 3x3 grid
            this.gbDrive = new System.Windows.Forms.GroupBox()
            {
                Text = "Drive Controls",
                Dock = System.Windows.Forms.DockStyle.Fill,
                Padding = new System.Windows.Forms.Padding(12),
                BackColor = System.Drawing.Color.FromArgb(0xf4, 0xf4, 0xf4)
            };

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
                Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold),
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnTurn90Left = new System.Windows.Forms.Button()
            {
                Text = "90° LEFT",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular),
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnTurn90Left = new System.Windows.Forms.Button()
            {
                Text = "90° LEFT",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular),
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 40)
            };

            this.btnLeft = new System.Windows.Forms.Button()
            {
                Text = "TURN LEFT",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular),
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnStop = new System.Windows.Forms.Button()
            {
                Text = "STOP",
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.Red,
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold),
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(64, 52)
            };

            this.btnRight = new System.Windows.Forms.Button()
            {
                Text = "TURN RIGHT",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular),
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnTurn90Right = new System.Windows.Forms.Button()
            {
                Text = "90° RIGHT",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular),
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnTurn90Right = new System.Windows.Forms.Button()
            {
                Text = "90° RIGHT",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular),
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 40)
            };

            this.btnReverse = new System.Windows.Forms.Button()
            {
                Text = "REVERSE",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold),
                Margin = new System.Windows.Forms.Padding(6),
                MinimumSize = new System.Drawing.Size(60, 48)
            };

            this.btnJogForward = new System.Windows.Forms.Button()
            {
                Text = "JOG FORWARD",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular),
                Margin = new System.Windows.Forms.Padding(8),
                MinimumSize = new System.Drawing.Size(80, 60)
            };

            this.btnJogReverse = new System.Windows.Forms.Button()
            {
                Text = "JOG REVERSE",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular),
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
                Padding = new System.Windows.Forms.Padding(12),
                BackColor = System.Drawing.Color.FromArgb(0xf4, 0xf4, 0xf4)
            };

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
                Padding = new System.Windows.Forms.Padding(6),
                BackColor = System.Drawing.Color.FromArgb(0xf4, 0xf4, 0xf4)
            };

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

            this.numMaxSpeed.Maximum = 1000000m;
            this.numMaxSpeed.Value = 10000m;

            this.numAccel.Maximum = 100000m;
            this.numAccel.Value = 1000m;

            this.numDecel.Maximum = 100000m;
            this.numDecel.Value = 1000m;

            this.numStepsPerMm.Maximum = 10000m;
            this.numStepsPerMm.Value = 100m;

            this.numJogSteps.Maximum = 1000000m;
            this.numJogSteps.Value = 1000m;

            ((System.ComponentModel.ISupportInitialize)(this.numMaxSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAccel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDecel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStepsPerMm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numJogSteps)).EndInit();

            tlpSettings.RowStyles.Clear();
            for (int i = 0; i < 6; i++)
                tlpSettings.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            tlpSettings.Controls.Add(new System.Windows.Forms.Label() { Text = "Max Speed", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
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
                RowCount = 5,
                ColumnCount = 1
            };

            leftCol.RowStyles.Clear();
            leftCol.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // connection
            leftCol.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));  // drive
            leftCol.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // probe
            leftCol.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));  // settings
            leftCol.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // simulation

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
            tlpConn.Controls.Add(pnlInputMode);

            this.gbConnection.Controls.Add(tlpConn);

            // Add left column controls
            leftCol.Controls.Add(this.gbConnection, 0, 0);
            leftCol.Controls.Add(this.gbDrive, 0, 1);
            leftCol.Controls.Add(this.gbProbe, 0, 2);
            leftCol.Controls.Add(this.gbSettings, 0, 3);
            leftCol.Controls.Add(this.chkSimulation, 0, 4);


            // Right: telemetry and debug
            var rightCol = new System.Windows.Forms.TableLayoutPanel(); rightCol.Dock = System.Windows.Forms.DockStyle.Fill; rightCol.RowCount = 2; rightCol.ColumnCount = 1;
            rightCol.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            rightCol.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));

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
            this.pnlLeftBarBg = new System.Windows.Forms.Panel() { Dock = System.Windows.Forms.DockStyle.Fill, Height = 16, BackColor = System.Drawing.Color.LightGray, Margin = new System.Windows.Forms.Padding(4) };
            this.pnlLeftBarFill = new System.Windows.Forms.Panel() { BackColor = System.Drawing.Color.Green, Width = 0, Height = 16, Dock = System.Windows.Forms.DockStyle.Left };
            this.pnlLeftBarBg.Controls.Add(this.pnlLeftBarFill);

            this.pnlRightBarBg = new System.Windows.Forms.Panel() { Dock = System.Windows.Forms.DockStyle.Fill, Height = 16, BackColor = System.Drawing.Color.LightGray, Margin = new System.Windows.Forms.Padding(4) };
            this.pnlRightBarFill = new System.Windows.Forms.Panel() { BackColor = System.Drawing.Color.Green, Width = 0, Height = 16, Dock = System.Windows.Forms.DockStyle.Left };
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

            // Debug signals box
            this.gbDebug = new System.Windows.Forms.GroupBox() { Text = "Debug Signals", Dock = System.Windows.Forms.DockStyle.Fill };
            this.gbDebug.Padding = new System.Windows.Forms.Padding(12);
            this.gbDebug.Margin = new System.Windows.Forms.Padding(8);
            var tlpDebug = new System.Windows.Forms.TableLayoutPanel() { Dock = System.Windows.Forms.DockStyle.Fill, ColumnCount = 1, RowCount = 8 };
            tlpDebug.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tlpDebug.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tlpDebug.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tlpDebug.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tlpDebug.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tlpDebug.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tlpDebug.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tlpDebug.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

            this.lblDbgInputFwd = new System.Windows.Forms.Label() { Text = "Input Forward: 0.000", Dock = System.Windows.Forms.DockStyle.Fill, Font = mono };
            this.lblDbgInputTurn = new System.Windows.Forms.Label() { Text = "Input Turn: 0.000", Dock = System.Windows.Forms.DockStyle.Fill, Font = mono };
            this.lblDbgInputProbe = new System.Windows.Forms.Label() { Text = "Input Probe: 0.000", Dock = System.Windows.Forms.DockStyle.Fill, Font = mono };
            this.lblDbgMixerLeft = new System.Windows.Forms.Label() { Text = "Mixer Left: 0.000", Dock = System.Windows.Forms.DockStyle.Fill, Font = mono };
            this.lblDbgMixerRight = new System.Windows.Forms.Label() { Text = "Mixer Right: 0.000", Dock = System.Windows.Forms.DockStyle.Fill, Font = mono };
            this.lblDbgCmdLeft = new System.Windows.Forms.Label() { Text = "Cmd Left: 0", Dock = System.Windows.Forms.DockStyle.Fill, Font = mono };
            this.lblDbgCmdRight = new System.Windows.Forms.Label() { Text = "Cmd Right: 0", Dock = System.Windows.Forms.DockStyle.Fill, Font = mono };
            this.lblDbgCmdProbe = new System.Windows.Forms.Label() { Text = "Cmd Probe: 0", Dock = System.Windows.Forms.DockStyle.Fill, Font = mono };

            tlpDebug.Controls.Add(this.lblDbgInputFwd);
            tlpDebug.Controls.Add(this.lblDbgInputTurn);
            tlpDebug.Controls.Add(this.lblDbgInputProbe);
            tlpDebug.Controls.Add(this.lblDbgMixerLeft);
            tlpDebug.Controls.Add(this.lblDbgMixerRight);
            tlpDebug.Controls.Add(this.lblDbgCmdLeft);
            tlpDebug.Controls.Add(this.lblDbgCmdRight);
            tlpDebug.Controls.Add(this.lblDbgCmdProbe);
            this.gbDebug.Controls.Add(tlpDebug);

            rightCol.Controls.Add(this.gbTelemetry, 0, 0);
            rightCol.Controls.Add(this.gbDebug, 0, 1);

            tlpMain.Controls.Add(leftCol, 0, 0);
            tlpMain.Controls.Add(rightCol, 1, 0);

            // Command stream / diagnostics area
            var gbCmd = new System.Windows.Forms.GroupBox() { Text = "Controller Command Stream / Diagnostics", Dock = System.Windows.Forms.DockStyle.Fill };
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

            // Galil console (manual commands)
            var gbConsole = new GroupBox() { Dock = DockStyle.Fill };
            gbConsole.Padding = new System.Windows.Forms.Padding(12);
            gbConsole.Margin = new System.Windows.Forms.Padding(8);
            var tlpConsole = new System.Windows.Forms.TableLayoutPanel() { Dock = System.Windows.Forms.DockStyle.Fill, RowCount = 2, ColumnCount = 2 };
            tlpConsole.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            tlpConsole.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.txtGalilCmd = new System.Windows.Forms.TextBox() { Dock = System.Windows.Forms.DockStyle.Fill };
            this.btnSendGalil = new System.Windows.Forms.Button() { Text = "Send", Dock = System.Windows.Forms.DockStyle.Fill };
            this.lstGalilHistory = new System.Windows.Forms.ListBox() { Dock = System.Windows.Forms.DockStyle.Fill, Font = mono };
            tlpConsole.Controls.Add(this.txtGalilCmd, 0, 0); tlpConsole.Controls.Add(this.btnSendGalil, 1, 0); tlpConsole.Controls.Add(this.lstGalilHistory, 0, 1); gbConsole.Controls.Add(tlpConsole);

            // Put controls into root
            tlpRoot.Controls.Add(header, 0, 0);
            tlpRoot.Controls.Add(tlpMain, 0, 1);
            var bottomRow = new System.Windows.Forms.TableLayoutPanel() { Dock = System.Windows.Forms.DockStyle.Fill, ColumnCount = 2 };
            bottomRow.ColumnStyles.Clear();
            bottomRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            bottomRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            //bottomRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            //bottomRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            bottomRow.Controls.Add(gbCmd, 0, 0);
            bottomRow.Controls.Add(gbConsole, 1, 0);
            tlpRoot.Controls.Add(bottomRow, 0, 2);

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

        private GroupBox gbDebug;
        private Label lblDbgInputFwd;
        private Label lblDbgInputTurn;
        private Label lblDbgInputProbe;
        private Label lblDbgMixerLeft;
        private Label lblDbgMixerRight;
        private Label lblDbgCmdLeft;
        private Label lblDbgCmdRight;
        private Label lblDbgCmdProbe;

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
