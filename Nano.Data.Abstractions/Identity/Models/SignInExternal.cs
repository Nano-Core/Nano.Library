using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// 
/// </summary>
public class SignInExternal
{
    /// <summary>
    /// External LogIn Data.
    /// </summary>
    [Required]
    public virtual ExternalLogInData ExternalLogInData { get; set; }

    /// <summary>
    /// Is Remember Me.
    /// </summary>
    [Required]
    [DefaultValue(false)]
    public virtual bool IsRememberMe { get; set; } = false;
}