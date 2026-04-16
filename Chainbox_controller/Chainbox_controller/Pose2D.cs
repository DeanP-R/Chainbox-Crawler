using System;

namespace Chainbox_controller
{
    public struct Pose2D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Theta { get; set; } // radians

        public Pose2D(double x, double y, double theta)
        {
            X = x; Y = y; Theta = theta;
        }

        public void NormalizeTheta()
        {
            Theta = (Theta + Math.PI) % (2.0 * Math.PI);
            if (Theta < 0) Theta += 2.0 * Math.PI;
            Theta -= Math.PI;
        }
    }
}