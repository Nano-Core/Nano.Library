using Microsoft.Extensions.Options;
using Nano.App.Web.Config;
using Nano.App.Web.Identity.Abstractions;

namespace Nano.App.Web.Identity;

/// <inheritdoc cref="IAuthTransientRepository" />
public class DefaultAuthTransientRepository : BaseAuthTransientRepository
{
    /// <inheritdoc />
    public DefaultAuthTransientRepository(IOptionsMonitor<WebOptions> options, IIdentityJwtRepository identityJwtRepository)
        : base(options, identityJwtRepository)
    {
    }
}