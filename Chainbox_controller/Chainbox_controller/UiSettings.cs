namespace Chainbox_controller
{
    public class UiSettings
    {
        public string IpAddress { get; set; } = "192.168.0.101";
        public bool SimulationMode { get; set; } = false;
        public int InputModeIndex { get; set; } = 0;

        public double MaxVelocityStepsPerSec { get; set; } = 320000.0;
        public double AccelStepsPerSec2 { get; set; } = 320000.0;
        public double DecelStepsPerSec2 { get; set; } = 320000.0;
        public double StepsPerMm { get; set; } = 100.0;
        public double JogSteps { get; set; } = 640000.0;

        public bool UseCustomTurn90Counts { get; set; } = false;
        public double Turn90Counts { get; set; } = 2986068.0;

        public double LeftMmPerCount { get; set; } = 202.0 / 1_280_000.0;
        public double RightMmPerCount { get; set; } = 202.0 / 1_280_000.0;
        public double EffectiveTrackWidthMm { get; set; } = 600.0;
        public bool SwapAxes { get; set; } = false;

        public double RobotWidthMm { get; set; } = 683.0;
        public double RobotLengthMm { get; set; } = 752.0;
        public double ProbeWidthMm { get; set; } = 600.0;
        public double ProbeForwardOffsetMm { get; set; } = 367.0;
        public double ProbeLateralOffsetMm { get; set; } = 0.0;
        public double GridResolutionMm { get; set; } = 1.0;
    }
}