using System;
using System.Collections.Generic;

namespace Nano.Security.Models;

/// <summary>
/// Access Token Data.
/// </summary>
public class AccessTokenData
{
    /// <summary>
    /// Id.
    /// </summary>
    public virtual string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// App Id.
    /// </summary>
    public virtual string AppId { get; set; }

    /// <summary>
    /// User Id.
    /// </summary>
    public virtual string UserId { get; set; }

    /// <summary>
    /// User Name.
    /// </summary>
    public virtual string UserName { get; set; }

    /// <summary>
    /// User Email.
    /// </summary>
    public virtual string UserEmail { get; set; }

    /// <summary>
    /// External Token.
    /// </summary>
    public virtual ExternalLoginTokenData ExternalToken { get; set; } = new();

    /// <summary>
    /// Claims.
    /// </summary>
    public virtual IEnumerable<System.Security.Claims.Claim> Claims { get; set; } = new List<System.Security.Claims.Claim>();
}