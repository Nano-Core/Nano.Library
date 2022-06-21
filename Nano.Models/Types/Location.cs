using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Nano.Models.Types;

/// <summary>
/// Location.
/// </summary>
public class Location
{
    /// <summary>
    /// Latitude.
    /// </summary>
    [Required]
    [DefaultValue(0.00)]
    [Range(-90.00, 90.00)]
    public virtual double Latitude { get; set; }

    /// <summary>
    /// Longitude.
    /// </summary>
    [Required]
    [DefaultValue(0.00)]
    [Range(-180.00, 180.00)]
    public virtual double Longitude { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Latitude},{this.Longitude}";
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
            Latitude = doubles[0],
            Longitude = doubles[1]
        };
    }
}