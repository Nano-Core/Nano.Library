using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// External login user data resolved from an authentication provider.
/// </summary>
public class AddExternalLoginDirect
{
    /// <summary>
    /// The unique identifier of the external user.
    /// </summary>
    [Required]
    public virtual ExternalProvider Provider { get; set; } = new();
}