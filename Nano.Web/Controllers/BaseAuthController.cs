using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Security;
using Nano.Security.Const;
using Nano.Security.Models;
using Nano.Web.Const;
using Nano.Web.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Nano.Web.Controllers
{
    /// <summary>
    /// Auth Controller.
    /// </summary>
    [Route("Auth")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.SERVICE + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.READER)]
    public abstract class BaseAuthController<TIdentity> : BaseController 
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Identity Manager.
        /// </summary>
        protected virtual BaseIdentityManager<TIdentity> BaseIdentityManager { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="baseIdentityManager">The <see cref="BaseIdentityManager"/>.</param>
        protected BaseAuthController(ILogger logger, BaseIdentityManager<TIdentity> baseIdentityManager)
            : base(logger)
        {
            this.BaseIdentityManager = baseIdentityManager ?? throw new ArgumentNullException(nameof(baseIdentityManager));
        }

        /// <summary>
        /// Options.
        /// Any route can be called with http options, to return options header information.
        /// </summary>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        [AllowAnonymous]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public override IActionResult Options()
        {
            return base.Options();
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
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new [] { "Auth" })]
        public virtual async Task<IActionResult> LogInAsync([FromBody][Required]LogIn logIn, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInAsync(logIn, cancellationToken);

            if (accessToken == null)
            {
                this.Unauthorized();
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
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> LogInRefreshAsync([FromBody][Required]LogInRefresh logInRefresh, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInRefreshAsync(logInRefresh, cancellationToken);

            if (accessToken == null)
            {
                this.Unauthorized();
            }

            return this.Ok(accessToken);
        }

        /// <summary>
        /// Sign-in a user transient, from data recieved from a separate authentication.
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> LogInExternalDirectAsync([FromBody][Required]LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInExternalAsync(logInExternalDirect, cancellationToken);

            if (accessToken == null)
            {
                this.Unauthorized();
            }

            return this.Ok(accessToken);
        }

        /// <summary>
        /// Sign-in a user, from data recieved from a separate authentication.
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> LogInExternalDirectTransientAsync([FromBody][Required]LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInExternalTransientAsync(logInExternalDirect.ExternalLogInData, logInExternalDirect.TransientRoles, logInExternalDirect.TransientClaims, cancellationToken);

            if (accessToken == null)
            {
                this.Unauthorized();
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> LogInExternalGoogleAsync([FromBody][Required]LogInExternalGoogle logInExternal, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInExternalAsync(logInExternal, cancellationToken);

            if (accessToken == null)
            {
                this.Unauthorized();
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> LogInExternalGoogleTransientAsync([FromBody][Required]LogInExternalGoogle logInExternal, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInExternalTransientAsync(logInExternal, cancellationToken);

            if (accessToken == null)
            {
                this.Unauthorized();
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> LogInExternalFacebookAsync([FromBody][Required]LogInExternalFacebook logInExternal, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInExternalAsync(logInExternal, cancellationToken);

            if (accessToken == null)
            {
                this.Unauthorized();
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> LogInExternalFacebookTransientAsync([FromBody][Required]LogInExternalFacebook logInExternal, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInExternalTransientAsync(logInExternal, cancellationToken);

            if (accessToken == null)
            {
                this.Unauthorized();
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> LogInExternalMicrosoftAsync([FromBody][Required]LogInExternalMicrosoft logInExternal, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInExternalAsync(logInExternal, cancellationToken);

            if (accessToken == null)
            {
                this.Unauthorized();
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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> LogInExternalMicrosoftTransientAsync([FromBody][Required]LogInExternalMicrosoft logInExternal, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInExternalTransientAsync(logInExternal, cancellationToken);

            if (accessToken == null)
            {
                this.Unauthorized();
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
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> LogOutAsync(CancellationToken cancellationToken = default)
        {
            await this.BaseIdentityManager
                .SignOutAsync(cancellationToken);

            await this.HttpContext
                .SignOutAsync(IdentityConstants.ExternalScheme);

            return this.Ok();
        }

        /// <summary>
        /// Get external login data from an external Google authentication provider.
        /// </summary>
        /// <param name="logInExternalProvider">The external login provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The external login data.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("external/google/data")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> GetExternalLoginData([FromBody][Required]LogInExternalProviderGoogle logInExternalProvider, CancellationToken cancellationToken = default)
        {
            var externalLoginData = await this.BaseIdentityManager
                .GetExternalProviderLogInData(logInExternalProvider, cancellationToken);

            if (externalLoginData == null)
            {
                this.Unauthorized();
            }

            return this.Ok(externalLoginData);
        }

        /// <summary>
        /// Get external login data from an external Facebook authentication provider.
        /// </summary>
        /// <param name="logInExternalProvider">The external login provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The external login data.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("external/facebook/data")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> GetExternalLoginData([FromBody][Required]LogInExternalProviderFacebook logInExternalProvider, CancellationToken cancellationToken = default)
        {
            var externalLoginData = await this.BaseIdentityManager
                .GetExternalProviderLogInData(logInExternalProvider, cancellationToken);

            if (externalLoginData == null)
            {
                this.Unauthorized();
            }

            return this.Ok(externalLoginData);
        }

        /// <summary>
        /// Get external login data from an external Microsoft authentication provider.
        /// </summary>
        /// <param name="logInExternalProvider">The external login provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The external login data.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("external/microsoft/data")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(ExternalLogInData), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> GetExternalLoginData([FromBody][Required]LogInExternalProviderMicrosoft logInExternalProvider, CancellationToken cancellationToken = default)
        {
            var externalLoginData = await this.BaseIdentityManager
                .GetExternalProviderLogInData(logInExternalProvider, cancellationToken);

            if (externalLoginData == null)
            {
                this.Unauthorized();
            }

            return this.Ok(externalLoginData);
        }

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
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(LogInProvider), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> GetExternalSchemesAsync(CancellationToken cancellationToken = default)
        {
            var logInProviders = await this.BaseIdentityManager
                .GetExternalProviderSchemesAsync(cancellationToken);

            if (logInProviders == null)
            {
                this.NotFound();
            }

            return this.Ok(logInProviders);
        }
    }
}