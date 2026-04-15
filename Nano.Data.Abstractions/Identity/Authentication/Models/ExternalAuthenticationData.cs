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
    public virtual required string Id { get; set; }

    /// <summary>
    /// The name of the external user.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Name { get; set; }

    /// <summary>
    /// The username of the external user.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual required string Username { get; set; }

    /// <summary>
    /// The email address of the external user.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public virtual required string EmailAddress { get; set; }

    /// <summary>
    /// The user's phone number (optional, international format supported).
    /// </summary>
    [InternationalPhone]
    public virtual string? PhoneNumber { get; set; }

    /// <summary>
    /// The authentication token associated with the external login.
    /// </summary>
    public virtual required ExternalAuthenticationToken ExternalToken { get; set; }

    /// <summary>
    /// Non-persisted claims added to the issued JWT during login.
    /// </summary>
    [Required]
    public virtual IDictionary<string, string> TransientClaims { get; set; } = new Dictionary<string, string>();
}