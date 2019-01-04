using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Nano.Models.Auth;
using Nano.Security.Exceptions;
using Nano.Security.Extensions;

namespace Nano.Security
{
    /// <summary>
    /// Security Manager.
    /// </summary>
    public class SecurityManager
    {
        /// <summary>
        /// Options.
        /// </summary>
        protected virtual SecurityOptions Options { get; }

        /// <summary>
        /// User Manager.
        /// </summary>
        protected virtual UserManager<IdentityUser> UserManager { get; }

        /// <summary>
        /// Sign In Manager.
        /// </summary>
        protected virtual SignInManager<IdentityUser> SignInManager { get; }

        /// <summary>
        /// The user authenticates and on success recieves a jwt token for use with auhtorization.
        /// </summary>
        /// <param name="signInManager">The <see cref="SignInManager{T}"/>.</param>
        /// <param name="userManager">The <see cref="UserManager{T}"/>.</param>
        /// <param name="options">The <see cref="SecurityOptions"/>.</param>
        public SecurityManager(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, SecurityOptions options)
        {
            this.UserManager = userManager?? throw new ArgumentNullException(nameof(userManager));
            this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// The user authenticates, and on success recieves a jwt token for use with auhtorization.
        /// If two-factor authentication is enabled, a indicating flag is returned, and no access token is generated.
        /// If the user is locked out, or sign-in is unsuccessful, an exception is thrown.
        /// </summary>
        /// <param name="login">The <see cref="Login"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> SignInAsync(Login login, CancellationToken cancellationToken = default)
        {
            if (login == null) 
                throw new ArgumentNullException(nameof(login));
            
            var result = await this.SignInManager
                .PasswordSignInAsync(login.Username, login.Password, login.IsRememerMe, this.Options.Lockout.AllowedForNewUsers);

            if (result.Succeeded)
            {
                var user = await this.UserManager
                    .FindByNameAsync(login.Username);

                return await this.UserManager
                    .GenerateJwtToken(user, this.Options);
            }

            if (result.IsLockedOut)
                throw new UnauthorizedLockoutException();
            
            throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// Signs in a user, using an external login provider.
        /// This method should be invoked after getting callback response from external login provider.
        /// </summary>
        /// <param name="loginExternalCallback">The <see cref="LoginExternalCallback"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> SignInExternalAsync(LoginExternalCallback loginExternalCallback, CancellationToken cancellationToken = default)
        {
            if (loginExternalCallback == null)
                throw new ArgumentNullException(nameof(loginExternalCallback));

            if (loginExternalCallback.RemoteError != null)
                throw new UnauthorizedAccessException(loginExternalCallback.RemoteError);
            
            var externalLoginInfo = await this.SignInManager
                .GetExternalLoginInfoAsync();
            
            if (externalLoginInfo == null)
                throw new NullReferenceException(nameof(externalLoginInfo));

            var result = await this.SignInManager
                .ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, false, true);

            IdentityUser user;
            if (result.Succeeded)
            {
                user = await this.UserManager
                    .FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
            
                return await this.UserManager
                    .GenerateJwtToken(user, this.Options);
            }

            if (result.IsLockedOut)
                throw new UnauthorizedLockoutException();

            var emailClaim = externalLoginInfo.Principal.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (emailClaim == null)
                throw new UnauthorizedEmailException();

            var signupExternal = new SignupExternal
            {
                Username = emailClaim.Value,
                Email = emailClaim.Value
            };

            user = await this.SignUpExternalAsync(signupExternal, cancellationToken);

            return await this.UserManager
                .GenerateJwtToken(user, this.Options);
        }

        /// <summary>
        /// The user is logged out, and the token is invalidated.
        /// </summary>
        /// <returns>Void.</returns>
        public virtual async Task SignOutAsync(CancellationToken cancellationToken = default)
        {
            await this.SignInManager
                .SignOutAsync();
        }

        /// <summary>
        /// Registers a new user.
        /// The new user is signed in as well, and an email confirmation is sent, if enabled. 
        /// </summary>
        /// <param name="signup">The <see cref="Signup"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IdentityUser"/>.</returns>
        public virtual async Task<IdentityUser> SignupAsync(Signup signup, CancellationToken cancellationToken = default)
        {
            if (signup == null) 
                throw new ArgumentNullException(nameof(signup));
            
            var user = new IdentityUser
            {
                Email = signup.Email,
                UserName = signup.Username 
            };

            var result = await this.UserManager
                .CreateAsync(user, signup.Password);
            
            if (!result.Succeeded)
                this.ThrowErrors(result.Errors);

            return user;
        }

        /// <summary>
        /// Completes a sign-up after successfull external logn.
        /// The user is created and linked to the external login provider.
        /// </summary>
        /// <param name="signupExternal">The <see cref="SignupExternal"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IdentityUser"/>.</returns>
        public virtual async Task<IdentityUser> SignUpExternalAsync(SignupExternal signupExternal, CancellationToken cancellationToken = default)
        {
            if (signupExternal == null) 
                throw new ArgumentNullException(nameof(signupExternal));

            var externalLoginInfo = await this.SignInManager
                .GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
                throw new NullReferenceException(nameof(externalLoginInfo));

            var user = new IdentityUser
            {
                UserName = signupExternal.Username, 
                Email = signupExternal.Email
            };
            
            var result = await this.UserManager
                .CreateAsync(user);

            if (!result.Succeeded)
                this.ThrowErrors(result.Errors);

            result = await this.UserManager
                .AddLoginAsync(user, externalLoginInfo);
                
            if (!result.Succeeded)
                this.ThrowErrors(result.Errors);

            return user;
        }

        /// <summary>
        /// Gets all the external logins configured to be allowed for authentication.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The collection of <see cref="LoginExternalProvider"/>.</returns>
        public virtual async Task<IEnumerable<LoginExternalProvider>> GetExternalLoginsAsync(CancellationToken cancellationToken = default)
        {
            var schemes = await this.SignInManager
                .GetExternalAuthenticationSchemesAsync();

            return schemes
                .Select(x => new LoginExternalProvider
                {
                    Name = x.Name,
                    DisplayName = x.DisplayName 
                });
        }

        /// <summary>
        /// Gets external logins for a user.
        /// </summary>
        /// <param name="getExternalLogins">The <see cref="GetExternalLogins"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A collection of <see cref="GetExternalLogin"/>.</returns>
        public virtual async Task<IEnumerable<GetExternalLogin>> GetExternalLoginsAsync(GetExternalLogins getExternalLogins, CancellationToken cancellationToken = default)
        {
            if (getExternalLogins == null) 
                throw new ArgumentNullException(nameof(getExternalLogins));

            var user = await this.UserManager
                .FindByIdAsync(getExternalLogins.UserId);

            var logins = await this.UserManager
                .GetLoginsAsync(user);

            return logins
                .Select(x => new GetExternalLogin
                {
                    UserId = user.Id,
                    ProviderKey = x.ProviderKey,
                    LoginProvider = x.LoginProvider,
                    AllowRemoval = user.PasswordHash != null || logins.Count > 1
                });
        }

        /// <summary>
        /// Gets the external authentication properties of the <see cref="LoginExternal"/> passed.
        /// </summary>
        /// <param name="loginExternal">The <see cref="LoginExternalProvider"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AuthenticationProperties"/>.</returns>
        public virtual async Task<AuthenticationProperties> GetExternalLoginsPropertiesAsync(LoginExternal loginExternal, CancellationToken cancellationToken = default)
        {
            if (loginExternal == null)
                throw new ArgumentNullException(nameof(loginExternal));

            return await Task.Factory
                .StartNew(() => this.SignInManager
                    .ConfigureExternalAuthenticationProperties(loginExternal.Name, loginExternal.CallbackUrl), cancellationToken);
        }

        /// <summary>
        /// Changes the username of a user.
        /// </summary>
        /// <param name="setUsername">The <see cref="SetUsername"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task SetUsernameAsync(SetUsername setUsername, CancellationToken cancellationToken = default)
        {
            if (setUsername == null) 
                throw new ArgumentNullException(nameof(setUsername));
            
            var user = await this.UserManager
                .FindByIdAsync(setUsername.UserId);

            await this.UserManager
                .SetUserNameAsync(user, setUsername.Username);
        }

        /// <summary>
        /// Createa a password for a user.
        /// Used when logged in using external login provider, and no password has been created for the user.
        /// </summary>
        /// <param name="setPassword">The <see cref="SetPassword"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task SetPasswordAsync(SetPassword setPassword, CancellationToken cancellationToken = default)
        {
            if (setPassword == null) 
                throw new ArgumentNullException(nameof(setPassword));
            
            var user = await this.UserManager
                .FindByIdAsync(setPassword.UserId);

            await this.UserManager
                .AddPasswordAsync(user, setPassword.NewPassword);
        }

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="changePassword">The <see cref="ChangePassword"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task ChangePasswordAsync(ChangePassword changePassword, CancellationToken cancellationToken = default)
        {
            if (changePassword == null) 
                throw new ArgumentNullException(nameof(changePassword));

            var user = await this.UserManager
                .FindByIdAsync(changePassword.UserId);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);

            if (result.Succeeded)
            {
                await this.SignInManager
                    .RefreshSignInAsync(user);
                
                return;
            }

            this.ThrowErrors(result.Errors);
        }

        /// <summary>
        /// Resets the password of a user.
        /// </summary>
        /// <param name="resetPassword">The <see cref="ResetPassword"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task ResetPasswordAsync(ResetPassword resetPassword, CancellationToken cancellationToken = default)
        {
            if (resetPassword == null) 
                throw new ArgumentNullException(nameof(resetPassword));

            var user = await this.UserManager
                .FindByEmailAsync(resetPassword.Email);
            
            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .ResetPasswordAsync(user, resetPassword.Code, resetPassword.Password);
            
            if (result.Succeeded)
                return;

            this.ThrowErrors(result.Errors);
        }

        /// <summary>
        /// Generates an reset password token for a user.
        /// </summary>
        /// <param name="getResetPassword">The <see cref="GetResetPassword"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ResetPassword"/>.</returns>
        public virtual async Task<ResetPassword> GetResetPasswordAsync(GetResetPassword getResetPassword, CancellationToken cancellationToken = default)
        {
            if (getResetPassword == null) 
                throw new ArgumentNullException(nameof(getResetPassword));
            
            var user = await this.UserManager
                .FindByEmailAsync(getResetPassword.Email);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var isEmailConfirm = await this.UserManager
                .IsEmailConfirmedAsync(user);

            if (!isEmailConfirm)
                return new ResetPassword();

            var code = await this.UserManager
                .GeneratePasswordResetTokenAsync(user);

            return new ResetPassword
            {
                Email = user.Email,
                Code = code
            };
        }

        /// <summary>
        /// Removes the extenral login of a user.
        /// </summary>
        /// <param name="removeExternalLogin">The <see cref="RemoveExternalLogin"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task RemoveExternalLoginAsync(RemoveExternalLogin removeExternalLogin, CancellationToken cancellationToken = default)
        {
            if (removeExternalLogin == null) 
                throw new ArgumentNullException(nameof(removeExternalLogin));

            var user = await this.UserManager
                .FindByIdAsync(removeExternalLogin.UserId);
            
            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .RemoveLoginAsync(user, removeExternalLogin.LoginProvider, removeExternalLogin.ProviderKey);
            
            if (result.Succeeded)
            {
                await this.SignInManager
                    .RefreshSignInAsync(user);
            }
        }

        /// <summary>
        /// Confirms the email of a user.
        /// </summary>
        /// <param name="confirmEmail">The <see cref="ConfirmEmail"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task ConfirmEmailAsync(ConfirmEmail confirmEmail, CancellationToken cancellationToken = default)
        {
            if (confirmEmail == null) 
                throw new ArgumentNullException(nameof(confirmEmail));
            
            var user = await this.UserManager
                .FindByIdAsync(confirmEmail.UserId);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .ConfirmEmailAsync(user, confirmEmail.Code);

            if (result.Succeeded)
                return;

            this.ThrowErrors(result.Errors);
        }

        /// <summary>
        /// Generates an email confirmation token for a user.
        /// </summary>
        /// <param name="getConfirmEmail">The <see cref="GetConfirmEmail"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ConfirmEmail"/>.</returns>
        public virtual async Task<ConfirmEmail> GetConfirmEmailAsync(GetConfirmEmail getConfirmEmail, CancellationToken cancellationToken = default)
        {
            if (getConfirmEmail == null) 
                throw new ArgumentNullException(nameof(getConfirmEmail));
            
            var user = await this.UserManager
                .FindByIdAsync(getConfirmEmail.UserId);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var code = await this.UserManager
                .GenerateEmailConfirmationTokenAsync(user);

            return new ConfirmEmail
            {
                Code = code,
                UserId = user.Id
            };
        }

        private void ThrowErrors(IEnumerable<IdentityError> errors)
        {
            if (errors == null) 
                throw new ArgumentNullException(nameof(errors));
            
            var exceptions = errors
                .Select(x => new InvalidOperationException($"{x.Code}: {x.Description}"));

            throw new AggregateException(exceptions);
        }
    }
}