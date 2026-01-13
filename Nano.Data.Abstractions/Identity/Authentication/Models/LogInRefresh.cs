using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Log In Refresh.
/// </summary>
public class LogInRefresh
{
    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// Refresh Token.
    /// </summary>
    [Required]
    public virtual string RefreshToken { get; set; } = null!;

    /// <summary>
    /// Transient Roles.
    /// Non persisted roles, that is added to the jwt-token when logging in.
    /// </summary>
    [Required]
    public virtual IEnumerable<string> TransientRoles { get; set; } = [];

    /// <summary>
    /// Transient Claims.
    /// Non persisted claims, that is added to the jwt-token when logging in.
    /// </summary>
    [Required]
    public virtual IDictionary<string, string> TransientClaims { get; set; } = new Dictionary<string, string>();
}