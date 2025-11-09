using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <inheritdoc />
public class RemoveExternalLogin : RemoveExternalLogin<Guid>;

/// <summary>
/// Remove External Login.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class RemoveExternalLogin<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    [Required]
    public virtual TIdentity UserId { get; set; }

    /// <summary>
    /// Provider Key.
    /// </summary>
    [Required]
    public virtual string ProviderKey { get; set; }

    /// <summary>
    /// Provider Name.
    /// </summary>
    [Required]
    public virtual string ProviderName { get; set; }
}