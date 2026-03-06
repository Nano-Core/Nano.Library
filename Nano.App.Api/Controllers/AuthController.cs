using System;
using Microsoft.Extensions.Logging;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
public class AuthController : BaseAuthController<Guid>
{
    /// <inheritdoc />
    public AuthController(ILogger<AuthController> logger, IIdentityAuthRepository? identityAuthRepository = null, IAuthTransientRepository? authTransientRepository = null, IAuthRootRepository? authRootRepository = null, IAuthExternalRepository? authExternalRepository = null)
        : base(logger, identityAuthRepository, authTransientRepository, authRootRepository, authExternalRepository)
    {
    }
}