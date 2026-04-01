using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to sign in with api key.
/// </summary>
public class SignInApiKey
{
    /// <summary>
    /// The api key signing in.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string ApiKey { get; set; } = null!;
}