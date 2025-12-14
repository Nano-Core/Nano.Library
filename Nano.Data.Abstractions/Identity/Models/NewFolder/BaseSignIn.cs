using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Base Sign In (abstract).
/// </summary>
public abstract class BaseSignIn
{
    /// <summary>
    /// Is Remember Me.
    /// </summary>
    [Required]
    [DefaultValue(false)]
    public virtual bool IsRememberMe { get; set; } = false;
}