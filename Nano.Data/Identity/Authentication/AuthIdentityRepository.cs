using System;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.Data.Identity.Authentication;

/// <inheritdoc />
public class AuthIdentityRepository<TIdentity>(IIdentityRepository<TIdentity> identityRepository, IAuthJwtRepository authJwtRepository, IAuthExternalRepositoryAggregator authExternalRepository)
    : BaseAuthIdentityRepository<TIdentity>(identityRepository, authJwtRepository, authExternalRepository)
    where TIdentity : IEquatable<TIdentity>;

/// <inheritdoc cref="IAuthIdentityRepository" />
public class AuthIdentityRepository(IIdentityRepository identityRepository, IAuthJwtRepository authJwtRepository, IAuthExternalRepositoryAggregator authExternalRepository)
    : BaseAuthIdentityRepository<Guid>(identityRepository, authJwtRepository, authExternalRepository),
        IAuthIdentityRepository;