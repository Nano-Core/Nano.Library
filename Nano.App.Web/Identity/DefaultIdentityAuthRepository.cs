using System;
using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Abstractions;
using Nano.Web;

namespace Nano.App.Web.Identity;

/// <inheritdoc />
public class DefaultIdentityAuthRepository : BaseIdentityAuthRepository<Guid>
{
    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="options">The <see cref="IdentityOptions"/>.</param>
    /// <param name="identityRepository"></param>
    /// <param name="identityJwtRepository"></param>
    public DefaultIdentityAuthRepository(IdentityOptions options, IIdentityRepository<Guid> identityRepository, IIdentityJwtRepository<Guid> identityJwtRepository)
        : base(options, identityRepository, identityJwtRepository)
    {
    }
}