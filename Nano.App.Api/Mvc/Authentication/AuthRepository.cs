using System;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc cref="IAuthRepository" />
public class AuthRepository(IIdentityAuthRepository<Guid>? identityAuthRepository = null, IAuthTransientRepository? authTransientRepository = null, IAuthRootRepository? authRootRepository = null, IAuthExternalRepository? authExternalRepository = null)
    : AuthRepository<Guid>(identityAuthRepository, authTransientRepository, authRootRepository, authExternalRepository), IAuthRepository;

/// <inheritdoc />
public class AuthRepository<TIdentity>(IIdentityAuthRepository<TIdentity>? identityAuthRepository = null, IAuthTransientRepository? authTransientRepository = null, IAuthRootRepository? authRootRepository = null, IAuthExternalRepository? authExternalRepository = null)
    : IAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public IIdentityAuthRepository<TIdentity>? IdentityAuthRepository { get; } = identityAuthRepository;

    /// <inheritdoc />
    public IAuthTransientRepository? AuthTransientRepository { get; } = authTransientRepository;

    /// <inheritdoc />
    public IAuthRootRepository? AuthRootRepository { get; } = authRootRepository;

    /// <inheritdoc />
    public IAuthExternalRepository? AuthExternalRepository { get; } = authExternalRepository;
}