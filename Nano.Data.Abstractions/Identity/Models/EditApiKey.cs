using System;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <inheritdoc />
public class EditApiKey : EditApiKey<Guid>;

/// <summary>
/// Represents a request to edit an API key.
/// </summary>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
public class EditApiKey<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the API key to edit.
    /// </summary>
    [Required]
    public virtual TIdentity Id { get; set; } = default!;

    /// <summary>
    /// The updated name for the API key.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public virtual string Name { get; set; } = null!;
}