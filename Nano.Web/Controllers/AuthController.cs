using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nano.Models;
using Nano.Security;
using Nano.Security.Const;
using Nano.Security.Extensions;
using Nano.Security.Models;
using Nano.Web.Hosting;
    
namespace Nano.Web.Controllers
{
    /// <summary>
    /// Auth Controller.
    /// </summary>
    [Authorize(Roles = BuiltInUserRoles.Administrator + "," + BuiltInUserRoles.Service + "," + BuiltInUserRoles.Writer + "," + BuiltInUserRoles.Reader)]
    public class AuthController : BaseController
    {
        /// <summary>
        /// Security Manager.
        /// </summary>
        protected virtual IdentityManager IdentityManager { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="identityManager">The <see cref="IdentityManager"/>.</param>
        public AuthController(IdentityManager identityManager)
        {
            this.IdentityManager = identityManager ?? throw new ArgumentNullException(nameof(identityManager));
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
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> LogInAsync([FromBody][Required]Login login, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.IdentityManager
                .SignInAsync(login, cancellationToken);

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
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("login/refresh")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> LoginRefreshAsync([FromBody][Required]LoginRefresh loginRefresh, CancellationToken cancellationToken = default)
        {
            var jwtToken = this.HttpContext.GetJwtToken();

            var accessToken = await this.IdentityManager
                .SignInRefreshAsync(jwtToken, loginRefresh.RefreshToken, cancellationToken);

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
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("login/external")]
        [AllowAnonymous]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(ExternalLoginResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SignInExternalAsync([FromBody][Required]LoginExternal loginExternal, CancellationToken cancellationToken = default)
        {
            var accessToken = await this.IdentityManager
                .SignInExternalAsync(loginExternal, cancellationToken);

            ExternalLoginData externalLoginData = null;
            if (accessToken == null)
            {
                externalLoginData = await this.IdentityManager
                    .GetExternalProviderInfoAsync(loginExternal, cancellationToken);
            }

            var response = new ExternalLoginResponse
            {
                Data = externalLoginData,
                AccessToken = accessToken
            };

            return this.Ok(response);
        }

        /// <summary>
        /// Logs out a user.
        /// The jwt-token and any external login is invalidated.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Nothing.</returns>
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
        public virtual async Task<IActionResult> LogOutAsync(CancellationToken cancellationToken = default)
        {
            await this.IdentityManager
                .SignOutAsync(cancellationToken);

            await this.HttpContext
                .SignOutAsync(IdentityConstants.ExternalScheme);

            return this.Ok();
        }

        /// <summary>
        /// Gets all the configured external login schemes.
        /// E.g. Google, Facebook, etc.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection of external login providers.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="500">Error occurred.</response>
        [HttpGet]
        [Route("external/schemes")]
        [AllowAnonymous]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(LoginProvider), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> GetExternalSchemesAsync(CancellationToken cancellationToken = default)
        {
            var externalLogins = await this.IdentityManager
                .GetExternalSchemesAsync(cancellationToken);

            return this.Ok(externalLogins);
        }
    }
}