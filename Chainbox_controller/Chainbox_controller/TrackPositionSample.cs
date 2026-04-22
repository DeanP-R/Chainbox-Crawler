using System;

namespace Chainbox_controller
{
    public class TrackPositionSample
    {
        public DateTime TimestampUtc { get; set; }
        public long LeftCounts { get; set; }
        public long RightCounts { get; set; }
        public long ProbeCounts { get; set; }

        public TrackPositionSample() { TimestampUtc = DateTime.UtcNow; }
    }
}