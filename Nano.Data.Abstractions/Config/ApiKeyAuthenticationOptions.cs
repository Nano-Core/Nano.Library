using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Options for API key-based authentication.
/// </summary>
public class ApiKeyAuthenticationOptions
{
    /// <summary>
    /// Gets or sets the secret key used for API authentication.
    /// This value is required and cannot be <c>null</c>.
    /// </summary>
    [Required]
    public virtual string Secret { get; set; } = null!;
}