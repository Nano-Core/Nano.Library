using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;
/// <summary>
/// Represents a request to refresh an access token.
/// </summary>
public class LogInRefresh
{
    /// <summary>
    /// The expired or soon-to-expire access token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; } = null!;

    /// <summary>
    /// The refresh token used to issue a new access token.
    /// </summary>
    [Required]
    public virtual string RefreshToken { get; set; } = null!;

    /// <summary>
    /// Non-persisted roles added to the issued JWT during refresh.
    /// </summary>
    [Required]
    public virtual IEnumerable<string> TransientRoles { get; set; } = [];

    /// <summary>
    /// Non-persisted claims added to the issued JWT during refresh.
    /// </summary>
    [Required]
    public virtual IDictionary<string, string> TransientClaims { get; set; } = new Dictionary<string, string>();
}