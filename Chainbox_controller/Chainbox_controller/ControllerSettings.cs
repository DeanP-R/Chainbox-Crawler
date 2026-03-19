using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chainbox_controller
{
    public class ControllerSettings
    {
        public double MaxVelocityStepsPerSec { get; set; } = 10000;
        public double AccelStepsPerSec2 { get; set; } = 1000;
        public double DecelStepsPerSec2 { get; set; } = 1000;
        public double StepsPerMm { get; set; } = 100;
    }
}