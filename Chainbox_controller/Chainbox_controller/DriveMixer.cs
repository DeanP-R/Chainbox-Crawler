using System;

namespace Chainbox_controller
{
    public class DriveMixer
    {
        public struct MixerOutput
        {
            public double Left;
            public double Right;
        }

        public MixerOutput Mix(double forward, double turn)
        {
            double left = forward + turn;
            double right = forward - turn;

            double maxAbs = Math.Max(Math.Abs(left), Math.Abs(right));
            if (maxAbs > 1.0)
            {
                left /= maxAbs;
                right /= maxAbs;
            }

            return new MixerOutput
            {
                Left = Clamp(left, -1.0, 1.0),
                Right = Clamp(right, -1.0, 1.0)
            };
        }

        private static double Clamp(double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }
    }
}