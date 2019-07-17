using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nano.Models.Exceptions;
using Nano.Security.Exceptions;
using Nano.Security.Extensions;
using Nano.Security.Models;
using Newtonsoft.Json;

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
        public virtual UserManager<IdentityUser> UserManager { get; }

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
        /// Gets all the configured external logins schemes.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The collection of <see cref="LoginProvider"/>'s.</returns>
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
        /// Signs in a user.
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

                return await this.GetJwtTokenAsync(user);
            }

            if (result.IsLockedOut)
                throw new UnauthorizedLockedOutException();

            if (result.RequiresTwoFactor)
                throw new UnauthorizedTwoFactorRequiredException();

            throw new UnauthorizedException();
        }

        /// <summary>
        /// Signs in a user, from external login.
        /// </summary>
        /// <param name="loginExternal">The <see cref="LoginExternal"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        public virtual async Task<AccessToken> SignInExternalAsync(LoginExternal loginExternal, CancellationToken cancellationToken = default)
        {
            if (loginExternal == null) 
                throw new ArgumentNullException(nameof(loginExternal));

            var identityUser = await this.UserManager
                .FindByLoginAsync(loginExternal.LoginProvider, loginExternal.ProviderKey);

            var success = await this.ValidateExternalAccessToken(loginExternal, cancellationToken);
            
            if (!success)
                throw new UnauthorizedException();
            
            await this.SignInManager
                .SignInAsync(identityUser, loginExternal.IsRememerMe);

            return await this.GetJwtTokenAsync(identityUser);
        }

        /// <summary>
        /// Configures the external authentication properties and returns a <see cref="ChallengeResult"/>.
        /// </summary>
        /// <param name="loginProvider">The login provider.</param>
        /// <param name="redirectUrl">The redirect url.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ChallengeResult"/>.</returns>
        public virtual async Task<ChallengeResult> SignInExternalChallangeAsync(string loginProvider, string redirectUrl, CancellationToken cancellationToken = default)
        {
            if (loginProvider == null) 
                throw new ArgumentNullException(nameof(loginProvider));

            if (redirectUrl == null) 
                throw new ArgumentNullException(nameof(redirectUrl));

            return await Task.Factory
                .StartNew(() =>
                {
                    var properties = this.SignInManager
                        .ConfigureExternalAuthenticationProperties(loginProvider, redirectUrl);
                    
                    return new ChallengeResult(loginProvider, properties);
                }, cancellationToken);
        }

        /// <summary>
        /// Callback for signing in a user with external login info,
        /// from external login cookie data.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ExternalLoginData"/>.</returns>
        public virtual async Task<ExternalLoginData> SignInExternalCallbackAsync(CancellationToken cancellationToken = default)
        {
            var externalLoginInfo = await this.SignInManager
                .GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
                throw new UnauthorizedException();

            var result = await this.SignInManager
                .ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, false, true);

            if (result.Succeeded)
                return null;
            
            if (result.IsLockedOut)
                throw new UnauthorizedLockedOutException();
            
            if (result.RequiresTwoFactor)
                throw new UnauthorizedTwoFactorRequiredException();

            var id = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var name = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name);
            var address = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.StreetAddress);
            var emailAddress = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            var birthDay = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.DateOfBirth);
                
            if (emailAddress == null)
                throw new UnauthorizedEmailAddressNotFoundException();

            return new ExternalLoginData
            {
                Id = id,
                Name = name,
                Address = address,
                Email = emailAddress,
                BirthDay = birthDay == null ? (DateTime?)null : DateTime.Parse(birthDay)
            };
        }
        
        /// <summary>
        /// Callback for signing in a user with external login info,
        /// from passed in <see cref="LoginExternal"/> data.
        /// </summary>
        /// <param name="loginExternal">The <see cref="LoginExternal"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ExternalLoginData"/>.</returns>
        public virtual async Task<ExternalLoginData> SignInExternalCallbackAsync(LoginExternal loginExternal, CancellationToken cancellationToken = default)
        {
            if (loginExternal == null) 
                throw new ArgumentNullException(nameof(loginExternal));

            var result = await this.SignInManager
                .ExternalLoginSignInAsync(loginExternal.LoginProvider, loginExternal.ProviderKey, false, true);

            if (result.Succeeded)
                return null;
            
            if (result.IsLockedOut)
                throw new UnauthorizedLockedOutException();

            if (result.RequiresTwoFactor)
                throw new UnauthorizedTwoFactorRequiredException();

            return await this.GetExternalProviderInfo(loginExternal, cancellationToken);
        }

        /// <summary>
        /// Logs out a user.
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
            
            var identityUser = new IdentityUser
            {
                Email = signUp.EmailAddress,
                UserName = signUp.Username
            };

            var result = await this.UserManager
                .CreateAsync(identityUser, signUp.Password);

            if (!result.Succeeded)
                this.ThrowIdentityExceptions(result.Errors);

            identityUser = await this.UserManager
                .FindByNameAsync(signUp.Username);

            await this.UserManager
                .AddToRolesAsync(identityUser, this.Options.User.DefaultRoles);

            if (!result.Succeeded)
                this.ThrowIdentityExceptions(result.Errors);

            return identityUser;
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
                Email = signUpExternal.EmailAddress,
                UserName = signUpExternal.EmailAddress
            };

            var createResult = await this.UserManager
                .CreateAsync(identityUser);

            if (!createResult.Succeeded)
                this.ThrowIdentityExceptions(createResult.Errors);

            var addLoginResult = await this.UserManager
                .AddLoginAsync(identityUser, new UserLoginInfo(signUpExternal.ExternalLogin.LoginProvider, signUpExternal.ExternalLogin.ProviderKey, signUpExternal.ExternalLogin.LoginProvider));

            if (!addLoginResult.Succeeded)
                this.ThrowIdentityExceptions(createResult.Errors);

            await this.UserManager
                .AddToRolesAsync(identityUser, this.Options.User.DefaultRoles);

            await this.SignInManager
                .SignInAsync(identityUser, signUpExternal.ExternalLogin.IsRememerMe);

            return identityUser;
        }

        /// <summary>
        /// Removes the extenral login of a user.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task RemoveExternalLoginAsync(CancellationToken cancellationToken = default)
        {
            var externalLoginInfo = await this.SignInManager
                .GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
                throw new UnauthorizedException();

            var identityUser = await this.UserManager
                .FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);

            if (identityUser == null)
                throw new NullReferenceException(nameof(identityUser));

            var result = await this.UserManager
                .RemoveLoginAsync(identityUser, externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);

            if (result.Succeeded)
            {
                await this.SignInManager
                    .RefreshSignInAsync(identityUser);
            }
        }

        /// <summary>
        /// Sets a username for a user.
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
        /// Sets a password for a user.
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

            var hasPassword = await this.UserManager
                .HasPasswordAsync(user);

            if (hasPassword)
                throw new UnauthorizedSetPasswordException();

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
                .FindByEmailAsync(resetPassword.EmailAddress);

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
        /// Changes the email address of a user.
        /// </summary>
        /// <param name="changeEmail">The <see cref="ChangeEmail"/>.</param>
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
                .ChangeEmailAsync(user, changeEmail.NewEmailAddress, changeEmail.Token);

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
                .FindByEmailAsync(confirmEmail.EmailAddress);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .ConfirmEmailAsync(user, confirmEmail.Token);

            if (result.Succeeded)
                return;

            this.ThrowIdentityExceptions(result.Errors);
        }

        /// <summary>
        /// Generates an reset password token for a user.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ResetPasswordToken"/>.</returns>
        public virtual async Task<ResetPasswordToken> GenerateResetPasswordTokenAsync(string emailAddress, CancellationToken cancellationToken = default)
        {
            if (emailAddress == null)
                throw new ArgumentNullException(nameof(emailAddress));

            var user = await this.UserManager
                .FindByEmailAsync(emailAddress);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var token = await this.UserManager
                .GeneratePasswordResetTokenAsync(user);

            return new ResetPasswordToken
            {
                Token = token,
                EmailAddress = emailAddress
            };
        }

        /// <summary>
        /// Generates an email confirmation token for a user.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ConfirmEmailToken"/>.</returns>
        public virtual async Task<ConfirmEmailToken> GenerateConfirmEmailTokenAsync(string emailAddress, CancellationToken cancellationToken = default)
        {
            if (emailAddress == null)
                throw new ArgumentNullException(nameof(emailAddress));

            var user = await this.UserManager
                .FindByEmailAsync(emailAddress);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var token = await this.UserManager
                .GenerateEmailConfirmationTokenAsync(user);

            return new ConfirmEmailToken
            {
                Token = token,
                EmailAddress = emailAddress
            };
        }

        /// <summary>
        /// Generates an change email token for a user.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="newEmailAddress">The new email address.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ResetPasswordToken"/>.</returns>
        public virtual async Task<ChangeEmailToken> GenerateChangeEmailTokenAsync(string emailAddress, string newEmailAddress, CancellationToken cancellationToken = default)
        {
            if (emailAddress == null)
                throw new ArgumentNullException(nameof(emailAddress));

            if (newEmailAddress == null) 
                throw new ArgumentNullException(nameof(newEmailAddress));

            var user = await this.UserManager
                .FindByEmailAsync(emailAddress);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var token = await this.UserManager
                .GenerateChangeEmailTokenAsync(user, newEmailAddress);

            return new ChangeEmailToken
            {
                Token = token,
                EmailAddress = emailAddress,
                NewEmailAddress = newEmailAddress
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
        private async Task<AccessToken> GetJwtTokenAsync(IdentityUser identityUser)
        {
            if (identityUser == null) 
                throw new ArgumentNullException(nameof(identityUser));
            
            return await this.UserManager
                .GenerateJwtToken(identityUser, this.Options);
        }
        private async Task<ExternalLoginData> GetExternalProviderInfo(LoginExternal loginExternal, CancellationToken cancellationToken = default)
        {
            switch (loginExternal.LoginProvider)
            {
                case "Facebook":
                    using (var client = new HttpClient())
                    {
                        const string HOST = "https://graph.facebook.com";
                        const string FIELDS = "id,name,address,email,birthday";
                        
                        var url = $"{HOST}/{loginExternal.ProviderKey}/?fields={FIELDS}&access_token={loginExternal.AccessToken}";
                        var response = await client.GetAsync(url, cancellationToken);
                        var content = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<ExternalLoginData>(content);
                    }
                    
                default:
                    throw new NotSupportedException(loginExternal.LoginProvider);
            }
        }
        private async Task<bool> ValidateExternalAccessToken(LoginExternal loginExternal, CancellationToken cancellationToken = default)
        {
            switch (loginExternal.LoginProvider)
            {
                case "Facebook":
                    using (var client = new HttpClient())
                    {
                        const string HOST = "https://graph.facebook.com";
                     
                        var url = $"{HOST}/app?access_token={loginExternal.AccessToken}";
                        var response = await client.GetAsync(url, cancellationToken);

                        if (!response.IsSuccessStatusCode)
                            return false;
                        
                        var content = await response.Content.ReadAsStringAsync();
                        var validation = JsonConvert.DeserializeObject<dynamic>(content);

                        return validation.id?.ToString() == loginExternal.ProviderKey;
                    }

                default:
                    throw new NotSupportedException(loginExternal.LoginProvider);
            }
        }
    }
}