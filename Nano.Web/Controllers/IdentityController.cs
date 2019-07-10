using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Repository.Interfaces;
using Nano.Security;
using Nano.Security.Exceptions;
using Nano.Security.Models;
using Nano.Web.Hosting;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class IdentityController<TEntity, TCriteria> : DefaultControllerUpdatable<TEntity, TCriteria>
        where TEntity : DefaultEntityUser, new()
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <summary>
        /// Security Manager.
        /// </summary>
        protected virtual IdentityManager IdentityManager { get; }

        /// <inheritdoc />
        protected IdentityController(ILogger logger, IRepository repository, IEventing eventing, IdentityManager identityManager) 
            : base(logger, repository, eventing)
        {
            this.IdentityManager = identityManager ?? throw new ArgumentNullException(nameof(identityManager));
        }

        /// <summary>
        /// Signup.
        /// Registers a new user.
        /// </summary>
        /// <param name="signUp">The signuo request.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The created user.</returns>
        /// <response code="201">Created.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("signup")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SignUpAsync([FromBody][Required]SignUp<TEntity> signUp, CancellationToken cancellationToken = default)
        {
            var identityUser = await this.IdentityManager
                .SignUpAsync(signUp, cancellationToken);

            signUp.User.Id = Guid.Parse(identityUser.Id);
            signUp.User.IdentityUserId = identityUser.Id;

            var result = await this.Repository
                .AddAsync(signUp.User, cancellationToken);

            result.IdentityUser = identityUser;

            return this.Created("signup", result);
        }
        
        /// <summary>
        /// Authenticates an external login, and returns the result.
        /// </summary>
        /// <param name="loginProvider">The login provider request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The authentication challange result of the extenal provider.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpGet]
        [HttpPost]
        [Route("external/signup")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ChallengeResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SignUpExternalAsync([FromQuery][Required]string loginProvider, CancellationToken cancellationToken = default)
        {
            var controller = $"{typeof(TEntity).Name.ToLower()}s";
            var redirectUrl = Url.Action(nameof(SignUpExternalCallbackAsync), controller);

            this.Logger.LogWarning("EXTERNAL_REDIRECT_URL: " + redirectUrl); // BUG: Logging

            return await this.IdentityManager
                .SignInExternalAsync(loginProvider, redirectUrl, cancellationToken);
        }

        /// <summary>
        /// Completes Authentication with external login.
        /// On success a jwt-token is created and returned, for later use with authorization.
        /// </summary>
        /// <param name="remoteError">Error text from provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpGet]
        [Route("external/signup/callback")]
        [AllowAnonymous]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(AccessToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SignUpExternalCallbackAsync([FromQuery]string remoteError = null, CancellationToken cancellationToken = default)
        {
            if (remoteError != null)
                throw new UnauthorizedException(remoteError);

            var identityUser = await this.IdentityManager
                .SignUpExternalCallbackAsync(cancellationToken);

            var userId = Guid.Parse(identityUser.Id);
            var user = await this.Repository
                .GetAsync<TEntity>(userId, cancellationToken);

            if (user == null)
            {
                user = new TEntity
                {
                    Id = userId,
                    IdentityUserId = identityUser.Id
                };

                await this.Repository
                    .AddAsync(user, cancellationToken);
            }

            var accessToken = await this.IdentityManager
                .GetJwtTokenAsync(identityUser, cancellationToken);

            return this.Ok(accessToken);
        }

        /// <summary>
        /// Sets the username of a user.
        /// </summary>
        /// <param name="setUsername">The set username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("username/set")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SetUsernameAsync([FromBody][Required]SetUsername setUsername, CancellationToken cancellationToken = default)
        {
            await this.IdentityManager
                .SetUsernameAsync(setUsername, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Sets the password of a user.
        /// Only use to set a password for a user without one.
        /// Users authenticated with external login providers, doesn't initially have a password.
        /// </summary>
        /// <param name="setPassword">The set password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("password/set")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SetPasswordAsync([FromBody][Required]SetPassword setPassword, CancellationToken cancellationToken = default)
        {
            await this.IdentityManager
                .SetPasswordAsync(setPassword, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Resets the password of a user.
        /// </summary>
        /// <param name="resetPassword">The reset password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("password/reset")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> ResetPasswordAsync([FromBody][Required]ResetPassword resetPassword, CancellationToken cancellationToken = default)
        {
            await this.IdentityManager
                .ResetPasswordAsync(resetPassword, cancellationToken);

            return this.Ok();
        }
        
        /// <summary>
        /// Gets the password reset token, used to change the password of a user.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The reset password token.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("password/reset/token")]
        [AllowAnonymous]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(ResetPasswordToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> GetResetPasswordTokenAsync([FromQuery][Required]string emailAddress, CancellationToken cancellationToken = default)
        {
            var resetPasswordToken = await this.IdentityManager
                .GenerateResetPasswordTokenAsync(emailAddress, cancellationToken);

            return this.Ok(resetPasswordToken);
        }

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="changePassword">The change password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("password/change")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> ChangePasswordAsync([FromBody][Required]ChangePassword changePassword, CancellationToken cancellationToken = default)
        {
            await this.IdentityManager
                .ChangePasswordAsync(changePassword, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Changes the email of a user.
        /// </summary>
        /// <param name="changeEmail">The change email.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("email/change")]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> ChangeEmailAsync([FromBody][Required]ChangeEmail changeEmail, CancellationToken cancellationToken = default)
        {
            await this.IdentityManager
                .ChangeEmailAsync(changeEmail, cancellationToken);

            return this.Ok();
        }

        /// <summary>
        /// Gets the change email token, used to change the email address of a user.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="newEmailAddress">The new email address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The change email token.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("email/change/token")]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(ChangeEmailToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> GetChangeEmailTokenAsync([FromQuery][Required]string emailAddress, [Required][FromQuery]string newEmailAddress, CancellationToken cancellationToken = default)
        {
            var changeEmailToken = await this.IdentityManager
                .GenerateChangeEmailTokenAsync(emailAddress, newEmailAddress, cancellationToken);

            return this.Ok(changeEmailToken);
        }

        /// <summary>
        /// Confirms the email address of a user.
        /// </summary>
        /// <param name="confirmEmail">The confirm email.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("email/confirm")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> ConfirmEmailAsync([FromBody][Required]ConfirmEmail confirmEmail, CancellationToken cancellationToken = default)
        {
            await this.IdentityManager
                .ConfirmEmailAsync(confirmEmail, cancellationToken);

            return this.Ok();
        }
        
        /// <summary>
        /// Gets the confirm email token, used to confirm the email address of a user.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The confirm email token.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpGet]
        [Route("email/confirm/token")]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(ConfirmEmailToken), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> GetConfirmEmailTokenAsync([FromQuery][Required]string emailAddress, CancellationToken cancellationToken = default)
        {
            var confirmEmailToken = await this.IdentityManager
                .GenerateConfirmEmailTokenAsync(emailAddress, cancellationToken);

            return this.Ok(confirmEmailToken);
        }
        
        /// <summary>
        /// Removes an external login for a user.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Void.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="404">Not Found.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("external/login/remove")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> RemoveExternalLoginAsync(CancellationToken cancellationToken = default)
        {
            await this.IdentityManager
                .RemoveExternalLoginAsync(cancellationToken);

            return this.Ok();
        }
    }
}