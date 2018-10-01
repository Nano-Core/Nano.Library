using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nano.Models;
using Nano.Security;
using Nano.Web.Hosting;
using Nano.Web.Hosting.Extensions;

namespace Nano.Web.Controllers
{
    /// <summary>
    /// Auth Controller.
    /// </summary>
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        /// <summary>
        /// User Manager.
        /// </summary>
        protected virtual SecurityManager SecurityManager { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="securityManager">The <see cref="SecurityManager"/>.</param>
        public AuthController(SecurityManager securityManager)
        {
            this.SecurityManager = securityManager ?? throw new ArgumentNullException(nameof(securityManager));
        }

        /// <summary>
        /// Gets the view for login.
        /// </summary>
        /// <returns>The 'login' view.</returns>
        [HttpGet]
        [Route("login")]
        [Produces(HttpContentType.HTML)]
        public virtual IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Gets the view for access denied.
        /// </summary>
        /// <returns>The 'forbidden' view.</returns>
        [HttpGet]
        [Route("forbidden")]
        [Produces(HttpContentType.HTML)]
        public virtual IActionResult Forbidden()
        {
            return View();
        }

        /// <summary>
        /// The user authenticates and on success recieves a jwt token for use with auhtorization.
        /// </summary>
        /// <param name="login">The login model.</param>
        /// <param name="returnUrl"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A jwt token.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("login")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Login([FromBody][Required]Login login, string returnUrl = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var accessToken = await this.SecurityManager
                    .SignInAsync(login);

                if (this.Request.IsContentTypeHtml())
                    return this.LocalRedirect(returnUrl);

                return this.Ok(accessToken);
            }
            catch (UnauthorizedAccessException)
            {
                if (this.Request.IsContentTypeHtml())
                    return this.RedirectToAction("forbidden");

                return this.Unauthorized();
            }
        }

        /// <summary>
        /// The user is logged out, and the token is invalidated.
        /// Usually, it's not needed to call this method, unless having a specific reason for invalidating a token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> (void).</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="500">Error occurred.</response>
        [HttpGet]
        [HttpPost]
        [Route("logout")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
        {
            await this.SecurityManager.SignOutAsync();

            if (this.Request.IsContentTypeHtml())
                return this.View();

            return this.Ok();
        }
    }
}