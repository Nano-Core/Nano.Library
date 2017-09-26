namespace Nano.Models.Types
{
    /// <summary>
    /// Distance.
    /// </summary>
    public class Distance
    {
        private const double RATIO_METER_TO_MILE = 0.000621371D;

        /// <summary>
        /// Meters.
        /// </summary>
        public virtual double? Meters { get; set; }

        /// <summary>
        /// Miles.
        /// </summary>
        public virtual double? Miles => this.Meters / Distance.RATIO_METER_TO_MILE;
    }
}