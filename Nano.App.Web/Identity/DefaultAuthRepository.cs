using System;
using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions.Identity.Abstractions;
using Nano.Web;

namespace Nano.App.Web.Identity;

/// <inheritdoc cref="IAuthRepository" />
public class DefaultAuthRepository : BaseAuthRepository<Guid>, IAuthRepository
{
    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="options">The <see cref="IdentityOptions"/>.</param>
    /// <param name="identityRepository"></param>
    /// <param name="identityJwtRepository"></param>
    public DefaultAuthRepository(IdentityOptions options, IIdentityRepository identityRepository, IIdentityJwtRepository identityJwtRepository)
        : base(options, identityRepository, identityJwtRepository)
    {
    }
}