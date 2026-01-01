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
using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Consts;
using Nano.App.ApiClient.Models.Identity;
using Nano.App.Web.Config;
using Nano.App.Web.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;

namespace Nano.App.Web.Controllers;

/// <summary>
/// Auth Controller.
/// </summary>
[Route(ControllerRoutes.AUTH_CONTROLLER_ROUTE)]
[Route($"v{{v:apiVersion}}/{ControllerRoutes.AUTH_CONTROLLER_ROUTE}")]
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.CREATOR + "," + BuiltInUserRoles.EDITOR + "," + BuiltInUserRoles.DELETER + "," + BuiltInUserRoles.READER)]
public abstract class BaseAuthController<TIdentity> : BaseController
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Auth Repository.
    /// </summary>
    protected virtual IOptionsMonitor<WebOptions> Options { get; }

    /// <summary>
    /// Auth Repository.
    /// </summary>
    protected virtual IAuthRepository<TIdentity> AuthRepository { get; }

    /// <summary>
    /// 
    /// </summary>
    protected virtual IAuthTransientRepository AuthTransientRepository { get; }

    /// <summary>
    ///
    /// </summary>
    protected virtual IAuthExternalRepository AuthExternalRepository { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="options"></param>
    /// <param name="authRepository"></param>
    /// <param name="authTransientRepository">The <see cref="IAuthTransientRepository"/>.</param>
    /// <param name="authExternalRepository"></param>
    protected BaseAuthController(ILogger logger, IOptionsMonitor<WebOptions> options, IAuthRepository<TIdentity> authRepository = null, IAuthTransientRepository authTransientRepository = null, IAuthExternalRepository authExternalRepository = null)
        : base(logger)
    {
        this.Options = options;
        this.AuthRepository = authRepository;
        this.AuthTransientRepository = authTransientRepository;
        this.AuthExternalRepository = authExternalRepository;
    }

    #region Login

    /// <summary>
    /// Authenticates and signs in a user.
    /// On success a jwt-token is created and returned, for use with auhtorization.
    /// </summary>
    /// <param name="logIn">The login model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInAsync([FromBody][Required]LogIn logIn, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthRepository
            .LogInAsync(logIn, this.Options.CurrentValue.Identity.Authentication.Jwt.RefreshExpirationInHours, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Authenticates and signs in the root user from configuration.
    /// On success a jwt-token is created and returned, for use with auhtorization.
    /// </summary>
    /// <param name="logInRoot">The login model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login/root")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInRootAsync([FromBody][Required]LogInRoot logInRoot, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthTransientRepository
            .LogInRootTransientAsync(logInRoot);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Refreshes a user's token.
    /// </summary>
    /// <param name="logInRefresh">The login refresh.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login/refresh")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInRefreshAsync([FromBody][Required] LogInRefresh logInRefresh, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthRepository
            .LogInRefreshAsync(logInRefresh, this.Options.CurrentValue.Identity.Authentication.Jwt.RefreshExpirationInHours, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Sign-in a user, from data received from a separate authentication.
    /// </summary>
    /// <param name="logInExternalDirect">The external login direct.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login/external/direct")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalDirectAsync([FromBody][Required] LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthRepository
            .LogInExternalDirectAsync(logInExternalDirect, this.Options.CurrentValue.Identity.Authentication.Jwt.RefreshExpirationInHours, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Sign-in a user, from data received from a separate authentication.
    /// </summary>
    /// <param name="logInExternalDirect">The external login direct.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login/external/direct/transient")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalDirectTransientAsync([FromBody][Required] LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthTransientRepository
            .LogInExternalTransientAsync(logInExternalDirect.ExternalLogInData, logInExternalDirect.TransientRoles, logInExternalDirect.TransientClaims, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Sign-in a user, from external Google authentication using auth-code flow.
    /// </summary>
    /// <param name="logInExternal">The external login.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login/external/google")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalGoogleAsync([FromBody][Required] LogInExternalGoogle logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthRepository
            .LogInExternalAsync(logInExternal, this.Options.CurrentValue.Identity.Authentication.Jwt.RefreshExpirationInHours, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Sign-in a user transient, from external Google authentication using auth-code flow.
    /// </summary>
    /// <param name="logInExternal">The external login.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login/external/google/transient")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalGoogleTransientAsync([FromBody][Required] LogInExternalGoogle logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthTransientRepository
            .LogInExternalTransientAsync(logInExternal, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Sign-in a user transient, from external Facebook authentication using auth-code flow.
    /// </summary>
    /// <param name="logInExternal">The external login.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login/external/facebook")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalFacebookAsync([FromBody][Required] LogInExternalFacebook logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthRepository
            .LogInExternalAsync(logInExternal, this.Options.CurrentValue.Identity.Authentication.Jwt.RefreshExpirationInHours, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Sign-in a user transient, from external Facebook authentication using auth-code flow.
    /// </summary>
    /// <param name="logInExternal">The external login.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login/external/facebook/transient")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalFacebookTransientAsync([FromBody][Required] LogInExternalFacebook logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthTransientRepository
            .LogInExternalTransientAsync(logInExternal, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Sign-in a user, from external Microsoft authentication using auth-code flow.
    /// </summary>
    /// <param name="logInExternal">The external login.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login/external/microsoft")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalMicrosoftAsync([FromBody][Required] LogInExternalMicrosoft logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthRepository
            .LogInExternalAsync(logInExternal, this.Options.CurrentValue.Identity.Authentication.Jwt.RefreshExpirationInHours, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Sign-in a user transient, from external Microsoft authentication using auth-code flow.
    /// </summary>
    /// <param name="logInExternal">The external login.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login/external/microsoft/transient")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInExternalMicrosoftTransientAsync([FromBody][Required] LogInExternalMicrosoft logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthTransientRepository
            .LogInExternalTransientAsync(logInExternal, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Refreshes a user's external token and token.
    /// </summary>
    /// <param name="logInExternalTransientRefresh">The login refresh.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("login/external/refresh")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInRefreshExternalAsync([FromBody][Required]LogInExternalTransientRefresh logInExternalTransientRefresh, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.AuthTransientRepository
            .LogInExternalTransientRefreshAsync(logInExternalTransientRefresh, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Logs out the user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("logout")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogOutAsync(CancellationToken cancellationToken = default)
    {
        await this.AuthRepository
            .SignOutAsync(cancellationToken);

        await this.HttpContext
            .SignOutAsync(IdentityConstants.ExternalScheme);

        return this.Ok();
    }

    #endregion


    #region External Data

    /// <summary>
    /// Gets all the configured external authentication schemes.
    /// E.g. Google, Facebook, etc.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The external authentication schemes.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpGet]
    [Route("external/schemes")]
    [AllowAnonymous]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<ExternalLoginProvider>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetExternalSchemesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<ExternalLoginProvider> logInProviders;

        if (this.AuthRepository == null)
        {
            logInProviders = await this.AuthTransientRepository
                .GetExternalProviderSchemesAsync(cancellationToken);
        }
        else
        {
            var authenticationSchemes = await this.AuthRepository
                .GetExternalProviderSchemesAsync(cancellationToken);

            logInProviders = authenticationSchemes?
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
    /// Get external login data from an external Google authentication provider.
    /// </summary>
    /// <param name="externalLoginProvider">The external login provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The external login data.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("external/google/data")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetExternalLoginDataGoogleAsync([FromBody][Required] ExternalLoginProviderGoogle externalLoginProvider, CancellationToken cancellationToken = default)
    {
        var externalLoginData = await this.AuthExternalRepository
            .Authenticate(externalLoginProvider, cancellationToken);

        if (externalLoginData == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(externalLoginData);
    }

    /// <summary>
    /// Get external login data from an external Facebook authentication provider.
    /// </summary>
    /// <param name="externalLoginProvider">The external login provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The external login data.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("external/facebook/data")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetExternalLoginDataFaceBookAsync([FromBody][Required] ExternalLoginProviderFacebook externalLoginProvider, CancellationToken cancellationToken = default)
    {
        var externalLoginData = await this.AuthExternalRepository
            .Authenticate(externalLoginProvider, cancellationToken);

        if (externalLoginData == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(externalLoginData);
    }

    /// <summary>
    /// Get external login data from an external Microsoft authentication provider.
    /// </summary>
    /// <param name="externalLoginProviderMicrosoft">The external login provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The external login data.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("external/microsoft/data")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetExternalLoginDataMicrosoftAsync([FromBody][Required] ExternalLoginProviderMicrosoft externalLoginProviderMicrosoft, CancellationToken cancellationToken = default)
    {
        var externalLoginData = await this.AuthExternalRepository
            .Authenticate(externalLoginProviderMicrosoft, cancellationToken);

        if (externalLoginData == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(externalLoginData);
    }

    #endregion
}