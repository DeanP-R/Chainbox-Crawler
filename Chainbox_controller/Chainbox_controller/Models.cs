namespace Chainbox_controller
{
    public class InputState
    {
        public double Forward { get; set; } = 0.0;
        public double Turn { get; set; } = 0.0;
        public double Probe { get; set; } = 0.0;
    }

    public struct MixerOutput
    {
        public double Left;
        public double Right;
    }
}