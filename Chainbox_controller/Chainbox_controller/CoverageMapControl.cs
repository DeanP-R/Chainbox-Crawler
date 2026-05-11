using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Chainbox_controller
{
    public class CoverageMapControl : UserControl
    {
        private CoverageCell[,]? _grid;
        private double _gridXMinMm = -2500.0;
        private double _gridYMinMm = -2500.0;
        private double _cellSizeMm = 10.0;

        private Pose2D _pose = new Pose2D(0, 0, 0);
        private EstimatorParameters? _parameters;

        private readonly List<PointF> _traceMm = new();

        public CoverageMapControl()
        {
            BackColor = Color.WhiteSmoke;
            BorderStyle = BorderStyle.FixedSingle;
            DoubleBuffered = true;
            MinimumSize = new Size(160, 140);

            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.UserPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw, true);
        }

        public void SetPose(double xMm, double yMm, double thetaRad)
        {
            _pose = new Pose2D(xMm, yMm, thetaRad);
            AppendTracePoint((float)xMm, (float)yMm);
            Invalidate();
        }

        public void UpdateMap(
            CoverageCell[,]? grid,
            double gridXMinMm,
            double gridYMinMm,
            double cellSizeMm,
            Pose2D pose,
            EstimatorParameters? parameters = null)
        {
            _grid = grid;
            _gridXMinMm = gridXMinMm;
            _gridYMinMm = gridYMinMm;
            _cellSizeMm = Math.Max(1e-6, cellSizeMm);
            _pose = pose;
            if (parameters != null)
                _parameters = parameters;

            AppendTracePoint((float)pose.X, (float)pose.Y);
            Invalidate();
        }

        private void AppendTracePoint(float xMm, float yMm)
        {
            if (_traceMm.Count == 0)
            {
                _traceMm.Add(new PointF(xMm, yMm));
                return;
            }

            PointF last = _traceMm[_traceMm.Count - 1];
            float dx = xMm - last.X;
            float dy = yMm - last.Y;
            if ((dx * dx + dy * dy) < 0.25f)
                return;

            _traceMm.Add(new PointF(xMm, yMm));
            if (_traceMm.Count > 20000)
                _traceMm.RemoveRange(0, _traceMm.Count - 20000);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(34, 37, 42));

            Rectangle plotRect = new Rectangle(50, 25, Math.Max(10, Width - 70), Math.Max(10, Height - 60));
            using var borderPen = new Pen(Color.Gray, 1f);
            g.DrawRectangle(borderPen, plotRect);

            if (_grid != null)
                DrawGrid(g, plotRect);

            DrawRobot(g, plotRect);
            DrawTitle(g);
        }

        private void DrawGrid(Graphics g, Rectangle plotRect)
        {
            if (_grid == null) return;

            int rows = _grid.GetLength(0);
            int cols = _grid.GetLength(1);

            double xMaxMm = _gridXMinMm + cols * _cellSizeMm;
            double yMaxMm = _gridYMinMm + rows * _cellSizeMm;

            float pxPerMmX = (float)(plotRect.Width / Math.Max(1.0, xMaxMm - _gridXMinMm));
            float pxPerMmY = (float)(plotRect.Height / Math.Max(1.0, yMaxMm - _gridYMinMm));
            float pxPerMm = Math.Min(pxPerMmX, pxPerMmY);

            using var visitedBrush = new SolidBrush(Color.Goldenrod);
            using var scannedBrush = new SolidBrush(Color.ForestGreen);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var cell = _grid[row, col];
                    if (!cell.Visited && !cell.Scanned)
                        continue;

                    float x = plotRect.Left + (float)(col * _cellSizeMm * pxPerMm);
                    float y = plotRect.Bottom - (float)((row + 1) * _cellSizeMm * pxPerMm);
                    float size = Math.Max(1f, (float)(_cellSizeMm * pxPerMm));

                    g.FillRectangle(cell.Scanned ? scannedBrush : visitedBrush, x, y, size, size);
                }
            }
        }

        private void DrawRobot(Graphics g, Rectangle plotRect)
        {
            double robotLengthMm = _parameters?.RobotLengthMm ?? 752.0;
            double robotWidthMm = _parameters?.RobotWidthMm ?? 683.0;

            float cx = plotRect.Left + plotRect.Width / 2f;
            float cy = plotRect.Top + plotRect.Height / 2f;

            float rw = Math.Max(12f, (float)(robotLengthMm * 0.03));
            float rh = Math.Max(10f, (float)(robotWidthMm * 0.03));

            GraphicsState state = g.Save();
            g.TranslateTransform(cx, cy);
            g.RotateTransform((float)(-_pose.Theta * 180.0 / Math.PI));

            using var fill = new SolidBrush(Color.Red);
            using var outline = new Pen(Color.Black, 1.2f);

            RectangleF rect = new RectangleF(-rw / 2f, -rh / 2f, rw, rh);
            g.FillRectangle(fill, rect);
            g.DrawRectangle(outline, rect.X, rect.Y, rect.Width, rect.Height);

            PointF[] arrow =
            {
                new PointF(rw / 2f + 8f, 0f),
                new PointF(rw / 2f - 4f, -5f),
                new PointF(rw / 2f - 4f, 5f)
            };
            g.FillPolygon(Brushes.Yellow, arrow);
            g.DrawPolygon(outline, arrow);

            g.Restore(state);
        }

        private void DrawTitle(Graphics g)
        {
            using var font = new Font("Segoe UI", 9f, FontStyle.Bold);
            using var brush = new SolidBrush(Color.White);
            g.DrawString("Coverage Map", font, brush, new PointF(10, 5));
        }
    }
}