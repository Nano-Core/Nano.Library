using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External login request using externally supplied user data.
/// </summary>
public class LogInExternalDirect : LogInExternal
{
    /// <summary>
    /// The external user data used to complete the login.
    /// </summary>
    [Required]
    public virtual ExternalAuthenticationData ExternalAuthenticationData { get; set; } = new();
}