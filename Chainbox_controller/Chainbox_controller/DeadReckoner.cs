using System;

namespace Chainbox_controller
{
    // Simple differential-drive dead-reckoner integrating left/right linear velocities
    // into a planar pose (x, y, theta). Velocities are expected in mm/s.
    public class DeadReckoner
    {
        public double X { get; private set; } = 0.0; // mm
        public double Y { get; private set; } = 0.0; // mm
        // radians
        public double Theta { get; private set; } = 0.0;

        public void Reset(double x = 0, double y = 0, double thetaRad = 0)
        {
            X = x; Y = y; Theta = thetaRad;
        }

        // Update using left and right linear velocities (mm/s) and timestep dt (s)
        public void Update(double vLeftMmPerSec, double vRightMmPerSec, double trackWidthMm, double dt)
        {
            if (dt <= 0) return;

            // Linear and angular velocity
            double v = 0.5 * (vLeftMmPerSec + vRightMmPerSec);
            double omega = 0.0;
            if (Math.Abs(trackWidthMm) > 1e-6)
                omega = (vRightMmPerSec - vLeftMmPerSec) / trackWidthMm; // rad/s

            // Integrate using simple first-order Euler method with midpoint orientation
            double dtheta = omega * dt;
            double thetaMid = Theta + dtheta * 0.5;
            double dx = v * Math.Cos(thetaMid) * dt;
            double dy = v * Math.Sin(thetaMid) * dt;

            X += dx;
            Y += dy;
            Theta = NormalizeAngle(Theta + dtheta);
        }

        // Integrate using absolute wheel displacements (mm)
        public void IntegrateDisplacement(double dLeftMm, double dRightMm, double trackWidthMm)
        {
            double dCenter = 0.5 * (dLeftMm + dRightMm);
            double dTheta = 0.0;
            if (Math.Abs(trackWidthMm) > 1e-6)
                dTheta = (dRightMm - dLeftMm) / trackWidthMm;

            double thetaMid = Theta + dTheta * 0.5;
            double dx = dCenter * Math.Cos(thetaMid);
            double dy = dCenter * Math.Sin(thetaMid);

            X += dx;
            Y += dy;
            Theta = NormalizeAngle(Theta + dTheta);
        }

        private static double NormalizeAngle(double a)
        {
            while (a <= -Math.PI) a += 2 * Math.PI;
            while (a > Math.PI) a -= 2 * Math.PI;
            return a;
        }
    }
}
