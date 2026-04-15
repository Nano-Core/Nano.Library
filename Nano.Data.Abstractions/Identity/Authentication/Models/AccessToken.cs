using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents an issued access token.
/// </summary>
public class AccessToken
{
    /// <summary>
    /// The application identifier associated with the token.
    /// </summary>
    [MaxLength(256)]
    public virtual string? AppId { get; set; }

    /// <summary>
    /// The user identifier associated with the token.
    /// </summary>
    public virtual string? UserId { get; set; }

    /// <summary>
    /// The serialized access token value.
    /// </summary>
    [Required]
    public virtual required string Token { get; set; }

    /// <summary>
    /// The UTC date and time at which the token expires.
    /// </summary>
    [Required]
    public virtual DateTimeOffset ExpireAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Indicates whether the token has expired.
    /// </summary>
    public virtual bool IsExpired => this.ExpireAt <= DateTimeOffset.UtcNow;

    /// <summary>
    /// The refresh token associated with this access token, if available.
    /// </summary>
    public virtual RefreshToken? RefreshToken { get; set; }
}