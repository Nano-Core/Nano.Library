using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Models.Identity.External.Providers;

/// <summary>
/// External Login  Provider Implicit.
/// </summary>
public class ExternalLoginProviderImplicit : BaseLogInExternalProvider
{
    /// <summary>
    /// Access Token.
    /// </summary>
    [Required]
    public virtual string AccessToken { get; set; }
}