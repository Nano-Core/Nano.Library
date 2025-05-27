using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Api.Requests.Identity;
using Nano.Models;
using Nano.Models.Data;
using Nano.Models.Interfaces;
using Nano.Security;
using Nano.Security.Models;

namespace Nano.App.Api;

/// <summary>
/// Default Identity Api.
/// </summary>
public abstract class BaseIdentityApi<TUser, TIdentity> : BaseApi<TIdentity>
    where TUser : class, IEntityUser<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Identity Controller.
    /// </summary>
    protected static string IdentityController => $"{typeof(TUser).Name.ToLower()}s";

    /// <inheritdoc />
    protected BaseIdentityApi(ApiOptions apiOptions)
        : base(apiOptions)
    {
    }

    /// <summary>
    /// Sign Up Async.
    /// </summary>
    /// <param name="request">The <see cref="SignUpRequest{TUser}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of <typeparamref name="TUser"/>.</returns>
    public virtual Task<TUser> SignUpAsync(SignUpRequest<TUser, TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<SignUpRequest<TUser, TIdentity>, TUser>(request, cancellationToken);
    }

    /// <summary>
    /// Sign Up External Callback Async.
    /// </summary>
    /// <typeparam name="TSignUp">The signup type.</typeparam>
    /// <param name="request">The <see cref="BaseSignUpExternalRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The instance of <typeparamref name="TUser"/>.</returns>
    public virtual Task<TUser> SignUpExternalAsync<TSignUp>(TSignUp request, CancellationToken cancellationToken = default)
        where TSignUp : BaseSignUpExternalRequest
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return this.InvokeAsync<TSignUp, TUser>(request, cancellationToken);
    }

    /// <summary>
    /// Set Username Async.
    /// </summary>
    /// <param name="request">The <see cref="SetUsernameRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task SetUsernameAsync(SetUsernameRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

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
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Reset Password Async.
    /// </summary>
    /// <param name="request">The <see cref="ResetPasswordRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ResetPasswordAsync(ResetPasswordRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

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
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Is Email Address Taken Async.
    /// </summary>
    /// <param name="request">The <see cref="IsEmailAddressTakenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IsEmailAddressTaken"/>.</returns>
    public virtual Task<IsEmailAddressTaken> IsEmailAddressTakenAsync(IsEmailAddressTakenRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<IsEmailAddressTakenRequest, IsEmailAddressTaken>(request, cancellationToken);
    }

    /// <summary>
    /// Is Phone Number Taken Async.
    /// </summary>
    /// <param name="request">The <see cref="IsPhoneNumberTakenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IsPhoneNumberTaken"/>.</returns>
    public virtual Task<IsPhoneNumberTaken> IsPhoneNumberTakenAsync(IsPhoneNumberTakenRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<IsPhoneNumberTakenRequest, IsPhoneNumberTaken>(request, cancellationToken);
    }

    /// <summary>
    /// Change Email Async.
    /// </summary>
    /// <param name="request">The <see cref="ChangeEmailRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ChangeEmailAsync(ChangeEmailRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Confirm Email Async.
    /// </summary>
    /// <param name="request">The <see cref="ConfirmEmailRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ConfirmEmailAsync(ConfirmEmailRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Change Email Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateChangeEmailTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangeEmailToken{TIdentity}"/>.</returns>
    public virtual Task<ChangeEmailToken<TIdentity>> GetChangeEmailTokenAsync(GenerateChangeEmailTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GenerateChangeEmailTokenRequest<TIdentity>, ChangeEmailToken<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Get Confirm Email Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateConfirmEmailTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmEmailToken{TIdentity}"/>.</returns>
    public virtual Task<ConfirmEmailToken<TIdentity>> GetConfirmEmailTokenAsync(GenerateConfirmEmailTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GenerateConfirmEmailTokenRequest<TIdentity>, ConfirmEmailToken<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Change Phone Async.
    /// </summary>
    /// <param name="request">The <see cref="ChangePhoneRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ChangePhoneAsync(ChangePhoneRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Confirm Phone Async.
    /// </summary>
    /// <param name="request">The <see cref="ConfirmPhoneRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ConfirmPhoneAsync(ConfirmPhoneRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Change Phone Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateChangePhoneTokenRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task<ChangePhoneNumberToken<TIdentity>> GetChangePhoneTokenAsync(GenerateChangePhoneTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GenerateChangePhoneTokenRequest<TIdentity>, ChangePhoneNumberToken<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Get Confirm Phone Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateConfirmPhoneTokenRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmPhoneNumberToken{TIdentity}"/>.</returns>
    public virtual Task<ConfirmPhoneNumberToken<TIdentity>> GetConfirmPhoneTokenAsync(GenerateConfirmPhoneTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GenerateConfirmPhoneTokenRequest<TIdentity>, ConfirmPhoneNumberToken<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Get Reset Password Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateResetPasswordTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task<ResetPasswordToken<TIdentity>> GetResetPasswordTokenAsync(GenerateResetPasswordTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GenerateResetPasswordTokenRequest, ResetPasswordToken<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Get Password Options Async.
    /// </summary>
    /// <param name="request">The <see cref="GetPasswordOptionsRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task<SecurityOptions.PasswordOptions> GetPasswordOptionsAsync(GetPasswordOptionsRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GetPasswordOptionsRequest, SecurityOptions.PasswordOptions>(request, cancellationToken);
    }

    /// <summary>
    /// Get Custom Purpose Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GenerateChangeEmailTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangeEmailToken{TIdentity}"/>.</returns>
    public virtual Task<CustomPurposeToken<TIdentity>> GetCustomPurposeTokenAsync(GenerateCustomPurposeTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GenerateCustomPurposeTokenRequest<TIdentity>, CustomPurposeToken<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Verify Custom Token Async.
    /// </summary>
    /// <param name="request">The <see cref="VerifyCustomTokenRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task VerifyCustomTokenAsync(VerifyCustomTokenRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get External Logins Async.
    /// </summary>
    /// <param name="request">The <see cref="GetExternalLoginsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ExternalLogin"/>.</returns>
    public virtual Task<IEnumerable<ExternalLogin>> GetExternalLoginsAsync(GetExternalLoginsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GetExternalLoginsRequest<TIdentity>, IEnumerable<ExternalLogin>>(request, cancellationToken);
    }

    /// <summary>
    /// Add External Login Async.
    /// </summary>
    /// <param name="request">The <see cref="BaseAddExternalLoginRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ExternalLogin"/>.</returns>
    public virtual Task<ExternalLogin> AddExternalLoginAsync<TLogin>(TLogin request, CancellationToken cancellationToken = default)
        where TLogin : BaseAddExternalLoginRequest
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

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
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="GetApiKeysRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual Task<IEnumerable<IdentityApiKey<TIdentity>>> GetApiKeysAsync(GetApiKeysRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GetApiKeysRequest<TIdentity>, IEnumerable<IdentityApiKey<TIdentity>>>(request, cancellationToken);
    }

    /// <summary>
    /// Create Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="CreateApiKeyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangeEmailToken{TIdentity}"/>.</returns>
    public virtual Task<IdentityApiKeyCreated<TIdentity>> CreateApiKeysAsync(CreateApiKeyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<CreateApiKeyRequest<TIdentity>, IdentityApiKeyCreated<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Edit Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="EditApiKeyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual Task<IdentityApiKey<TIdentity>> EditApiKeysAsync(EditApiKeyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<EditApiKeyRequest<TIdentity>, IdentityApiKey<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Revoke Api Keys Async.
    /// </summary>
    /// <param name="request">The <see cref="RevokeApiKeyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual Task<IdentityApiKey<TIdentity>> RevokeApiKeysAsync(RevokeApiKeyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<RevokeApiKeyRequest<TIdentity>, IdentityApiKey<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Get User Roles Async.
    /// </summary>
    /// <param name="request">The <see cref="GetRolesRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The roles.</returns>
    public virtual Task<IEnumerable<string>> GetRolesAsync(GetRolesRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GetRolesRequest<TIdentity>, IEnumerable<string>>(request, cancellationToken);
    }

    /// <summary>
    /// Assign Role Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignRoleRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task AssignUserRoleAsync(AssignRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove Role Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveRoleRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task RemoveUserRoleAsync(RemoveRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Role Claims Async.
    /// </summary>
    /// <param name="request">The <see cref="GetRoleClaimsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ClaimSimple"/>.</returns>
    public virtual Task<IEnumerable<ClaimSimple>> GetRoleClaimsAsync(GetRoleClaimsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GetRoleClaimsRequest<TIdentity>, IEnumerable<ClaimSimple>>(request, cancellationToken);
    }

    /// <summary>
    /// Assign Role Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignRoleClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task AssignRoleClaimAsync(AssignRoleClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

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
        if (request == null)
            throw new ArgumentNullException(nameof(request));

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
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get User Claims Async.
    /// </summary>
    /// <param name="request">The <see cref="GetClaimsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ClaimSimple"/>.</returns>
    public virtual Task<IEnumerable<ClaimSimple>> GetUserClaimsAsync(GetClaimsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync<GetClaimsRequest<TIdentity>, IEnumerable<ClaimSimple>>(request, cancellationToken);
    }

    /// <summary>
    /// Assign User Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task AssignUserClaimAsync(AssignClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove User Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task RemoveUserClaimAsync(RemoveClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Replace User Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="ReplaceClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual Task ReplaceUserClaimAsync(ReplaceClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

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
        if (request == null)
            throw new ArgumentNullException(nameof(request));

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
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return this.InvokeAsync(request, cancellationToken);
    }
}