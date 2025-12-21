using System;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Identity;

namespace Nano.Data.Abstractions.Models.Abstractions;

/// <summary>
/// Entity User inteface.
/// </summary>
/// <typeparam name="TIdentity">The identity type.</typeparam>
public interface IEntityUser<TIdentity> : IEntityIdentity<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Identity User.
    /// </summary>
    IdentityUserExt<TIdentity> IdentityUser { get; set; }
}