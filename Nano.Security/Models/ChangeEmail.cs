using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <inheritdoc />
public class ChangeEmail : ChangeEmail<Guid>;

/// <summary>
/// Change Email.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class ChangeEmail<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Token.
    /// </summary>
    [Required]
    public virtual string Token { get; set; }

    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }
}