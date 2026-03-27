using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.App.Consts;
using Nano.Common.Annotations;
using Nano.Common.Consts;
using Nano.Data.Abstractions;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Eventing.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using PasswordOptions = Nano.Data.Abstractions.Config.PasswordOptions;

namespace Nano.App.Api.Controllers;

// TODO: API-KEY: Distributed architecture(Add Validate api-key endpoint, Still needs ApiKeyAuthenticationHandler? Kubernetes ingress nginx integration)
// - (API-KEY: IdentityApiKey Roles and Claims (don't inherit from IdentityUser))  https://chatgpt.com/c/695ceb26-c6e4-832f-8840-b36bd21b5be9

/// <inheritdoc />
public abstract class BaseIdentityController<TEntity, TCriteria> : BaseIdentityController<TEntity, Guid, TCriteria>
    where TEntity : class, IEntityUser<Guid>, new()
    where TCriteria : class, IQueryCriteria, new()
{
    /// <inheritdoc />
    protected BaseIdentityController(ILogger<BaseIdentityController<TEntity, TCriteria>> logger, IRepository repository, IIdentityRepository identityRepository, IAuthExternalRepositoryAggregator? authExternalRepository = null)
        : base(logger, repository, identityRepository, authExternalRepository)
    {
    }

    /// <inheritdoc />
    protected BaseIdentityController(ILogger<BaseIdentityController<TEntity, TCriteria>> logger, IRepository repository, IEventing eventing, IIdentityRepository<Guid> identityRepository, IAuthExternalRepositoryAggregator? authExternalRepository = null)
        : base(logger, repository, eventing, identityRepository, authExternalRepository)
    {
    }
}

/// <inheritdoc />
[Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.IDENTITY)]
public abstract class BaseIdentityController<TEntity, TIdentity, TCriteria> : BaseEntityUpdatableController<TEntity, TIdentity, TCriteria>
    where TEntity : class, IEntityUser<TIdentity>, new()
    where TIdentity : IEquatable<TIdentity>
    where TCriteria : class, IQueryCriteria, new()
{
    /// <summary>
    /// Get or set the identity repository.
    /// </summary>
    protected readonly IIdentityRepository<TIdentity> identityRepository;

    /// <summary>
    /// Get or set the external authentication repository.
    /// </summary>
    protected readonly IAuthExternalRepositoryAggregator? authExternalRepository;

    /// <inheritdoc />
    protected BaseIdentityController(ILogger<BaseIdentityController<TEntity, TIdentity, TCriteria>> logger, IRepository repository, IIdentityRepository<TIdentity> identityRepository, IAuthExternalRepositoryAggregator? authExternalRepository = null)
        : base(logger, repository)
    {
        this.identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        this.authExternalRepository = authExternalRepository;
    }

    /// <inheritdoc />
    protected BaseIdentityController(ILogger<BaseIdentityController<TEntity, TIdentity, TCriteria>> logger, IRepository repository, IEventing eventing, IIdentityRepository<TIdentity> identityRepository, IAuthExternalRepositoryAggregator? authExternalRepository = null)
        : base(logger, repository, eventing)
    {
        this.identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        this.authExternalRepository = authExternalRepository;
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
    [Route(ActionRoutes.DELETE)]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.DELETER)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> DeleteAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .DeleteUserAsync(id, cancellationToken);

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
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.DELETER)]
    [Route(ActionRoutes.DELETE_MANY)]
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
            await this.identityRepository
                .DeleteUserAsync(id, cancellationToken);
        }

        await this.Repository
            .SaveChangesAsync(cancellationToken);

        return this.Ok();
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
    [Route(ActionRoutes.IDENTITY_PASSWORD_OPTIONS)]
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
    [Route(ActionRoutes.IDENTITY_EMAIL_IS_TAKEN)]
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
    [Route(ActionRoutes.IDENTITY_PHONE_IS_TAKEN)]
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
    [Route(ActionRoutes.IDENTITY_SIGNUP)]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(BaseEntityUser), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpAsync([FromBody][Required] SignUp<TEntity, TIdentity> signUp, CancellationToken cancellationToken = default)
    {
        var user = await this.identityRepository
            .SignUpAsync(signUp, cancellationToken);

        return this.Created(ActionRoutes.IDENTITY_SIGNUP, user);
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
    [Route(ActionRoutes.IDENTITY_SIGNUP_EXTERNAL_FACEBOOK)]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(BaseEntityUser), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalFacebookAsync([FromBody][Required] SignUpExternalFacebook<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            return this.NotFound();
        }

        var authenticationData = await this.authExternalRepository
            .AuthenticateAsync(signUpExternal.Provider, cancellationToken);

        var user = await this.identityRepository
            .SignUpExternalAsync(new SignUpExternal<TEntity, TIdentity>
            {
                User = signUpExternal.User,
                Username = authenticationData.Username,
                EmailAddress = authenticationData.EmailAddress,
                PhoneNumber = authenticationData.PhoneNumber,
                ExternalProvider =
                {
                    Name = authenticationData.ExternalToken.Name,
                    UserId = authenticationData.Id
                },
                Roles = signUpExternal.Roles,
                Claims = signUpExternal.Claims
            }, cancellationToken);

        return this.Created(ActionRoutes.IDENTITY_SIGNUP_EXTERNAL_FACEBOOK, user);
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
    [Route(ActionRoutes.IDENTITY_SIGNUP_EXTERNAL_GOOGLE)]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(BaseEntityUser), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalGoogleAsync([FromBody][Required] SignUpExternalGoogle<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            return this.NotFound();
        }

        var authenticationData = await this.authExternalRepository
            .AuthenticateAsync(signUpExternal.Provider, cancellationToken);

        var user = await this.identityRepository
            .SignUpExternalAsync(new SignUpExternal<TEntity, TIdentity>
            {
                User = signUpExternal.User,
                Username = authenticationData.Username,
                EmailAddress = authenticationData.EmailAddress,
                PhoneNumber = authenticationData.PhoneNumber,
                ExternalProvider =
                {
                    Name = authenticationData.ExternalToken.Name,
                    UserId = authenticationData.Id
                },
                Roles = signUpExternal.Roles,
                Claims = signUpExternal.Claims
            }, cancellationToken);

        return this.Created(ActionRoutes.IDENTITY_SIGNUP_EXTERNAL_GOOGLE, user);
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
    [Route(ActionRoutes.IDENTITY_SIGNUP_EXTERNAL_MICROSOFT)]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(BaseEntityUser), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SignUpExternalMicrosoftAsync([FromBody][Required] SignUpExternalMicrosoft<TEntity, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            return this.NotFound();
        }

        var authenticationData = await this.authExternalRepository
            .AuthenticateAsync(signUpExternal.Provider, cancellationToken);

        var user = await this.identityRepository
            .SignUpExternalAsync(new SignUpExternal<TEntity, TIdentity>
            {
                User = signUpExternal.User,
                Username = authenticationData.Username,
                EmailAddress = authenticationData.EmailAddress,
                PhoneNumber = authenticationData.PhoneNumber,
                ExternalProvider =
                {
                    Name = authenticationData.ExternalToken.Name,
                    UserId = authenticationData.Id
                },
                Roles = signUpExternal.Roles,
                Claims = signUpExternal.Claims
            }, cancellationToken);

        return this.Created(ActionRoutes.IDENTITY_SIGNUP_EXTERNAL_MICROSOFT, user);
    }

    #endregion


    #region User

    /// <summary>
    /// Sets the username of a user.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="setUsername">The username update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_USERNAME_SET)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SetUsernameAsync([FromRoute][Required]TIdentity id, [FromBody][Required] SetUsername setUsername, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .SetUsernameAsync(id, setUsername, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Sets the password of a user.
    /// Only use this operation to assign a password to a user that does not already have one.
    /// Users authenticated through external login providers do not initially have a password.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="setPassword">The password assignment request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_PASSWORD_SET)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> SetPasswordAsync([FromRoute][Required] TIdentity id, [FromBody][Required] SetPassword setPassword, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .SetPasswordAsync(id, setPassword, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Changes the password of an existing user.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="changePassword">The password change request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_PASSWORD_CHANGE)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ChangePasswordAsync([FromRoute][Required] TIdentity id, [FromBody][Required] ChangePassword changePassword, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ChangePasswordAsync(id, changePassword, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Resets the password of a user using a reset token.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="resetPassword">The password reset request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_PASSWORD_RESET)]
    [AllowAnonymous]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ResetPasswordAsync([FromRoute][Required] TIdentity id, [FromBody][Required] ResetPassword resetPassword, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ResetPasswordAsync(id, resetPassword, cancellationToken);

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
    [Route(ActionRoutes.IDENTITY_PASSWORD_RESET_TOKEN)]
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
    /// <param name="id">The id of the user.</param>
    /// <param name="changeEmail">The email change request.</param>
    /// <param name="setUsername">Indicates whether the username should also be updated to match the new email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_EMAIL_CHANGE)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ChangeEmailAsync([FromRoute][Required] TIdentity id, [FromBody][Required] ChangeEmail changeEmail, [FromQuery] bool setUsername = false, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ChangeEmailAsync(id, changeEmail, setUsername, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Generates an email change token used to change a user's email address.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="generateChangeEmailToken">The email change token generation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email change token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_EMAIL_CHANGE_TOKEN)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ChangeEmailToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetChangeEmailTokenAsync([FromRoute][Required] TIdentity id, [FromBody][Required] GenerateChangeEmailToken generateChangeEmailToken, CancellationToken cancellationToken = default)
    {
        var changeEmailToken = await this.identityRepository
            .GenerateChangeEmailTokenAsync(id, generateChangeEmailToken, cancellationToken);

        return this.Ok(changeEmailToken);
    }

    /// <summary>
    /// Confirms the email address of a user.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="confirmEmail">The email confirmation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_EMAIL_CONFIRM)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ConfirmEmailAsync([FromRoute][Required] TIdentity id, [FromBody][Required] ConfirmEmail confirmEmail, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ConfirmEmailAsync(id, confirmEmail, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Generates an email confirmation token used to confirm a user's email address.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The email confirmation token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_EMAIL_CONFIRM_TOKEN)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ConfirmEmailToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetConfirmEmailTokenAsync([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = default)
    {
        var confirmEmailToken = await this.identityRepository
            .GenerateConfirmEmailTokenAsync(id, cancellationToken);

        return this.Ok(confirmEmailToken);
    }

    /// <summary>
    /// Changes the phone number of a user.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="changePhoneNumber">The phone number change request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_PHONE_CHANGE)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ChangePhoneAsync([FromRoute][Required] TIdentity id, [FromBody][Required] ChangePhoneNumber changePhoneNumber, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ChangePhoneNumberAsync(id, changePhoneNumber, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Generates a phone number change token used to change a user's phone number.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="generateChangePhoneToken">The phone number change token generation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The phone number change token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_PHONE_CHANGE_TOKEN)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ChangePhoneNumberToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetChangePhoneTokenAsync([FromRoute][Required] TIdentity id, [FromBody][Required] GenerateChangePhoneToken generateChangePhoneToken, CancellationToken cancellationToken = default)
    {
        var changeEmailToken = await this.identityRepository
            .GenerateChangePhoneNumberTokenAsync(id, generateChangePhoneToken, cancellationToken);

        return this.Ok(changeEmailToken);
    }

    /// <summary>
    /// Confirms the phone number of a user.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="confirmPhoneNumber">The phone number confirmation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_PHONE_CONFIRM)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ConfirmPhoneAsync([FromRoute][Required] TIdentity id, [FromBody][Required] ConfirmPhoneNumber confirmPhoneNumber, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ConfirmPhoneNumberAsync(id, confirmPhoneNumber, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Generates a phone number confirmation token used to confirm a user's phone number.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The phone number confirmation token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_PHONE_CONFIRM_TOKEN)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ConfirmPhoneNumberToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetConfirmPhoneTokenAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        var confirmEmailToken = await this.identityRepository
            .GenerateConfirmPhoneNumberTokenAsync(id, cancellationToken);

        return this.Ok(confirmEmailToken);
    }

    /// <summary>
    /// Confirms a previously generated custom-purpose token for a user.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="confirmCustomPurpose">The custom-purpose token confirmation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_CUSTOM_PURPOSE_CONFIRM)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public virtual async Task<IActionResult> GetCustomPurposeTokenAsync([FromRoute][Required] TIdentity id, [FromBody][Required] ConfirmCustomPurpose confirmCustomPurpose, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ConfirmCustomPurposeTokenAsync(id, confirmCustomPurpose, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Generates a custom-purpose token for a user.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <param name="generateCustomPurposeToken">The custom-purpose token generation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The custom-purpose token.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_CUSTOM_PURPOSE_CONFIRM_TOKEN)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(ConfirmCustomPurposeToken), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ConfirmCustomPurposeTokenAsync([FromRoute][Required] TIdentity id, [FromBody][Required] GenerateCustomPurposeToken generateCustomPurposeToken, CancellationToken cancellationToken = default)
    {
        var customPurposeToken = await this.identityRepository
            .GenerateCustomPurposeTokenAsync(id, generateCustomPurposeToken, cancellationToken);

        return this.Ok(customPurposeToken);
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
    [Route(ActionRoutes.IDENTITY_ACTIVATE)]
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
    [Route(ActionRoutes.IDENTITY_DEACTIVATE)]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR + "," + BuiltInUserRoles.WRITER + "," + BuiltInUserRoles.DELETER)]
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

    #endregion


    #region User Roles

    /// <summary>
    /// Retrieves all roles assigned to a specific user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of role names assigned to the user.</returns>
    /// <response code="200">Success. Returns the user's roles.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpGet]
    [Route(ActionRoutes.IDENTITY_ROLES_USER)]
    [ProducesResponseType(typeof(IEnumerable<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetUserRolesAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        var roles = await this.identityRepository
            .GetUserRolesAsync(id, cancellationToken);

        return this.Ok(roles);
    }

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="assignUserRole">The user role assignment request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The role was assigned to the user.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user or role does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_ROLES_USER_ASSIGN)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignUserRoleAsync([FromRoute][Required] TIdentity id, [FromBody][Required] AssignUserRole assignUserRole, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .AssignUserRoleAsync(id, assignUserRole, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
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
    [Route(ActionRoutes.IDENTITY_ROLES_USER_REMOVE)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveUserRoleAsync([FromRoute][Required] TIdentity id, [FromBody][Required] RemoveUserRole removeUserRole, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .RemoveUserRoleAsync(id, removeUserRole, cancellationToken);

        return this.Ok();
    }

    #endregion


    #region User Claims

    /// <summary>
    /// Retrieves all claims assigned to a specific user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of <see cref="Claim"/> objects for the user.</returns>
    /// <response code="200">Success. Returns the user's claims.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpGet]
    [Route(ActionRoutes.IDENTITY_CLAIMS)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<Claim>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetClaimsAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        var userClaims = await this.identityRepository
            .GetUserClaimsAsync(id, cancellationToken);

        return this.Ok(userClaims);
    }

    /// <summary>
    /// Assigns a new claim to a user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="assignUserClaim">The claim assignment request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was assigned.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_CLAIMS_ASSIGN)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignClaimAsync([FromRoute][Required] TIdentity id, [FromBody][Required] AssignUserClaim assignUserClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .AssignUserClaimAsync(id, assignUserClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Replaces an existing claim of a user with a new claim.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="replaceUserClaim">The claim replacement request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was replaced.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The claim or user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPut]
    [Route(ActionRoutes.IDENTITY_CLAIMS_REPLACE)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ReplaceClaimAsync([FromRoute][Required] TIdentity id, [FromBody][Required] ReplaceUserClaim replaceUserClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ReplaceUserClaimAsync(id, replaceUserClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Assigns or replaces an existing claim of a user with a new claim.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="assignOrReplaceUserClaim">The claim assign or replacement request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was assigned or replaced.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The claim or user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPut]
    [Route(ActionRoutes.IDENTITY_CLAIMS_ASSIGN_OR_REPLACE)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignOrReplaceClaimAsync([FromRoute][Required] TIdentity id, [FromBody][Required] AssignOrReplaceUserClaim assignOrReplaceUserClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .AssignOrReplaceUserClaimAsync(id, assignOrReplaceUserClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Removes a claim from a user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
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
    [Route(ActionRoutes.IDENTITY_CLAIMS_REMOVE)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveClaimAsync([FromRoute][Required] TIdentity id, [FromBody][Required] RemoveUserClaim removeUserClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .RemoveUserClaimAsync(id, removeUserClaim, cancellationToken);

        return this.Ok();
    }

    #endregion


    #region User External Logins

    /// <summary>
    /// Retrieves the external login providers associated with a user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>The collection of external logins.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpGet]
    [Route(ActionRoutes.IDENTITY_EXTERNAL_LOGINS)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<ExternalLogin>), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetExternalLoginsAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        var userLoginInfos = await this.identityRepository
            .GetUserExternalLoginsAsync(id, cancellationToken);

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
    /// Adds a facebook external login to a user account.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="addExternalLogin">The request containing Microsoft external login data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added external login.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_EXTERNAL_LOGINS_ADD_FACEBOOK)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<ExternalLogin>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AddExternalLoginFacebookAsync([FromRoute][Required] TIdentity id, [FromBody][Required] AddExternalLoginFacebook addExternalLogin, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            return this.NotFound();
        }

        var externalProviderLogInData = await this.authExternalRepository
            .AuthenticateAsync(addExternalLogin.Provider, cancellationToken);

        var userLoginInfo = await this.identityRepository
            .AddExternalLoginAsync(id, new ExternalProvider
            {
                Name = externalProviderLogInData.ExternalToken.Name,
                UserId = externalProviderLogInData.Id
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
    /// Adds a Google external login to a user account.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="addExternalLogin">The request containing Google external login data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added external login.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_EXTERNAL_LOGINS_ADD_GOOGLE)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<ExternalLogin>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AddExternalLoginGoogleAsync([FromRoute][Required] TIdentity id, [FromBody][Required] AddExternalLoginGoogle addExternalLogin, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            return this.NotFound();
        }

        var externalProviderLogInData = await this.authExternalRepository
            .AuthenticateAsync(addExternalLogin.Provider, cancellationToken);

        var userLoginInfo = await this.identityRepository
            .AddExternalLoginAsync(id, new ExternalProvider
            {
                Name = externalProviderLogInData.ExternalToken.Name,
                UserId = externalProviderLogInData.Id
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
    /// <param name="id">The identifier of the user.</param>
    /// <param name="addExternalLogin">The request containing Microsoft external login data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added external login.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_EXTERNAL_LOGINS_ADD_MICROSOFT)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<ExternalLogin>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AddExternalLoginMicrosoftAsync([FromRoute][Required] TIdentity id, [FromBody][Required] AddExternalLoginMicrosoft addExternalLogin, CancellationToken cancellationToken = default)
    {
        if (this.authExternalRepository == null)
        {
            return this.NotFound();
        }

        var externalProviderLogInData = await this.authExternalRepository
            .AuthenticateAsync(addExternalLogin.Provider, cancellationToken);

        var userLoginInfo = await this.identityRepository
            .AddExternalLoginAsync(id, new ExternalProvider
            {
                Name = externalProviderLogInData.ExternalToken.Name,
                UserId = externalProviderLogInData.Id
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
    /// Removes an facebook login from a user account.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [HttpDelete]
    [Route(ActionRoutes.IDENTITY_EXTERNAL_LOGINS_REMOVE_FACEBOOK)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveExternalLoginFacebookAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .RemoveExternalLoginAsync(id, new RemoveExternalLogin { LoginProvider = BuiltInExternalLogInProviderNames.FACEBOOK }, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Removes an google login from a user account.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [HttpDelete]
    [Route(ActionRoutes.IDENTITY_EXTERNAL_LOGINS_REMOVE_GOOGLE)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveExternalLoginGoogleAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .RemoveExternalLoginAsync(id, new RemoveExternalLogin { LoginProvider = BuiltInExternalLogInProviderNames.GOOGLE }, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Removes an microsoft login from a user account.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">Success.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Error occurred.</response>
    [HttpPost]
    [HttpDelete]
    [Route(ActionRoutes.IDENTITY_EXTERNAL_LOGINS_REMOVE_MICROSOFT)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveExternalLoginMicrosoftAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .RemoveExternalLoginAsync(id, new RemoveExternalLogin { LoginProvider = BuiltInExternalLogInProviderNames.MICROSOFT }, cancellationToken);

        return this.Ok();
    }

    #endregion


    #region Refresh Tokens

    /// <summary>
    /// Retrieves all refresh tokens associated with a specific user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of refresh tokens.</returns>
    /// <response code="200">Success. Returns the refresh tokens.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpGet]
    [Route(ActionRoutes.IDENTITY_REFRESH_TOKENS)]
    [ProducesResponseType(typeof(IEnumerable<IdentityUserRefreshToken<string>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetRefreshTokensAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUserRefreshTokens = await this.identityRepository
            .GetRefreshTokens(id, cancellationToken);

        return this.Ok(identityUserRefreshTokens);
    }

    /// <summary>
    /// Retrieves all active (non-revoked) refresh tokens for a specific user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of active refresh tokens.</returns>
    /// <response code="200">Success. Returns the active refresh tokens.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpGet]
    [Route(ActionRoutes.IDENTITY_REFRESH_TOKENS_ACTIVE)]
    [ProducesResponseType(typeof(IEnumerable<IdentityUserRefreshToken<string>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetActiveRefreshTokensAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityUserRefreshTokens = await this.identityRepository
            .GetActiveRefreshTokens(id, cancellationToken);

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
    [Route(ActionRoutes.IDENTITY_REFRESH_TOKENS_DELETE)]
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
    /// <param name="id">The identifier of the user.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>A collection of API keys for the user.</returns>
    /// <response code="200">Success. Returns the API keys.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpGet]
    [Route(ActionRoutes.IDENTITY_API_KEYS)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IEnumerable<IdentityApiKey<string>>), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> GetApiKeysAsync([FromRoute][Required] TIdentity id, CancellationToken cancellationToken = default)
    {
        var identityApiKeys = await this.identityRepository
            .GetApiKeysAsync(id, cancellationToken);

        return this.Ok(identityApiKeys);
    }

    /// <summary>
    /// Creates a new API key for a user.
    /// </summary>
    /// <param name="id">The identifier of the user.</param>
    /// <param name="createApiKey">The API key creation request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>The created API key along with its unencrypted hash.</returns>
    /// <response code="201">Created. Returns the created API key.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Not Found. The user does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_API_KEYS_CREATE)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IdentityApiKeyCreated<string>), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateApiKeyAsync([FromRoute][Required] TIdentity id, [FromBody][Required] CreateApiKey createApiKey, CancellationToken cancellationToken = default)
    {
        var identityApiKeyCreated = await this.identityRepository
            .CreateApiKeyAsync(id, createApiKey, cancellationToken);

        return this.Created(ActionRoutes.IDENTITY_API_KEYS_CREATE, identityApiKeyCreated);
    }

    /// <summary>
    /// Edits an existing API key.
    /// </summary>
    /// <param name="apiKeyid">The identifier of the api-key.</param>
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
    [Route(ActionRoutes.IDENTITY_API_KEYS_EDIT)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IdentityApiKey<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> EditApiKeyAsync([FromRoute][Required] TIdentity apiKeyid, [FromBody][Required] EditApiKey editApiKey, CancellationToken cancellationToken = default)
    {
        var identityApiKey = await this.identityRepository
            .EditApiKeyAsync(apiKeyid, editApiKey, cancellationToken);

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
    [Route(ActionRoutes.IDENTITY_API_KEYS_REVOKE)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IdentityApiKey<Guid>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RevokeApiKeyAsync([FromRoute][Required] TIdentity apiKeyId, [FromQuery] DateTimeOffset? revokeAt, CancellationToken cancellationToken = default)
    {
        var identityApiKey = await this.identityRepository
            .RevokeApiKeyAsync(apiKeyId, new RevokeApiKey<TIdentity>
            {
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
    [Route(ActionRoutes.IDENTITY_ROLES)]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [ProducesResponseType(typeof(IEnumerable<IdentityRole<string>>), (int)HttpStatusCode.OK)]
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
    [Route(ActionRoutes.IDENTITY_ROLES_CREATE)]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [Produces(HttpContentType.JSON)]
    [ProducesResponseType(typeof(IdentityRole<string>), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> CreateRoleAsync([FromBody][Required] CreateRole assignRole, CancellationToken cancellationToken = default)
    {
        var identityRole = await this.identityRepository
            .CreateRoleAsync(assignRole.Name, cancellationToken);

        return this.Created(ActionRoutes.IDENTITY_ROLES_CREATE, identityRole);
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
    [Route(ActionRoutes.IDENTITY_ROLES_DELETE)]
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
    [Route(ActionRoutes.IDENTITY_ROLES_CLAIMS)]
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
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="assignRoleClaim">The role claim assignment request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was assigned to the role.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized. User is not allowed to assign role claims.</response>
    /// <response code="404">Not Found. The role does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPost]
    [Route(ActionRoutes.IDENTITY_ROLES_CLAIMS_ASSIGN)]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignRoleClaimAsync([FromRoute][Required] TIdentity roleId, [FromBody][Required] AssignRoleClaim assignRoleClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .AssignRoleClaimAsync(roleId, assignRoleClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Replaces a claim of a role with a new claim.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="replaceClaim">The role claim replacement request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was replaced for the role.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized. User is not allowed to replace role claims.</response>
    /// <response code="404">Not Found. The role or claim does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPut]
    [Route(ActionRoutes.IDENTITY_ROLES_CLAIMS_REPLACE)]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> ReplaceRoleClaimAsync([FromRoute][Required] TIdentity roleId, [FromBody][Required] ReplaceRoleClaim replaceClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .ReplaceRoleClaimAsync(roleId, replaceClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Assigns or Replaces a claim of a role with a new claim.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="replaceClaim">The role claim assignment or replacement request.</param>
    /// <param name="cancellationToken">The token used to cancel the request.</param>
    /// <returns>Void.</returns>
    /// <response code="200">Success. The claim was assigned or replaced for the role.</response>
    /// <response code="400">Bad Request.</response>
    /// <response code="401">Unauthorized. User is not allowed to replace role claims.</response>
    /// <response code="404">Not Found. The role or claim does not exist.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpPut]
    [Route(ActionRoutes.IDENTITY_ROLES_CLAIMS_ASSIGN_OR_REPLACE)]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> AssignOrReplaceRoleClaimAsync([FromRoute][Required] TIdentity roleId, [FromBody][Required] AssignOrReplaceRoleClaim replaceClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .AssignOrReplaceRoleClaimAsync(roleId, replaceClaim, cancellationToken);

        return this.Ok();
    }

    /// <summary>
    /// Removes a claim from a role.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
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
    [Route(ActionRoutes.IDENTITY_ROLES_CLAIMS_REMOVE)]
    [Authorize(Roles = BuiltInUserRoles.ADMINISTRATOR)]
    [Consumes(HttpContentType.JSON)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public virtual async Task<IActionResult> RemoveRoleClaimAsync([FromRoute][Required] TIdentity roleId, [FromBody][Required] RemoveRoleClaim removeClaim, CancellationToken cancellationToken = default)
    {
        await this.identityRepository
            .RemoveRoleClaimAsync(roleId, removeClaim, cancellationToken);

        return this.Ok();
    }

    #endregion
}