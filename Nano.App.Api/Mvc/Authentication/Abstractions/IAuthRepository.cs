using System;
using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.App.Api.Mvc.Authentication.Abstractions;

/// <summary>
/// Non-generic authentication repository using <see cref="Guid"/> as the identity type.
/// </summary>
public interface IAuthRepository : IAuthRepository<Guid>;

/// <summary>
/// Provides access to authentication-related repositories for a given identity type.
/// </summary>
/// <typeparam name="TIdentity">The type used for the identity user key, which must implement <see cref="IEquatable{T}"/>.</typeparam>
public interface IAuthRepository<in TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Defines a repository for managing external authentication for users.
    /// </summary>
    IAuthExternalRepositoryAggregator AuthExternalRepositoryAggregator { get; }

    /// <summary>
    /// Defines a repository for managing authentication for identity users.
    /// </summary>
    IAuthIdentityRepository<TIdentity>? AuthIdentityRepository { get; }

    /// <summary>
    /// Provides authentication for the root/admin user.
    /// </summary>
    IAuthRootRepository? AuthRootRepository { get; }

    /// <summary>
    /// Defines a transient authentication repository for handling external login flows,
    /// </summary>
    IAuthTransientRepository? AuthTransientRepository { get; }
}