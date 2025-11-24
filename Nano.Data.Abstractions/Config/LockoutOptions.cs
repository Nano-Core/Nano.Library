using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Lockout Options (nested class).
/// </summary>
public class LockoutOptions
{
    /// <summary>
    /// Allowed For New Users.
    /// </summary>
    [Required]
    public virtual bool AllowedForNewUsers { get; set; } = true;

    /// <summary>
    /// Max Failed Access Attempts.
    /// </summary>
    [Required]
    public virtual int MaxFailedAccessAttempts { get; set; } = 3;

    /// <summary>
    /// Default Lockout TimeSpan.
    /// </summary>
    [Required]
    public virtual TimeSpan DefaultLockoutTimeSpan { get; set; } = TimeSpan.FromMinutes(30);
}