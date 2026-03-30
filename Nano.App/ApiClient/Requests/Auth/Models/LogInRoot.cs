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
    public virtual string Username { get; set; } = null!;

    /// <summary>
    /// The password for login.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Password { get; set; } = null!;

    /// <summary>
    /// Non-persisted claims added to the issued JWT during login.
    /// </summary>
    [Required]
    public virtual IDictionary<string, string> TransientClaims { get; set; } = new Dictionary<string, string>();
}