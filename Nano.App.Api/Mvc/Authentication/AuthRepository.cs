using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication;
using System;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc cref="IAuthRepository" />
public class AuthRepository(IAuthIdentityRepository<Guid>? authIdentityRepository = null, IAuthTransientRepository? authTransientRepository = null, IAuthRootRepository? authRootRepository = null)
    : AuthRepository<Guid>(authIdentityRepository, authTransientRepository, authRootRepository), IAuthRepository;

/// <inheritdoc />
public class AuthRepository<TIdentity>(IAuthIdentityRepository<TIdentity>? authIdentityRepository = null, IAuthTransientRepository? authTransientRepository = null, IAuthRootRepository? authRootRepository = null)
    : IAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public IAuthIdentityRepository<TIdentity>? AuthIdentityRepository { get; } = authIdentityRepository;

    /// <inheritdoc />
    public IAuthTransientRepository? AuthTransientRepository { get; } = authTransientRepository;

    /// <inheritdoc />
    public IAuthRootRepository? AuthRootRepository { get; } = authRootRepository;
}