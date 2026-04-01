using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to validate an API key for a user.
/// </summary>
public class ValidateApiKey
{
    /// <summary>
    /// The API key.
    /// </summary>
    [Required]
    public virtual string ApiKey { get; set; } = null!;
}