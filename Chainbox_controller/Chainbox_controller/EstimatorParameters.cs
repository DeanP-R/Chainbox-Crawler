namespace Chainbox_controller
{
    /// <summary>
    /// Parameters for odometry and coverage estimation.
    ///
    /// All units are in millimetres unless otherwise stated.
    /// Robot coordinate frame:
    ///   - Origin at centre of crawler (midpoint between tracks)
    ///   - +X forward
    ///   - +Y to the left
    /// </summary>
    public class EstimatorParameters
    {
        // ─────────────────────────────────────────────────────
        // ODOMETRY (CRITICAL - MUST BE CALIBRATED)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Distance travelled per count (left track) [mm/count]
        /// This should be calibrated, not just computed.
        /// </summary>
        public double LeftMmPerCount { get; set; } = 202.0 / 1_280_000.0;

        /// <summary>
        /// Distance travelled per count (right track) [mm/count]
        /// This should be calibrated, not just computed.
        /// </summary>
        public double RightMmPerCount { get; set; } = 202.0 / 1_280_000.0;

        /// <summary>
        /// Effective track width [mm]
        /// This is NOT the physical outer width. Must be tuned.
        /// </summary>
        public double EffectiveTrackWidthMm { get; set; } = 600.0;


        // ─────────────────────────────────────────────────────
        // ROBOT GEOMETRY (FOR VISUALISATION / COVERAGE ONLY)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Total crawler width (outer track to outer track) [mm]
        /// </summary>
        public double RobotWidthMm { get; set; } = 683.0;

        /// <summary>
        /// Total crawler length (rear to front of probe) [mm]
        /// </summary>
        public double RobotLengthMm { get; set; } = 752.0;


        // ─────────────────────────────────────────────────────
        // PROBE GEOMETRY (FOR SCANNED AREA)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Probe scan width [mm]
        /// </summary>
        public double ProbeWidthMm { get; set; } = 600.0;

        /// <summary>
        /// Forward offset from robot centre to probe centre [mm]
        /// </summary>
        public double ProbeForwardOffsetMm { get; set; } = 367.0;

        /// <summary>
        /// Lateral offset from robot centre (positive = left) [mm]
        /// </summary>
        public double ProbeLateralOffsetMm { get; set; } = 0.0;


        // ─────────────────────────────────────────────────────
        // GRID SETTINGS
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Map resolution [mm per cell]
        /// </summary>
        public double GridResolutionMm { get; set; } = 10.0;

        /// <summary>
        /// Initial grid size [mm]
        /// </summary>
        public double InitialGridWidthMm { get; set; } = 5000.0;

        public double InitialGridHeightMm { get; set; } = 5000.0;


        // ─────────────────────────────────────────────────────
        // OPTIONAL: RAW HARDWARE PARAMETERS (REFERENCE ONLY)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Track circumference [mm]
        /// (Used only for initial estimate of MmPerCount)
        /// </summary>
        public double TrackCircumferenceMm { get; set; } = 202.0;

        /// <summary>
        /// Counts per revolution (motor * gearbox * microstepping)
        /// </summary>
        public double CountsPerRevolution { get; set; } = 1_280_000.0;


        // ─────────────────────────────────────────────────────
        // DERIVED HELPERS (DO NOT USE FOR FINAL CALCULATIONS)
        // ─────────────────────────────────────────────────────

        /// <summary>
        /// Initial estimate of mm per count (use only as a starting point)
        /// </summary>
        public double EstimatedMmPerCount =>
            TrackCircumferenceMm / System.Math.Max(1.0, CountsPerRevolution);

        /// <summary>
        /// Estimated counts per mm (inverse)
        /// </summary>
        public double EstimatedCountsPerMm =>
            System.Math.Max(1e-9, CountsPerRevolution / TrackCircumferenceMm);

        /// <summary>
        /// If true, swap A/B axis mapping (useful when controller axes are wired the
        /// opposite way to the assumed mapping). When true, A is treated as right and
        /// B as left.
        /// </summary>
        public bool SwapAxes { get; set; } = false;
    }
}