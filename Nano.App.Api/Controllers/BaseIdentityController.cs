using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.ApiClient.Consts;
using Nano.App.ApiClient.Models.Identity;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Eventing.Abstractions;
using PasswordOptions = Nano.Data.Abstractions.Config.PasswordOptions;

namespace Nano.App.Api.Controllers;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRepository"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TIdentity"></typeparam>
/// <typeparam name="TCriteria"></typeparam>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.IDENTITY)]
public abstract class BaseIdentityController<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerUpdatable<TRepository, TEntity, TIdentity, TCriteria>
    where TRepository : class, IRepository
    where TEntity : class, IEntityUser<TIdentity>, IEntityCreatable, IEntityUpdatable, IEntityDeletable, IEntityIdentity<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
    where TCriteria : class, IQueryCriteria, new()
{
    private readonly IIdentityRepository<TIdentity> identityRepository;
    private readonly IAuthExternalRepository? authExternalRepository;

    /// <inheritdoc />
    protected BaseIdentityController(ILogger logger, TRepository repository, IIdentityRepository<TIdentity> identityRepository, IAuthExternalRepository? authExternalRepository = null)
        : base(logger, repository)
    {
        this.identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        this.authExternalRepository = authExternalRepository;
    }

    /// <inheritdoc />
    protected BaseIdentityController(ILogger logger, TRepository repository, IEventing eventing, IIdentityRepository<TIdentity> identityRepository, IAuthExternalRepository? authExternalRepository = null)
        : base(logger, repository, eventing)
    {
        this.identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        this.authExternalRepository = authExternalRepository;
    }


    #region Sign Up

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetPasswordOptionsAsync(CancellationToken cancellationToken = default)
    {
        var passwordOptions = await this.identityRepository
            .GetPaswordOptionsAsync(cancellationToken);

        if (passwordOptions == null)
        {
            return this.NotFound();
        }

        return this.Ok(passwordOptions);
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> IsEmailAddressTakenAsync([FromQuery][Required]string emailAddress, CancellationToken cancellationToken = default)
    {
        var isEmailAddressTaken = await this.identityRepository
            .IsEmailAddressTakenAsync(emailAddress, cancellationToken);

        var response = new IsEmailAddressTaken
        {
            IsTaken = isEmailAddressTaken
        };

        return this.Ok(response);
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> IsPhoneNumberTakenAsync([FromQuery][Required]string phoneNumber, CancellationToken cancellationToken = default)
    {
        var isPhoneNumberTaken = await this.identityRepository
            .IsPhoneNumberTakenAsync(phoneNumber, cancellationToken);

        var response = new IsPhoneNumberTaken
        {
            IsTaken = isPhoneNumberTaken
        };

        return this.Ok(response);
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpAsync([FromBody][Required] SignUp<TEntity, TIdentity> signUp, CancellationToken cancellationToken = default)
    {
        var user = await this.identityRepository
            .SignUpAsync(signUp, cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalDirectAsync([FromBody][Required] SignUpExternalDirect<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        var user = await this.identityRepository
            .SignUpExternalAsync(new SignUpExternal<TEntity, TIdentity>
            {
                User = signUpExternal.User,
                Email = signUpExternal.ExternalLogInData.Email,
                ExternalProvider =
                {
                    LoginProvider = signUpExternal.ExternalLogInData.ExternalToken.Name,
                    ProviderKey = signUpExternal.ExternalLogInData.Id
                },
                Roles = signUpExternal.Roles,
                Claims = signUpExternal.Claims
            }, cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalGoogleAsync([FromBody][Required] SignUpExternalGoogle<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        if (authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        var externalProviderLogInData = await this.authExternalRepository
            .AuthenticateAsync(signUpExternal.Provider, cancellationToken);

        var user = await this.identityRepository
            .SignUpExternalAsync(new SignUpExternal<TEntity, TIdentity>
            {
                User = signUpExternal.User,
                Email = externalProviderLogInData.Email,
                ExternalProvider =
                {
                    LoginProvider = externalProviderLogInData.ExternalToken.Name,
                    ProviderKey = externalProviderLogInData.Id
                },
                Roles = signUpExternal.Roles,
                Claims = signUpExternal.Claims
            }, cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalFacebookAsync([FromBody][Required] SignUpExternalFacebook<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        if (authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        var externalProviderLogInData = await this.authExternalRepository
            .AuthenticateAsync(signUpExternal.Provider, cancellationToken);

        var user = await this.identityRepository
            .SignUpExternalAsync(new SignUpExternal<TEntity, TIdentity>
            {
                User = signUpExternal.User,
                Email = externalProviderLogInData.Email,
                ExternalProvider =
                {
                    LoginProvider = externalProviderLogInData.ExternalToken.Name,
                    ProviderKey = externalProviderLogInData.Id
                },
                Roles = signUpExternal.Roles,
                Claims = signUpExternal.Claims
            }, cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalMicrosoftAsync([FromBody][Required] SignUpExternalMicrosoft<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        if (authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        var externalProviderLogInData = await this.authExternalRepository
            .AuthenticateAsync(signUpExternal.Provider, cancellationToken);

        var user = await this.identityRepository
            .SignUpExternalAsync(new SignUpExternal<TEntity, TIdentity>
            {
                User = signUpExternal.User,
                Email = externalProviderLogInData.Email,
                ExternalProvider =
                {
                    LoginProvider = externalProviderLogInData.ExternalToken.Name,
                    ProviderKey = externalProviderLogInData.Id
                },
                Roles = signUpExternal.Roles,
                Claims = signUpExternal.Claims
            }, cancellationToken);

        return this.Created("signup/external/microsoft", user);
    }

    #endregion


    #region User

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SetUsernameAsync([FromBody][Required] SetUsername<TIdentity> setUsername, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SetPasswordAsync([FromBody][Required] SetPassword<TIdentity> setPassword, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .SetPasswordAsync(setPassword, cancellationToken);

        return this.Ok();
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ChangePasswordAsync([FromBody][Required] ChangePassword<TIdentity> changePassword, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ChangePasswordAsync(changePassword, cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ResetPasswordAsync([FromBody][Required] ResetPassword<TIdentity> resetPassword, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
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
    [ProducesResponseType(typeof(ResetPasswordToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetResetPasswordTokenAsync([FromBody][Required] GenerateResetPasswordToken generateResetPasswordToken, CancellationToken cancellationToken = default)
    {
        var resetPasswordToken = await this.identityRepository
            .GenerateResetPasswordTokenAsync(generateResetPasswordToken, cancellationToken);

        return this.Ok(resetPasswordToken);
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ChangeEmailAsync([FromBody][Required] ChangeEmail<TIdentity> changeEmail, [FromQuery] bool setUsername = false, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
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
    [ProducesResponseType(typeof(ChangeEmailToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetChangeEmailTokenAsync([FromBody][Required] GenerateChangeEmailToken<TIdentity> generateChangeEmailToken, CancellationToken cancellationToken = default)
    {
        var changeEmailToken = await this.identityRepository
            .GenerateChangeEmailTokenAsync(generateChangeEmailToken, cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ConfirmEmailAsync([FromBody][Required] ConfirmEmail<TIdentity> confirmEmail, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
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
    [ProducesResponseType(typeof(ConfirmEmailToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetConfirmEmailTokenAsync([FromBody][Required] GenerateConfirmEmailToken<TIdentity> generateConfirmEmailToken, CancellationToken cancellationToken = default)
    {
        var confirmEmailToken = await this.identityRepository
            .GenerateConfirmEmailTokenAsync(generateConfirmEmailToken, cancellationToken);

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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ChangePhoneAsync([FromBody][Required] ChangePhoneNumber<TIdentity> changePhoneNumber, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
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
    [ProducesResponseType(typeof(ChangePhoneNumberToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetChangePhoneTokenAsync([FromBody][Required] GenerateChangePhoneToken<TIdentity> generateChangePhoneToken, CancellationToken cancellationToken = default)
    {
        var changeEmailToken = await this.identityRepository
            .GenerateChangePhoneNumberTokenAsync(generateChangePhoneToken, cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ConfirmPhoneAsync([FromBody][Required] ConfirmPhoneNumber<TIdentity> confirmPhoneNumber, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
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
    [ProducesResponseType(typeof(ConfirmPhoneNumberToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetConfirmPhoneTokenAsync([FromBody][Required] GenerateConfirmPhoneToken<TIdentity> generateConfirmPhoneToken, CancellationToken cancellationToken = default)
    {
        var confirmEmailToken = await this.identityRepository
            .GenerateConfirmPhoneNumberTokenAsync(generateConfirmPhoneToken, cancellationToken);

        return this.Ok(confirmEmailToken);
    }

    /// <summary>
    /// Confirms a cstuom purpose token of a user.
    /// </summary>
    /// <param name="confirmEmail">The custom purpose token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The custom purpose token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("token/custom-purpose")]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ConfirmCustomPurposeToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ConfirmCustomPurposeTokenAsync([FromBody][Required] GenerateCustomPurposeToken<TIdentity> confirmEmail, CancellationToken cancellationToken = default)
    {
        var customPurposeToken = await this.identityRepository
            .GenerateCustomPurposeTokenAsync(confirmEmail, cancellationToken);

        return this.Ok(customPurposeToken);
    }

    /// <summary>
    /// Generates a cstuom purpose token of a user.
    /// </summary>
    /// <param name="confirmCustomPurpose">The generate confirm email token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("token/custom-purpose/confirm")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetCustomPurposeTokenAsync([FromBody][Required] ConfirmCustomPurpose<TIdentity> confirmCustomPurpose, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ConfirmCustomPurposeTokenAsync(confirmCustomPurpose, cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ActivateAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ActivateIdentityUser(id, cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeactivateAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .DeactivateIdentityUser(id, cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
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

    #endregion


    #region External Logins

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetExternalLoginsAsync([FromRoute][Required] TIdentity userId, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        var userLoginInfos = await this.identityRepository
            .GetUserExternalLoginsAsync(userId, cancellationToken);

        var externalLogins = userLoginInfos
            .Select(x => new ExternalLogin
            {
                Key = x.ProviderKey,
                Provider = new ExternalLoginProvider
                {
                    Name = x.LoginProvider,
                    DisplayName = x.ProviderDisplayName
                }
            });

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AddExternalLoginGoogleAsync([FromBody][Required] AddExternalLoginGoogle<TIdentity> addExternalLogin, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        var externalProviderLogInData = await this.authExternalRepository
            .AuthenticateAsync(addExternalLogin.Provider, cancellationToken);

        var userLoginInfo = await this.identityRepository
            .AddExternalLoginAsync(addExternalLogin.UserId, new ExternalProvider
            {
                LoginProvider = externalProviderLogInData.ExternalToken.Name,
                ProviderKey = externalProviderLogInData.Id
            }, cancellationToken);

        var externalLogin = new ExternalLogin
        {
            Key = userLoginInfo.ProviderKey,
            Provider = new ExternalLoginProvider
            {
                Name = userLoginInfo.LoginProvider,
                DisplayName = userLoginInfo.ProviderDisplayName
            }
        };

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AddExternalLoginFacebookAsync([FromBody][Required] AddExternalLoginFacebook<TIdentity> addExternalLogin, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        var externalProviderLogInData = await this.authExternalRepository
            .AuthenticateAsync(addExternalLogin.Provider, cancellationToken);

        var userLoginInfo = await this.identityRepository
            .AddExternalLoginAsync(addExternalLogin.UserId, new ExternalProvider
            {
                LoginProvider = externalProviderLogInData.ExternalToken.Name,
                ProviderKey = externalProviderLogInData.Id
            }, cancellationToken);

        var externalLogin = new ExternalLogin
        {
            Key = userLoginInfo.ProviderKey,
            Provider = new ExternalLoginProvider
            {
                Name = userLoginInfo.LoginProvider,
                DisplayName = userLoginInfo.ProviderDisplayName
            }
        };

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AddExternalLoginMicrosoftAsync([FromBody][Required] AddExternalLoginMicrosoft<TIdentity> addExternalLogin, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            throw new NullReferenceException(nameof(this.authExternalRepository));
        }

        var externalProviderLogInData = await this.authExternalRepository
            .AuthenticateAsync(addExternalLogin.Provider, cancellationToken);

        var userLoginInfo = await this.identityRepository
            .AddExternalLoginAsync(addExternalLogin.UserId, new ExternalProvider
            {
                LoginProvider = externalProviderLogInData.ExternalToken.Name,
                ProviderKey = externalProviderLogInData.Id
            }, cancellationToken);

        var externalLogin = new ExternalLogin
        {
            Key = userLoginInfo.ProviderKey,
            Provider = new ExternalLoginProvider
            {
                Name = userLoginInfo.LoginProvider,
                DisplayName = userLoginInfo.ProviderDisplayName
            }
        };

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveExternalLoginAsync([FromBody][Required] RemoveExternalLogin<TIdentity> removeExternalLogin, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .RemoveExternalLoginAsync(removeExternalLogin, cancellationToken);

        return this.Ok();
    }

    #endregion


    #region Refresh Tokens

    /// <summary>
    /// Gets refresh tokens of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The refresh tokens.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("refresh-tokens/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<IdentityUserRefreshToken<Guid>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetRefreshTokensAsync([FromRoute][Required] TIdentity userId, CancellationToken cancellationToken = default)
    {
        var identityUserRefreshTokens = await this.identityRepository
            .GetRefreshTokens(userId, cancellationToken);

        return this.Ok(identityUserRefreshTokens);
    }

    /// <summary>
    /// Gets active refresh tokens of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The refresh tokens.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("refresh-tokens/active/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<IdentityUserRefreshToken<Guid>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetActiveRefreshTokensAsync([FromRoute][Required] TIdentity userId, CancellationToken cancellationToken = default)
    {
        var identityUserRefreshTokens = await this.identityRepository
            .GetActiveRefreshTokens(userId, cancellationToken);

        return this.Ok(identityUserRefreshTokens);
    }

    /// <summary>
    /// Delete Refresh Token.
    /// </summary>
    /// <param name="refreshTokenId">The refresh token id.</param>
    /// <param name="cancellationToken">The token used when request is cancelled.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Ok.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpDelete]
    [Route("refresh-tokens/delete/{refreshTokenId}")]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteRefreshTokenAsync([FromRoute][Required] TIdentity refreshTokenId, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .DeleteRefreshTokenAsync(refreshTokenId, cancellationToken);

        return this.Ok();
    }

    #endregion


    #region Api Keys

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetApiKeysAsync([FromRoute][Required] TIdentity userId, CancellationToken cancellationToken = default)
    {
        var identityApiKeys = await this.identityRepository
            .GetApiKeysAsync(userId, cancellationToken);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateApiKeyAsync([FromBody][Required] CreateApiKey<TIdentity> createApiKey, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        var identityApiKey = this.identityRepository
            .CreateApiKeyAsync(createApiKey, out var apiKey);

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditApiKeyAsync([FromBody][Required] EditApiKey<TIdentity> editApiKey, CancellationToken cancellationToken = default)
    {
        var identityApiKey = await this.identityRepository
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RevokeApiKeyAsync([FromRoute][Required] TIdentity apiKeyId, [FromQuery] DateTimeOffset? revokeAt, CancellationToken cancellationToken = default)
    {
        var identityApiKey = await this.identityRepository
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

    #endregion


    #region Roles

    /// <summary>
    /// Gets roles.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The roles.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpGet]
    [Route("roles")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [ProducesResponseType(typeof(IEnumerable<IdentityRole<Guid>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await this.identityRepository
            .GetRolesAsync(cancellationToken);

        return this.Ok(roles);
    }

    /// <summary>
    /// Create a role.
    /// </summary>
    /// <param name="assignRole">The create role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The role.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("roles/create")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<IdentityRole<Guid>>), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateRoleAsync([FromBody][Required] CreateRole assignRole, CancellationToken cancellationToken = default)
    {
        var identityRole = await this.identityRepository
            .CreateRoleAsync(assignRole.Name, cancellationToken);

        return this.Created("roles/create", identityRole);
    }

    /// <summary>
    /// Delete a role.
    /// </summary>
    /// <param name="removeRole">The delete role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [HttpDelete]
    [Route("roles/delete")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteRoleAsync([FromBody][Required] DeleteRole removeRole, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .DeleteRoleAsync(removeRole.Name, cancellationToken);

        return this.Ok();
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
    [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetUserRolesAsync([FromRoute][Required] TIdentity userId, CancellationToken cancellationToken = default)
    {
        var roles = await this.identityRepository
            .GetUserRolesAsync(userId, cancellationToken);

        return this.Ok(roles);
    }

    /// <summary>
    /// Assign a role to a user.
    /// </summary>
    /// <param name="assignUserRole">The assign role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("roles/user/assign")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignUserRoleAsync([FromBody][Required] AssignUserRole<TIdentity> assignUserRole, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .AssignUserRoleAsync(assignUserRole, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Remove a role from a user.
    /// </summary>
    /// <param name="removeUserRole">The remove role.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [HttpDelete]
    [Route("roles/user/remove")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveUserRoleAsync([FromBody][Required] RemoveUserRole<TIdentity> removeUserRole, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .RemoveUserRoleAsync(removeUserRole, cancellationToken);

        return this.Ok();
    }

    #endregion


    #region Role Claims

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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetRoleClaimsAsync([FromRoute][Required] TIdentity roleId, CancellationToken cancellationToken = default)
    {
        var roleClaims = await this.identityRepository
            .GetRoleClaimsAsync(roleId, cancellationToken);

        return this.Ok(roleClaims);
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignRoleClaimAsync([FromBody][Required] AssignRoleClaim<TIdentity> assignRoleClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveRoleClaimAsync([FromBody][Required] RemoveRoleClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ReplaceRoleClaimAsync([FromBody][Required] ReplaceRoleClaim<TIdentity> replaceClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ReplaceRoleClaimAsync(replaceClaim, cancellationToken);

        return this.Ok();
    }

    #endregion


    #region Claims

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
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<Claim>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetClaimsAsync([FromRoute][Required] TIdentity userId, CancellationToken cancellationToken = default)
    {
        var userClaims = await this.identityRepository
            .GetUserClaimsAsync(userId, cancellationToken);

        return this.Ok(userClaims);
    }

    /// <summary>
    /// Assign a claim to a user.
    /// </summary>
    /// <param name="assignUserClaim">The assign claim.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPost]
    [Route("claims/assign")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignClaimAsync([FromBody][Required] AssignUserClaim<TIdentity> assignUserClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .AssignUserClaimAsync(assignUserClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Remove a claim from a user.
    /// </summary>
    /// <param name="removeUserClaim">The remove claim.</param>
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
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveClaimAsync([FromBody][Required] RemoveUserClaim<TIdentity> removeUserClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .RemoveUserClaimAsync(removeUserClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Replace a claim to a user.
    /// </summary>
    /// <param name="replaceUserClaim">The replace claim.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occured.</response>
    [HttpPut]
    [Route("claims/replace")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ReplaceClaimAsync([FromBody][Required] ReplaceUserClaim<TIdentity> replaceUserClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ReplaceUserClaimAsync(replaceUserClaim, cancellationToken);

        return this.Ok();
    }

    #endregion
}