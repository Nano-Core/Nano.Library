using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Consts;
using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Models;
using Nano.Models.Const;
using Nano.Web.Controllers;
using Swashbuckle.AspNetCore.Annotations;

namespace Nano.App.Web.Controllers;

/// <summary>
/// Auth Controller.
/// </summary>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.SERVICE + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.READER)]
public abstract class BaseAuthController<TIdentity> : BaseController
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Identity Auth Repository.
    /// </summary>
    protected virtual IIdentityAuthRepository<TIdentity> IdentityAuthRepository { get; }

    /// <summary>
    /// Identity Transient Repository.
    /// </summary>
    protected virtual IIdentityAuthTransientRepository<TIdentity> IdentityAuthTransientRepository { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="identityAuthRepository"></param>
    /// <param name="identityTransientRepository">The <see cref="IIdentityAuthTransientRepository{TIdentity}"/>.</param>
    protected BaseAuthController(ILogger logger, IIdentityAuthRepository<TIdentity> identityAuthRepository = null, IIdentityAuthTransientRepository<TIdentity> identityTransientRepository = null)
        : base(logger)
    {
        this.IdentityAuthRepository = identityAuthRepository;
        this.IdentityAuthTransientRepository = identityTransientRepository;
    }

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
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> LogInAsync([FromBody][Required]LogIn logIn, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.IdentityAuthRepository
            .LogInAsync(logIn, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    // BUG: Login Root Transient

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
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> GetExternalSchemesAsync(CancellationToken cancellationToken = default)
    {
        var logInProviders = await this.IdentityAuthTransientRepository
            .GetExternalProviderSchemesAsync(cancellationToken);

        if (logInProviders == null)
        {
            return this.NotFound();
        }

        return this.Ok(logInProviders);
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
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> LogInExternalDirectAsync([FromBody][Required] LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.IdentityAuthRepository
            .LogInExternalDirectAsync(logInExternalDirect, cancellationToken);

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
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> LogInExternalDirectTransientAsync([FromBody][Required] LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.IdentityAuthTransientRepository
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
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> LogInExternalGoogleAsync([FromBody][Required] LogInExternalGoogle logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.IdentityAuthRepository
            .LogInExternalAsync(logInExternal, cancellationToken);

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
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> LogInExternalGoogleTransientAsync([FromBody][Required] LogInExternalGoogle logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.IdentityAuthTransientRepository
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
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> LogInExternalFacebookAsync([FromBody][Required] LogInExternalFacebook logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.IdentityAuthRepository
            .LogInExternalAsync(logInExternal, cancellationToken);

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
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> LogInExternalFacebookTransientAsync([FromBody][Required] LogInExternalFacebook logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.IdentityAuthTransientRepository
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
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> LogInExternalMicrosoftAsync([FromBody][Required] LogInExternalMicrosoft logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.IdentityAuthRepository
            .LogInExternalAsync(logInExternal, cancellationToken);

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
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> LogInExternalMicrosoftTransientAsync([FromBody][Required] LogInExternalMicrosoft logInExternal, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.IdentityAuthTransientRepository
            .LogInExternalTransientAsync(logInExternal, cancellationToken);

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
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> LogInRefreshAsync([FromBody][Required] LogInRefresh logInRefresh, CancellationToken cancellationToken = default)
    {
        var accessToken = await this.IdentityAuthRepository
            .LogInRefreshAsync(logInRefresh, cancellationToken);

        if (accessToken == null)
        {
            return this.Unauthorized();
        }

        return this.Ok(accessToken);
    }

    // BUG: Refresh External Transient

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
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    public virtual async Task<IActionResult> LogOutAsync(CancellationToken cancellationToken = default)
    {
        await this.IdentityAuthRepository
            .SignOutAsync(cancellationToken);

        await this.HttpContext
            .SignOutAsync(IdentityConstants.ExternalScheme);

        return this.Ok();
    }

    // BUG: Do we need GetExternalLoginData exposed?
    ///// <summary>
    ///// Get external login data from an external Google authentication provider.
    ///// </summary>
    ///// <param name="externalLoginProvider">The external login provider.</param>
    ///// <param name="cancellationToken">The cancellation token.</param>
    ///// <returns>The external login data.</returns>
    ///// <response code="200">Success.</response>
    ///// <response code="400">Bad Request.</response>
    ///// <response code="404">Not Found.</response>
    ///// <response code="500">Error occurred.</response>
    //[HttpPost]
    //[Route("external/google/data")]
    //[AllowAnonymous]
    //[Consumes(HttpContentType.JSON)]
    //[Produces(HttpContentType.JSON)]
    //[ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
    //[ProducesResponseType((int)HttpStatusCode.NotFound)]
    //[ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    //[ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    //[SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    //public virtual async Task<IActionResult> GetExternalLoginData([FromBody][Required] ExternalLoginProviderGoogle externalLoginProvider, CancellationToken cancellationToken = default)
    //{
    //    var externalLoginData = await this.IdentityRepository
    //        .GetExternalProviderLogInData(externalLoginProvider, cancellationToken);

    //    if (externalLoginData == null)
    //    {
    //        return this.Unauthorized();
    //    }

    //    return this.Ok(externalLoginData);
    //}

    ///// <summary>
    ///// Get external login data from an external Facebook authentication provider.
    ///// </summary>
    ///// <param name="externalLoginProvider">The external login provider.</param>
    ///// <param name="cancellationToken">The cancellation token.</param>
    ///// <returns>The external login data.</returns>
    ///// <response code="200">Success.</response>
    ///// <response code="400">Bad Request.</response>
    ///// <response code="404">Not Found.</response>
    ///// <response code="500">Error occurred.</response>
    //[HttpPost]
    //[Route("external/facebook/data")]
    //[AllowAnonymous]
    //[Consumes(HttpContentType.JSON)]
    //[Produces(HttpContentType.JSON)]
    //[ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
    //[ProducesResponseType((int)HttpStatusCode.NotFound)]
    //[ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    //[ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    //[SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    //public virtual async Task<IActionResult> GetExternalLoginData([FromBody][Required] ExternalLoginProviderFacebook externalLoginProvider, CancellationToken cancellationToken = default)
    //{
    //    var externalLoginData = await this.IdentityRepository
    //        .GetExternalProviderLogInData(externalLoginProvider, cancellationToken);

    //    if (externalLoginData == null)
    //    {
    //        return this.Unauthorized();
    //    }

    //    return this.Ok(externalLoginData);
    //}

    ///// <summary>
    ///// Get external login data from an external Microsoft authentication provider.
    ///// </summary>
    ///// <param name="externalLoginProviderMicrosoft">The external login provider.</param>
    ///// <param name="cancellationToken">The cancellation token.</param>
    ///// <returns>The external login data.</returns>
    ///// <response code="200">Success.</response>
    ///// <response code="400">Bad Request.</response>
    ///// <response code="404">Not Found.</response>
    ///// <response code="500">Error occurred.</response>
    //[HttpPost]
    //[Route("external/microsoft/data")]
    //[AllowAnonymous]
    //[Consumes(HttpContentType.JSON)]
    //[Produces(HttpContentType.JSON)]
    //[ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
    //[ProducesResponseType((int)HttpStatusCode.NotFound)]
    //[ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    //[ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    //[SwaggerOperation(Tags = [ControllerRoutes.AUTH_CONTROLLER_ROUTE])]
    //public virtual async Task<IActionResult> GetExternalLoginData([FromBody][Required] ExternalLoginProviderMicrosoft externalLoginProviderMicrosoft, CancellationToken cancellationToken = default)
    //{
    //    var externalLoginData = await this.IdentityRepository
    //        .GetExternalProviderLogInData(externalLoginProviderMicrosoft, cancellationToken);

    //    if (externalLoginData == null)
    //    {
    //        return this.Unauthorized();
    //    }

    //    return this.Ok(externalLoginData);
    //}
}