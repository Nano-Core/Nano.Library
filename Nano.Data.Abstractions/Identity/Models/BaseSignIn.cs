using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Base type for sign-in requests.
/// </summary>
public abstract class BaseSignIn
{
    /// <summary>
    /// Indicates whether the sign-in should be persisted across sessions.
    /// </summary>
    [Required]
    [DefaultValue(false)]
    public virtual bool IsRememberMe { get; set; } = false;
}