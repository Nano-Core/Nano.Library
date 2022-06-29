using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Models.Interfaces;
using Nano.Security.Models;
using Nano.Web.Api.Requests.Identity;

namespace Nano.Web.Api;

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
    public virtual async Task<TUser> SignUpAsync(SignUpRequest<TUser, TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return await this.InvokeAsync<SignUpRequest<TUser, TIdentity>, TUser>(request, cancellationToken);
    }

    /// <summary>
    /// Sign Up External Callback Async.
    /// </summary>
    /// <typeparam name="TSignUp">The signup type.</typeparam>
    /// <param name="request">The <see cref="BaseSignUpExternalRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual async Task<TUser> SignUpExternalAsync<TSignUp>(TSignUp request, CancellationToken cancellationToken = default)
        where TSignUp : BaseSignUpExternalRequest
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return await this.InvokeAsync<TSignUp, TUser>(request, cancellationToken);
    }

    /// <summary>
    /// Set Username Async.
    /// </summary>
    /// <param name="request">The <see cref="SetUsernameRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task SetUsernameAsync(SetUsernameRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Set Password Async.
    /// </summary>
    /// <param name="request">The <see cref="SetPasswordRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task SetPasswordAsync(SetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Reset Password Async.
    /// </summary>
    /// <param name="request">The <see cref="ResetPasswordRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Change Password Async.
    /// </summary>
    /// <param name="request">The <see cref="ChangePasswordRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task ChangePasswordAsync(ChangePasswordRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Change Email Async.
    /// </summary>
    /// <param name="request">The <see cref="ChangeEmailRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task ChangeEmailAsync(ChangeEmailRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Confirm Email Async.
    /// </summary>
    /// <param name="request">The <see cref="ConfirmEmailRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Change Email Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GetChangeEmailTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task<ChangeEmailToken> GetChangeEmailTokenAsync(GetChangeEmailTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetChangeEmailTokenRequest, ChangeEmailToken>(request, cancellationToken);
    }

    /// <summary>
    /// Get Confirm Email Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GetConfirmEmailTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task<ConfirmEmailToken> GetConfirmEmailTokenAsync(GetConfirmEmailTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetConfirmEmailTokenRequest, ConfirmEmailToken>(request, cancellationToken);
    }

    /// <summary>
    /// Change Phone Async.
    /// </summary>
    /// <param name="request">The <see cref="ChangePhoneRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task ChangePhoneAsync(ChangePhoneRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Confirm Phone Async.
    /// </summary>
    /// <param name="request">The <see cref="ConfirmPhoneRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task ConfirmPhoneAsync(ConfirmPhoneRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Change Phone Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GetChangePhoneTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task<ChangePhoneNumberToken> GetChangePhoneTokenAsync(GetChangePhoneTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetChangePhoneTokenRequest, ChangePhoneNumberToken>(request, cancellationToken);
    }

    /// <summary>
    /// Get Confirm Phone Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GetConfirmPhoneTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task<ConfirmPhoneNumberToken> GetConfirmPhoneTokenAsync(GetConfirmPhoneTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetConfirmPhoneTokenRequest, ConfirmPhoneNumberToken>(request, cancellationToken);
    }

    /// <summary>
    /// Get Reset Password Token Async.
    /// </summary>
    /// <param name="request">The <see cref="GetResetPasswordTokenRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task<ResetPasswordToken> GetResetPasswordTokenAsync(GetResetPasswordTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        return await this.InvokeAsync<GetResetPasswordTokenRequest, ResetPasswordToken>(request, cancellationToken);
    }

    /// <summary>
    /// Remove External Login Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveExternalLogInRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task RemoveExternalLoginAsync(RemoveExternalLogInRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Roles Async.
    /// </summary>
    /// <param name="request">The <see cref="GetRolesRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task GetRolesAsync(GetRolesRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Assign Role Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignRoleRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task AssignUserRoleAsync(AssignRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove Role Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveRoleRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task RemoveUserRoleAsync(RemoveRoleRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Get Claims Async.
    /// </summary>
    /// <param name="request">The <see cref="GetClaimsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task GetClaimsAsync(GetClaimsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Assign Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="AssignClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task AssignUserClaimAsync(AssignClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }

    /// <summary>
    /// Remove Claim Async.
    /// </summary>
    /// <param name="request">The <see cref="RemoveClaimRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task RemoveUserClaimAsync(RemoveClaimRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        request.Controller = BaseIdentityApi<TUser, TIdentity>.IdentityController;

        await this.InvokeAsync(request, cancellationToken);
    }
}