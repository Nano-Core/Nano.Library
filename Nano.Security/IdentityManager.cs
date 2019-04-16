using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nano.Models.Exceptions;
using Nano.Security.Exceptions;
using Nano.Security.Extensions;
using Nano.Security.Models;

namespace Nano.Security
{
    /// <summary>
    /// Security Manager.
    /// </summary>
    public class IdentityManager
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
        public IdentityManager(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, SecurityOptions options)
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
                throw new UnauthorizedLockedOutException();

            if (result.RequiresTwoFactor)
                throw new UnauthorizedTwoFactorRequiredException();

            throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// The user authenticates, using external login info.
        /// </summary>
        /// <param name="loginExternal">The external logn (optional).</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> SignInExternalAsync(LoginExternal loginExternal, CancellationToken cancellationToken = default)
        {
            if (loginExternal == null) 
                throw new ArgumentNullException(nameof(loginExternal));
            
            var result = await this.SignInManager
                .ExternalLoginSignInAsync(loginExternal.LoginProvider, loginExternal.ProviderKey, false, true);

            if (!result.Succeeded)
                throw new UnauthorizedAccessException();

            var identityUser = await this.UserManager
                .FindByLoginAsync(loginExternal.LoginProvider, loginExternal.ProviderKey);

            return await this.UserManager
                .GenerateJwtToken(identityUser, this.Options);
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
        /// </summary>
        /// <param name="signUp">The <see cref="SignUp"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IdentityUser"/>.</returns>
        public virtual async Task<IdentityUser> SignUpAsync(SignUp signUp, CancellationToken cancellationToken = default)
        {
            if (signUp == null) 
                throw new ArgumentNullException(nameof(signUp));
            
            var user = new IdentityUser
            {
                Email = signUp.Email,
                UserName = signUp.Username
            };

            var result = await this.UserManager
                .CreateAsync(user, signUp.Password);

            if (!result.Succeeded)
                this.ThrowIdentityExceptions(result.Errors);

            user = await this.UserManager
                .FindByNameAsync(signUp.Username);

            await this.UserManager
                .AddToRolesAsync(user, this.Options.User.DefaultRoles);

            if (!result.Succeeded)
                this.ThrowIdentityExceptions(result.Errors);

            return user;
        }

        /// <summary>
        /// Registers a new user using an external login provider.
        /// </summary>
        /// <param name="signUpExternal">The <see cref="SignUpExternal"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IdentityUser"/>.</returns>
        public virtual async Task<IdentityUser> SignUpExternalAsync(SignUpExternal signUpExternal, CancellationToken cancellationToken = default)
        {
            if (signUpExternal == null) 
                throw new ArgumentNullException(nameof(signUpExternal));
            
            var identityUser = new IdentityUser
            {
                Email = signUpExternal.Email,
                UserName = signUpExternal.Email
            };

            var identityResult = await this.UserManager
                .CreateAsync(identityUser);

            if (!identityResult.Succeeded)
                this.ThrowIdentityExceptions(identityResult.Errors);

            identityResult = await this.UserManager
                .AddLoginAsync(identityUser, new UserLoginInfo(signUpExternal.LoginProvider, signUpExternal.ProviderKey, ""));

            if (!identityResult.Succeeded)
                this.ThrowIdentityExceptions(identityResult.Errors);

            await this.SignInManager
                .SignInAsync(identityUser, false);

            return identityUser;
        }

        /// <summary>
        /// Gets all the configured external logins.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The collection of <see cref="LoginProvider"/>.</returns>
        public virtual async Task<IEnumerable<LoginProvider>> GetExternalSchemesAsync(CancellationToken cancellationToken = default)
        {
            var schemes = await this.SignInManager
                .GetExternalAuthenticationSchemesAsync();

            return schemes
                .Select(x => new LoginProvider
                {
                    Name = x.Name,
                    DisplayName = x.DisplayName
                });
        }

        /// <summary>
        /// Removes the extenral login of a user.
        /// </summary>
        /// <param name="externalLogin">The <see cref="LoginExternal"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task RemoveExternalLoginAsync(LoginExternal externalLogin, CancellationToken cancellationToken = default)
        {
            if (externalLogin == null)
                throw new ArgumentNullException(nameof(externalLogin));

            var identityUser = await this.UserManager
                .FindByLoginAsync(externalLogin.LoginProvider, externalLogin.ProviderKey);

            if (identityUser == null)
                throw new NullReferenceException(nameof(identityUser));

            var result = await this.UserManager
                .RemoveLoginAsync(identityUser, externalLogin.LoginProvider, externalLogin.ProviderKey);

            if (result.Succeeded)
            {
                await this.SignInManager
                    .RefreshSignInAsync(identityUser);
            }
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
                .SetUserNameAsync(user, setUsername.NewUsername);
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
                .FindByIdAsync(resetPassword.UserId);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);

            if (!result.Succeeded)
                this.ThrowIdentityExceptions(result.Errors);
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

            if (!result.Succeeded)
                this.ThrowIdentityExceptions(result.Errors);

            await this.SignInManager
                .RefreshSignInAsync(user);
        }

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="changeEmail">The <see cref="ChangePassword"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task ChangeEmailAsync(ChangeEmail changeEmail, CancellationToken cancellationToken = default)
        {
            if (changeEmail == null)
                throw new ArgumentNullException(nameof(changeEmail));

            var user = await this.UserManager
                .FindByIdAsync(changeEmail.UserId);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .ChangeEmailAsync(user, changeEmail.NewEmail, changeEmail.Token);

            if (!result.Succeeded)
                this.ThrowIdentityExceptions(result.Errors);
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
                .ConfirmEmailAsync(user, confirmEmail.Token);

            if (result.Succeeded)
                return;

            this.ThrowIdentityExceptions(result.Errors);
        }

        /// <summary>
        /// Generates an email confirmation token for a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ConfirmEmailToken"/>.</returns>
        public virtual async Task<ConfirmEmailToken> GenerateConfirmEmailTokenAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            var user = await this.UserManager
                .FindByIdAsync(userId);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var token = await this.UserManager
                .GenerateEmailConfirmationTokenAsync(user);

            return new ConfirmEmailToken
            {
                UserId = userId,
                Token = token
            };
        }

        /// <summary>
        /// Generates an reset password token for a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ResetPasswordToken"/>.</returns>
        public virtual async Task<ResetPasswordToken> GenerateResetPasswordTokenAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            var user = await this.UserManager
                .FindByIdAsync(userId);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var token = await this.UserManager
                .GeneratePasswordResetTokenAsync(user);

            return new ResetPasswordToken
            {
                UserId = userId,
                Token = token
            };
        }

        /// <summary>
        /// Generates an change email token for a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="newEmail">The new email.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ResetPasswordToken"/>.</returns>
        public virtual async Task<ChangeEmailToken> GenerateChangeEmailTokenAsync(string userId, string newEmail, CancellationToken cancellationToken = default)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            var user = await this.UserManager
                .FindByIdAsync(userId);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var token = await this.UserManager
                .GenerateChangeEmailTokenAsync(user, newEmail);

            return new ChangeEmailToken
            {
                UserId = userId,
                Token = token,
                NewEmail = newEmail
            };
        }

        private void ThrowIdentityExceptions(IEnumerable<IdentityError> errors)
        {
            if (errors == null) 
                throw new ArgumentNullException(nameof(errors));
            
            var exceptions = errors
                .Select(x => new TranslationException(x.Description));

            throw new AggregateException(exceptions);
        }
    }
}