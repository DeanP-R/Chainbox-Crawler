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
            var tlpRoot = new System.Windows.Forms.TableLayoutPanel();

            tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            tlpRoot.RowCount = 3; tlpRoot.ColumnCount = 1;
            tlpRoot.RowStyles.Clear();
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // header
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // main
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 250F)); // diagnostics
            tlpRoot.Padding = new System.Windows.Forms.Padding(12);
            var tlpStatus = new System.Windows.Forms.TableLayoutPanel();

            // Status bar (dark header)
            var header = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(0x2b, 0x2b, 0x2b),
                Padding = new Padding(8, 2, 8, 2)
            };

            var lblTitle = new Label()
            {
                Text = "CHAINBOX CRAWLER MANUAL CONTROL",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };

            var headerLayout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };

            headerLayout.RowStyles.Clear();
            headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));  // title
            headerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));  // status row

            headerLayout.Controls.Add(lblTitle, 0, 0);
            headerLayout.Controls.Add(tlpStatus, 0, 1);

            header.Controls.Clear();
            header.Controls.Add(headerLayout); 


            tlpStatus.Dock = System.Windows.Forms.DockStyle.Fill; 

            tlpStatus.RowCount = 1;
            tlpStatus.ColumnCount = 4;
            tlpStatus.ColumnStyles.Clear();
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpStatus.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Controls - keep names used elsewhere to avoid larger refactor
            var headerFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblControllerStatus = new System.Windows.Forms.Label() { Text = "Controller: DISCONNECTED", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ForeColor = System.Drawing.Color.FromArgb(0xd4,0xd4,0xd4), Font = headerFont };
            this.lblMotorsStatus = new System.Windows.Forms.Label() { Text = "Motors: DISABLED", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ForeColor = System.Drawing.Color.Orange, Font = headerFont };
            this.lblGamepad = new System.Windows.Forms.Label() { Text = "Gamepad: DISCONNECTED", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ForeColor = System.Drawing.Color.FromArgb(0xd4,0xd4,0xd4), Font = headerFont };
            this.lblLoopRate = new System.Windows.Forms.Label() { Text = "Loop Rate: 0.0 Hz", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft, ForeColor = System.Drawing.Color.FromArgb(0xd4,0xd4,0xd4), Font = headerFont };
            this.lblInputMode = new System.Windows.Forms.Label() { Text = "Input Mode: AUTOMATIC", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleRight, ForeColor = System.Drawing.Color.FromArgb(0xd4,0xd4,0xd4), Font = headerFont };

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
            var tlpMain = new System.Windows.Forms.TableLayoutPanel();
            tlpMain.Dock = System.Windows.Forms.DockStyle.Fill; tlpMain.ColumnCount = 2; tlpMain.RowCount = 1;
            tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            tlpMain.Padding = new System.Windows.Forms.Padding(12);
            tlpMain.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;

            // Left: control panel - groupbox with drive + probe + settings
            this.gbDrive = new System.Windows.Forms.GroupBox() { 
                Text = "Drive Controls", 
                Dock = System.Windows.Forms.DockStyle.Fill, 
                Padding = new System.Windows.Forms.Padding(12), 
                BackColor = System.Drawing.Color.FromArgb(0xf4,0xf4,0xf4) 
            };
            this.gbDrive.Dock = DockStyle.Fill;
            var tlpDrive = new System.Windows.Forms.TableLayoutPanel(); 
            tlpDrive.Dock = System.Windows.Forms.DockStyle.Fill; 
            tlpDrive.RowCount = 3; 
            tlpDrive.ColumnCount = 3;
            tlpDrive.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            tlpDrive.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34F));
            tlpDrive.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            tlpDrive.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            tlpDrive.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            tlpDrive.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));

            this.btnForward = new System.Windows.Forms.Button() { Text = "FORWARD", Dock = System.Windows.Forms.DockStyle.Fill, Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold), Margin = new System.Windows.Forms.Padding(8), MinimumSize = new Size(10, 6) };
            this.btnLeft = new System.Windows.Forms.Button() { Text = "TURN LEFT", Dock = System.Windows.Forms.DockStyle.Fill, Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular), Margin = new System.Windows.Forms.Padding(8), MinimumSize = new Size(10, 6) };
            this.btnStop = new System.Windows.Forms.Button() { Text = "STOP", Dock = System.Windows.Forms.DockStyle.Fill, BackColor = System.Drawing.Color.Red, ForeColor = System.Drawing.Color.White, Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold), Margin = new System.Windows.Forms.Padding(8), MinimumSize = new Size(10, 20) };
            this.btnRight = new System.Windows.Forms.Button() { Text = "TURN RIGHT", Dock = System.Windows.Forms.DockStyle.Fill, Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular), Margin = new System.Windows.Forms.Padding(8), MinimumSize = new Size(10, 6) };
            this.btnReverse = new System.Windows.Forms.Button() { Text = "REVERSE", Dock = System.Windows.Forms.DockStyle.Fill, Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold), Margin = new System.Windows.Forms.Padding(8), MinimumSize = new Size(10, 6) };

            tlpDrive.RowStyles.Clear();
            tlpDrive.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            tlpDrive.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            tlpDrive.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            tlpDrive.Controls.Add(this.btnForward, 1, 0);
            tlpDrive.Controls.Add(this.btnLeft, 0, 1);
            tlpDrive.Controls.Add(this.btnStop, 1, 1);
            tlpDrive.Controls.Add(this.btnRight, 2, 1);
            tlpDrive.Controls.Add(this.btnReverse, 1, 2);

            // Probe controls below drive
            var gbProbe = new System.Windows.Forms.GroupBox() { Text = "Probe Controls", Dock = System.Windows.Forms.DockStyle.Fill, Padding = new System.Windows.Forms.Padding(12), BackColor = System.Drawing.Color.FromArgb(0xf4,0xf4,0xf4) };
            var tlpProbe = new System.Windows.Forms.TableLayoutPanel() { Dock = System.Windows.Forms.DockStyle.Fill, ColumnCount = 3, RowCount = 1 };
            tlpProbe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpProbe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tlpProbe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            this.btnProbeLeft = new System.Windows.Forms.Button() { Text = "Move Left", Dock = System.Windows.Forms.DockStyle.Fill };
            this.btnProbeStop = new System.Windows.Forms.Button() { Text = "Stop", Dock = System.Windows.Forms.DockStyle.Fill };
            this.btnProbeRight = new System.Windows.Forms.Button() { Text = "Move Right", Dock = System.Windows.Forms.DockStyle.Fill };
            this.numProbeSpeed = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numProbeSpeed)).BeginInit();
            this.numProbeSpeed.Maximum = 1000000m;
            this.numProbeSpeed.Value = 20000m;
            ((System.ComponentModel.ISupportInitialize)(this.numProbeSpeed)).EndInit();

            tlpProbe.Controls.Add(this.btnProbeLeft, 0, 0);
            tlpProbe.Controls.Add(this.btnProbeStop, 1, 0);
            tlpProbe.Controls.Add(this.btnProbeRight, 2, 0);
            gbProbe.Controls.Add(tlpProbe);

            // Settings panel (compact)
            var gbSettings = new System.Windows.Forms.GroupBox() { Text = "Drive Settings", Dock = System.Windows.Forms.DockStyle.Fill, Padding = new System.Windows.Forms.Padding(6), BackColor = System.Drawing.Color.FromArgb(0xf4, 0xf4, 0xf4) };
            var settingsScroll = new System.Windows.Forms.Panel() { Dock = System.Windows.Forms.DockStyle.Fill, AutoScroll = true };
            var tlpSettings = new System.Windows.Forms.TableLayoutPanel() { AutoSize = true, AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink, ColumnCount = 2, RowCount = 4 };
            tlpSettings.ColumnStyles.Clear();
            tlpSettings.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     // labels
            tlpSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // inputs

            this.numMaxSpeed = new System.Windows.Forms.NumericUpDown();
            this.numAccel = new System.Windows.Forms.NumericUpDown();
            this.numDecel = new System.Windows.Forms.NumericUpDown();
            this.numStepsPerMm = new System.Windows.Forms.NumericUpDown();

            ((System.ComponentModel.ISupportInitialize)(this.numMaxSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAccel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDecel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStepsPerMm)).BeginInit();

            // Set Maximum before Value to avoid ArgumentOutOfRangeException
            this.numMaxSpeed.Maximum = 1000000m; this.numMaxSpeed.Value = 10000m;
            this.numAccel.Maximum = 100000m; this.numAccel.Value = 1000m;
            this.numDecel.Maximum = 100000m; this.numDecel.Value = 1000m;
            this.numStepsPerMm.Maximum = 10000m; this.numStepsPerMm.Value = 100m;

            ((System.ComponentModel.ISupportInitialize)(this.numMaxSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAccel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDecel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStepsPerMm)).EndInit();

            tlpSettings.RowCount = 5;
            tlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            tlpSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

            tlpSettings.Controls.Add(new System.Windows.Forms.Label() { Text = "Max Speed (steps/s)", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 0);
            tlpSettings.Controls.Add(this.numMaxSpeed, 1, 0);
            tlpSettings.Controls.Add(new System.Windows.Forms.Label() { Text = "Accel (steps/s²)", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 1);
            tlpSettings.Controls.Add(this.numAccel, 1, 1);
            tlpSettings.Controls.Add(new System.Windows.Forms.Label() { Text = "Decel (steps/s²)", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 2);
            tlpSettings.Controls.Add(this.numDecel, 1, 2);
            tlpSettings.Controls.Add(new System.Windows.Forms.Label() { Text = "Steps / mm", Anchor = System.Windows.Forms.AnchorStyles.Left }, 0, 3);
            tlpSettings.Controls.Add(this.numStepsPerMm, 1, 3);

            this.btnApplySettings = new System.Windows.Forms.Button() { Text = "Apply Settings", Dock = System.Windows.Forms.DockStyle.Right };
            // status indicator for settings
            this.lblSettingsStatus = new System.Windows.Forms.Label() { Text = "Settings: APPLIED", Dock = System.Windows.Forms.DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };

            tlpSettings.Controls.Add(this.btnApplySettings, 0, 4);
            tlpSettings.Controls.Add(this.lblSettingsStatus, 1, 4);
            settingsScroll.Controls.Add(tlpSettings);
            gbSettings.Controls.Add(settingsScroll);

            // Simulation checkbox
            this.chkSimulation = new System.Windows.Forms.CheckBox() { Text = "Simulation Mode", Dock = System.Windows.Forms.DockStyle.Left, AutoSize = true };
            this.chkSimulation.Margin = new System.Windows.Forms.Padding(8);

            // Assemble left column (connection + drive + probe + settings + simulation)
            var leftCol = new System.Windows.Forms.TableLayoutPanel(); leftCol.Dock = System.Windows.Forms.DockStyle.Fill; leftCol.RowCount = 5; leftCol.ColumnCount = 1;
            leftCol.RowStyles.Clear();
            leftCol.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // connection
            leftCol.RowStyles.Add(new RowStyle(SizeType.Percent, 60F)); // drive
            leftCol.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // probe
            leftCol.RowStyles.Add(new RowStyle(SizeType.Percent, 40F)); // settings
            leftCol.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // simulation

            // Connection group at top of left column
            var gbConnection = new System.Windows.Forms.GroupBox() { 
                Text = "Controller Connection", 
                Dock = System.Windows.Forms.DockStyle.Fill 
            };
            gbConnection.AutoSize = true;
            gbConnection.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gbConnection.Padding = new System.Windows.Forms.Padding(6);
            gbConnection.Margin = new System.Windows.Forms.Padding(4);

            var tlpConn = new TableLayoutPanel()
            {
                Dock = DockStyle.Top,  
                AutoSize = true,
                ColumnCount = 4,
                RowCount = 1
            };
            tlpConn.RowStyles.Clear();
            tlpConn.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpConn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tlpConn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            tlpConn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17F));
            tlpConn.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17F));

            this.txtIp = new System.Windows.Forms.TextBox() { Text = "192.168.0.101", Dock = System.Windows.Forms.DockStyle.Fill };
            this.btnConnect = new System.Windows.Forms.Button() { Text = "Connect", Dock = System.Windows.Forms.DockStyle.Fill };
            this.btnDisconnect = new System.Windows.Forms.Button() { Text = "Disconnect", Dock = System.Windows.Forms.DockStyle.Fill };
            this.btnEnableMotors = new System.Windows.Forms.Button() { Text = "Enable", Dock = System.Windows.Forms.DockStyle.Fill };
            this.btnDisableMotors = new System.Windows.Forms.Button() { Text = "Disable", Dock = System.Windows.Forms.DockStyle.Fill };

            if (this.txtIp != null) tlpConn.Controls.Add(this.txtIp, 0, 0);
            if (this.btnConnect != null) tlpConn.Controls.Add(this.btnConnect, 1, 0);
            if (this.btnDisconnect != null) tlpConn.Controls.Add(this.btnDisconnect, 2, 0);
            if (this.btnEnableMotors != null) tlpConn.Controls.Add(this.btnEnableMotors, 3, 0);
            gbConnection.Controls.Add(tlpConn);

            leftCol.Controls.Add(gbConnection, 0, 0);

            // Build drive layout FIRST
            var tlpDriveMain = new System.Windows.Forms.TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };

            tlpDriveMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpDriveMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            tlpDriveMain.Controls.Add(tlpDrive, 0, 0);

            // Input mode row
            var flInputMode = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true
            };

            var lblInputModeSel = new Label()
            {
                Text = "Input Mode:",
                AutoSize = true
            };

            this.cmbInputMode = new ComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 140
            };

            this.cmbInputMode.Items.AddRange(new object[] { "Gamepad", "Keyboard" });
            this.cmbInputMode.SelectedIndex = 0;

            flInputMode.Controls.Add(lblInputModeSel);
            flInputMode.Controls.Add(this.cmbInputMode);

            tlpDriveMain.Controls.Add(flInputMode, 0, 1);

            // attach to groupbox
            this.gbDrive.Controls.Add(tlpDriveMain);

            // add gbDrive to layout
            leftCol.Controls.Add(this.gbDrive, 0, 1);

            leftCol.Controls.Add(gbProbe, 0, 2);
            leftCol.Controls.Add(gbSettings, 0, 3);
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

            this.lstCommandStream = new System.Windows.Forms.ListBox() { Dock = System.Windows.Forms.DockStyle.Fill, Font = mono };
            tlpCmd.Controls.Add(this.lstCommandStream, 0, 0);
            this.txtLog = new System.Windows.Forms.TextBox() { Dock = System.Windows.Forms.DockStyle.Fill, Multiline = true, ReadOnly = true, Font = mono, ScrollBars = System.Windows.Forms.ScrollBars.Vertical };



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
        private Button btnDisableMotors;
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
    }
}
