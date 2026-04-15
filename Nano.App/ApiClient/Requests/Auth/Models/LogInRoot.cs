using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Auth.Models;

/// <summary>
/// Represents the credentials required for logging in as root.
/// </summary>
public class LogInRoot
{
    /// <summary>
    /// The username for login.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Username { get; set; }

    /// <summary>
    /// The password for login.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Password { get; set; }

    /// <summary>
    /// Non-persisted claims added to the issued JWT during login.
    /// </summary>
    [Required]
    public virtual IDictionary<string, string> TransientClaims { get; set; } = new Dictionary<string, string>();
}