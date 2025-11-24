using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Abstractions;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Eventing.Abstractions;
using Nano.Models;
using Nano.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.ApiClient.Consts;
using Nano.App.ApiClient.Models;
using Nano.App.ApiClient.Models.Identity;
using IdentityOptions = Nano.Web.IdentityOptions;

namespace Nano.App.Web.Controllers;

// BUG: We should hide/remove controller actions that are not configured (jwt, api-key). possibly return 404 in middleware

/// <inheritdoc />
public abstract class BaseIdentityController<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerUpdatable<TRepository, TEntity, TIdentity, TCriteria>
    where TRepository : IRepository
    where TEntity : class, IEntityUser<TIdentity>, IEntityCreatable, IEntityUpdatable, IEntityDeletable, IEntityIdentity<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
    where TCriteria : class, IQueryCriteria, new()
{
    /// <summary>
    /// 
    /// </summary>
    protected virtual IdentityOptions Options { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected virtual IIdentityRepository<TIdentity> IdentityRepository { get; }

    /// <summary>
    /// 
    /// </summary>
    protected virtual IAuthRepository<TIdentity> IAuthRepository { get; }

    /// <inheritdoc />
    protected BaseIdentityController(ILogger logger, TRepository repository, IIdentityRepository<TIdentity> identityRepository, IAuthRepository<TIdentity> authRepository)
        : this(logger, repository, null, identityRepository, authRepository)
    {
    }

    /// <inheritdoc />
    protected BaseIdentityController(ILogger logger, TRepository repository, IEventing eventing, IIdentityRepository<TIdentity> identityRepository, IAuthRepository<TIdentity> authRepository)
        : base(logger, repository, eventing)
    {
        this.IdentityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        this.IAuthRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
    }

    /// <summary>
    /// Gets the password options.
    /// </summary>
    /// <returns>The password options.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("password/options")]
    [AllowAnonymous]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(PasswordOptions), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetPasswordOptionsAsync(CancellationToken cancellationToken = default)
    {
        var passwordOptions = await this.IdentityRepository
            .GetPaswordOptionsAsync(cancellationToken);

        if (passwordOptions == null)
        {
            return this.NotFound();
        }

        return this.Ok(passwordOptions);
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
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(DefaultEntityUser), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpAsync([FromBody][Required] SignUp<TEntity, TIdentity> signUp, CancellationToken cancellationToken = default)
    {
        var user = await this.IdentityRepository
            .SignUpAsync(signUp, cancellationToken);

        if (user == null)
        {
            return this.NotFound();
        }

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
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(DefaultEntityUser), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalDirectAsync([FromBody][Required] SignUpExternalDirect<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        var user = await this.IdentityRepository
            .SignUpExternalAsync(new SignUpExternal<TEntity, TIdentity>
            {
                User = signUpExternal.User,
                ExternalLogInData = signUpExternal.ExternalLogInData,
                Roles = signUpExternal.Roles,
                Claims = signUpExternal.Claims
            }, cancellationToken);

        if (user == null)
        {
            return this.NotFound();
        }

        return this.Created("signup/external/direct", user);
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
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(DefaultEntityUser), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalGoogleAsync([FromBody][Required] SignUpExternalGoogle<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        var externalProviderLogInData = await this.IAuthRepository
            .GetExternalProviderLogInData(signUpExternal.Provider, cancellationToken);

        var user = await this.IdentityRepository
            .SignUpExternalAsync(new SignUpExternal<TEntity, TIdentity>
            {
                User = signUpExternal.User,
                ExternalLogInData = externalProviderLogInData,
                Roles = signUpExternal.Roles,
                Claims = signUpExternal.Claims
            }, cancellationToken);

        if (user == null)
        {
            return this.NotFound();
        }

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
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(DefaultEntityUser), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalFacebookAsync([FromBody][Required] SignUpExternalFacebook<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        var externalProviderLogInData = await this.IAuthRepository
            .GetExternalProviderLogInData(signUpExternal.Provider, cancellationToken);

        var user = await this.IdentityRepository
            .SignUpExternalAsync(new SignUpExternal<TEntity, TIdentity>
            {
                User = signUpExternal.User,
                ExternalLogInData = externalProviderLogInData,
                Roles = signUpExternal.Roles,
                Claims = signUpExternal.Claims
            }, cancellationToken);

        if (user == null)
        {
            return this.NotFound();
        }

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
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(DefaultEntityUser), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalMicrosoftAsync([FromBody][Required] SignUpExternalMicrosoft<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        var externalProviderLogInData = await this.IAuthRepository
            .GetExternalProviderLogInData(signUpExternal.Provider, cancellationToken);

        var user = await this.IdentityRepository
            .SignUpExternalAsync(new SignUpExternal<TEntity, TIdentity>
            {
                User = signUpExternal.User,
                ExternalLogInData = externalProviderLogInData,
                Roles = signUpExternal.Roles,
                Claims = signUpExternal.Claims
            }, cancellationToken);

        if (user == null)
        {
            return this.NotFound();
        }

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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SetUsernameAsync([FromBody][Required] SetUsername<TIdentity> setUsername, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SetPasswordAsync([FromBody][Required] SetPassword<TIdentity> setPassword, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ResetPasswordAsync([FromBody][Required] ResetPassword<TIdentity> resetPassword, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .ResetPasswordAsync(resetPassword, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Get the password reset token, used to change the password of a user.
    /// </summary>
    /// <param name="generateResetPasswordToken">The generate reset password token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The reset password token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("password/reset/token")]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ResetPasswordToken<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetResetPasswordTokenAsync([FromBody][Required] GenerateResetPasswordToken generateResetPasswordToken, CancellationToken cancellationToken = default)
    {
        var resetPasswordToken = await this.IdentityRepository
            .GenerateResetPasswordTokenAsync(generateResetPasswordToken, cancellationToken);

        if (resetPasswordToken == null)
        {
            return this.NotFound();
        }

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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ChangePasswordAsync([FromBody][Required] ChangePassword<TIdentity> changePassword, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .ChangePasswordAsync(changePassword, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Is Email Address Taken.
    /// </summary>
    /// <returns>Whether the email address is already taken.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("email/is-taken")]
    [AllowAnonymous]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IsEmailAddressTaken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> IsEmailAddressTakenAsync([FromQuery][Required] string emailAddress, CancellationToken cancellationToken = default)
    {
        var isEmailAddressTaken = await this.IdentityRepository
            .IsEmailAddressTakenAsync(emailAddress, cancellationToken);

        return this.Ok(isEmailAddressTaken);
    }

    /// <summary>
    /// Changes the email of a user.
    /// </summary>
    /// <param name="changeEmail">The change email.</param>
    /// <param name="setUsername">Set username.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("email/change")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ChangeEmailAsync([FromBody][Required] ChangeEmail<TIdentity> changeEmail, [FromQuery] bool setUsername = false, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .ChangeEmailAsync(changeEmail, setUsername, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Get the change email token, used to change the email address of a user.
    /// </summary>
    /// <param name="generateChangeEmailToken">The genereate change email token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The change email token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("email/change/token")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ChangeEmailToken<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetChangeEmailTokenAsync([FromBody][Required] GenerateChangeEmailToken<TIdentity> generateChangeEmailToken, CancellationToken cancellationToken = default)
    {
        var changeEmailToken = await this.IdentityRepository
            .GenerateChangeEmailTokenAsync(generateChangeEmailToken, cancellationToken);

        if (changeEmailToken == null)
        {
            return this.NotFound();
        }

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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ConfirmEmailAsync([FromBody][Required] ConfirmEmail<TIdentity> confirmEmail, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .ConfirmEmailAsync(confirmEmail, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Get the confirm email token, used to confirm the email address of a user.
    /// </summary>
    /// <param name="generateConfirmEmailToken">The generate confirm email token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The confirm email token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("email/confirm/token")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ConfirmEmailToken<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetConfirmEmailTokenAsync([FromBody][Required] GenerateConfirmEmailToken<TIdentity> generateConfirmEmailToken, CancellationToken cancellationToken = default)
    {
        var confirmEmailToken = await this.IdentityRepository
            .GenerateConfirmEmailTokenAsync(generateConfirmEmailToken, cancellationToken);

        if (confirmEmailToken == null)
        {
            return this.NotFound();
        }

        return this.Ok(confirmEmailToken);
    }

    /// <summary>
    /// Is Phone Number Taken.
    /// </summary>
    /// <returns>Whether the phone number is already taken.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("phone/is-taken")]
    [AllowAnonymous]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IsPhoneNumberTaken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> IsPhoneNumberTakenAsync([FromQuery][Required] string phoneNumber, CancellationToken cancellationToken = default)
    {
        var isPhoneNumberTaken = await this.IdentityRepository
            .IsPhoneNumberTakenAsync(phoneNumber, cancellationToken);

        return this.Ok(isPhoneNumberTaken);
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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ChangePhoneAsync([FromBody][Required] ChangePhoneNumber<TIdentity> changePhoneNumber, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .ChangePhoneNumberAsync(changePhoneNumber, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Gets the change phone number token, used to change the phone number of a user.
    /// </summary>
    /// <param name="generateChangePhoneToken">The generate change phone token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The change phone number token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("phone/change/token")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ChangePhoneNumberToken<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetChangePhoneTokenAsync([FromBody][Required] GenerateChangePhoneToken<TIdentity> generateChangePhoneToken, CancellationToken cancellationToken = default)
    {
        var changeEmailToken = await this.IdentityRepository
            .GenerateChangePhoneNumberTokenAsync(generateChangePhoneToken, cancellationToken);

        if (changeEmailToken == null)
        {
            return this.NotFound();
        }

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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ConfirmPhoneAsync([FromBody][Required] ConfirmPhoneNumber<TIdentity> confirmPhoneNumber, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .ConfirmPhoneNumberAsync(confirmPhoneNumber, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Gets the confirm phone number token, used to confirm the phone number of a user.
    /// </summary>
    /// <param name="generateConfirmPhoneToken">The generate confirm phoneNumber token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The confirm phone token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("phone/confirm/token")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ConfirmPhoneNumberToken<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetConfirmPhoneTokenAsync([FromBody][Required] GenerateConfirmPhoneToken<TIdentity> generateConfirmPhoneToken, CancellationToken cancellationToken = default)
    {
        var confirmEmailToken = await this.IdentityRepository
            .GenerateConfirmPhoneNumberTokenAsync(generateConfirmPhoneToken, cancellationToken);

        if (confirmEmailToken == null)
        {
            return this.NotFound();
        }

        return this.Ok(confirmEmailToken);
    }

    /// <summary>
    /// Generates a cstuom purpose token of a user.
    /// </summary>
    /// <param name="confirmEmail">The custom purpose token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The custom purpose token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("token/custom")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(CustomPurposeToken<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> VerifyCustomPurposeTokenAsync([FromBody][Required] GenerateCustomPurposeToken<TIdentity> confirmEmail, CancellationToken cancellationToken = default)
    {
        var customPurposeToken = await this.IdentityRepository
            .GenerateCustomTokenAsync(confirmEmail, cancellationToken);

        if (customPurposeToken == null)
        {
            return this.NotFound();
        }

        return this.Ok(customPurposeToken);
    }

    /// <summary>
    /// Verifies a cstuom purpose token of a user.
    /// </summary>
    /// <param name="customPurposeToken">The generate confirm email token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("token/custom/verify")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetConfirmEmailTokenAsync([FromBody][Required] CustomPurposeToken<TIdentity> customPurposeToken, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .VerifyCustomTokenAsync(customPurposeToken, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Get External Logins for a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The external logins.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("external-logins/{userId}")]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<ExternalLogin>), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetExternalLoginsAsync([FromRoute][Required] TIdentity userId, CancellationToken cancellationToken = default)
    {
        var externalLogins = await this.IdentityRepository
            .GetUserExternalLoginsAsync(userId, cancellationToken);

        return this.Ok(externalLogins);
    }

    /// <summary>
    /// Add Google external login for a user.
    /// </summary>
    /// <param name="addExternalLogin">The external login</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("external-logins/add/google")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<ExternalLogin>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AddExternalLoginGoogleAsync([FromBody][Required] AddExternalLoginGoogle<TIdentity> addExternalLogin, CancellationToken cancellationToken = default)
    {
        var externalProviderLogInData = await this.IAuthRepository
            .GetExternalProviderLogInData(addExternalLogin.Provider, cancellationToken);

        var externalLogin = await this.IdentityRepository
            .AddExternalLoginAsync(addExternalLogin.UserId, externalProviderLogInData, cancellationToken);

        return this.Ok(externalLogin);
    }

    /// <summary>
    /// Add Facebook external login for a user.
    /// </summary>
    /// <param name="addExternalLogin">The external login</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("external-logins/add/facebook")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<ExternalLogin>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AddExternalLoginFacebookAsync([FromBody][Required] AddExternalLoginFacebook<TIdentity> addExternalLogin, CancellationToken cancellationToken = default)
    {
        var externalProviderLogInData = await this.IAuthRepository
            .GetExternalProviderLogInData(addExternalLogin.Provider, cancellationToken);

        var externalLogin = await this.IdentityRepository
            .AddExternalLoginAsync(addExternalLogin.UserId, externalProviderLogInData, cancellationToken);

        return this.Ok(externalLogin);
    }

    /// <summary>
    /// Add Microsoft external login for a user.
    /// </summary>
    /// <param name="addExternalLogin">The external login</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("external-logins/add/microsoft")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<ExternalLogin>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AddExternalLoginMicrosoftAsync([FromBody][Required] AddExternalLoginMicrosoft<TIdentity> addExternalLogin, CancellationToken cancellationToken = default)
    {
        var externalProviderLogInData = await this.IAuthRepository
            .GetExternalProviderLogInData(addExternalLogin.Provider, cancellationToken);

        var externalLogin = await this.IdentityRepository
            .AddExternalLoginAsync(addExternalLogin.UserId, externalProviderLogInData, cancellationToken);

        return this.Ok(externalLogin);
    }

    /// <summary>
    /// Removes an external login for a user.
    /// </summary>
    /// <param name="removeExternalLogin">The remove external login request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [HttpDelete]
    [Route("external-logins/remove")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveExternalLoginAsync([FromBody][Required] RemoveExternalLogin<TIdentity> removeExternalLogin, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .RemoveExternalLoginAsync(removeExternalLogin, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Get Api Keys.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The api keys.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("api-keys/{userId}")]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<IdentityApiKey<Guid>>), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetApiKeysAsync([FromRoute][Required] TIdentity userId, CancellationToken cancellationToken = default)
    {
        var identityApiKeys = await this.IdentityRepository
            .GetApiKeysAsync(userId, cancellationToken);

        if (identityApiKeys == null)
        {
            return this.NotFound();
        }

        return this.Ok(identityApiKeys);
    }

    /// <summary>
    /// Create Api Key.
    /// </summary>
    /// <param name="createApiKey">The create api key.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The create api key response.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("api-keys/create")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IdentityApiKeyCreated<Guid>), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateApiKeyAsync([FromBody][Required] CreateApiKey<TIdentity> createApiKey, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var identityApiKey = this.IdentityRepository
            .CreateApiKeyAsync(createApiKey, this.Options.Authentication.ApiKey.Secret, out var apiKey);

        if (identityApiKey == null)
        {
            return this.NotFound();
        }

        var identityApiKeyCreated = new IdentityApiKeyCreated<TIdentity>
        {
            IdentityApiKey = identityApiKey,
            UnencryptedHash = apiKey
        };

        return this.Created("api-keys/create", identityApiKeyCreated);
    }

    /// <summary>
    /// Edit Api Key.
    /// </summary>
    /// <param name="editApiKey">The edit api key.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The identity api key.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPut]
    [HttpPost]
    [Route("api-keys/edit")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IdentityApiKey<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditApiKeyAsync([FromBody][Required] EditApiKey<TIdentity> editApiKey, CancellationToken cancellationToken = default)
    {
        var identityApiKey = await this.IdentityRepository
            .EditApiKeyAsync(editApiKey, cancellationToken);

        if (identityApiKey == null)
        {
            return this.NotFound();
        }

        return this.Ok(identityApiKey);
    }

    /// <summary>
    /// Revoke Api Key.
    /// </summary>
    /// <param name="apiKeyId">The api key id.</param>
    /// <param name="revokeAt">The date time when the api key will be revoked.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>The identity api key.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpDelete]
    [Route("api-keys/revoke/{apiKeyId}")]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IdentityApiKey<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RevokeApiKeyAsync([FromRoute][Required] TIdentity apiKeyId, [FromQuery] DateTimeOffset? revokeAt, CancellationToken cancellationToken = default)
    {
        var identityApiKey = await this.IdentityRepository
            .RevokeApiKeyAsync(new RevokeApiKey<TIdentity>
            {
                Id = apiKeyId,
                RevokeAt = revokeAt
            }, cancellationToken);

        if (identityApiKey == null)
        {
            return this.NotFound();
        }

        return this.Ok(identityApiKey);
    }

    /// <summary>
    /// Gets roles of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The roles.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("roles/{userId}")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetRolesAsync([FromRoute][Required] TIdentity userId, CancellationToken cancellationToken = default)
    {
        var roles = await this.IdentityRepository
            .GetUserRolesAsync(userId, cancellationToken);

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
    [Route("roles/assign")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignRoleAsync([FromBody][Required] AssignRole<TIdentity> assignRole, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
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
    [Route("roles/remove")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveRoleAsync([FromBody][Required] RemoveRole<TIdentity> removeRole, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .RemoveUserRoleAsync(removeRole, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Gets claims of a role.
    /// </summary>
    /// <param name="roleId">The role id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The role claims.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("roles/claims/{roleId}")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<Claim>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetRoleClaimsAsync([FromRoute][Required] TIdentity roleId, CancellationToken cancellationToken = default)
    {
        var userRoleClaims = await this.IdentityRepository
            .GetRoleClaimsAsync(roleId, cancellationToken);

        if (userRoleClaims == null)
        {
            return this.NotFound();
        }

        return this.Ok(userRoleClaims);
    }

    /// <summary>
    /// Assign a claim to a role.
    /// </summary>
    /// <param name="assignRoleClaim">The assign role claim.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("roles/claims/assign")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignRoleClaimAsync([FromBody][Required] AssignRoleClaim<TIdentity> assignRoleClaim, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .AssignRoleClaimAsync(assignRoleClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Remove a claim from a role.
    /// </summary>
    /// <param name="removeClaim">The remove role claim.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [HttpDelete]
    [Route("roles/claims/remove")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveRoleClaimAsync([FromBody][Required] RemoveRoleClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .RemoveRoleClaimAsync(removeClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Replace a claim of a role.
    /// </summary>
    /// <param name="replaceClaim">The replace role claim.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPut]
    [Route("roles/claims/replace")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ReplaceRoleClaimAsync([FromBody][Required] ReplaceRoleClaim<TIdentity> replaceClaim, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .ReplaceRoleClaimAsync(replaceClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Gets claims of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The claims.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("claims/{userId}")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<Claim>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetClaimsAsync([FromRoute][Required] TIdentity userId, CancellationToken cancellationToken = default)
    {
        var userClaims = await this.IdentityRepository
            .GetUserClaimsAsync(userId, cancellationToken);

        if (userClaims == null)
        {
            return this.NotFound();
        }

        return this.Ok(userClaims);
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
    [Route("claims/assign")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignClaimAsync([FromBody][Required] AssignClaim<TIdentity> assignClaim, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
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
    [Route("claims/remove")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveClaimAsync([FromBody][Required] RemoveClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .RemoveUserClaimAsync(removeClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Replace a claim to a user.
    /// </summary>
    /// <param name="replaceClaim">The replace claim.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPut]
    [Route("claims/replace")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ReplaceClaimAsync([FromBody][Required] ReplaceClaim<TIdentity> replaceClaim, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .ReplaceUserClaimAsync(replaceClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Activate the model with the passed identifier.
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
    [Route("activate/{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ActivateAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .ActivateIdentityUser<TEntity>(id, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Deactivate the model with the passed identifier.
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
    [Route("deactivate/{id}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeactivateAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        await this.IdentityRepository
            .DeactivateIdentityUser<TEntity>(id, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        var user = await this.Repository
            .GetAsync<TEntity, TIdentity>(id, cancellationToken);

        if (user == null)
        {
            return this.NotFound();
        }

        await this.Repository
            .DeleteAsync(user, cancellationToken);

        await this.Repository
            .SaveChangesAsync(cancellationToken);

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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteManyAsync([FromBody][Required] TIdentity[] ids, CancellationToken cancellationToken = default)
    {
        foreach (var id in ids)
        {
            var user = await this.Repository
                .GetAsync<TEntity, TIdentity>(id, cancellationToken);

            if (user == null)
            {
                continue;
            }

            await this.Repository
                .DeleteAsync(user, cancellationToken);
        }

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
    }
}