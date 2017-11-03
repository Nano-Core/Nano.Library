using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Nano.Models.Types
{
    // TODO: Spatial: Implement Polygon, LineString. Inherit from som interface, so spatial services and controllers can treat them alike.
    // TODO: Add Map extensions method. Add data annotations for validation.

    /// <summary>
    /// Location.
    /// </summary>
    public class Location
    {
        private double latitude;
        private double longitude;

        /// <summary>
        /// Latitude.
        /// </summary>
        [Required]
        public virtual double Latitude
        {
            get => this.latitude;
            set
            {
                if (value > 90 || value < -90)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.latitude = value;
            }
        }

        /// <summary>
        /// Longitude.
        /// </summary>
        [Required]
        public virtual double Longitude
        {
            get => this.longitude;
            set
            {
                if (value > 180 || value < -180)
                    throw new ArgumentOutOfRangeException(nameof(value));

                this.longitude = value;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{this.latitude},{this.longitude}";
        }

        /// <summary>
        /// Converts a <see cref="string"/> into a <see cref="Location"/>.
        /// Format: "latitude,longitude".
        /// </summary>
        /// <returns>The <see cref="Location"/>.</returns>
        public static Location FromString(string location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            var doubles = location
                .Split(',')
                .Select(x => Convert.ToDouble(x.Trim(), CultureInfo.InvariantCulture.NumberFormat))
                .ToArray();

            return new Location
            {
                latitude = doubles[0],
                longitude = doubles[1]
            };
        }
    }
}