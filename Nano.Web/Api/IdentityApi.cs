using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Models;
using Nano.Security.Models;
using Nano.Web.Api.Requests.Identity;
using Nano.Web.Api.Responses;

namespace Nano.Web.Api
{
    /// <summary>
    /// Default Identity Api.
    /// </summary>
    public class IdentityApi<TUser> : DefaultApi
        where TUser : DefaultEntityUser
    {
        /// <summary>
        /// Identity Controller.
        /// </summary>
        protected static string IdentityController => $"{typeof(TUser).Name.ToLower()}s";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="apiOptions">The <see cref="ApiOptions"/>.</param>
        public IdentityApi(ApiOptions apiOptions)
            : base(apiOptions)
        {

        }
            
        /// <summary>
        /// Sign Up Async.
        /// </summary>
        /// <param name="request">The <see cref="SignUpRequest{TUser}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The instance of <typeparamref name="TUser"/>.</returns>
        public virtual async Task<TUser> SignUpAsync(SignUpRequest<TUser> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<SignUpRequest<TUser>, TUser>(request, cancellationToken);
        }

        /// <summary>
        /// Sign Up External Async.
        /// </summary>
        /// <param name="request">The <see cref="SignUpExternalRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<ResponseRedirect> SignUpExternalAsync(SignUpExternalRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
      
            request.Controller = IdentityApi<TUser>.IdentityController;

            return await this.CustomAsync<SignUpExternalRequest, ResponseRedirect>(request, cancellationToken);
        }

        /// <summary>
        /// Sign Up External Callback Async.
        /// </summary>
        /// <param name="request">The <see cref="SignUpExternalRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> SignUpExternalCallbackAsync(SignUpExternalCallbackRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
      
            request.Controller = IdentityApi<TUser>.IdentityController;

            return await this.CustomAsync<SignUpExternalCallbackRequest, AccessToken>(request, cancellationToken);
        }

        /// <summary>
        /// Set Username Async.
        /// </summary>
        /// <param name="request">The <see cref="SetUsernameRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task SetUsernameAsync(SetUsernameRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Controller = IdentityApi<TUser>.IdentityController;
            
            await this.CustomAsync(request, cancellationToken);
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

            request.Controller = IdentityApi<TUser>.IdentityController;

            await this.CustomAsync(request, cancellationToken);
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
            
            request.Controller = IdentityApi<TUser>.IdentityController;

            await this.CustomAsync(request, cancellationToken);
        }

        /// <summary>
        /// Change Password Async.
        /// </summary>
        /// <param name="request">The <see cref="ChangePasswordRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Controller = IdentityApi<TUser>.IdentityController;
            
            await this.CustomAsync(request, cancellationToken);
        }

        /// <summary>
        /// Change Email Async.
        /// </summary>
        /// <param name="request">The <see cref="ChangeEmailRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task ChangeEmailAsync(ChangeEmailRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Controller = IdentityApi<TUser>.IdentityController;
            
            await this.CustomAsync(request, cancellationToken);
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

            request.Controller = IdentityApi<TUser>.IdentityController;
            
            await this.CustomAsync(request, cancellationToken);
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

            request.Controller = IdentityApi<TUser>.IdentityController;

            return await this.CustomAsync<GetChangeEmailTokenRequest, ChangeEmailToken>(request, cancellationToken);
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

            request.Controller = IdentityApi<TUser>.IdentityController;

            return await this.CustomAsync<GetConfirmEmailTokenRequest, ConfirmEmailToken>(request, cancellationToken);
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

            request.Controller = IdentityApi<TUser>.IdentityController;

            return await this.CustomAsync<GetResetPasswordTokenRequest, ResetPasswordToken>(request, cancellationToken);
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

            request.Controller = IdentityApi<TUser>.IdentityController;

            await this.CustomAsync(request, cancellationToken);
        }
    }
}