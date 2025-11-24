using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Sign-In Options (nested class).
/// </summary>
public class SignInOptions
{
    /// <summary>
    /// Require Confirmed Email-
    /// </summary>
    [Required]
    public virtual bool RequireConfirmedEmail { get; set; } = false;

    /// <summary>
    /// Require Confirmed PhoneNumber.
    /// </summary>
    [Required]
    public virtual bool RequireConfirmedPhoneNumber { get; set; } = false;
}