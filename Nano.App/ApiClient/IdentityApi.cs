using Microsoft.AspNetCore.Identity;
using Nano.App.ApiClient.Requests;
using Nano.App.ApiClient.Requests.Identity;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.ApiClient;

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
    /// Invokes the 'details/deactivated' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DetailsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public Task<TEntity?> DetailsAsync<TEntity>(DetailsDeactivatedRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.api
            .InvokeAsync<DetailsDeactivatedRequest<TIdentity>, TEntity>(request, cancellationToken);
    }


    #region Sign Up

    /// <summary>
    /// Get Password Options Async.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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
    /// Is Email Address Taken Async.
    /// </summary>
    /// <param name="request">The <see cref="IsEmailAddressTakenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IsEmailAddressTaken"/>.</returns>
    public async Task<IsEmailAddressTaken> IsEmailAddressTakenAsync(IsEmailAddressTakenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var isEmailAddressTaken = await api
            .InvokeAsync<IsEmailAddressTakenRequest, IsEmailAddressTaken>(request, cancellationToken);

        return isEmailAddressTaken ?? throw new NullReferenceException(nameof(isEmailAddressTaken));
    }

    /// <summary>
    /// Is Phone Number Taken Async.
    /// </summary>
    /// <param name="request">The <see cref="IsPhoneNumberTakenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IsPhoneNumberTaken"/>.</returns>
    public async Task<IsPhoneNumberTaken> IsPhoneNumberTakenAsync(IsPhoneNumberTakenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var isEmailAddressTaken = await api
            .InvokeAsync<IsPhoneNumberTakenRequest, IsPhoneNumberTaken>(request, cancellationToken);

        return isEmailAddressTaken ?? throw new NullReferenceException(nameof(isEmailAddressTaken));
    }

    /// <summary>
    /// Sign Up Async.
    /// </summary>
    /// <param name="request">The <see cref="SignUpRequest{TUser}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of the user.</returns>
    public async Task<TUser> SignUpAsync(SignUpRequest<TUser, TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var user = await api
            .InvokeAsync<SignUpRequest<TUser, TIdentity>, TUser>(request, cancellationToken);

        return user ?? throw new NullReferenceException(nameof(user));
    }

    /// <summary>
    /// Sign Up External Async.
    /// </summary>
    /// <typeparam name="TRequest">The signup request type.</typeparam>
    /// <typeparam name="TFlow">The flow type of the signup request type.</typeparam>
    /// <param name="request">The <see cref="BaseSignUpExternalRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of the user.</returns>
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
    /// Set Username Async.
    /// </summary>
    /// <param name="request">The <see cref="SetUsernameRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task SetUsernameAsync(SetUsernameRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Set Password Async.
    /// </summary>
    /// <param name="request">The <see cref="SetPasswordRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task SetPasswordAsync(SetPasswordRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Change Password Async.
    /// </summary>
    /// <param name="request">The <see cref="ChangePasswordRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task ChangePasswordAsync(ChangePasswordRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Reset Password Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateResetPasswordTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public async Task<ResetPasswordToken> GetResetPasswordTokenAsync(GenerateResetPasswordTokenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var resetPasswordToken = await api
            .InvokeAsync<GenerateResetPasswordTokenRequest, ResetPasswordToken>(request, cancellationToken);

        return resetPasswordToken ?? throw new NullReferenceException(nameof(resetPasswordToken));
    }

    /// <summary>
    /// Reset Password Async.
    /// </summary>
    /// <param name="request">The <see cref="ResetPasswordRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task ResetPasswordAsync(ResetPasswordRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Change Email Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateChangeEmailTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangeEmailToken"/>.</returns>
    public async Task<ChangeEmailToken> GetChangeEmailTokenAsync(GenerateChangeEmailTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var changeEmailToken = await api
            .InvokeAsync<GenerateChangeEmailTokenRequest<TIdentity>, ChangeEmailToken>(request, cancellationToken);

        return changeEmailToken ?? throw new NullReferenceException(nameof(changeEmailToken));
    }

    /// <summary>
    /// Change Email Async.
    /// </summary>
    /// <param name="request">The <see cref="ChangeEmailRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task ChangeEmailAsync(ChangeEmailRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Confirm Email Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateConfirmEmailTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmEmailToken"/>.</returns>
    public async Task<ConfirmEmailToken> GetConfirmEmailTokenAsync(GenerateConfirmEmailTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var confirmEmailToken = await api
            .InvokeAsync<GenerateConfirmEmailTokenRequest<TIdentity>, ConfirmEmailToken>(request, cancellationToken);

        return confirmEmailToken ?? throw new NullReferenceException(nameof(confirmEmailToken));
    }

    /// <summary>
    /// Confirm Email Async.
    /// </summary>
    /// <param name="request">The <see cref="ConfirmEmailRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task ConfirmEmailAsync(ConfirmEmailRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Change Phone Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateChangePhoneTokenRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public async Task<ChangePhoneNumberToken> GetChangePhoneTokenAsync(GenerateChangePhoneTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var changePhoneNumberToken = await api
            .InvokeAsync<GenerateChangePhoneTokenRequest<TIdentity>, ChangePhoneNumberToken>(request, cancellationToken);

        return changePhoneNumberToken ?? throw new NullReferenceException(nameof(changePhoneNumberToken));
    }

    /// <summary>
    /// Change Phone Async.
    /// </summary>
    /// <param name="request">The <see cref="ChangePhoneRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task ChangePhoneAsync(ChangePhoneRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Confirm Phone Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateConfirmPhoneTokenRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmPhoneNumberToken"/>.</returns>
    public async Task<ConfirmPhoneNumberToken> GetConfirmPhoneTokenAsync(GenerateConfirmPhoneTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var confirmPhoneNumberToken = await api
            .InvokeAsync<GenerateConfirmPhoneTokenRequest<TIdentity>, ConfirmPhoneNumberToken>(request, cancellationToken);

        return confirmPhoneNumberToken ?? throw new NullReferenceException(nameof(confirmPhoneNumberToken));
    }

    /// <summary>
    /// Confirm Phone Async.
    /// </summary>
    /// <param name="request">The <see cref="ConfirmPhoneRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task ConfirmPhoneAsync(ConfirmPhoneRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Custom Purpose Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateChangeEmailTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmCustomPurposeToken"/>.</returns>
    public async Task<ConfirmCustomPurposeToken> GetCustomPurposeTokenAsync(GenerateCustomPurposeTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var customPurposeToken = await api
            .InvokeAsync<GenerateCustomPurposeTokenRequest<TIdentity>, ConfirmCustomPurposeToken>(request, cancellationToken);

        return customPurposeToken ?? throw new NullReferenceException(nameof(customPurposeToken));
    }

    /// <summary>
    /// Verify Custom Token Async.
    /// </summary>
    /// <param name="request">The <see cref="ConfirmCustomPurposeRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task ConfirmCustomPurposeTokenAsync(ConfirmCustomPurposeRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Activate User Async.
    /// </summary>
    /// <param name="request">The <see cref="ActivateUserRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task ActivateUserAsync(ActivateUserRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Deactivate User Async.
    /// </summary>
    /// <param name="request">The <see cref="ActivateUserRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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
    /// Get User Roles Async.
    /// </summary>
    /// <param name="request">The <see cref="GetUserRolesRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The roles.</returns>
    public async Task<IEnumerable<string>> GetUserRolesAsync(GetUserRolesRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await api
            .InvokeAsync<GetUserRolesRequest<TIdentity>, IEnumerable<string>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Assign Role Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignUserRoleRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task AssignUserRoleAsync(AssignUserRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove Role Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveUserRoleRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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
    /// Get User Claims Async.
    /// </summary>
    /// <param name="request">The <see cref="GetUserClaimsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="Claim"/>.</returns>
    public async Task<IEnumerable<Claim>> GetUserClaimsAsync(GetUserClaimsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await api
            .InvokeAsync<GetUserClaimsRequest<TIdentity>, IEnumerable<Claim>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Assign User Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignUserClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task AssignUserClaimAsync(AssignUserClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Replace User Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="ReplaceUserClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task ReplaceUserClaimAsync(ReplaceUserClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Assign Or Replace User Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignOrReplaceUserClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task AssignOrReplaceUserClaimAsync(AssignOrReplaceUserClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove User Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveUserClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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
    /// Get External Logins Async.
    /// </summary>
    /// <param name="request">The <see cref="GetExternalLoginsRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ExternalLogin"/>.</returns>
    public Task<IEnumerable<ExternalLogin>?> GetExternalLoginsAsync(GetExternalLoginsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync<GetExternalLoginsRequest<TIdentity>, IEnumerable<ExternalLogin>>(request, cancellationToken);
    }

    /// <summary>
    /// Add External Login Async.
    /// </summary>
    /// <typeparam name="TRequest">The type of external request.</typeparam>
    /// <typeparam name="TFlow">The flow type of the add external login request.</typeparam>
    /// <param name="request">The <see cref="BaseAddExternalLoginRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ExternalLogin"/>.</returns>
    /// <exception cref="NotFoundException"></exception>
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
    /// Remove External Login Async.
    /// </summary>
    /// <typeparam name="TRequest">The type of external request.</typeparam>
    /// <param name="request">The <see cref="BaseRemoveExternalLoginRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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
    /// Get User Refresh Tokens Async.
    /// </summary>
    /// <param name="request">The <see cref="GetRefreshTokensRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The refresh tokens.</returns>
    public async Task<IEnumerable<RefreshToken>> GetRefreshTokensAsync(GetRefreshTokensRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await this.api
            .InvokeAsync<GetRefreshTokensRequest<TIdentity>, IEnumerable<RefreshToken>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Get User Active Refresh Tokens Async.
    /// </summary>
    /// <param name="request">The <see cref="GetActiveRefreshTokensRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The refresh tokens.</returns>
    public async Task<IEnumerable<RefreshToken>> GetActiveRefreshTokensAsync(GetActiveRefreshTokensRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await this.api
            .InvokeAsync<GetActiveRefreshTokensRequest<TIdentity>, IEnumerable<RefreshToken>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Delete User Refresh Token Async.
    /// </summary>
    /// <param name="request">The <see cref="DeleteRefreshTokenRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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
    /// Get Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="GetApiKeysRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public async Task<IEnumerable<IdentityApiKey<TIdentity>>> GetApiKeysAsync(GetApiKeysRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await this.api
            .InvokeAsync<GetApiKeysRequest<TIdentity>, IEnumerable<IdentityApiKey<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Create Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="CreateApiKeyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKeyCreated{TIdentity}"/>.</returns>
    public async Task<IdentityApiKeyCreated<TIdentity>> CreateApiKeysAsync(CreateApiKeyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var apiKey = await this.api
            .InvokeAsync<CreateApiKeyRequest<TIdentity>, IdentityApiKeyCreated<TIdentity>>(request, cancellationToken);

        return apiKey ?? throw new NullReferenceException(nameof(apiKey));
    }

    /// <summary>
    /// Edit Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="EditApiKeyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public Task<IdentityApiKey<TIdentity>?> EditApiKeysAsync(EditApiKeyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync<EditApiKeyRequest<TIdentity>, IdentityApiKey<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Revoke Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="RevokeApiKeyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
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
    /// Get Api-Key Roles Async.
    /// </summary>
    /// <param name="request">The <see cref="GetUserRolesRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The roles.</returns>
    public async Task<IEnumerable<string>> GetApiKeyRolesAsync(GetApiKeyRolesRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await this.api
            .InvokeAsync<GetApiKeyRolesRequest<TIdentity>, IEnumerable<string>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Assign Api-Key Role Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignApiKeyRoleRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task AssignApiKeyRoleAsync(AssignApiKeyRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove Api-Key Role Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveApiKeyRoleRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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
    /// Get Api-Key Claims Async.
    /// </summary>
    /// <param name="request">The <see cref="GetApiKeyClaimsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="Claim"/>.</returns>
    public async Task<IEnumerable<Claim>> GetApiKeyClaimsAsync(GetApiKeyClaimsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await this.api
            .InvokeAsync<GetApiKeyClaimsRequest<TIdentity>, IEnumerable<Claim>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Assign Api-Key Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignApiKeyClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task AssignApiKeyClaimAsync(AssignApiKeyClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Replace Api-Key Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="ReplaceApiKeyClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task ReplaceApiKeyClaimAsync(ReplaceApiKeyClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Assign Or Replace Api-Key Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignOrReplaceApiKeyClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task AssignOrReplaceApiKeyClaimAsync(AssignOrReplaceApiKeyClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove Api-Key Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveApiKeyClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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
    /// Get Roles Async.
    /// </summary>
    /// <param name="request">The <see cref="GetRolesRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The roles.</returns>
    public async Task<IEnumerable<IdentityRole<TIdentity>>> GetRolesAsync(GetRolesRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await api
            .InvokeAsync<GetRolesRequest, IEnumerable<IdentityRole<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Create Role Async.
    /// </summary>
    /// <param name="request">The <see cref="CreateRoleRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The role.</returns>
    public async Task<IdentityRole<TIdentity>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        var identityRole = await api
            .InvokeAsync<CreateRoleRequest, IdentityRole<TIdentity>>(request, cancellationToken);

        return identityRole ?? throw new NullReferenceException(nameof(identityRole));
    }

    /// <summary>
    /// Delete Role Async.
    /// </summary>
    /// <param name="request">The <see cref="DeleteRoleRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
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
    /// Get Role Claims Async.
    /// </summary>
    /// <param name="request">The <see cref="GetRoleClaimsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="Claim"/>.</returns>
    public async Task<IEnumerable<Claim>> GetRoleClaimsAsync(GetRoleClaimsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return await api
            .InvokeAsync<GetRoleClaimsRequest<TIdentity>, IEnumerable<Claim>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Assign Role Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignRoleClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task AssignRoleClaimAsync(AssignRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Replace Role Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="ReplaceRoleClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task ReplaceRoleClaimAsync(ReplaceRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Assign Or Replace Role Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignOrReplaceRoleClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task AssignOrReplaceRoleClaimAsync(AssignOrReplaceRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove Role Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveRoleClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public Task RemoveRoleClaimAsync(RemoveRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = IdentityController;

        return this.api
            .InvokeAsync(request, cancellationToken);
    }

    #endregion
}
