using Nano.Common.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// External login user data resolved from an authentication provider.
/// </summary>
public class ExternalAuthenticationData
{
    /// <summary>
    /// The unique identifier of the external user.
    /// </summary>
    [Required]
    public virtual string Id { get; set; } = null!;

    /// <summary>
    /// The name of the external user.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// The username of the external user.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Username { get; set; } = null!;

    /// <summary>
    /// The email address of the external user.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public virtual string EmailAddress { get; set; } = null!;

    /// <summary>
    /// The user's phone number (optional, international format supported).
    /// </summary>
    [InternationalPhone]
    public virtual string? PhoneNumber { get; set; }

    /// <summary>
    /// The authentication token associated with the external login.
    /// </summary>
    public virtual ExternalAuthenticationToken ExternalToken { get; set; } = new();

    /// <summary>
    /// Non-persisted claims added to the issued JWT during login.
    /// </summary>
    [Required]
    public virtual IDictionary<string, string> TransientClaims { get; set; } = new Dictionary<string, string>();
}