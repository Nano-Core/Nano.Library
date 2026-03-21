using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to generate a custom-purpose token for a user.
/// </summary>
public class GenerateCustomPurposeToken
{
    /// <summary>
    /// The purpose for which the token is generated.
    /// </summary>
    [MaxLength(256)]
    public virtual string Purpose { get; set; } = null!;
}