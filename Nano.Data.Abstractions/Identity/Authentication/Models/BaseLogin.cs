using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;
/// <summary>
/// Base class for login requests.
/// </summary>
public abstract class BaseLogIn
{
    /// <summary>
    /// The application identifier associated with the login request.
    /// </summary>
    [MaxLength(256)]
    public virtual string? AppId { get; set; }

    /// <summary>
    /// Indicates whether the login should be persisted across sessions.
    /// Not relevant for transient logins.
    /// </summary>
    [Required]
    [DefaultValue(false)]
    public virtual bool IsRememberMe { get; set; } = false;

    /// <summary>
    /// Indicates whether the login supports token refresh.
    /// Not relevant for transient logins.
    /// </summary>
    [Required]
    [DefaultValue(false)]
    public virtual bool IsRefreshable { get; set; } = false;

    /// <summary>
    /// Non-persisted roles added to the issued JWT during login.
    /// </summary>
    [Required]
    public virtual IEnumerable<string> TransientRoles { get; set; } = [];

    /// <summary>
    /// Non-persisted claims added to the issued JWT during login.
    /// </summary>
    [Required]
    public virtual IDictionary<string, string> TransientClaims { get; set; } = new Dictionary<string, string>();
}