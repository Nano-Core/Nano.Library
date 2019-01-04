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
using Nano.Models.Auth;
using Nano.Security;
using Nano.Web.Hosting;
    
namespace Nano.Web.Controllers
{
    /// <summary>
    /// Auth Controller.
    /// </summary>
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
        /// Authenticates and signs in a user.
        /// On success a jwt-token is created and returned, for later use with auhtorization.
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
        public virtual async Task<IActionResult> Login([FromBody][Required]Login login, CancellationToken cancellationToken = default)
        {
            try
            {
                var accessToken = await this.SecurityManager
                    .SignInAsync(login, cancellationToken);

                return this.Ok(accessToken);
            }
            catch
            {
                return this.Unauthorized();
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
        [HttpGet]
        [HttpPost]
         [Route("logout")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
        {
            await this.SecurityManager
                .SignOutAsync(cancellationToken);

            await HttpContext
                .SignOutAsync(IdentityConstants.ExternalScheme);

            return this.Ok();
        }

        /// <summary>
        /// Signs up a user for an account.
        /// </summary>
        /// <param name="signup">The signup.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created user.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("signup")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(IdentityUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Signup([FromBody][Required]Signup signup, CancellationToken cancellationToken = default)
        {
            var user = await this.SecurityManager
                .SignupAsync(signup, cancellationToken);

            return this.Created("Signup", user);
        }

        /// <summary>
        /// Gets the external logins, that is currently assoicated with a user.
        /// </summary>
        /// <param name="getExternalLogins">The get external logins</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        /// <returns></returns>
        [HttpGet]
        [Route("external/logins")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(LoginExternalProvider), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetExternalLogins([FromBody][Required]GetExternalLogins getExternalLogins, CancellationToken cancellationToken = default)
        {
            var logins = await this.SecurityManager
                .GetExternalLoginsAsync(getExternalLogins, cancellationToken);

            return this.Ok(logins);
        }

        /// <summary>
        /// Gets all the configured external logins, that is available.
        /// E.g. Google, Facebook, etc.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The collection of external login providers.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="500">Error occurred.</response>
        [HttpGet]
        [Route("external/providers")]
        [AllowAnonymous]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(LoginExternalProvider), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> GetExternalLogins(CancellationToken cancellationToken = default)
        {
            var externalLogins = await this.SecurityManager
                .GetExternalLoginsAsync(cancellationToken);

            return this.Ok(externalLogins);
        }

        /// <summary>
        /// Authenticates and signs in a user, using an external login provider.
        /// A callback is invoked on successful login, completing the authentication.
        /// </summary>
        /// <param name="loginExternal">The external login.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The challange result.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("external/login")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(ChallengeResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> LoginExternal([FromBody][Required]LoginExternal loginExternal, CancellationToken cancellationToken = default)
        {
            await this.Logout(cancellationToken);

            var properties = await this.SecurityManager
                .GetExternalLoginsPropertiesAsync(loginExternal, cancellationToken);

            return new ChallengeResult(loginExternal.Name, properties);
        }

        /// <summary>
        /// Completes authentication using external login provider.
        /// On success a jwt-token is created and returned, for later use with auhtorization.
        /// </summary>
        /// <param name="loginExternalCallback">The login external callback.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The identity user.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("external/login/callback")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(IdentityUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> LoginExternalCallback([FromBody][Required]LoginExternalCallback loginExternalCallback, CancellationToken cancellationToken = default)
        {
            try
            {
                var accessToken = await this.SecurityManager
                    .SignInExternalAsync(loginExternalCallback, cancellationToken);

                return this.Ok(accessToken);
            }
            catch
            {
                return this.Unauthorized();
            }
        }

        /// <summary>
        /// Creaes a user account for a user, that is logged in, using external logn provider.
        /// </summary>
        /// <param name="signupExternal">The sign-up external.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The identity user.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("external/signup")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(IdentityUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SignUpExternal([FromBody][Required]SignupExternal signupExternal, CancellationToken cancellationToken = default)
        {
            var user = await this.SecurityManager
                .SignUpExternalAsync(signupExternal, cancellationToken);

            return this.Created("Signup", user);
        }
         
        /// <summary>
        /// Removes an external login.
        /// </summary>
        /// <param name="removeExternalLogin">the remove external login.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("external/remove")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> RemoveExternalLogin(RemoveExternalLogin removeExternalLogin, CancellationToken cancellationToken = default)
        {
            await this.SecurityManager
                .RemoveExternalLoginAsync(removeExternalLogin, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Sets / changes the username of a user.
        /// </summary>
        /// <param name="setUsername">The set username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Nothing (Void).</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("username/set")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SetPassword([FromBody][Required]SetUsername setUsername, CancellationToken cancellationToken = default)
        {
            await this.SecurityManager
                .SetUsernameAsync(setUsername, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Sets the password of a user that has logged in with an external provider.
        /// Faulure when the user already has a password set. use a´'Change Password' instead. 
        /// </summary>
        /// <param name="setPassword">The set password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Nothing (Void).</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("password/set")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SetPassword([FromBody][Required]SetPassword setPassword, CancellationToken cancellationToken = default)
        {
            await this.SecurityManager
                .SetPasswordAsync(setPassword, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="changePassword">The change password request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("password/change")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> ChangePasswordAsync([FromBody][Required]ChangePassword changePassword, CancellationToken cancellationToken = default)
        {
            await this.SecurityManager
                .ChangePasswordAsync(changePassword, cancellationToken);
     
            return this.Ok();
        }

        /// <summary>
        /// Gets the forgot password code, used to reset the password of a user.
        /// </summary>
        /// <param name="getResetPassword">The get reset password request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("password/forgot")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(ResetPassword), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> GetResetPasswordAsync([FromBody][Required]GetResetPassword getResetPassword, CancellationToken cancellationToken = default)
        {
            var resetPassword = await this.SecurityManager
                .GetResetPasswordAsync(getResetPassword, cancellationToken);

            return this.Ok(resetPassword);
        }

        /// <summary>
        /// Resets the password of a user.
        /// </summary>
        /// <param name="resetPassword">The reset password request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("password/reset")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> ResetPasswordAsync([FromBody][Required]ResetPassword resetPassword, CancellationToken cancellationToken = default)
        {
             await this.SecurityManager
                .ResetPasswordAsync(resetPassword, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Sends an email confirmation to a user.
        /// </summary>
        /// <param name="getConfirmEmail">The get confirm email request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("email/sendconfirm")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SendEmailConfirmation([FromBody][Required]GetConfirmEmail getConfirmEmail, CancellationToken cancellationToken = default)
        {
            await this.SecurityManager
                .GetConfirmEmailAsync(getConfirmEmail, cancellationToken);

            // BUG: Email confirmation event.
            //var callbackUrl = Url.Page("/Account/ConfirmEmail", pageHandler: null, values: new { userId = user.Id, code = code }, protocol: Request.Scheme);
            //await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return this.Ok();
        }

        /// <summary>
        /// Confirms an email of a user.
        /// </summary>
        /// <param name="confirmEmail">The confirm email request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("email/confirm")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> ConfirmEmail([FromBody][Required]ConfirmEmail confirmEmail, CancellationToken cancellationToken = default)
        {
            await this.SecurityManager
                .ConfirmEmailAsync(confirmEmail, cancellationToken);

            return this.Ok();
        }
    }
}