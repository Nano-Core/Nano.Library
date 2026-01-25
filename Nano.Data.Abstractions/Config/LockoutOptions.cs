using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Options for account lockout policies.
/// </summary>
public class LockoutOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether lockout is allowed for new users.
    /// Defaults to <c>true</c>.
    /// </summary>
    [Required]
    public virtual bool AllowedForNewUsers { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of failed access attempts before a user is locked out.
    /// Defaults to <c>3</c>.
    /// </summary>
    [Required]
    public virtual int MaxFailedAccessAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the default lockout duration for a user.
    /// Defaults to <c>30 minutes</c>.
    /// </summary>
    [Required]
    public virtual TimeSpan DefaultLockoutTimeSpan { get; set; } = TimeSpan.FromMinutes(30);
}