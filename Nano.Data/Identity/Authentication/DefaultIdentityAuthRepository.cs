using System;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.Data.Identity.Authentication;

/// <inheritdoc />
public class DefaultIdentityAuthRepository<TIdentity> : BaseIdentityAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="identityRepository"></param>
    /// <param name="authJwtRepository"></param>
    /// <param name="authExternalRepository"></param>
    public DefaultIdentityAuthRepository(IIdentityRepository<TIdentity> identityRepository, IAuthJwtRepository authJwtRepository, IAuthExternalRepository? authExternalRepository = null)
        : base(identityRepository, authJwtRepository, authExternalRepository)
    {
    }
}

/// <inheritdoc cref="IIdentityAuthRepository" />
public class DefaultIdentityAuthRepository : BaseIdentityAuthRepository<Guid>, IIdentityAuthRepository
{
    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="identityRepository"></param>
    /// <param name="authJwtRepository"></param>
    /// <param name="authExternalRepository"></param>
    public DefaultIdentityAuthRepository(IIdentityRepository identityRepository, IAuthJwtRepository authJwtRepository, IAuthExternalRepository? authExternalRepository = null)
        : base(identityRepository, authJwtRepository, authExternalRepository)
    {
    }
}