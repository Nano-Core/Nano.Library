using Microsoft.Extensions.Options;
using Nano.App.Web.Config;
using Nano.App.Web.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Abstractions;

namespace Nano.App.Web.Identity.Authentication;

/// <inheritdoc cref="IAuthTransientRepository" />
public class DefaultAuthTransientRepository : BaseAuthTransientRepository
{
    /// <inheritdoc />
    public DefaultAuthTransientRepository(IOptionsMonitor<WebOptions> options, IAuthJwtRepository authJwtRepository, IAuthExternalRepository authExternalRepository)
        : base(options, authJwtRepository, authExternalRepository)
    {
    }
}