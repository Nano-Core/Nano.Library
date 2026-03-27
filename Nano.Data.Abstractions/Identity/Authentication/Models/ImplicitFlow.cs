using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Identity.Authentication.Models.Enums;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Implicit authentication flow.
/// </summary>
public class ImplicitFlow() : BaseAuthFlow(AuthFlowType.Implicit)
{
    /// <summary>
    /// The access token issued by the external provider.
    /// </summary>
    [Required]
    public virtual string AccessToken { get; set; } = null!;
}
