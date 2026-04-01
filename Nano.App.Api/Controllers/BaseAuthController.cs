using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.App.ApiClient.Requests.Auth.Models;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Controllers;

/// <inheritdoc />
public abstract class BaseAuthController(ILogger<BaseAuthController> logger, IAuthRepository<Guid> authRepository)
    : BaseAuthController<Guid>(logger, authRepository);

/// <summary>
/// Base controller for authentication-related operations.
/// Handles login, logout, token refresh, and external authentication.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
[Route(ControllerRoutes.AUTH)]
[Route($"{ControllerRoutes.ROUTE_VERSION_PREFIX}/{ControllerRoutes.AUTH}")]
[AllowAnonymous]
public abstract class BaseAuthController<TIdentity>(ILogger<BaseAuthController<TIdentity>> logger, IAuthRepository<TIdentity> authRepository)
    : BaseController(logger)
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Get or set the authentication repository.
    /// </summary>
    protected readonly IAuthRepository<TIdentity> authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));

    #region Login

    /// <summary>
    /// Retrieves all configured external authentication methods, e.g., Google, Facebook.
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
        await Task.CompletedTask;

        if (this.authRepository.AuthIdentityRepository == null && this.authRepository.AuthTransientRepository == null)
        {
            return this.NotFound();
        }

        var externalLoginProviders = this.authRepository.AuthExternalRepositoryAggregator
            .GetExternalProviderSchemes(cancellationToken);

        return this.Ok(externalLoginProviders);
    }

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
        if (this.authRepository.AuthIdentityRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.authRepository.AuthIdentityRepository
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
        if (this.authRepository.AuthRootRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.authRepository.AuthRootRepository
            .LogInRootAsync(logInRoot);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Authenticates the api-key and returns an access token.
    /// </summary>
    /// <param name="logInApiKey">The api-key login.</param>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>The generated <see cref="AccessToken"/>.</returns>
    /// <response code="200">Authentication succeeded and token returned.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Authentication failed.</response>
    /// <response code="404">Root user not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGIN_API_KEY)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogInApiKeyAsync([FromBody][Required] LogInApiKey logInApiKey, CancellationToken cancellationToken = default)
    {
        if (this.authRepository.AuthIdentityRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.authRepository.AuthIdentityRepository
            .LogInApiKeyAsync(logInApiKey, cancellationToken);

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
        if (this.authRepository.AuthIdentityRepository == null)
        {
            return this.NotFound();
        }

        var accessToken = await this.authRepository.AuthIdentityRepository
            .LogInRefreshAsync(logInRefresh, cancellationToken);

        return this.Ok(accessToken);
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>Returns HTTP 200 on success.</returns>
    /// <response code="200">Logout succeeded.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [Route(ActionRoutes.AUTH_LOGOUT)]
    [Authorize]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> LogOutAsync(CancellationToken cancellationToken = default)
    {
        if (this.authRepository.AuthIdentityRepository == null)
        {
            return this.NotFound();
        }

        var appId = this.HttpContext
            .GetJwtAppId();

        var userId = this.HttpContext
            .GetJwtUserId();

        if (appId == null || userId == null)
        {
            return this.NotFound();
        }

        var userIdConverted = userId
            .ConvertToIdentity<TIdentity>();

        await this.authRepository.AuthIdentityRepository
            .LogOutAsync(userIdConverted, appId, cancellationToken);

        return this.Ok();
    }

    #endregion
}