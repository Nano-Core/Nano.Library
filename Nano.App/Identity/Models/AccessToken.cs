using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

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

    /// <summary>
    /// 
    /// </summary>
    public AccessToken()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jwtToken"></param>
    /// <param name="refreshToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AccessToken(JwtToken jwtToken, RefreshToken refreshToken = null)
    {
        if (jwtToken == null) 
            throw new ArgumentNullException(nameof(jwtToken));
        
        this.AppId = jwtToken.AppId;
        this.UserId = jwtToken.UserId;
        this.Token = jwtToken.Token;
        this.ExpireAt = jwtToken.ExpireAt;
        this.RefreshToken = refreshToken;
    }
}