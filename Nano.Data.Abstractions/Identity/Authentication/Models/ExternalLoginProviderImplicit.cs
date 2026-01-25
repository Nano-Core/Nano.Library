using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External login provider using an implicit authentication flow.
/// </summary>
public class ExternalLoginProviderImplicit : BaseLogInExternalProvider
{
    /// <summary>
    /// The access token issued by the external provider.
    /// </summary>
    [Required]
    public virtual string AccessToken { get; set; } = null!;
}
