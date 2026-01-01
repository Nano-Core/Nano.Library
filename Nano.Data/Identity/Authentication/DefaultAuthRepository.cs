using System;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication.Abstractions;

namespace Nano.Data.Identity.Authentication;

/// <inheritdoc />
public class DefaultAuthRepository<TIdentity> : BaseAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="authJwtRepository"></param>
    /// <param name="authExternalRepository"></param>
    /// <param name="identityRepository"></param>
    public DefaultAuthRepository(IAuthJwtRepository authJwtRepository, IAuthExternalRepository authExternalRepository, IIdentityRepository<TIdentity> identityRepository)
        : base(authJwtRepository, authExternalRepository, identityRepository)
    {
    }
}

/// <inheritdoc cref="IAuthRepository" />
public class DefaultAuthRepository : BaseAuthRepository<Guid>, IAuthRepository
{
    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="authJwtRepository"></param>
    /// <param name="authExternalRepository"></param>
    /// <param name="identityRepository"></param>
    public DefaultAuthRepository(IAuthJwtRepository authJwtRepository, IAuthExternalRepository authExternalRepository, IIdentityRepository identityRepository)
        : base(authJwtRepository, authExternalRepository, identityRepository)
    {
    }
}