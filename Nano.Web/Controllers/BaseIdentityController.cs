using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Interfaces;
using Nano.Repository.Interfaces;
using Nano.Security;
using Nano.Security.Const;
using Nano.Security.Models;
using Nano.Web.Const;
using Nano.Web.Models;
using Claim = Nano.Security.Models.Claim;

namespace Nano.Web.Controllers;

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
    protected virtual BaseIdentityManager<TIdentity> IdentityManager { get; }

    /// <inheritdoc />
    protected BaseIdentityController(ILogger logger, TRepository repository, BaseIdentityManager<TIdentity> baseIdentityManager)
        : this(logger, repository, new NullEventing(), baseIdentityManager)
    {

    }

    /// <inheritdoc />
    protected BaseIdentityController(ILogger logger, TRepository repository, IEventing eventing, BaseIdentityManager<TIdentity> baseIdentityManager)
        : base(logger, repository, eventing)
    {
        this.IdentityManager = baseIdentityManager ?? throw new ArgumentNullException(nameof(baseIdentityManager));
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
        var identityUser = await this.IdentityManager
            .SignUpAsync(signUp, cancellationToken);

        var user = await this.IdentityManager
            .CreateUser(signUp.User, identityUser, cancellationToken);

        return this.Created("signup", user);
    }

    /// <summary>
    /// Sign-up a user based on external login data directly.
    /// This can be used when the external provider is verified separately, and the data is passed instead of retrieved from the external provider.
    /// </summary>
    /// <param name="signUpExternal">The signup external direct.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("signup/external/direct")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalDirectAsync([FromBody][Required]SignUpExternalDirect<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.IdentityManager
            .SignUpExternalAsync(signUpExternal.ExternalLogInData, signUpExternal.Roles, signUpExternal.Claims, cancellationToken);

        var user = await this.IdentityManager
            .CreateUser(signUpExternal.User, identityUser, cancellationToken);

        return this.Created("signup/external", user);
    }

    /// <summary>
    /// Sign-up a user based on external Google logIn provider.
    /// </summary>
    /// <param name="signUpExternal">The signup external.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("signup/external/google")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalGoogleAsync([FromBody][Required]SignUpExternalGoogle<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.IdentityManager
            .SignUpExternalAsync(signUpExternal, cancellationToken);

        var user = await this.IdentityManager
            .CreateUser(signUpExternal.User, identityUser, cancellationToken);

        return this.Created("signup/external/google", user);
    }

    /// <summary>
    /// Sign-up a user based on external Facebook logIn provider.
    /// </summary>
    /// <param name="signUpExternal">The signup external.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("signup/external/facebook")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalFacebookAsync([FromBody][Required]SignUpExternalFacebook<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.IdentityManager
            .SignUpExternalAsync(signUpExternal, cancellationToken);

        var user = await this.IdentityManager
            .CreateUser(signUpExternal.User, identityUser, cancellationToken);

        return this.Created("signup/external/facebook", user);
    }

    /// <summary>
    /// Sign-up a user based on external Microsoft logIn provider.
    /// </summary>
    /// <param name="signUpExternal">The signup external.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access token.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route("signup/external/microsoft")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalMicrosoftAsync([FromBody][Required]SignUpExternalMicrosoft<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.IdentityManager
            .SignUpExternalAsync(signUpExternal, cancellationToken);

        var user = await this.IdentityManager
            .CreateUser(signUpExternal.User, identityUser, cancellationToken);

        return this.Created("signup/external/microsoft", user);
    }

    /// <summary>
    /// Sets the emailAddress of a user.
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
    public virtual async Task<IActionResult> SetUsernameAsync([FromBody][Required]SetUsername<TIdentity> setUsername, CancellationToken cancellationToken = default)
    {
        await this.IdentityManager
            .SetUsernameAsync(setUsername, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Sets the password of a user.
    /// Only use to set a password for a user without one.
    /// Users authenticated with external logIn providers, doesn't initially have a password.
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
    public virtual async Task<IActionResult> SetPasswordAsync([FromBody][Required]SetPassword<TIdentity> setPassword, CancellationToken cancellationToken = default)
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
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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
    public virtual async Task<IActionResult> ChangePasswordAsync([FromBody][Required]ChangePassword<TIdentity> changePassword, CancellationToken cancellationToken = default)
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
    public virtual async Task<IActionResult> ChangeEmailAsync([FromBody][Required]ChangeEmail<TIdentity> changeEmail, CancellationToken cancellationToken = default)
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
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetConfirmEmailTokenAsync([FromQuery][Required]string emailAddress, CancellationToken cancellationToken = default)
    {
        var confirmEmailToken = await this.IdentityManager
            .GenerateConfirmEmailTokenAsync(emailAddress, cancellationToken);

        return this.Ok(confirmEmailToken);
    }

    /// <summary>
    /// Changes the phone number of a user.
    /// </summary>
    /// <param name="changePhoneNumber">The change phone number.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("phone/change")]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ChangePhoneAsync([FromBody][Required]ChangePhoneNumber<TIdentity> changePhoneNumber, CancellationToken cancellationToken = default)
    {
        await this.IdentityManager
            .ChangePhoneNumberAsync(changePhoneNumber, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Gets the change phone number token, used to change the phone number of a user.
    /// </summary>
    /// <param name="phoneNumber">The phone number.</param>
    /// <param name="newPhoneNumber">The new phone number.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The change phone number token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("phone/change/token")]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(ChangePhoneNumberToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetChangePhoneTokenAsync([FromQuery][Required]string phoneNumber, [Required][FromQuery]string newPhoneNumber, CancellationToken cancellationToken = default)
    {
        var changeEmailToken = await this.IdentityManager
            .GenerateChangePhoneNumberTokenAsync(phoneNumber, newPhoneNumber, cancellationToken);

        return this.Ok(changeEmailToken);
    }

    /// <summary>
    /// Confirms the phone number of a user.
    /// </summary>
    /// <param name="confirmPhoneNumber">The confirm phone number.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("phone/confirm")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ConfirmPhoneAsync([FromBody][Required]ConfirmPhoneNumber confirmPhoneNumber, CancellationToken cancellationToken = default)
    {
        await this.IdentityManager
            .ConfirmPhoneNumberAsync(confirmPhoneNumber, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Gets the confirm phone number token, used to confirm the phone number of a user.
    /// </summary>
    /// <param name="phoneNumber">The phone number.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The confirm phone token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("phone/confirm/token")]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(ConfirmPhoneNumberToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetConfirmPhoneTokenAsync([FromQuery][Required]string phoneNumber, CancellationToken cancellationToken = default)
    {
        var confirmEmailToken = await this.IdentityManager
            .GenerateConfirmPhoneNumberTokenAsync(phoneNumber, cancellationToken);

        return this.Ok(confirmEmailToken);
    }

    /// <summary>
    /// Removes an external logIn for a user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("external/logIn/remove")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveExternalLoginAsync(CancellationToken cancellationToken = default)
    {
        this.HttpContext.Request.Scheme = "https";

        await this.IdentityManager
            .RemoveExternalLoginAsync(cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Gets roles of a user.
    /// </summary>
    /// <param name="id">The user id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The roles.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("roles/{id}")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetRolesAsync([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
    {
        var roles = await this.IdentityManager
            .GetUserRolesAsync(id, cancellationToken);

        return this.Ok(roles);
    }

    /// <summary>
    /// Assign a role to a user.
    /// </summary>
    /// <param name="assignRole">The assign role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("role/assign")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignRoleAsync([FromBody][Required]AssignRole<TIdentity> assignRole, CancellationToken cancellationToken = default)
    {
        await this.IdentityManager
            .AssignUserRoleAsync(assignRole, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Remove a role from a user.
    /// </summary>
    /// <param name="removeRole">The remove role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [HttpDelete]
    [Route("role/remove")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveRoleAsync([FromBody][Required]RemoveRole<TIdentity> removeRole, CancellationToken cancellationToken = default)
    {
        await this.IdentityManager
            .RemoveUserRoleAsync(removeRole, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Gets claims of a user.
    /// </summary>
    /// <param name="id">The user id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The claims.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("claims/{id}")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType(typeof(IEnumerable<Claim>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetClaimsAsync([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
    {
        var userClaims = await this.IdentityManager
            .GetUserClaimsAsync(id, cancellationToken);

        var claims = userClaims
            .Select(x => new Claim
            {
                Type = x.Type,
                Value = x.Value
            });

        return this.Ok(claims);
    }

    /// <summary>
    /// Assign a claim to a user.
    /// </summary>
    /// <param name="assignClaim">The assign claim.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("claim/assign")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignClaimAsync([FromBody][Required]AssignClaim<TIdentity> assignClaim, CancellationToken cancellationToken = default)
    {
        await this.IdentityManager
            .AssignUserClaimAsync(assignClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Remove a claim from a user.
    /// </summary>
    /// <param name="removeClaim">The remove claim.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [HttpDelete]
    [Route("claim/remove")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveClaimAsync([FromBody][Required]RemoveClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default)
    {
        await this.IdentityManager
            .RemoveUserClaimAsync(removeClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Delete the model with the passed identifier.
    /// </summary>
    /// <param name="id">The identifier of the model to delete.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [HttpDelete]
    [Route("delete/{id}")]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteAsync([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
    {
        var user = await this.Repository
            .GetAsync<TEntity, TIdentity>(id, cancellationToken);

        await this.Repository
            .DeleteAsync(user, cancellationToken);

        await this.IdentityManager
            .DeleteIdentityUser(user.IdentityUser, cancellationToken);

        await this.Repository
            .SaveChanges(cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Delete the models with the passed identifiers.
    /// </summary>
    /// <param name="ids">The identifiers of the models to delete.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [HttpDelete]
    [Route("delete/many")]
    [Consumes(HttpContentType.JSON, HttpContentType.XML)]
    [Produces(HttpContentType.JSON, HttpContentType.XML)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteManyAsync([FromBody][Required]TIdentity[] ids, CancellationToken cancellationToken = default)
    {
        foreach (var id in ids)
        {
            var user = await this.Repository
                .GetAsync<TEntity, TIdentity>(id, cancellationToken);

            await this.Repository
                .DeleteAsync(user, cancellationToken);

            await this.IdentityManager
                .DeleteIdentityUser(user.IdentityUser, cancellationToken);
        }

        await this.Repository
            .SaveChanges(cancellationToken);

        return this.Ok();
    }
}