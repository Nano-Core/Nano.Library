using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.App.ApiClient.Requests.Auth.Models;
using Nano.App.Consts;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
public abstract class BaseAuthController : BaseAuthController<Guid>
{
    /// <inheritdoc />
    protected BaseAuthController(ILogger<BaseAuthController> logger, IIdentityAuthRepository? identityAuthRepository = null, IAuthTransientRepository? authTransientRepository = null, IAuthRootRepository? authRootRepository = null, IAuthExternalRepository? authExternalRepository = null)
        : base(logger, identityAuthRepository, authTransientRepository, authRootRepository, authExternalRepository)
    {
    }
}

/// <summary>
/// Base controller for authentication-related operations.
/// Handles login, logout, token refresh, and external authentication.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
[Route(ControllerRoutes.AUTH_CONTROLLER_ROUTE)]
[Route($"{ControllerRoutes.ROUTE_VERSION_PREFIX}/{ControllerRoutes.AUTH_CONTROLLER_ROUTE}")]
[AllowAnonymous]
public abstract class BaseAuthController<TIdentity> : BaseController
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IIdentityAuthRepository<TIdentity>? identityAuthRepository;
    private readonly IAuthTransientRepository? authTransientRepository;
    private readonly IAuthRootRepository? authRootRepository;
    private readonly IAuthExternalRepository? authExternalRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAuthController{TIdentity}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="identityAuthRepository">Repository for identity-based authentication.</param>
    /// <param name="authTransientRepository">Repository for transient authentication.</param>
    /// <param name="authRootRepository">Repository for root authentication.</param>
    /// <param name="authExternalRepository">Repository for external authentication.</param>
    protected BaseAuthController(ILogger<BaseAuthController<TIdentity>> logger, IIdentityAuthRepository<TIdentity>? identityAuthRepository = null, IAuthTransientRepository? authTransientRepository = null, IAuthRootRepository? authRootRepository = null, IAuthExternalRepository? authExternalRepository = null)
        : base(logger)
    {
        this.identityAuthRepository = identityAuthRepository;
        this.authTransientRepository = authTransientRepository;
        this.authRootRepository = authRootRepository;
        this.authExternalRepository = authExternalRepository;
    }


    #region Login

    /// <summary>
    /// Authenticates a user and generates an access token (JWT) for authorization.
    /// </summary>
    /// <param name="logIn">The login credentials.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The generated <see cref="AccessToken"/>.</returns>
    /// <response code="200">Authentication succeeded and token returned.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInAsync([FromBody][Required] LogIn logIn, CancellationToken cancellationToken = default)
    {
        if (this.identityAuthRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.identityAuthRepository
            .LogInAsync(logIn, cancellationToken);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Authenticates the root user from configuration and returns an access token.
    /// </summary>
    /// <param name="logInRoot">The root login credentials.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The generated <see cref="AccessToken"/>.</returns>
    /// <response code="200">Authentication succeeded and token returned.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">Root user not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN_ROOT)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInRootAsync([FromBody][Required] LogInRoot logInRoot, CancellationToken cancellationToken = default)
    {
        if (this.authRootRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.authRootRepository
            .LogInRootAsync(logInRoot);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Signs in a user via external authentication data.
    /// </summary>
    /// <param name="logInExternalDirect">The external login credentials.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The generated <see cref="AccessToken"/>.</returns>
    /// <response code="200">Authentication succeeded and token returned.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN_EXTERNAL_DIRECT)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalDirectAsync([FromBody][Required] LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        if (this.identityAuthRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.identityAuthRepository
            .LogInExternalAsync(logInExternalDirect, cancellationToken);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Signs in a transient user via external authentication data.
    /// </summary>
    /// <param name="logInExternalDirect">The external login credentials.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The generated <see cref="AccessToken"/>.</returns>
    /// <response code="200">Authentication succeeded and token returned.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN_EXTERNAL_DIRECT_TRANSIENT)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalDirectTransientAsync([FromBody][Required] LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        if (this.authTransientRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.authTransientRepository
            .LogInExternalAsync(logInExternalDirect, cancellationToken);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Signs in a user via external Facebook authentication.
    /// </summary>
    /// <param name="logInExternal">The external login credentials.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The generated <see cref="AccessToken"/>.</returns>
    /// <response code="200">Authentication succeeded and token returned.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN_EXTERNAL_FACEBOOK)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalFacebookAsync([FromBody][Required] LogInExternalFacebook logInExternal, CancellationToken cancellationToken = default)
    {
        if (this.identityAuthRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.identityAuthRepository
            .LogInExternalAsync(logInExternal, cancellationToken);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Signs in a transient user via external Facebook authentication.
    /// </summary>
    /// <param name="logInExternal">The external login credentials.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The generated <see cref="AccessToken"/>.</returns>
    /// <response code="200">Authentication succeeded and token returned.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN_EXTERNAL_FACEBOOK_TRANSIENT)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalFacebookTransientAsync([FromBody][Required] LogInExternalFacebook logInExternal, CancellationToken cancellationToken = default)
    {
        if (this.authTransientRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.authTransientRepository
            .LogInExternalAsync(logInExternal, cancellationToken);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Signs in a user via external Google authentication.
    /// </summary>
    /// <param name="logInExternal">The external login credentials.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The generated <see cref="AccessToken"/>.</returns>
    /// <response code="200">Authentication succeeded and token returned.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN_EXTERNAL_GOOGLE)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalGoogleAsync([FromBody][Required] LogInExternalGoogle logInExternal, CancellationToken cancellationToken = default)
    {
        if (this.identityAuthRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.identityAuthRepository
            .LogInExternalAsync(logInExternal, cancellationToken);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Signs in a transient user via external Google authentication.
    /// </summary>
    /// <param name="logInExternal">The external login credentials.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The generated <see cref="AccessToken"/>.</returns>
    /// <response code="200">Authentication succeeded and token returned.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN_EXTERNAL_GOOGLE_TRANSIENT)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalGoogleTransientAsync([FromBody][Required] LogInExternalGoogle logInExternal, CancellationToken cancellationToken = default)
    {
        if (this.authTransientRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.authTransientRepository
            .LogInExternalAsync(logInExternal, cancellationToken);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Signs in a user via external Microsoft authentication using the auth-code flow.
    /// </summary>
    /// <param name="logInExternal">The external login credentials.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The generated <see cref="AccessToken"/>.</returns>
    /// <response code="200">Authentication succeeded and token returned.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN_EXTERNAL_MICROSOFT)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalMicrosoftAsync([FromBody][Required] LogInExternalMicrosoft logInExternal, CancellationToken cancellationToken = default)
    {
        if (this.identityAuthRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.identityAuthRepository
            .LogInExternalAsync(logInExternal, cancellationToken);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Signs in a transient user via external Microsoft authentication using the auth-code flow.
    /// </summary>
    /// <param name="logInExternal">The external login credentials.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The generated <see cref="AccessToken"/>.</returns>
    /// <response code="200">Authentication succeeded and token returned.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN_EXTERNAL_MICROSOFT_TRANSIENT)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalMicrosoftTransientAsync([FromBody][Required] LogInExternalMicrosoft logInExternal, CancellationToken cancellationToken = default)
    {
        if (this.authTransientRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.authTransientRepository
            .LogInExternalAsync(logInExternal, cancellationToken);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Refreshes an existing access token for a user.
    /// </summary>
    /// <param name="logInRefresh">The refresh token request.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The refreshed <see cref="AccessToken"/>.</returns>
    /// <response code="200">Token refreshed successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN_REFRESH)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInRefreshAsync([FromBody][Required] LogInRefresh logInRefresh, CancellationToken cancellationToken = default)
    {
        if (this.identityAuthRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.identityAuthRepository
            .LogInRefreshAsync(logInRefresh, cancellationToken);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Logs out the current user and clears external authentication cookies.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>Returns HTTP 200 on success.</returns>
    /// <response code="200">Logout succeeded.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGOUT)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogOutAsync(CancellationToken cancellationToken = default)
    {
        if (this.identityAuthRepository == null)
        {
            return this.NotFound();
        }

        await this.identityAuthRepository
            .LogOutAsync(cancellationToken);

        await this.HttpContext
            .SignOutAsync(IdentityConstants.ExternalScheme);

        return this.Ok();
    }

    #endregion


    #region External Data

    /// <summary>
    /// Retrieves all configured external authentication schemes, e.g., Google, Facebook.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>A collection of external authentication providers.</returns>
    /// <response code="200">External providers returned successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="404">No external providers found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [Route(ActionRoutes.AUTH_EXTERNAL_SCHEMES)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<ExternalLoginProvider>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetExternalSchemesAsync(CancellationToken cancellationToken = default)
    {
        if (this.identityAuthRepository == null && this.authTransientRepository == null)
        {
            return this.NotFound();
        }

        IEnumerable<ExternalLoginProvider>? logInProviders = null;

        if (this.identityAuthRepository == null)
        {
            if (this.authTransientRepository != null)
            {
                logInProviders = await this.authTransientRepository
                    .GetExternalProviderSchemesAsync(cancellationToken);
            }
        }
        else
        {
            var authenticationSchemes = await this.identityAuthRepository
                .GetExternalProviderSchemesAsync(cancellationToken);

            logInProviders = authenticationSchemes
                .Select(x => new ExternalLoginProvider
                {
                    Name = x.Name,
                    DisplayName = x.DisplayName
                });
        }

        if (logInProviders == null)
        {
            return this.NotFound();
        }

        return this.Ok(logInProviders);
    }

    /// <summary>
    /// Retrieves external login data from Facebook authentication provider.
    /// </summary>
    /// <param name="externalLoginProvider">Facebook authentication provider info.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The <see cref="ExternalLogInData"/>.</returns>
    /// <response code="200">Data retrieved successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="404">Data not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_EXTERNAL_FACEBOOK_DATA)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetExternalLoginDataFaceBookAsync([FromBody][Required] ExternalLoginProviderFacebook externalLoginProvider, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            return this.NotFound();
        }

        var externalLoginData = await this.authExternalRepository
            .AuthenticateAsync(externalLoginProvider, cancellationToken);

        return this.Ok(externalLoginData);
    }

    /// <summary>
    /// Retrieves external login data from Google authentication provider.
    /// </summary>
    /// <param name="externalLoginProvider">Google authentication provider info.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The <see cref="ExternalLogInData"/>.</returns>
    /// <response code="200">Data retrieved successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="404">Data not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_EXTERNAL_GOOGLE_DATA)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetExternalLoginDataGoogleAsync([FromBody][Required] ExternalLoginProviderGoogle externalLoginProvider, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            return this.NotFound();
        }

        var externalLoginData = await this.authExternalRepository
            .AuthenticateAsync(externalLoginProvider, cancellationToken);

        return this.Ok(externalLoginData);
    }

    /// <summary>
    /// Retrieves external login data from Microsoft authentication provider.
    /// </summary>
    /// <param name="externalLoginProvider">Microsoft authentication provider info.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The <see cref="ExternalLogInData"/>.</returns>
    /// <response code="200">Data retrieved successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="404">Data not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_EXTERNAL_MICROSOFT_DATA)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetExternalLoginDataMicrosoftAsync([FromBody][Required] ExternalLoginProviderMicrosoft externalLoginProvider, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            return this.NotFound();
        }

        var externalLoginData = await this.authExternalRepository
            .AuthenticateAsync(externalLoginProvider, cancellationToken);

        return this.Ok(externalLoginData);
    }

    #endregion
}