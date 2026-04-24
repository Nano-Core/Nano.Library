using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Options for sign-in requirements.
/// </summary>
public class SignInOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether users must have a confirmed email to sign in.
    /// Defaults to <c>false</c>.
    /// </summary>
    [Required]
    public virtual bool RequireConfirmedEmail { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether users must have a confirmed phone number to sign in.
    /// Defaults to <c>false</c>.
    /// </summary>
    [Required]
    public virtual bool RequireConfirmedPhoneNumber { get; set; } = false;
}