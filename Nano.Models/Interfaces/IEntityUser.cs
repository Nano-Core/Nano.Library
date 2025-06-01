using System;
using Microsoft.AspNetCore.Identity;

namespace Nano.Models.Interfaces;

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
    IdentityUser<TIdentity> IdentityUser { get; set; }

    /// <summary>
    /// Is Active.
    /// </summary>
    bool IsActive { get; set; }
}