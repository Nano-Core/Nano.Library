using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Security.Models;

/// <inheritdoc />
public class EditApiKey : EditApiKey<Guid>;

/// <summary>
/// Remove Claim.
/// </summary>
/// <typeparam name="TIdentity">The identity key type</typeparam>
public class EditApiKey<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Id.
    /// </summary>
    [Required]
    public virtual TIdentity Id { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name { get; set; }
}