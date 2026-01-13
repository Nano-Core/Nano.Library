using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Refresh Token.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// Expire At.
    /// </summary>
    [Required]
    public virtual DateTimeOffset? ExpireAt { get; set; }

    /// <summary>
    /// Is Expired.
    /// </summary>
    public virtual bool IsExpired => this.ExpireAt <= DateTimeOffset.UtcNow;
}