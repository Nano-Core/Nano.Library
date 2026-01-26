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
using Nano.Common.Annotations;
using Nano.Common.Consts;
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
/// Identity controller providing identity-related endpoints.
/// </summary>
/// <typeparam name="TRepository">The repository type.</typeparam>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TIdentity">The identity key type.</typeparam>
/// <typeparam name="TCriteria">The query criteria type.</typeparam>
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.IDENTITY)]
public abstract class BaseIdentityController<TRepository, TEntity, TIdentity, TCriteria> : BaseControllerUpdatable<TRepository, TEntity, TIdentity, TCriteria>
    where TRepository : class, IRepository
    where TEntity : class, IEntityUser<TIdentity>, IEntityUpdatable, IEntityDeletable, IEntityIdentity<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
    where TCriteria : class, IQueryCriteria, new()
{
    private readonly IIdentityRepository<TIdentity> identityRepository;
    private readonly IAuthExternalRepository? authExternalRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseIdentityController{TRepository,TEntity,TIdentity,TCriteria}"/> class.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="repository">The <see cref="IRepository"/>.</param>
    /// <param name="identityRepository">The <see cref="IIdentityRepository{TIdentity}"/>.</param>
    /// <param name="authExternalRepository">The optional <see cref="IAuthExternalRepository"/>.</param>
    protected BaseIdentityController(ILogger logger, TRepository repository, IIdentityRepository<TIdentity> identityRepository, IAuthExternalRepository? authExternalRepository = null)
        : base(logger, repository)
    {
        this.identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        this.authExternalRepository = authExternalRepository;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseIdentityController{TRepository,TEntity,TIdentity,TCriteria}"/> class with eventing support.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="repository">The <see cref="IRepository"/>.</param>
    /// <param name="eventing">The <see cref="IEventing"/>.</param>
    /// <param name="identityRepository">The <see cref="IIdentityRepository{TIdentity}"/>.</param>
    /// <param name="authExternalRepository">The optional <see cref="IAuthExternalRepository"/>.</param>
    protected BaseIdentityController(ILogger logger, TRepository repository, IEventing eventing, IIdentityRepository<TIdentity> identityRepository, IAuthExternalRepository? authExternalRepository = null)
        : base(logger, repository, eventing)
    {
        this.identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        this.authExternalRepository = authExternalRepository;
    }


    #region Sign Up

    /// <summary>
    /// Retrieves the configured password options.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The password options.</returns>
    /// <response code="200">Success.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Determines whether an email address is already in use.
    /// </summary>
    /// <param name="emailAddress">The email address to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Whether the email address is taken.</returns>
    /// <response code="200">Success.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="500">Error occurred.</response>
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
        var response = await this.identityRepository
            .IsEmailAddressTakenAsync(emailAddress, cancellationToken);

        return this.Ok(response);
    }

    /// <summary>
    /// Determines whether a phone number is already in use.
    /// </summary>
    /// <param name="phoneNumber">The phone number to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Whether the phone number is taken.</returns>
    /// <response code="200">Success.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="500">Error occurred.</response>
    [HttpGet]
    [Route("phone/is-taken")]
    [AllowAnonymous]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IsPhoneNumberTaken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> IsPhoneNumberTakenAsync([FromQuery][Required][InternationalPhone]string phoneNumber, CancellationToken cancellationToken = default)
    {
        var response = await this.identityRepository
            .IsPhoneNumberTakenAsync(phoneNumber, cancellationToken);

        return this.Ok(response);
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="signUp">The sign-up request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user.</returns>
    /// <response code="201">Created.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Registers a new user using externally provided login data.
    /// </summary>
    /// <param name="signUpExternal">The external sign-up request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user.</returns>
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
    /// Registers a new user using an external Google login provider.
    /// </summary>
    /// <param name="signUpExternal">The external Google sign-up request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user.</returns>
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
    /// Registers a new user using an external Facebook login provider.
    /// </summary>
    /// <param name="signUpExternal">The external Facebook sign-up request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user.</returns>
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
    /// Registers a new user using an external Microsoft login provider.
    /// </summary>
    /// <param name="signUpExternal">The external Microsoft sign-up request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user.</returns>
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
    /// Sets the username of a user.
    /// </summary>
    /// <param name="setUsername">The username update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Only use this operation to assign a password to a user that does not already have one.
    /// Users authenticated through external login providers do not initially have a password.
    /// </summary>
    /// <param name="setPassword">The password assignment request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Changes the password of an existing user.
    /// </summary>
    /// <param name="changePassword">The password change request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Resets the password of a user using a reset token.
    /// </summary>
    /// <param name="resetPassword">The password reset request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Generates a password reset token used to reset a user's password.
    /// </summary>
    /// <param name="generateResetPasswordToken">The reset password token generation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The password reset token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Changes the email address of a user.
    /// </summary>
    /// <param name="changeEmail">The email change request.</param>
    /// <param name="setUsername">
    /// Indicates whether the username should also be updated to match the new email address.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Generates an email change token used to change a user's email address.
    /// </summary>
    /// <param name="generateChangeEmailToken">The email change token generation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email change token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// <param name="confirmEmail">The email confirmation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Generates an email confirmation token used to confirm a user's email address.
    /// </summary>
    /// <param name="generateConfirmEmailToken">The email confirmation token generation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email confirmation token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// <param name="changePhoneNumber">The phone number change request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Generates a phone number change token used to change a user's phone number.
    /// </summary>
    /// <param name="generateChangePhoneToken">The phone number change token generation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The phone number change token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// <param name="confirmPhoneNumber">The phone number confirmation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Generates a phone number confirmation token used to confirm a user's phone number.
    /// </summary>
    /// <param name="generateConfirmPhoneToken">The phone number confirmation token generation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The phone number confirmation token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Generates a custom-purpose token for a user.
    /// </summary>
    /// <param name="confirmEmail">The custom-purpose token generation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The custom-purpose token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Confirms a previously generated custom-purpose token for a user.
    /// </summary>
    /// <param name="confirmCustomPurpose">The custom-purpose token confirmation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Activates the user with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the user to activate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Deactivates the user with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the user to deactivate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Deletes the user with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the user to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Deletes multiple users with the specified identifiers.
    /// </summary>
    /// <param name="ids">The identifiers of the users to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Retrieves the external login providers associated with a user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>The collection of external logins.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Adds a Google external login to a user account.
    /// </summary>
    /// <param name="addExternalLogin">The request containing Google external login data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added external login.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Adds a Microsoft external login to a user account.
    /// </summary>
    /// <param name="addExternalLogin">The request containing Microsoft external login data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added external login.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Adds a Microsoft external login to a user account.
    /// </summary>
    /// <param name="addExternalLogin">The request containing Microsoft external login data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added external login.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Removes an external login from a user account.
    /// </summary>
    /// <param name="removeExternalLogin">The request identifying the external login to remove.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
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
    /// Retrieves all refresh tokens associated with a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of refresh tokens.</returns>
    /// <response code="200">Success. Returns the refresh tokens.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Retrieves all active (non-revoked) refresh tokens for a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of active refresh tokens.</returns>
    /// <response code="200">Success. Returns the active refresh tokens.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Deletes a specific refresh token by its identifier.
    /// </summary>
    /// <param name="refreshTokenId">The identifier of the refresh token to delete.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success. The refresh token was deleted.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The refresh token does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Retrieves all API keys associated with a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of API keys for the user.</returns>
    /// <response code="200">Success. Returns the API keys.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Creates a new API key for a user.
    /// </summary>
    /// <param name="createApiKey">The API key creation request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>The created API key along with its unencrypted hash.</returns>
    /// <response code="201">Created. Returns the created API key.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Edits an existing API key.
    /// </summary>
    /// <param name="editApiKey">The API key edit request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>The updated API key.</returns>
    /// <response code="200">Success. Returns the updated API key.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The API key does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Revokes a specific API key, optionally at a future date.
    /// </summary>
    /// <param name="apiKeyId">The identifier of the API key to revoke.</param>
    /// <param name="revokeAt">The optional date and time when the API key will be revoked.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>The revoked API key.</returns>
    /// <response code="200">Success. Returns the revoked API key.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The API key does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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


    #region Claims

    /// <summary>
    /// Retrieves all claims assigned to a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of <see cref="Claim"/> objects for the user.</returns>
    /// <response code="200">Success. Returns the user's claims.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Assigns a new claim to a user.
    /// </summary>
    /// <param name="assignUserClaim">The claim assignment request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was assigned.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Replaces an existing claim of a user with a new claim.
    /// </summary>
    /// <param name="replaceUserClaim">The claim replacement request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was replaced.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The claim or user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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

    /// <summary>
    /// Assigns or replaces an existing claim of a user with a new claim.
    /// </summary>
    /// <param name="assignOrReplaceUserClaim">The claim assign or replacement request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was assigned or replaced.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The claim or user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPut]
    [Route("claims/assign-or-replace")]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignOrReplaceClaimAsync([FromBody][Required]AssignOrReplaceUserClaim<TIdentity> assignOrReplaceUserClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .AssignOrReplaceUserClaimAsync(assignOrReplaceUserClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Removes a claim from a user.
    /// </summary>
    /// <param name="removeUserClaim">The claim removal request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was removed.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The claim or user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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

    #endregion


    #region Roles

    /// <summary>
    /// Retrieves all roles in the system.
    /// </summary>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of <see cref="IdentityRole{Guid}"/> objects.</returns>
    /// <response code="200">Success. Returns the list of roles.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized. User is not allowed to view roles.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Creates a new role.
    /// </summary>
    /// <param name="assignRole">The role creation request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>The created <see cref="IdentityRole{Guid}"/>.</returns>
    /// <response code="201">Created. Returns the newly created role.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized. User is not allowed to create roles.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Deletes a role from the system.
    /// </summary>
    /// <param name="removeRole">The role deletion request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The role was deleted.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized. User is not allowed to delete roles.</response>
    /// <response code="404">Not Found. The role does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Retrieves all roles assigned to a specific user.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of role names assigned to the user.</returns>
    /// <response code="200">Success. Returns the user's roles.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="assignUserRole">The user role assignment request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The role was assigned to the user.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user or role does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Removes a role from a user.
    /// </summary>
    /// <param name="removeUserRole">The user role removal request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The role was removed from the user.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user or role does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Retrieves all claims associated with a specific role.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of <see cref="Claim"/> objects for the role.</returns>
    /// <response code="200">Success. Returns the list of claims for the role.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized. User is not allowed to view role claims.</response>
    /// <response code="404">Not Found. The role does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Assigns a claim to a role.
    /// </summary>
    /// <param name="assignRoleClaim">The role claim assignment request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was assigned to the role.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized. User is not allowed to assign role claims.</response>
    /// <response code="404">Not Found. The role does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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
    /// Replaces a claim of a role with a new claim.
    /// </summary>
    /// <param name="replaceClaim">The role claim replacement request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was replaced for the role.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized. User is not allowed to replace role claims.</response>
    /// <response code="404">Not Found. The role or claim does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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

    /// <summary>
    /// Assigns or Replaces a claim of a role with a new claim.
    /// </summary>
    /// <param name="replaceClaim">The role claim assignment or replacement request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was assigned or replaced for the role.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized. User is not allowed to replace role claims.</response>
    /// <response code="404">Not Found. The role or claim does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPut]
    [Route("roles/claims/assign-or-replace")]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignOrReplaceRoleClaimAsync([FromBody][Required] AssignOrReplaceRoleClaim<TIdentity> replaceClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .AssignOrReplaceRoleClaimAsync(replaceClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Removes a claim from a role.
    /// </summary>
    /// <param name="removeClaim">The role claim removal request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was removed from the role.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized. User is not allowed to remove role claims.</response>
    /// <response code="404">Not Found. The role or claim does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
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

    #endregion
}