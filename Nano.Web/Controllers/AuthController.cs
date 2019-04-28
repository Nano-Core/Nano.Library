using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nano.Models;
using Nano.Security;
using Nano.Security.Exceptions;
using Nano.Security.Models;
using Nano.Web.Hosting;
    
namespace Nano.Web.Controllers
{
    /// <summary>
    /// Auth Controller.
    /// </summary>
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
        public virtual async Task<IActionResult> LogIn([FromBody][Required]Login login, CancellationToken cancellationToken = default)
        {
            try
            {
                var accessToken = await this.IdentityManager
                    .SignInAsync(login, cancellationToken);

                return this.Ok(accessToken);
            }
            catch (UnauthorizedException ex)
            {
                return this.Unauthorized(ex.Message);
            }
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
        public virtual async Task<IActionResult> LogOut(CancellationToken cancellationToken = default)
        {
            await this.IdentityManager
                .SignOutAsync(cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Authenticates and signs in a user with external login.
        /// On success a jwt-token is created and returned, for later use with auhtorization.
        /// </summary>
        /// <param name="loginExternal">The login external request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("external/login")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> LogInExternal(LoginExternal loginExternal, CancellationToken cancellationToken = default)
        {
            try
            {
                var accessToken = await this.IdentityManager
                    .SignInExternalAsync(loginExternal, cancellationToken);

                return this.Ok(accessToken);
            }
            catch (UnauthorizedException ex)
            {
                return this.Unauthorized(ex.Message);
            }
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
        public virtual async Task<IActionResult> GetExternalSchemes(CancellationToken cancellationToken = default)
        {
            var externalLogins = await this.IdentityManager
                .GetExternalSchemesAsync(cancellationToken);

            return this.Ok(externalLogins);
        }
    }
}