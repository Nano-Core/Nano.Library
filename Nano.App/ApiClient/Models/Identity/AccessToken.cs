using System;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Identity.Models;

namespace Nano.App.ApiClient.Models.Identity;

/// <summary>
/// Access Token.
/// </summary>
public class AccessToken
{
    /// <summary>
    /// App Id.
    /// </summary>
    [MaxLength(256)]
    public string AppId { get; set; }

    /// <summary>
    /// User Id.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public string Token { get; set; }

    /// <summary>
    /// Expire At.
    /// </summary>
    [Required]
    public DateTimeOffset ExpireAt { get; set; }

    /// <summary>
    /// Is Expired.
    /// </summary>
    public bool IsExpired => this.ExpireAt <= DateTimeOffset.UtcNow;

    /// <summary>
    /// Refresh Token.
    /// </summary>
    public RefreshToken RefreshToken { get; set; }
}