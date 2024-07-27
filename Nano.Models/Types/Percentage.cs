using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Models.Types;

// BUG: REmove types from Nano.

/// <summary>
/// Percentage.
/// </summary>
public class Percentage
{
    /// <summary>
    /// Gets the percentage value as a <see cref="decimal"/>.
    /// </summary>
    [Required]
    [DefaultValue(0.00)]
    public decimal AsDecimal { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public Percentage()
    {
    }

    /// <summary>
    /// Constructor that accepts a <see cref="Double"/>.
    /// </summary>
    /// <param name="double"></param>
    public Percentage(double @double)
        : this((decimal)@double)
    {
    }

    /// <summary>
    /// Constructor that accepts a <see cref="Decimal"/>.
    /// </summary>
    /// <param name="decimal"></param>
    public Percentage(decimal @decimal)
    {
        this.AsDecimal = @decimal;
    }
}