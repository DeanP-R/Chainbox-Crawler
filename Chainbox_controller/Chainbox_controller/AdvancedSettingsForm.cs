using System;
using System.Drawing;
using System.Windows.Forms;

namespace Chainbox_controller
{
    public class AdvancedSettingsForm : Form
    {
        public UiSettings Settings { get; private set; }

        private NumericUpDown numLeftMmPerCount;
        private NumericUpDown numRightMmPerCount;
        private NumericUpDown numTrackWidth;
        private CheckBox chkSwapAxes;

        private NumericUpDown numRobotWidth;
        private NumericUpDown numRobotLength;

        private NumericUpDown numProbeWidth;
        private NumericUpDown numProbeForwardOffset;
        private NumericUpDown numProbeLateralOffset;

        private NumericUpDown numGridResolution;

        private Button btnOk;
        private Button btnSave;
        private Button btnCancel;

        public AdvancedSettingsForm(UiSettings settings)
        {
            Settings = CloneSettings(settings);

            InitializeComponent();
            LoadFromSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Advanced Settings";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(520, 500);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(12)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            int row = 0;

            AddSectionHeader(content, "Odometry", ref row);

            numLeftMmPerCount = CreateDecimalBox(0.000001M, 1.0M, 6);
            AddRow(content, "Left mm/count", numLeftMmPerCount, ref row);

            numRightMmPerCount = CreateDecimalBox(0.000001M, 1.0M, 6);
            AddRow(content, "Right mm/count", numRightMmPerCount, ref row);

            numTrackWidth = CreateDecimalBox(1, 10000, 2);
            AddRow(content, "Effective track width (mm)", numTrackWidth, ref row);

            chkSwapAxes = new CheckBox { Text = "Swap A/B axes", AutoSize = true, Anchor = AnchorStyles.Left };
            content.Controls.Add(chkSwapAxes, 1, row);
            row++;

            AddSectionHeader(content, "Robot Geometry", ref row);

            numRobotWidth = CreateDecimalBox(1, 10000, 2);
            AddRow(content, "Robot width (mm)", numRobotWidth, ref row);

            numRobotLength = CreateDecimalBox(1, 10000, 2);
            AddRow(content, "Robot length (mm)", numRobotLength, ref row);

            AddSectionHeader(content, "Probe", ref row);

            numProbeWidth = CreateDecimalBox(1, 10000, 2);
            AddRow(content, "Probe width (mm)", numProbeWidth, ref row);

            numProbeForwardOffset = CreateDecimalBox(-10000, 10000, 2);
            AddRow(content, "Probe forward offset (mm)", numProbeForwardOffset, ref row);

            numProbeLateralOffset = CreateDecimalBox(-10000, 10000, 2);
            AddRow(content, "Probe lateral offset (mm)", numProbeLateralOffset, ref row);

            AddSectionHeader(content, "Map", ref row);

            numGridResolution = CreateDecimalBox(0.1M, 1000M, 2);
            AddRow(content, "Grid resolution (mm)", numGridResolution, ref row);

            scrollPanel.Controls.Add(content);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                WrapContents = false
            };

            btnOk = new Button { Text = "OK", AutoSize = true };
            btnSave = new Button { Text = "Save", AutoSize = true };
            btnCancel = new Button { Text = "Cancel", AutoSize = true };

            btnOk.Click += BtnOk_Click;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttons.Controls.Add(btnOk);
            buttons.Controls.Add(btnSave);
            buttons.Controls.Add(btnCancel);

            root.Controls.Add(scrollPanel, 0, 0);
            root.Controls.Add(buttons, 0, 1);

            this.Controls.Add(root);
        }

        private void LoadFromSettings()
        {
            numLeftMmPerCount.Value = ClampToRange((decimal)Settings.LeftMmPerCount, numLeftMmPerCount);
            numRightMmPerCount.Value = ClampToRange((decimal)Settings.RightMmPerCount, numRightMmPerCount);
            numTrackWidth.Value = ClampToRange((decimal)Settings.EffectiveTrackWidthMm, numTrackWidth);
            chkSwapAxes.Checked = Settings.SwapAxes;

            numRobotWidth.Value = ClampToRange((decimal)Settings.RobotWidthMm, numRobotWidth);
            numRobotLength.Value = ClampToRange((decimal)Settings.RobotLengthMm, numRobotLength);

            numProbeWidth.Value = ClampToRange((decimal)Settings.ProbeWidthMm, numProbeWidth);
            numProbeForwardOffset.Value = ClampToRange((decimal)Settings.ProbeForwardOffsetMm, numProbeForwardOffset);
            numProbeLateralOffset.Value = ClampToRange((decimal)Settings.ProbeLateralOffsetMm, numProbeLateralOffset);

            numGridResolution.Value = ClampToRange((decimal)Settings.GridResolutionMm, numGridResolution);
        }

        private void SaveToSettings()
        {
            Settings.LeftMmPerCount = (double)numLeftMmPerCount.Value;
            Settings.RightMmPerCount = (double)numRightMmPerCount.Value;
            Settings.EffectiveTrackWidthMm = (double)numTrackWidth.Value;
            Settings.SwapAxes = chkSwapAxes.Checked;

            Settings.RobotWidthMm = (double)numRobotWidth.Value;
            Settings.RobotLengthMm = (double)numRobotLength.Value;

            Settings.ProbeWidthMm = (double)numProbeWidth.Value;
            Settings.ProbeForwardOffsetMm = (double)numProbeForwardOffset.Value;
            Settings.ProbeLateralOffsetMm = (double)numProbeLateralOffset.Value;

            Settings.GridResolutionMm = (double)numGridResolution.Value;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            SaveToSettings();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            SaveToSettings();

            try
            {
                string settingsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ui_settings.json");
                string json = System.Text.Json.JsonSerializer.Serialize(
                    Settings,
                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

                System.IO.File.WriteAllText(settingsPath, json);
                MessageBox.Show(this, "Advanced settings saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to save settings: " + ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static NumericUpDown CreateDecimalBox(decimal min, decimal max, int decimals)
        {
            return new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                DecimalPlaces = decimals,
                Increment = decimals >= 4 ? 0.0001M : 0.1M,
                Width = 160,
                Anchor = AnchorStyles.Left
            };
        }

        private static void AddSectionHeader(TableLayoutPanel panel, string text, ref int row)
        {
            var lbl = new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Margin = new Padding(0, 12, 0, 6)
            };

            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(lbl, 0, row);
            panel.SetColumnSpan(lbl, 2);
            row++;
        }

        private static void AddRow(TableLayoutPanel panel, string labelText, Control control, ref int row)
        {
            var lbl = new Label
            {
                Text = labelText,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(0, 6, 8, 6)
            };

            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(lbl, 0, row);
            panel.Controls.Add(control, 1, row);
            row++;
        }

        private static UiSettings CloneSettings(UiSettings src)
        {
            return new UiSettings
            {
                IpAddress = src.IpAddress,
                SimulationMode = src.SimulationMode,
                InputModeIndex = src.InputModeIndex,
                MaxVelocityStepsPerSec = src.MaxVelocityStepsPerSec,
                AccelStepsPerSec2 = src.AccelStepsPerSec2,
                DecelStepsPerSec2 = src.DecelStepsPerSec2,
                StepsPerMm = src.StepsPerMm,
                JogSteps = src.JogSteps,
                UseCustomTurn90Counts = src.UseCustomTurn90Counts,
                Turn90Counts = src.Turn90Counts,
                LeftMmPerCount = src.LeftMmPerCount,
                RightMmPerCount = src.RightMmPerCount,
                EffectiveTrackWidthMm = src.EffectiveTrackWidthMm,
                SwapAxes = src.SwapAxes,
                RobotWidthMm = src.RobotWidthMm,
                RobotLengthMm = src.RobotLengthMm,
                ProbeWidthMm = src.ProbeWidthMm,
                ProbeForwardOffsetMm = src.ProbeForwardOffsetMm,
                ProbeLateralOffsetMm = src.ProbeLateralOffsetMm,
                GridResolutionMm = src.GridResolutionMm
            };
        }

        private static decimal ClampToRange(decimal value, NumericUpDown box)
        {
            if (value < box.Minimum) return box.Minimum;
            if (value > box.Maximum) return box.Maximum;
            return value;
        }
    }
}