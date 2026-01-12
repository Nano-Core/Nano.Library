using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External Login  Provider Implicit.
/// </summary>
public class ExternalLoginProviderImplicit : BaseLogInExternalProvider
{
    /// <summary>
    /// Access Token.
    /// </summary>
    [Required]
    public virtual string AccessToken { get; set; } = null!;
}