using Microsoft.Extensions.Options;
using Nano.App.Web.Config;
using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions.Identity;
using System;

namespace Nano.App.Web.Identity;

/// <inheritdoc cref="IAuthRepository" />
public class DefaultAuthRepository : BaseAuthRepository<Guid>, IAuthRepository
{
    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="options">The <see cref="IOptionsMonitor{WebOptions}"/>.</param>
    /// <param name="identityRepository"></param>
    /// <param name="identityJwtRepository"></param>
    public DefaultAuthRepository(IOptionsMonitor<WebOptions> options, IIdentityRepository identityRepository, IIdentityJwtRepository identityJwtRepository)
        : base(options, identityRepository, identityJwtRepository)
    {
    }
}