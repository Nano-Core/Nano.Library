using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents a refresh token issued alongside an access token.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// The serialized refresh token value.
    /// </summary>
    [Required]
    public virtual required string Token { get; set; }

    /// <summary>
    /// The UTC date and time at which the refresh token expires.
    /// </summary>
    [Required]
    public virtual DateTimeOffset? ExpireAt { get; set; }

    /// <summary>
    /// Indicates whether the refresh token has expired.
    /// </summary>
    public virtual bool IsExpired => this.ExpireAt <= DateTimeOffset.UtcNow;
}