using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Generate Jwt Token.
/// </summary>
public class GenerateJwtToken<TIdentity> 
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// App Id.
    /// </summary>
    [MaxLength(256)]
    public virtual string AppId { get; set; }

    /// <summary>
    /// User Id.
    /// </summary>
    [MaxLength(256)]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// User Name.
    /// </summary>
    [MaxLength(256)]
    public virtual string UserName { get; set; }

    /// <summary>
    /// Email.
    /// </summary>
    [MaxLength(256)]
    public virtual string Email { get; set; }

    /// <summary>
    /// External Token.
    /// </summary>
    public virtual ExternalLoginTokenData ExternalToken { get; set; }

    /// <summary>
    /// Claims.
    /// </summary>
    public virtual IEnumerable<Claim> Claims { get; set; } = [];
}
