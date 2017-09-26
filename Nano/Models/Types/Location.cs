namespace Nano.Models.Types
{
    /// <summary>
    /// Location.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Latitude.
        /// </summary>
        public virtual double Latitude { get; set; } = 0.00;

        /// <summary>
        /// Longitude.
        /// </summary>
        public virtual double Longitude { get; set; } = 0.00;
    }
}