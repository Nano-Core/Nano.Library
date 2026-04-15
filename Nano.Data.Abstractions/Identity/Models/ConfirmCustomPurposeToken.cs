using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to confirm a custom purpose using a token.
/// </summary>
public class ConfirmCustomPurposeToken
{
    /// <summary>
    /// The token used to confirm the custom purpose.
    /// </summary>
    [Required]
    public virtual required string Token { get; set; }

    /// <summary>
    /// The name of the custom purpose being confirmed.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Purpose { get; set; }
}