namespace Chainbox_controller
{
    public class DriveOutput
    {
        public double Left { get; set; }
        public double Right { get; set; }
    }

    public class DriveMixer
    {
        // Mix forward and turn into left/right track commands.
        // Inputs are expected in range -1..1. Outputs also in -1..1.

        public DriveOutput Mix(double forward, double turn)
        {
            var outp = new DriveOutput();
            outp.Left = forward + turn;
            outp.Right = forward - turn;

            // normalize if outside range
            double max = System.Math.Max(System.Math.Abs(outp.Left), System.Math.Abs(outp.Right));
            if (max > 1.0)
            {
                outp.Left /= max;
                outp.Right /= max;
            }

            return outp;
        }
    }
}
