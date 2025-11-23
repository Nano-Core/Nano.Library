using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Jwt Token.
/// </summary>
public class JwtToken
{
    /// <summary>
    /// App Id.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string AppId { get; set; }

    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
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
}