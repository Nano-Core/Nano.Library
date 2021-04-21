using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security;
using Nano.Security.Exceptions;
using Nano.Security.Models;
using Nano.Web.Const;
using Nano.Web.Models;

namespace Nano.Web.Controllers
{
    // BUG: are we allowing Transient Admin login

    /// <inheritdoc />
    public abstract class BaseIdentityController<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerUpdatable<TRepository, TEntity, TIdentity, TCriteria>
        where TRepository : IRepository
        where TEntity : BaseEntityUser<TIdentity>, IEntityUpdatable, IEntityIdentity<TIdentity>, new()
        where TIdentity : IEquatable<TIdentity>
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <summary>
        /// Identity Manager.
        /// </summary>
        protected virtual BaseIdentityManager<TIdentity> BaseIdentityManager { get; }

        /// <inheritdoc />
        protected BaseIdentityController(ILogger logger, TRepository repository, IEventing eventing, BaseIdentityManager<TIdentity> baseIdentityManager) 
            : base(logger, repository, eventing)
        {
            this.BaseIdentityManager = baseIdentityManager ?? throw new ArgumentNullException(nameof(baseIdentityManager));
        }

        /// <summary>
        /// Sign-up a new user.
        /// </summary>
        /// <param name="signUp">The signup request.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The created user.</returns>
        /// <response code="201">Created.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("signup")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SignUpAsync([FromBody][Required]SignUp<TEntity, TIdentity> signUp, CancellationToken cancellationToken = default)
        {
            var identityUser = await this.BaseIdentityManager
                .SignUpAsync(signUp, cancellationToken);

            signUp.User.Id = identityUser.Id.Parse<TIdentity>();
            signUp.User.IdentityUserId = identityUser.Id;

            TEntity user;
            try
            {
                user = await this.Repository
                    .AddAsync(signUp.User, cancellationToken);

                await this.Repository
                    .SaveChanges(cancellationToken);
            }
            catch
            {
                await this.BaseIdentityManager
                    .DeleteIdentityUser(identityUser);

                throw;
            }

            user.IdentityUser = identityUser;

            return this.Created("signup", user);
        }

        /// <summary>
        /// Sign-up a user based on external login provider.
        /// </summary>
        /// <param name="signUpExternal">The signup external.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The access token.</returns>
        /// <response code="201">Created.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="500">Error occurred.</response>
        [HttpPost]
        [Route("signup/external")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SignUpExternalAsync([FromBody][Required]SignUpExternal<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
        {
            var identityUser = await this.BaseIdentityManager
                .SignUpExternalAsync(signUpExternal, cancellationToken);

            signUpExternal.User.Id = identityUser.Id.Parse<TIdentity>();
            signUpExternal.User.IdentityUserId = identityUser.Id;

            TEntity user;
            try
            {
                user = await this.Repository
                    .AddAsync(signUpExternal.User, cancellationToken);

                await this.Repository
                    .SaveChanges(cancellationToken);

            }
            catch
            {
                await this.BaseIdentityManager
                    .DeleteIdentityUser(identityUser);

                throw;
            }

            user.IdentityUser = identityUser;

            return this.Created("signup/external", user);
        }
        
        /// <summary>
        /// Executes an external authentication challange for the external provider.
        /// </summary>
        /// <param name="loginProvider">The login provider request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The challange result.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="500">Error occurred.</response>
        [HttpGet]
        [HttpPost]
        [Route("signup/external/challange")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ChallengeResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SignUpExternalChallangeAsync([FromQuery][Required]string loginProvider, CancellationToken cancellationToken = default)
        {
            this.HttpContext.Request.Scheme = "https";

            await HttpContext
                .SignOutAsync(IdentityConstants.ExternalScheme);

            var controller = $"{typeof(TEntity).Name.ToLower()}s";
            var redirectUrl = Url.Action(nameof(SignUpExternalChallangeCallbackAsync), controller);

            return await this.BaseIdentityManager
                .SignInExternalChallangeAsync(loginProvider, redirectUrl, cancellationToken);
        }

        /// <summary>
        /// Callback for the sign-in external challange result.
        /// </summary>
        /// <param name="remoteError">Error text from provider.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created user.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="201">Created.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occurred.</response>
        [HttpGet]
        [Route("signup/external/challange/callback")]
        [AllowAnonymous]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SignUpExternalChallangeCallbackAsync([FromQuery]string remoteError = null, CancellationToken cancellationToken = default)
        {
            // BUG: Fix. There might be a problem, When User has dependences like OrganizationId. Might not be relavant - do we actually make a challange? - Check Facebook / Google implementation.

            if (remoteError != null)
                throw new UnauthorizedException(remoteError);

            var signUpExternalResponse = await this.BaseIdentityManager
                .SignInExternalChallangeCallbackAsync(cancellationToken);

            var signUpExternal = new SignUpExternal
            {
                EmailAddress = signUpExternalResponse.Email
            };

            var identityUser = await this.BaseIdentityManager
                .SignUpExternalAsync(signUpExternal, cancellationToken);

            var userId = identityUser.Id.Parse<TIdentity>();

            var user = await this.Repository
                .GetAsync<TEntity, TIdentity>(userId, cancellationToken);

            if (user == null)
            {
                user = new TEntity
                {
                    Id = userId.Parse<TIdentity>(),
                    IdentityUserId = identityUser.Id
                };

                var result = await this.Repository
                    .AddAsync(user, cancellationToken);

                await this.Repository
                    .SaveChanges(cancellationToken);

                result.IdentityUser = identityUser;

                return this.Created("external/challange/callback", result);
            }

            return this.Ok(user);
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
            await this.BaseIdentityManager
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
            await this.BaseIdentityManager
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
            await this.BaseIdentityManager
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
            var resetPasswordToken = await this.BaseIdentityManager
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
            await this.BaseIdentityManager
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
            await this.BaseIdentityManager
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
            var changeEmailToken = await this.BaseIdentityManager
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
            await this.BaseIdentityManager
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
            var confirmEmailToken = await this.BaseIdentityManager
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
            this.HttpContext.Request.Scheme = "https";

            await this.BaseIdentityManager
                .RemoveExternalLoginAsync(cancellationToken);

            return this.Ok();
        }
    }
}