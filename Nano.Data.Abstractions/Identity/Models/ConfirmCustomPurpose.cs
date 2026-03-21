using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to confirm a custom purpose.
/// </summary>
public class ConfirmCustomPurpose
{
    /// <summary>
    /// The token used to confirm the custom purpose.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// The name of the custom purpose being confirmed.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Purpose { get; set; } = null!;
}