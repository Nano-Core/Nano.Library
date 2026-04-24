using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication;
using System;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc cref="IAuthRepository" />
public class AuthRepository(IAuthExternalRepositoryAggregator authExternalRepositoryAggregator, IAuthIdentityRepository<Guid>? authIdentityRepository = null, IAuthTransientRepository? authTransientRepository = null, IAuthRootRepository? authRootRepository = null)
    : AuthRepository<Guid>(authExternalRepositoryAggregator, authIdentityRepository, authTransientRepository, authRootRepository), IAuthRepository;

/// <inheritdoc />
public class AuthRepository<TIdentity>(IAuthExternalRepositoryAggregator authExternalRepositoryAggregator, IAuthIdentityRepository<TIdentity>? authIdentityRepository = null, IAuthTransientRepository? authTransientRepository = null, IAuthRootRepository? authRootRepository = null)
    : IAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public IAuthExternalRepositoryAggregator AuthExternalRepositoryAggregator { get; } = authExternalRepositoryAggregator ?? throw new ArgumentNullException(nameof(authExternalRepositoryAggregator));

    /// <inheritdoc />
    public IAuthIdentityRepository<TIdentity>? AuthIdentityRepository { get; } = authIdentityRepository;

    /// <inheritdoc />
    public IAuthRootRepository? AuthRootRepository { get; } = authRootRepository;

    /// <inheritdoc />
    public IAuthTransientRepository? AuthTransientRepository { get; } = authTransientRepository;
}