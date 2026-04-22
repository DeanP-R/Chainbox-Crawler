using System;
using System.Windows.Forms;
using System.Drawing;

namespace Chainbox_controller
{
    // Minimal placeholder for the CoverageMapControl used by Form1.
    // Replace with full implementation later; this stub prevents build errors
    // and provides a simple visual placeholder in the UI.
    public class CoverageMapControl : UserControl
    {
        private double poseX = 0.0;
        private double poseY = 0.0;
        private double poseTheta = 0.0; // radians
        // store trace in mm coordinates (X mm, Y mm) so changing scale/redraw works
        private readonly System.Collections.Generic.List<System.Drawing.PointF> traceMm = new();

        // pixels-per-mm scale (default 0.5 px/mm). Exposed property so UI can change it.
        private float scale = 0.5f; // pixels per mm
        public float Scale
        {
            get => scale;
            set
            {
                if (value <= 0) return;
                scale = value;
                this.Invalidate();
            }
        }

        // grid spacing in mm for drawing major grid lines and labels
        public float GridSpacingMm { get; set; } = 100f;

        // whether to auto-center map at control center (true) or use absolute coords (false)
        public bool AutoCenter { get; set; } = true;

        // clear trace
        public void ClearTrace()
        {
            traceMm.Clear();
            this.Invalidate();
        }

        public void SetPose(double xMm, double yMm, double thetaRad)
        {
            poseX = xMm;
            poseY = yMm;
            poseTheta = thetaRad;
            // record trace point in mm
            try
            {
                traceMm.Add(new System.Drawing.PointF((float)poseX, (float)poseY));
                if (traceMm.Count > 20000) traceMm.RemoveRange(0, traceMm.Count - 20000);
            }
            catch { }
            this.Invalidate();
        }
        public CoverageMapControl()
        {
            this.BackColor = Color.WhiteSmoke;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.DoubleBuffered = true;
            this.MinimumSize = new Size(100, 100);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var f = new Font("Segoe UI", 9f))
            using (var brush = new SolidBrush(Color.DarkGray))
            {
                // Draw background
                e.Graphics.Clear(this.BackColor);

                var cx = this.ClientRectangle.Width / 2f;
                var cy = this.ClientRectangle.Height / 2f;

                // draw grid
                DrawGrid(e.Graphics, cx, cy);

                // draw trace (convert mm->px)
                if (traceMm.Count > 1)
                {
                    using (var pen = new Pen(Color.Blue, 2))
                    {
                        for (int i = 1; i < traceMm.Count; i++)
                        {
                            var p1 = new PointF(cx + traceMm[i - 1].X * scale, cy - traceMm[i - 1].Y * scale);
                            var p2 = new PointF(cx + traceMm[i].X * scale, cy - traceMm[i].Y * scale);
                            e.Graphics.DrawLine(pen, p1, p2);
                        }
                    }
                }

                // draw robot at current pose
                var robotX = cx + (float)(poseX * scale);
                var robotY = cy - (float)(poseY * scale);
                float rw = Math.Max(12f, 0.08f * Math.Min(this.Width, this.Height));
                float rh = rw * 0.6f; // robot icon size in pixels
                e.Graphics.TranslateTransform(robotX, robotY);
                e.Graphics.RotateTransform((float)(poseTheta * 180.0 / Math.PI));
                using (var b = new SolidBrush(Color.Red))
                using (var p = new Pen(Color.Black, 1))
                {
                    var rect = new RectangleF(-rw/2, -rh/2, rw, rh);
                    e.Graphics.FillRectangle(b, rect);
                    e.Graphics.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
                    // heading arrow
                    var arrow = new PointF[] { new PointF(rw/2, 0), new PointF(rw/2 - 8, -6), new PointF(rw/2 - 8, 6) };
                    e.Graphics.FillPolygon(Brushes.Yellow, arrow);
                    e.Graphics.DrawPolygon(p, arrow);
                }
                e.Graphics.ResetTransform();

                // overlay title and axes labels
                e.Graphics.DrawString("Coverage Map", f, brush, new RectangleF(0, 0, this.Width, 20), sf);
                DrawAxisLabels(e.Graphics, cx, cy);
            }
        }

        private void DrawGrid(Graphics g, float cx, float cy)
        {
            try
            {
                float spacingPx = GridSpacingMm * scale;
                if (spacingPx < 6) return; // too dense

                var rect = this.ClientRectangle;
                using (var pen = new Pen(Color.LightGray, 1))
                using (var penMajor = new Pen(Color.Gray, 1.5f))
                {
                    // vertical lines
                    for (float x = cx % spacingPx; x < rect.Width; x += spacingPx)
                        g.DrawLine(pen, x, rect.Top, x, rect.Bottom);
                    // horizontal lines
                    for (float y = cy % spacingPx; y < rect.Height; y += spacingPx)
                        g.DrawLine(pen, rect.Left, y, rect.Right, y);
                }
            }
            catch { }
        }

        private void DrawAxisLabels(Graphics g, float cx, float cy)
        {
            try
            {
                using (var f = new Font("Segoe UI", 8f))
                using (var brush = new SolidBrush(Color.Black))
                {
                    // draw a few tick labels centered
                    float spacingPx = GridSpacingMm * scale;
                    if (spacingPx < 6) return;

                    // x labels (every grid line +/- 5 lines)
                    for (int i = -5; i <= 5; i++)
                    {
                        float x = cx + i * spacingPx;
                        float labelMm = i * GridSpacingMm;
                        g.DrawString($"{labelMm:0}", f, brush, x - 10, cy + 4);
                    }

                    // y labels
                    for (int i = -5; i <= 5; i++)
                    {
                        float y = cy + i * spacingPx;
                        float labelMm = -i * GridSpacingMm;
                        g.DrawString($"{labelMm:0}", f, brush, cx + 4, y - 8);
                    }
                }
            }
            catch { }
        }
    }
}
