using System;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.Data.Identity.Authentication;

/// <inheritdoc />
public class DefaultIdentityAuthRepository<TIdentity>(IIdentityRepository<TIdentity> identityRepository, IAuthJwtRepository authJwtRepository, IAuthExternalRepository? authExternalRepository = null)
    : BaseIdentityAuthRepository<TIdentity>(identityRepository, authJwtRepository, authExternalRepository)
    where TIdentity : IEquatable<TIdentity>;

/// <inheritdoc cref="IIdentityAuthRepository" />
public class DefaultIdentityAuthRepository(IIdentityRepository identityRepository, IAuthJwtRepository authJwtRepository, IAuthExternalRepository? authExternalRepository = null)
    : BaseIdentityAuthRepository<Guid>(identityRepository, authJwtRepository, authExternalRepository),
        IIdentityAuthRepository;