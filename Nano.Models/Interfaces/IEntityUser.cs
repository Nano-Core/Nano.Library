using System;

namespace Nano.Models.Interfaces;

/// <summary>
/// Entity User inteface.
/// </summary>
/// <typeparam name="TIdentity">The identity type.</typeparam>
public interface IEntityUser<TIdentity> : IEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Identity User Id.
    /// </summary>
    TIdentity IdentityUserId { get; set; }

    /// <summary>
    /// Identity User.
    /// </summary>
    IdentityUserExpanded<TIdentity> IdentityUser { get; set; }
}