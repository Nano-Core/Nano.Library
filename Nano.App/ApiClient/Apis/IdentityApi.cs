using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nano.App.ApiClient.Requests.Identity;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Identity;

namespace Nano.App.ApiClient.Apis;

/// <summary>
/// 
/// </summary>
public sealed class IdentityApi<TUser, TIdentity>(ApiClient api)
    where TUser : class, IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Identity Controller.
    /// </summary>
    private static string IdentityController => $"{typeof(TUser).Name.ToLower()}s";

    private readonly ApiClient api = api ?? throw new ArgumentNullException(nameof(api));

    /// <summary>
    /// Executes <c>identity/details/deactivated</c> to retrieve a deactivated entity by identifier.
    /// </summary>
    /// <typeparam name="TEntity">The entity type implementing <see cref="IEntityIdentity{TIdentity}"/>.</typeparam>
    /// <param name="request">The details request for a deactivated entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching deactivated entity, or <c>null</c> if not found.</returns>
    public Task<TEntity?> DetailsDeactivatedAsync<TEntity>(DetailsDeactivatedRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.api
            .InvokeAsync<DetailsDeactivatedRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    #region Sign Up

    /// <summary>
    /// Executes <c>identity/password/options</c> to retrieve password policy configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The password options, or <c>null</c> if not configured.</returns>
    public Task<PasswordOptions?> GetPasswordOptionsAsync(CancellationToken cancellationToken = default)
    {
        var request = new GetPasswordOptionsRequest
        {
            Controller = IdentityController
        };

        return this.api
            .InvokeAsync<GetPasswordOptionsRequest, PasswordOptions>(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/email/taken</c> to determine whether an email address is already registered.
    /// </summary>
    /// <param name="request">The request containing the email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result indicating whether the email address is taken.</returns>
    /// <exception cref="NullReferenceException">Thrown if the response is unexpectedly null.</exception>
    public async Task<IsEmailAddressTaken> IsEmailAddressTakenAsync(IsEmailAddressTakenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var isEmailAddressTaken = await api
            .InvokeAsync<IsEmailAddressTakenRequest, IsEmailAddressTaken>(request, cancellationToken);

        return isEmailAddressTaken ?? throw new NullReferenceException(nameof(isEmailAddressTaken));
    }

    /// <summary>
    /// Executes <c>identity/phone/taken</c> to determine whether a phone number is already registered.
    /// </summary>
    /// <param name="request">The request containing the phone number.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result indicating whether the phone number is taken.</returns>
    /// <exception cref="NullReferenceException">Thrown if the response is unexpectedly null.</exception>
    public async Task<IsPhoneNumberTaken> IsPhoneNumberTakenAsync(IsPhoneNumberTakenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var isEmailAddressTaken = await api
            .InvokeAsync<IsPhoneNumberTakenRequest, IsPhoneNumberTaken>(request, cancellationToken);

        return isEmailAddressTaken ?? throw new NullReferenceException(nameof(isEmailAddressTaken));
    }

    /// <summary>
    /// Executes <c>identity/signup</c> to register a new user.
    /// </summary>
    /// <param name="request">The signup request containing user details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user instance.</returns>
    /// <exception cref="NullReferenceException">Thrown if the response is unexpectedly null.</exception>
    public async Task<TUser> SignUpAsync(SignUpRequest<TUser, TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var user = await api
            .InvokeAsync<SignUpRequest<TUser, TIdentity>, TUser>(request, cancellationToken);

        return user ?? throw new NullReferenceException(nameof(user));
    }

    /// <summary>
    /// Executes <c>identity/signup/external</c> to register a new user via an external provider.
    /// </summary>
    /// <typeparam name="TRequest">The external signup request type.</typeparam>
    /// <typeparam name="TFlow">The authentication flow type.</typeparam>
    /// <param name="request">The external signup request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created user instance.</returns>
    /// <exception cref="NullReferenceException">Thrown if the response is unexpectedly null.</exception>
    public async Task<TUser> SignUpExternalAsync<TRequest, TFlow>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseSignUpExternalRequest<TFlow, TUser, TIdentity>
        where TFlow : BaseAuthFlow
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await api
            .InvokeAsync<TRequest, TUser>(request, cancellationToken);

        return user ?? throw new NullReferenceException(nameof(user));
    }

    #endregion


    #region User

    /// <summary>
    /// Executes <c>identity/username</c> to set or update the username for a user.
    /// </summary>
    /// <param name="request">The username update request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task SetUsernameAsync(SetUsernameRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/password/set</c> to set an initial password for a user.
    /// </summary>
    /// <param name="request">The password setup request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task SetPasswordAsync(SetPasswordRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/password/change</c> to change an existing user password.
    /// </summary>
    /// <param name="request">The password change request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task ChangePasswordAsync(ChangePasswordRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/password/reset/token</c> to generate a password reset token.
    /// </summary>
    /// <param name="request">The reset token generation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generated password reset token.</returns>
    public async Task<ResetPasswordToken> GetResetPasswordTokenAsync(GenerateResetPasswordTokenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var resetPasswordToken = await api
            .InvokeAsync<GenerateResetPasswordTokenRequest, ResetPasswordToken>(request, cancellationToken);

        return resetPasswordToken ?? throw new NullReferenceException(nameof(resetPasswordToken));
    }

    /// <summary>
    /// Executes <c>identity/password/reset</c> to reset a user password using a valid token.
    /// </summary>
    /// <param name="request">The password reset request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task ResetPasswordAsync(ResetPasswordRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/email/change/token</c> to generate a change email token.
    /// </summary>
    /// <param name="request">The change email token request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generated change email token.</returns>
    public async Task<ChangeEmailToken> GetChangeEmailTokenAsync(GenerateChangeEmailTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var changeEmailToken = await api
            .InvokeAsync<GenerateChangeEmailTokenRequest<TIdentity>, ChangeEmailToken>(request, cancellationToken);

        return changeEmailToken ?? throw new NullReferenceException(nameof(changeEmailToken));
    }

    /// <summary>
    /// Executes <c>identity/email/change</c> to update the user's email address.
    /// </summary>
    /// <param name="request">The email change request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task ChangeEmailAsync(ChangeEmailRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/email/confirm/token</c> to generate an email confirmation token.
    /// </summary>
    /// <param name="request">The email confirmation token request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generated confirmation token.</returns>
    public async Task<ConfirmEmailToken> GetConfirmEmailTokenAsync(GenerateConfirmEmailTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var confirmEmailToken = await api
            .InvokeAsync<GenerateConfirmEmailTokenRequest<TIdentity>, ConfirmEmailToken>(request, cancellationToken);

        return confirmEmailToken ?? throw new NullReferenceException(nameof(confirmEmailToken));
    }

    /// <summary>
    /// Executes <c>identity/email/confirm</c> to confirm a user's email address.
    /// </summary>
    /// <param name="request">The email confirmation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task ConfirmEmailAsync(ConfirmEmailRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/phone/change/token</c> to generate a phone number change token.
    /// </summary>
    /// <param name="request">The change phone token request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generated phone change token.</returns>
    public async Task<ChangePhoneNumberToken> GetChangePhoneTokenAsync(GenerateChangePhoneTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var changePhoneNumberToken = await api
            .InvokeAsync<GenerateChangePhoneTokenRequest<TIdentity>, ChangePhoneNumberToken>(request, cancellationToken);

        return changePhoneNumberToken ?? throw new NullReferenceException(nameof(changePhoneNumberToken));
    }

    /// <summary>
    /// Executes <c>identity/phone/change</c> to update the user's phone number.
    /// </summary>
    /// <param name="request">The phone change request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task ChangePhoneAsync(ChangePhoneRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/phone/confirm/token</c> to generate a phone confirmation token.
    /// </summary>
    /// <param name="request">The phone confirmation token request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generated confirmation token.</returns>
    public async Task<ConfirmPhoneNumberToken> GetConfirmPhoneTokenAsync(GenerateConfirmPhoneTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var confirmPhoneNumberToken = await api
            .InvokeAsync<GenerateConfirmPhoneTokenRequest<TIdentity>, ConfirmPhoneNumberToken>(request, cancellationToken);

        return confirmPhoneNumberToken ?? throw new NullReferenceException(nameof(confirmPhoneNumberToken));
    }

    /// <summary>
    /// Executes <c>identity/phone/confirm</c> to confirm a user's phone number.
    /// </summary>
    /// <param name="request">The phone confirmation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task ConfirmPhoneAsync(ConfirmPhoneRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/custom/token</c> to generate a custom purpose token.
    /// </summary>
    /// <param name="request">The custom token generation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generated custom purpose token.</returns>
    public async Task<ConfirmCustomPurposeToken> GetCustomPurposeTokenAsync(GenerateCustomPurposeTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var customPurposeToken = await api
            .InvokeAsync<GenerateCustomPurposeTokenRequest<TIdentity>, ConfirmCustomPurposeToken>(request, cancellationToken);

        return customPurposeToken ?? throw new NullReferenceException(nameof(customPurposeToken));
    }

    /// <summary>
    /// Executes <c>identity/custom/confirm</c> to validate a custom purpose token.
    /// </summary>
    /// <param name="request">The custom token confirmation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task ConfirmCustomPurposeTokenAsync(ConfirmCustomPurposeRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/user/activate</c> to activate a user account.
    /// </summary>
    /// <param name="request">The activation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task ActivateUserAsync(ActivateUserRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/user/deactivate</c> to deactivate a user account.
    /// </summary>
    /// <param name="request">The deactivation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task DeactivateUserAsync(DeactivateUserRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region User Roles

    /// <summary>
    /// Executes <c>identity/user/roles</c> to retrieve all roles assigned to a user.
    /// </summary>
    /// <param name="request">The user roles request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of role names assigned to the user.</returns>
    public async Task<IEnumerable<string>> GetUserRolesAsync(GetUserRolesRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await api
            .InvokeAsync<GetUserRolesRequest<TIdentity>, IEnumerable<string>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>identity/user/roles/assign</c> to assign a role to a user.
    /// </summary>
    /// <param name="request">The role assignment request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task AssignUserRoleAsync(AssignUserRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/user/roles/remove</c> to remove a role from a user.
    /// </summary>
    /// <param name="request">The role removal request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task RemoveUserRoleAsync(RemoveUserRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region User Claims

    /// <summary>
    /// Executes <c>identity/user/claims</c> to retrieve all claims assigned to a user.
    /// </summary>
    /// <param name="request">The user claims request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of user claims.</returns>
    public async Task<IEnumerable<Claim>> GetUserClaimsAsync(GetUserClaimsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await api
            .InvokeAsync<GetUserClaimsRequest<TIdentity>, IEnumerable<Claim>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>identity/user/claims/assign</c> to assign a claim to a user.
    /// </summary>
    /// <param name="request">The claim assignment request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task AssignUserClaimAsync(AssignUserClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/user/claims/replace</c> to replace an existing user claim.
    /// </summary>
    /// <param name="request">The claim replacement request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task ReplaceUserClaimAsync(ReplaceUserClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/user/claims/assign-or-replace</c> to assign or replace a user claim.
    /// </summary>
    /// <param name="request">The claim upsert request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task AssignOrReplaceUserClaimAsync(AssignOrReplaceUserClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/user/claims/remove</c> to remove a claim from a user.
    /// </summary>
    /// <param name="request">The claim removal request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task RemoveUserClaimAsync(RemoveUserClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region External Logins

    /// <summary>
    /// Executes <c>identity/external-logins</c> to retrieve all linked external login providers for a user.
    /// </summary>
    /// <param name="request">The external logins request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of external logins, or <c>null</c> if none exist.</returns>
    public Task<IEnumerable<ExternalLogin>?> GetExternalLoginsAsync(GetExternalLoginsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync<GetExternalLoginsRequest<TIdentity>, IEnumerable<ExternalLogin>>(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/external-logins/add</c> to link an external login provider to a user account.
    /// </summary>
    /// <typeparam name="TRequest">The external login request type.</typeparam>
    /// <typeparam name="TFlow">The authentication flow type.</typeparam>
    /// <param name="request">The external login add request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created external login entry, or <c>null</c> if not created.</returns>
    /// <exception cref="NotFoundException">Thrown if the external login could not be created.</exception>
    public Task<ExternalLogin?> AddExternalLoginAsync<TRequest, TFlow>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseAddExternalLoginRequest<TFlow, TIdentity>
        where TFlow : BaseAuthFlow
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var externalLogin = api
            .InvokeAsync<TRequest, ExternalLogin>(request, cancellationToken);

        return externalLogin ?? throw new NotFoundException(nameof(externalLogin));
    }

    /// <summary>
    /// Executes <c>identity/external-logins/remove</c> to unlink an external login provider from a user account.
    /// </summary>
    /// <typeparam name="TRequest">The external login removal request type.</typeparam>
    /// <param name="request">The external login removal request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task RemoveExternalLoginAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : BaseRemoveExternalLoginRequest<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region Refresh Tokens

    /// <summary>
    /// Executes <c>identity/refresh-tokens</c> to retrieve all refresh tokens for a user.
    /// </summary>
    /// <param name="request">The refresh token request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of refresh tokens.</returns>
    public async Task<IEnumerable<RefreshToken>> GetRefreshTokensAsync(GetRefreshTokensRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await this.api
            .InvokeAsync<GetRefreshTokensRequest<TIdentity>, IEnumerable<RefreshToken>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>identity/refresh-tokens/active</c> to retrieve active refresh tokens for a user.
    /// </summary>
    /// <param name="request">The active refresh token request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of active refresh tokens.</returns>
    public async Task<IEnumerable<RefreshToken>> GetActiveRefreshTokensAsync(GetActiveRefreshTokensRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await this.api
            .InvokeAsync<GetActiveRefreshTokensRequest<TIdentity>, IEnumerable<RefreshToken>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>identity/refresh-tokens/delete</c> to revoke a specific refresh token.
    /// </summary>
    /// <param name="request">The delete refresh token request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task DeleteRefreshTokenAsync(DeleteRefreshTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region Api Keys

    /// <summary>
    /// Executes <c>identity/api-keys</c> to retrieve all API keys for a user.
    /// </summary>
    /// <param name="request">The API keys request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of API keys.</returns>
    public async Task<IEnumerable<IdentityApiKey<TIdentity>>> GetApiKeysAsync(GetApiKeysRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await this.api
            .InvokeAsync<GetApiKeysRequest<TIdentity>, IEnumerable<IdentityApiKey<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>identity/api-keys/create</c> to create a new API key.
    /// </summary>
    /// <param name="request">The API key creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created API key.</returns>
    /// <exception cref="NullReferenceException">Thrown if the API key creation response is null.</exception>
    public async Task<IdentityApiKeyCreated<TIdentity>> CreateApiKeysAsync(CreateApiKeyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var apiKey = await this.api
            .InvokeAsync<CreateApiKeyRequest<TIdentity>, IdentityApiKeyCreated<TIdentity>>(request, cancellationToken);

        return apiKey ?? throw new NullReferenceException(nameof(apiKey));
    }

    /// <summary>
    /// Executes <c>identity/api-keys/edit</c> to update an existing API key.
    /// </summary>
    /// <param name="request">The API key edit request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated API key, or <c>null</c> if not found.</returns>
    public Task<IdentityApiKey<TIdentity>?> EditApiKeysAsync(EditApiKeyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync<EditApiKeyRequest<TIdentity>, IdentityApiKey<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/api-keys/revoke</c> to revoke an API key.
    /// </summary>
    /// <param name="request">The API key revocation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The revoked API key, or <c>null</c> if not found.</returns>
    public Task<IdentityApiKey<TIdentity>?> RevokeApiKeysAsync(RevokeApiKeyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync<RevokeApiKeyRequest<TIdentity>, IdentityApiKey<TIdentity>>(request, cancellationToken);
    }

    #endregion


    #region Api Keys Roles

    /// <summary>
    /// Executes <c>identity/api-keys/roles</c> to retrieve roles assigned to an API key.
    /// </summary>
    /// <param name="request">The API key roles request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of role names.</returns>
    public async Task<IEnumerable<string>> GetApiKeyRolesAsync(GetApiKeyRolesRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await this.api
            .InvokeAsync<GetApiKeyRolesRequest<TIdentity>, IEnumerable<string>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>identity/api-keys/roles/assign</c> to assign a role to an API key.
    /// </summary>
    /// <param name="request">The role assignment request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task AssignApiKeyRoleAsync(AssignApiKeyRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/api-keys/roles/remove</c> to remove a role from an API key.
    /// </summary>
    /// <param name="request">The role removal request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task RemoveApiKeyRoleAsync(RemoveApiKeyRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region Api Keys Claims

    /// <summary>
    /// Executes <c>identity/api-keys/claims</c> to retrieve claims assigned to an API key.
    /// </summary>
    /// <param name="request">The API key claims request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of claims.</returns>
    public async Task<IEnumerable<Claim>> GetApiKeyClaimsAsync(GetApiKeyClaimsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await this.api
            .InvokeAsync<GetApiKeyClaimsRequest<TIdentity>, IEnumerable<Claim>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>identity/api-keys/claims/assign</c> to assign a claim to an API key.
    /// </summary>
    /// <param name="request">The claim assignment request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task AssignApiKeyClaimAsync(AssignApiKeyClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/api-keys/claims/replace</c> to replace an existing claim on an API key.
    /// </summary>
    /// <param name="request">The claim replacement request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task ReplaceApiKeyClaimAsync(ReplaceApiKeyClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/api-keys/claims/assign-or-replace</c> to upsert a claim on an API key.
    /// </summary>
    /// <param name="request">The claim upsert request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task AssignOrReplaceApiKeyClaimAsync(AssignOrReplaceApiKeyClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/api-keys/claims/remove</c> to remove a claim from an API key.
    /// </summary>
    /// <param name="request">The claim removal request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task RemoveApiKeyClaimAsync(RemoveApiKeyClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region Roles

    /// <summary>
    /// Executes <c>identity/roles</c> to retrieve all roles.
    /// </summary>
    /// <param name="request">The roles request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of identity roles.</returns>
    public async Task<IEnumerable<IdentityRole<TIdentity>>> GetRolesAsync(GetRolesRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await api
            .InvokeAsync<GetRolesRequest, IEnumerable<IdentityRole<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>identity/roles/create</c> to create a new role.
    /// </summary>
    /// <param name="request">The role creation request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created role.</returns>
    /// <exception cref="NullReferenceException">Thrown if the created role response is null.</exception>
    public async Task<IdentityRole<TIdentity>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var identityRole = await api
            .InvokeAsync<CreateRoleRequest, IdentityRole<TIdentity>>(request, cancellationToken);

        return identityRole ?? throw new NullReferenceException(nameof(identityRole));
    }

    /// <summary>
    /// Executes <c>identity/roles/delete</c> to delete an existing role.
    /// </summary>
    /// <param name="request">The role deletion request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task DeleteRoleAsync(DeleteRoleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region Role Claims

    /// <summary>
    /// Executes <c>identity/roles/claims</c> to retrieve claims assigned to a role.
    /// </summary>
    /// <param name="request">The role claims request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of claims.</returns>
    public async Task<IEnumerable<Claim>> GetRoleClaimsAsync(GetRoleClaimsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await api
            .InvokeAsync<GetRoleClaimsRequest<TIdentity>, IEnumerable<Claim>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>identity/roles/claims/assign</c> to assign a claim to a role.
    /// </summary>
    /// <param name="request">The claim assignment request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task AssignRoleClaimAsync(AssignRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/roles/claims/replace</c> to replace an existing claim on a role.
    /// </summary>
    /// <param name="request">The claim replacement request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task ReplaceRoleClaimAsync(ReplaceRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/roles/claims/assign-or-replace</c> to upsert a claim on a role.
    /// </summary>
    /// <param name="request">The claim upsert request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task AssignOrReplaceRoleClaimAsync(AssignOrReplaceRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>identity/roles/claims/remove</c> to remove a claim from a role.
    /// </summary>
    /// <param name="request">The claim removal request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task RemoveRoleClaimAsync(RemoveRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    #endregion
}
