using System;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.Data.Identity.Authentication;

/// <inheritdoc />
public class IdentityAuthRepository<TIdentity>(IIdentityRepository<TIdentity> identityRepository, IAuthJwtRepository authJwtRepository, IAuthExternalRepository? authExternalRepository = null)
    : BaseIdentityAuthRepository<TIdentity>(identityRepository, authJwtRepository, authExternalRepository)
    where TIdentity : IEquatable<TIdentity>;

/// <inheritdoc cref="IIdentityAuthRepository" />
public class IdentityAuthRepository(IIdentityRepository identityRepository, IAuthJwtRepository authJwtRepository, IAuthExternalRepository? authExternalRepository = null)
    : BaseIdentityAuthRepository<Guid>(identityRepository, authJwtRepository, authExternalRepository),
        IIdentityAuthRepository;