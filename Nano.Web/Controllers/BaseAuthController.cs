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
        /// <param name="login">The login model.</param>
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
        public virtual async Task<IActionResult> LogInAsync([FromBody][Required]Login login, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInAsync(login, cancellationToken);

            if (accessToken == null)
                this.NotFound();

            return this.Ok(accessToken);
        }

        /// <summary>
        /// Refreshes a users authentication token.
        /// </summary>
        /// <param name="loginRefresh">The login refresh model.</param>
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
        public virtual async Task<IActionResult> LoginRefreshAsync([FromBody][Required]LoginRefresh loginRefresh, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInRefreshAsync(loginRefresh, cancellationToken);

            if (accessToken == null)
                this.NotFound();

            return this.Ok(accessToken);
        }

        /// <summary>
        /// Sign-in a user from an external login and authentication.
        /// </summary>
        /// <param name="loginExternal">The external login.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The external login response.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("login/external")]
        [AllowAnonymous]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(ExternalLoginResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> SignInExternalAsync([FromBody][Required]LoginExternal loginExternal, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInExternalAsync(loginExternal, cancellationToken);

            if (accessToken == null)
                this.NotFound();

            return this.Ok(accessToken);
        }

        /// <summary>
        /// Sign-in a user transiently from an external login and authentication.
        /// No Identity backing-store is used.
        /// </summary>
        /// <param name="loginExternalTransient">The external login transient.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The external login response.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("login/external/transient")]
        [AllowAnonymous]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(ExternalLoginResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> SignInExternalTransientAsync([FromBody][Required]LoginExternalTransient loginExternalTransient, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.BaseIdentityManager
                .SignInExternalTransientAsync(loginExternalTransient, cancellationToken);

            if (accessToken == null)
                return this.NotFound();

            var response = new ExternalLoginResponse
            {
                AccessToken = accessToken
            };

            return this.Ok(response);
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
        [ProducesResponseType(typeof(LoginProvider), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Auth" })]
        public virtual async Task<IActionResult> GetExternalSchemesAsync(CancellationToken cancellationToken = default)
        {
            var externalLogins = await this.BaseIdentityManager
                .GetExternalProvidersAsync(cancellationToken);

            if (externalLogins == null)
                this.NotFound();

            return this.Ok(externalLogins);
        }
    }
}