using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Types;

/// <summary>
/// Distance.
/// </summary>
public class Distance
{
    /// <summary>
    /// Ratio meter to mile.
    /// </summary>
    public const double RATIO_METER_TO_MILE = 1609.34;

    /// <summary>
    /// Ratio meter to kilometer.
    /// </summary>
    public const double RATIO_METER_TO_KILOMETER = 1000.00;

    /// <summary>
    /// Meters.
    /// </summary>
    [Required]
    [DefaultValue(0.00)]
    public virtual double Meters { get; set; } = 0.00;

    /// <summary>
    /// Miles.
    /// </summary>
    [Required]
    [DefaultValue(0.00)]
    public virtual double Miles => this.Meters / Distance.RATIO_METER_TO_MILE;

    /// <summary>
    /// Kilometers.
    /// </summary>
    [Required]
    [DefaultValue(0.00)]
    public virtual double Kilometers => this.Meters / Distance.RATIO_METER_TO_KILOMETER;
}