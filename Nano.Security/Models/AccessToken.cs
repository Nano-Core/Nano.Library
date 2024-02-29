using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <summary>
/// Access Token.
/// </summary>
public class AccessToken
{
    /// <summary>
    /// App Id.
    /// </summary>
    [MaxLength(256)]
    public virtual string AppId { get; set; }

    /// <summary>
    /// User Id.
    /// </summary>
    [MaxLength(256)]
    public virtual string UserId { get; set; }

    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; }

    /// <summary>
    /// Expire At.
    /// </summary>
    [Required]
    public virtual DateTimeOffset ExpireAt { get; set; }

    /// <summary>
    /// Is Expired.
    /// </summary>
    public virtual bool IsExpired => this.ExpireAt <= DateTimeOffset.UtcNow;

    /// <summary>
    /// Refresh Token.
    /// </summary>
    public virtual RefreshToken RefreshToken { get; set; }
}