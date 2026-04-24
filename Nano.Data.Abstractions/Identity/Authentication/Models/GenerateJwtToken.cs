using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents a request to generate a JWT token.
/// </summary>
public class GenerateJwtToken
{
    /// <summary>
    /// The unique identifier of the generated token.
    /// </summary>
    public virtual string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The application identifier associated with the token.
    /// </summary>
    public virtual string? AppId { get; set; }

    /// <summary>
    /// The user identifier associated with the token.
    /// </summary>
    public virtual string? UserId { get; set; }

    /// <summary>
    /// The username associated with the token.
    /// </summary>
    public virtual string? UserName { get; set; }

    /// <summary>
    /// The email address associated with the token.
    /// </summary>
    public virtual string? UserEmail { get; set; }

    /// <summary>
    /// External authentication token data, if applicable.
    /// </summary>
    public virtual ExternalAuthenticationToken? ExternalToken { get; set; }

    /// <summary>
    /// The claims included in the generated JWT.
    /// </summary>
    public virtual IEnumerable<Claim> Claims { get; set; } = [];
}