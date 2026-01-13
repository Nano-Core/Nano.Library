using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External Login.
/// </summary>
public class ExternalLogin
{
    /// <summary>
    /// Key.
    /// </summary>
    [Required]
    public virtual string Key { get; set; } = null!;

    /// <summary>
    /// Provider.
    /// </summary>
    [Required]
    public virtual ExternalLoginProvider Provider { get; set; } = new();
}