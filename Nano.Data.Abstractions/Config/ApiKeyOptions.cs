using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Options for API keys.
/// </summary>
public class ApiKeyOptions
{
    /// <summary>
    /// Gets or sets the secret key used to create and validate API keys
    /// This value is required and cannot be <c>null</c>.
    /// </summary>
    [Required]
    public virtual string Secret { get; set; } = null!;
}