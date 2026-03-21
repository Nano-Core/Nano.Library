using Microsoft.AspNetCore.Http;
using Nano.App.ApiClient.Config;
using Nano.App.ApiClient.Requests.Identity;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Abstractions.Models.Identity;
using PasswordOptions = Nano.Data.Abstractions.Config.PasswordOptions;

namespace Nano.App.ApiClient;

/// <inheritdoc />
public abstract class BaseIdentityApi<TUser> : BaseIdentityApi<TUser, Guid>
    where TUser : class, IEntityUser<Guid>
{
    /// <inheritdoc />
    protected BaseIdentityApi(ApiClientOptions apiClientOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiClientOptions, httpClient, httpContextAccessor)
    {
    }
}

/// <summary>
/// Default Identity Api.
/// </summary>
public abstract class BaseIdentityApi<TUser, TIdentity> : BaseAuthApi<TIdentity>
    where TUser : class, IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Identity Controller.
    /// </summary>
    protected static string IdentityController => $"{typeof(TUser).Name.ToLower()}s";

    /// <inheritdoc />
    protected BaseIdentityApi(ApiClientOptions apiClientOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiClientOptions, httpClient, httpContextAccessor)
    {
    }

    #region Sign Up

    /// <summary>
    /// Get Password Options Async.
    /// </summary>
    /// <param name="request">The <see cref="GetPasswordOptionsRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task<PasswordOptions?> GetPasswordOptionsAsync(GetPasswordOptionsRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GetPasswordOptionsRequest, PasswordOptions>(request, cancellationToken);
    }

    /// <summary>
    /// Is Email Address Taken Async.
    /// </summary>
    /// <param name="request">The <see cref="IsEmailAddressTakenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IsEmailAddressTaken"/>.</returns>
    public virtual async Task<IsEmailAddressTaken> IsEmailAddressTakenAsync(IsEmailAddressTakenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        var isEmailAddressTaken = await this.InvokeAsync<IsEmailAddressTakenRequest, IsEmailAddressTaken>(request, cancellationToken);

        return isEmailAddressTaken ?? throw new NullReferenceException(nameof(isEmailAddressTaken));
    }

    /// <summary>
    /// Is Phone Number Taken Async.
    /// </summary>
    /// <param name="request">The <see cref="IsPhoneNumberTakenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IsPhoneNumberTaken"/>.</returns>
    public virtual async Task<IsPhoneNumberTaken> IsPhoneNumberTakenAsync(IsPhoneNumberTakenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        var isEmailAddressTaken = await this.InvokeAsync<IsPhoneNumberTakenRequest, IsPhoneNumberTaken>(request, cancellationToken);

        return isEmailAddressTaken ?? throw new NullReferenceException(nameof(isEmailAddressTaken));
    }

    /// <summary>
    /// Sign Up Async.
    /// </summary>
    /// <param name="request">The <see cref="SignUpRequest{TUser}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of <typeparamref name="TUser"/>.</returns>
    public virtual async Task<TUser> SignUpAsync(SignUpRequest<TUser, TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await this.InvokeAsync<SignUpRequest<TUser, TIdentity>, TUser>(request, cancellationToken);

        return user ?? throw new NullReferenceException(nameof(user));
    }

    /// <summary>
    /// Sign Up External Callback Async.
    /// </summary>
    /// <typeparam name="TSignUp">The signup type.</typeparam>
    /// <param name="request">The <see cref="BaseSignUpExternalRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of <typeparamref name="TUser"/>.</returns>
    public virtual async Task<TUser> SignUpExternalAsync<TSignUp>(TSignUp request, CancellationToken cancellationToken = default)
        where TSignUp : BaseSignUpExternalRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await this.InvokeAsync<TSignUp, TUser>(request, cancellationToken);

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
    public virtual Task SetUsernameAsync(SetUsernameRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Set Password Async.
    /// </summary>
    /// <param name="request">The <see cref="SetPasswordRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task SetPasswordAsync(SetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Change Password Async.
    /// </summary>
    /// <param name="request">The <see cref="ChangePasswordRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ChangePasswordAsync(ChangePasswordRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Reset Password Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateResetPasswordTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task<ResetPasswordToken<TIdentity>> GetResetPasswordTokenAsync(GenerateResetPasswordTokenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        var resetPasswordToken = await this.InvokeAsync<GenerateResetPasswordTokenRequest, ResetPasswordToken<TIdentity>>(request, cancellationToken);

        return resetPasswordToken ?? throw new NullReferenceException(nameof(resetPasswordToken));
    }

    /// <summary>
    /// Reset Password Async.
    /// </summary>
    /// <param name="request">The <see cref="ResetPasswordRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ResetPasswordAsync(ResetPasswordRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Change Email Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateChangeEmailTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangeEmailToken{TIdentity}"/>.</returns>
    public virtual async Task<ChangeEmailToken<TIdentity>> GetChangeEmailTokenAsync(GenerateChangeEmailTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        var changeEmailToken = await this.InvokeAsync<GenerateChangeEmailTokenRequest<TIdentity>, ChangeEmailToken<TIdentity>>(request, cancellationToken);

        return changeEmailToken ?? throw new NullReferenceException(nameof(changeEmailToken));
    }

    /// <summary>
    /// Change Email Async.
    /// </summary>
    /// <param name="request">The <see cref="ChangeEmailRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ChangeEmailAsync(ChangeEmailRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Confirm Email Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateConfirmEmailTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmEmailToken{TIdentity}"/>.</returns>
    public virtual async Task<ConfirmEmailToken<TIdentity>> GetConfirmEmailTokenAsync(GenerateConfirmEmailTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        var confirmEmailToken = await this.InvokeAsync<GenerateConfirmEmailTokenRequest<TIdentity>, ConfirmEmailToken<TIdentity>>(request, cancellationToken);

        return confirmEmailToken ?? throw new NullReferenceException(nameof(confirmEmailToken));
    }

    /// <summary>
    /// Confirm Email Async.
    /// </summary>
    /// <param name="request">The <see cref="ConfirmEmailRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ConfirmEmailAsync(ConfirmEmailRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Change Phone Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateChangePhoneTokenRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task<ChangePhoneNumberToken<TIdentity>> GetChangePhoneTokenAsync(GenerateChangePhoneTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        var changePhoneNumberToken = await this.InvokeAsync<GenerateChangePhoneTokenRequest<TIdentity>, ChangePhoneNumberToken<TIdentity>>(request, cancellationToken);

        return changePhoneNumberToken ?? throw new NullReferenceException(nameof(changePhoneNumberToken));
    }

    /// <summary>
    /// Change Phone Async.
    /// </summary>
    /// <param name="request">The <see cref="ChangePhoneRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ChangePhoneAsync(ChangePhoneRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Confirm Phone Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateConfirmPhoneTokenRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmPhoneNumberToken{TIdentity}"/>.</returns>
    public virtual async Task<ConfirmPhoneNumberToken<TIdentity>> GetConfirmPhoneTokenAsync(GenerateConfirmPhoneTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        var confirmPhoneNumberToken = await this.InvokeAsync<GenerateConfirmPhoneTokenRequest<TIdentity>, ConfirmPhoneNumberToken<TIdentity>>(request, cancellationToken);

        return confirmPhoneNumberToken ?? throw new NullReferenceException(nameof(confirmPhoneNumberToken));
    }

    /// <summary>
    /// Confirm Phone Async.
    /// </summary>
    /// <param name="request">The <see cref="ConfirmPhoneRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ConfirmPhoneAsync(ConfirmPhoneRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Custom Purpose Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateChangeEmailTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangeEmailToken{TIdentity}"/>.</returns>
    public virtual async Task<ConfirmCustomPurposeToken<TIdentity>> GetCustomPurposeTokenAsync(GenerateCustomPurposeTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        var customPurposeToken = await this.InvokeAsync<GenerateCustomPurposeTokenRequest<TIdentity>, ConfirmCustomPurposeToken<TIdentity>>(request, cancellationToken);

        return customPurposeToken ?? throw new NullReferenceException(nameof(customPurposeToken));
    }

    /// <summary>
    /// Verify Custom Token Async.
    /// </summary>
    /// <param name="request">The <see cref="ConfirmCustomPurposeRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ConfirmCustomPurposeTokenAsync(ConfirmCustomPurposeRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Activate User Async.
    /// </summary>
    /// <param name="request">The <see cref="ActivateUserRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ActivateUserAsync(ActivateUserRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Deactivate User Async.
    /// </summary>
    /// <param name="request">The <see cref="ActivateUserRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task DeactivateUserAsync(DeactivateUserRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region External Logins

    /// <summary>
    /// Get External Logins Async.
    /// </summary>
    /// <param name="request">The <see cref="GetExternalLoginsRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ExternalLogin"/>.</returns>
    public virtual Task<IEnumerable<ExternalLogin>?> GetExternalLoginsAsync(GetExternalLoginsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GetExternalLoginsRequest<TIdentity>, IEnumerable<ExternalLogin>>(request, cancellationToken);
    }

    /// <summary>
    /// Add External Login Async.
    /// </summary>
    /// <param name="request">The <see cref="BaseAddExternalLoginRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ExternalLogin"/>.</returns>
    public virtual Task<ExternalLogin?> AddExternalLoginAsync<TLogin>(TLogin request, CancellationToken cancellationToken = default)
        where TLogin : BaseAddExternalLoginRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<TLogin, ExternalLogin>(request, cancellationToken);
    }

    /// <summary>
    /// Remove External Login Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveExternalLoginRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task RemoveExternalLoginAsync(RemoveExternalLoginRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region Refresh Tokens

    /// <summary>
    /// Get User Refresh Tokens Async.
    /// </summary>
    /// <param name="request">The <see cref="GetRefreshTokensRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The refresh tokens.</returns>
    public virtual async Task<IEnumerable<RefreshToken>> GetUserRefreshTokensAsync(GetRefreshTokensRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetRefreshTokensRequest<TIdentity>, IEnumerable<RefreshToken>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Get User Active Refresh Tokens Async.
    /// </summary>
    /// <param name="request">The <see cref="GetActiveRefreshTokensRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The refresh tokens.</returns>
    public virtual async Task<IEnumerable<RefreshToken>> GetUserActiveRefreshTokensAsync(GetActiveRefreshTokensRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetActiveRefreshTokensRequest<TIdentity>, IEnumerable<RefreshToken>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Delete User Refresh Token Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveExternalLoginRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task DeleteUserRefreshTokenAsync(DeleteUserRefreshTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region Api Keys

    /// <summary>
    /// Get Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="GetApiKeysRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual async Task<IEnumerable<IdentityApiKey<TIdentity>>> GetApiKeysAsync(GetApiKeysRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetApiKeysRequest<TIdentity>, IEnumerable<IdentityApiKey<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Create Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="CreateApiKeyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangeEmailToken{TIdentity}"/>.</returns>
    public virtual async Task<IdentityApiKeyCreated<TIdentity>> CreateApiKeysAsync(CreateApiKeyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        var apiKey = await this.InvokeAsync<CreateApiKeyRequest<TIdentity>, IdentityApiKeyCreated<TIdentity>>(request, cancellationToken);

        return apiKey ?? throw new NullReferenceException(nameof(apiKey));
    }

    /// <summary>
    /// Edit Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="EditApiKeyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual Task<IdentityApiKey<TIdentity>?> EditApiKeysAsync(EditApiKeyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<EditApiKeyRequest<TIdentity>, IdentityApiKey<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Revoke Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="RevokeApiKeyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual Task<IdentityApiKey<TIdentity>?> RevokeApiKeysAsync(RevokeApiKeyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<RevokeApiKeyRequest<TIdentity>, IdentityApiKey<TIdentity>>(request, cancellationToken);
    }

    #endregion


    #region Claims

    /// <summary>
    /// Get User Claims Async.
    /// </summary>
    /// <param name="request">The <see cref="GetClaimsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="Claim"/>.</returns>
    public virtual async Task<IEnumerable<Claim>> GetUserClaimsAsync(GetClaimsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetClaimsRequest<TIdentity>, IEnumerable<Claim>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Assign User Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignUserClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task AssignUserClaimAsync(AssignUserClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove User Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveUserClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task RemoveUserClaimAsync(RemoveUserClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Replace User Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="ReplaceUserClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ReplaceUserClaimAsync(ReplaceUserClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Assign Or Replace User Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignOrReplaceUserClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task AssignOrReplaceUserClaimAsync(AssignOrReplaceUserClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region Roles

    /// <summary>
    /// Get Roles Async.
    /// </summary>
    /// <param name="request">The <see cref="GetRolesRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The roles.</returns>
    public virtual async Task<IEnumerable<IdentityRole<TIdentity>>> GetRolesAsync(GetRolesRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetRolesRequest, IEnumerable<IdentityRole<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Create Role Async.
    /// </summary>
    /// <param name="request">The <see cref="CreateRoleRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The role.</returns>
    public virtual async Task<IdentityRole<TIdentity>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        var identityRole = await this.InvokeAsync<CreateRoleRequest, IdentityRole<TIdentity>>(request, cancellationToken);

        return identityRole ?? throw new NullReferenceException(nameof(identityRole));
    }

    /// <summary>
    /// Delete Role Async.
    /// </summary>
    /// <param name="request">The <see cref="DeleteRoleRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task DeleteRoleAsync(DeleteRoleRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get User Roles Async.
    /// </summary>
    /// <param name="request">The <see cref="GetUserRolesRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The roles.</returns>
    public virtual async Task<IEnumerable<string>> GetUserRolesAsync(GetUserRolesRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetUserRolesRequest<TIdentity>, IEnumerable<string>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Assign Role Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignUserRoleRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task AssignUserRoleAsync(AssignUserRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove Role Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveUserRoleRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task RemoveUserRoleAsync(RemoveUserRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    #endregion


    #region Role Claims

    /// <summary>
    /// Get Role Claims Async.
    /// </summary>
    /// <param name="request">The <see cref="GetRoleClaimsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="Claim"/>.</returns>
    public virtual async Task<IEnumerable<Claim>> GetRoleClaimsAsync(GetRoleClaimsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetRoleClaimsRequest<TIdentity>, IEnumerable<Claim>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Assign Role Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignRoleClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task AssignRoleClaimAsync(AssignRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove Role Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveRoleClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task RemoveRoleClaimAsync(RemoveRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Replace Role Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="ReplaceRoleClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ReplaceRoleClaimAsync(ReplaceRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Assign Or Replace Role Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignOrReplaceRoleClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task AssignOrReplaceRoleClaimAsync(AssignOrReplaceRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    #endregion
}