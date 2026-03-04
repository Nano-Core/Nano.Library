using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for authentication configuration.
/// </summary>
public class AuthenticationOptions
{
    /// <summary>
    /// JWT authentication options.
    /// </summary>
    public virtual JwtAuthenticationOptions? Jwt { get; set; }

    /// <summary>
    /// Hide authentication controller.
    /// </summary>
    [Required]
    public virtual bool HideAuthController { get; set; } = false;
}