using System;

namespace Chainbox_controller
{
    public class DeadReckoner
    {
        public double X { get; private set; } = 0.0;
        public double Y { get; private set; } = 0.0;
        public double Theta { get; private set; } = 0.0;

        public Pose2D CurrentPose => new Pose2D(X, Y, Theta);

        public CoverageCell[,] Grid { get; private set; }

        public double GridXMinMm { get; private set; }
        public double GridYMinMm { get; private set; }
        public double CellSizeMm { get; private set; }

        public int GridRows => Grid.GetLength(0);
        public int GridCols => Grid.GetLength(1);

        private readonly double _initialGridWidthMm;
        private readonly double _initialGridHeightMm;
        private readonly int _expandMarginCells;

        public DeadReckoner(
            double initialGridWidthMm = 5000.0,
            double initialGridHeightMm = 5000.0,
            double cellSizeMm = 1.0,
            int expandMarginCells = 50)
        {
            _initialGridWidthMm = Math.Max(100.0, initialGridWidthMm);
            _initialGridHeightMm = Math.Max(100.0, initialGridHeightMm);
            CellSizeMm = Math.Max(0.1, cellSizeMm);
            _expandMarginCells = Math.Max(1, expandMarginCells);

            int cols = Math.Max(1, (int)Math.Ceiling(_initialGridWidthMm / CellSizeMm));
            int rows = Math.Max(1, (int)Math.Ceiling(_initialGridHeightMm / CellSizeMm));

            Grid = new CoverageCell[rows, cols];
            GridXMinMm = -0.5 * cols * CellSizeMm;
            GridYMinMm = -0.5 * rows * CellSizeMm;
        }

        public void Reset(double x = 0.0, double y = 0.0, double thetaRad = 0.0)
        {
            X = x;
            Y = y;
            Theta = NormalizeAngle(thetaRad);

            int cols = Math.Max(1, (int)Math.Ceiling(_initialGridWidthMm / CellSizeMm));
            int rows = Math.Max(1, (int)Math.Ceiling(_initialGridHeightMm / CellSizeMm));
            Grid = new CoverageCell[rows, cols];
            GridXMinMm = -0.5 * cols * CellSizeMm;
            GridYMinMm = -0.5 * rows * CellSizeMm;
        }

        public void IntegrateDisplacement(double dLeftMm, double dRightMm, double trackWidthMm)
        {
            double dCenter = 0.5 * (dLeftMm + dRightMm);
            double dTheta = 0.0;

            if (Math.Abs(trackWidthMm) > 1e-9)
                dTheta = (dRightMm - dLeftMm) / trackWidthMm;

            double thetaMid = Theta + 0.5 * dTheta;
            double dx = dCenter * Math.Cos(thetaMid);
            double dy = dCenter * Math.Sin(thetaMid);

            X += dx;
            Y += dy;
            Theta = NormalizeAngle(Theta + dTheta);
        }

        public void IntegrateAndPaint(
            double dLeftMm,
            double dRightMm,
            EstimatorParameters parameters,
            bool scanEnabled)
        {
            double startX = X;
            double startY = Y;
            double startTheta = Theta;

            IntegrateDisplacement(dLeftMm, dRightMm, parameters.EffectiveTrackWidthMm);

            PaintSweptRobotFootprint(
                startX, startY, startTheta,
                X, Y, Theta,
                parameters);

            if (scanEnabled)
            {
                PaintSweptProbeFootprint(
                    startX, startY, startTheta,
                    X, Y, Theta,
                    parameters);
            }
        }

        private void PaintSweptRobotFootprint(
            double x0, double y0, double t0,
            double x1, double y1, double t1,
            EstimatorParameters p)
        {
            double dx = x1 - x0;
            double dy = y1 - y0;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            double robotRadiusMm = 0.5 * Math.Max(p.RobotWidthMm, p.RobotLengthMm);
            double sampleSpacingMm = Math.Max(0.5 * CellSizeMm, 0.25 * robotRadiusMm);

            int samples = Math.Max(1, (int)Math.Ceiling(dist / Math.Max(1.0, sampleSpacingMm)));

            for (int i = 0; i <= samples; i++)
            {
                double u = (double)i / samples;
                double x = Lerp(x0, x1, u);
                double y = Lerp(y0, y1, u);
                StampCircleVisited(x, y, robotRadiusMm);
            }
        }

        private void PaintSweptProbeFootprint(
            double x0, double y0, double t0,
            double x1, double y1, double t1,
            EstimatorParameters p)
        {
            GetProbeWorldPose(x0, y0, t0, p, out double px0, out double py0);
            GetProbeWorldPose(x1, y1, t1, p, out double px1, out double py1);

            double dx = px1 - px0;
            double dy = py1 - py0;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            double probeRadiusMm = 0.5 * p.ProbeWidthMm;
            double sampleSpacingMm = Math.Max(0.5 * CellSizeMm, 0.2 * probeRadiusMm);

            int samples = Math.Max(1, (int)Math.Ceiling(dist / Math.Max(1.0, sampleSpacingMm)));

            for (int i = 0; i <= samples; i++)
            {
                double u = (double)i / samples;
                double x = Lerp(px0, px1, u);
                double y = Lerp(py0, py1, u);
                StampCircleScanned(x, y, probeRadiusMm);
            }
        }

        private static void GetProbeWorldPose(
            double robotX, double robotY, double robotTheta,
            EstimatorParameters p,
            out double probeX, out double probeY)
        {
            double c = Math.Cos(robotTheta);
            double s = Math.Sin(robotTheta);

            probeX = robotX + p.ProbeForwardOffsetMm * c - p.ProbeLateralOffsetMm * s;
            probeY = robotY + p.ProbeForwardOffsetMm * s + p.ProbeLateralOffsetMm * c;
        }

        private void StampCircleVisited(double cxMm, double cyMm, double radiusMm)
        {
            EnsureBoundsForCircle(cxMm, cyMm, radiusMm);

            int minCol = WorldToCol(cxMm - radiusMm);
            int maxCol = WorldToCol(cxMm + radiusMm);
            int minRow = WorldToRow(cyMm - radiusMm);
            int maxRow = WorldToRow(cyMm + radiusMm);

            double r2 = radiusMm * radiusMm;

            for (int row = minRow; row <= maxRow; row++)
            {
                if (row < 0 || row >= GridRows) continue;

                for (int col = minCol; col <= maxCol; col++)
                {
                    if (col < 0 || col >= GridCols) continue;

                    GetCellCenterMm(row, col, out double xCell, out double yCell);

                    double dx = xCell - cxMm;
                    double dy = yCell - cyMm;
                    if (dx * dx + dy * dy <= r2)
                    {
                        var cell = Grid[row, col];
                        cell.Visited = true;
                        Grid[row, col] = cell;
                    }
                }
            }
        }

        private void StampCircleScanned(double cxMm, double cyMm, double radiusMm)
        {
            EnsureBoundsForCircle(cxMm, cyMm, radiusMm);

            int minCol = WorldToCol(cxMm - radiusMm);
            int maxCol = WorldToCol(cxMm + radiusMm);
            int minRow = WorldToRow(cyMm - radiusMm);
            int maxRow = WorldToRow(cyMm + radiusMm);

            double r2 = radiusMm * radiusMm;

            for (int row = minRow; row <= maxRow; row++)
            {
                if (row < 0 || row >= GridRows) continue;

                for (int col = minCol; col <= maxCol; col++)
                {
                    if (col < 0 || col >= GridCols) continue;

                    GetCellCenterMm(row, col, out double xCell, out double yCell);

                    double dx = xCell - cxMm;
                    double dy = yCell - cyMm;
                    if (dx * dx + dy * dy <= r2)
                    {
                        var cell = Grid[row, col];
                        cell.Visited = true;
                        cell.Scanned = true;
                        Grid[row, col] = cell;
                    }
                }
            }
        }

        private void EnsureBoundsForCircle(double cxMm, double cyMm, double radiusMm)
        {
            double minX = cxMm - radiusMm;
            double maxX = cxMm + radiusMm;
            double minY = cyMm - radiusMm;
            double maxY = cyMm + radiusMm;

            bool needsExpand =
                minX < GridXMinMm ||
                minY < GridYMinMm ||
                maxX > GridXMinMm + GridCols * CellSizeMm ||
                maxY > GridYMinMm + GridRows * CellSizeMm;

            if (!needsExpand)
                return;

            ExpandToInclude(minX, maxX, minY, maxY);
        }

        private void ExpandToInclude(double minX, double maxX, double minY, double maxY)
        {
            int addLeft = 0;
            int addRight = 0;
            int addBottom = 0;
            int addTop = 0;

            double currentXMax = GridXMinMm + GridCols * CellSizeMm;
            double currentYMax = GridYMinMm + GridRows * CellSizeMm;

            if (minX < GridXMinMm)
                addLeft = (int)Math.Ceiling((GridXMinMm - minX) / CellSizeMm) + _expandMarginCells;

            if (maxX > currentXMax)
                addRight = (int)Math.Ceiling((maxX - currentXMax) / CellSizeMm) + _expandMarginCells;

            if (minY < GridYMinMm)
                addBottom = (int)Math.Ceiling((GridYMinMm - minY) / CellSizeMm) + _expandMarginCells;

            if (maxY > currentYMax)
                addTop = (int)Math.Ceiling((maxY - currentYMax) / CellSizeMm) + _expandMarginCells;

            int newRows = GridRows + addBottom + addTop;
            int newCols = GridCols + addLeft + addRight;

            var newGrid = new CoverageCell[newRows, newCols];

            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridCols; col++)
                {
                    newGrid[row + addBottom, col + addLeft] = Grid[row, col];
                }
            }

            Grid = newGrid;
            GridXMinMm -= addLeft * CellSizeMm;
            GridYMinMm -= addBottom * CellSizeMm;
        }

        private int WorldToCol(double xMm)
        {
            return (int)Math.Floor((xMm - GridXMinMm) / CellSizeMm);
        }

        private int WorldToRow(double yMm)
        {
            return (int)Math.Floor((yMm - GridYMinMm) / CellSizeMm);
        }

        private void GetCellCenterMm(int row, int col, out double xMm, out double yMm)
        {
            xMm = GridXMinMm + (col + 0.5) * CellSizeMm;
            yMm = GridYMinMm + (row + 0.5) * CellSizeMm;
        }

        private static double NormalizeAngle(double a)
        {
            while (a <= -Math.PI) a += 2.0 * Math.PI;
            while (a > Math.PI) a -= 2.0 * Math.PI;
            return a;
        }

        private static double Lerp(double a, double b, double u)
        {
            return a + (b - a) * u;
        }
    }
}