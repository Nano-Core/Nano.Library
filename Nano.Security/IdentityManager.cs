using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Nano.Config;
using Nano.Models.Exceptions;
using Nano.Security.Const;
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
        internal const string DEFAULT_APP_ID = "Default";

        /// <summary>
        /// Db Context.
        /// </summary>
        protected virtual DbContext DbContext { get; }

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
        /// <param name="dbContext">The <see cref="SignInManager{T}"/>.</param>
        /// <param name="signInManager">The <see cref="SignInManager{T}"/>.</param>
        /// <param name="userManager">The <see cref="UserManager{T}"/>.</param>
        /// <param name="options">The <see cref="SecurityOptions"/>.</param>
        public IdentityManager(DbContext dbContext, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, SecurityOptions options)
        {
            this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Gets an <see cref="AccessToken"/> using the passed <see cref="AccessTokenData"/>.
        /// </summary>
        /// <param name="accessTokenData">The <see cref="AccessTokenData"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        public virtual async Task<AccessToken> GetJwtToken(AccessTokenData accessTokenData, CancellationToken cancellationToken = default)
        {
            if (accessTokenData == null)
                throw new ArgumentNullException(nameof(accessTokenData));

            return await this.GenerateJwtToken(accessTokenData, cancellationToken);
        }

        /// <summary>
        /// Gets all the configured external logins schemes.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The collection of <see cref="LoginProvider"/>'s.</returns>
        public virtual async Task<IEnumerable<LoginProvider>> GetExternalProvidersAsync(CancellationToken cancellationToken = default)
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
        /// Gets the external provider info.
        /// </summary>
        /// <param name="loginExternal">The <see cref="LoginExternalProvider"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ExternalLoginData"/></returns>
        public virtual async Task<ExternalLoginData> GetExternalProviderInfoAsync(LoginExternalProvider loginExternal, CancellationToken cancellationToken = default)
        {
            switch (loginExternal.LoginProvider)
            {
                case "Facebook":
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            const string HOST = "https://graph.facebook.com";
                            const string FIELDS = "id,name,address,email,birthday";
                        
                            var url = $"{HOST}/{loginExternal.ProviderKey}/?fields={FIELDS}&access_token={loginExternal.AccessToken}";

                            using (var response = await client.GetAsync(url, cancellationToken))
                            {
                                var content = await response.Content
                                    .ReadAsStringAsync();

                                return JsonConvert.DeserializeObject<ExternalLoginData>(content);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.UserManager.Logger.LogWarning(ex, ex.Message);

                            throw new UnauthorizedException();
                        }
                    }

                case "Google":
                    try
                    {
                        var payload = await GoogleJsonWebSignature
                            .ValidateAsync(loginExternal.AccessToken);

                        return new ExternalLoginData
                        {
                            Id = payload.Subject,
                            Name = payload.Name,
                            Email = payload.Email
                        };
                    }
                    catch (Exception ex)
                    {
                        this.UserManager.Logger.LogWarning(ex, ex.Message);

                        throw new UnauthorizedException();
                    }  

                default:
                    throw new NotSupportedException(loginExternal.LoginProvider);
            }
        }

        /// <summary>
        /// Validates the external provider access token.
        /// </summary>
        /// <param name="loginExternal">The <see cref="LoginExternalProvider"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Boolean, indicating if the external provider access token is still valid.</returns>
        public virtual async Task<bool> ValidateExternalProviderAccessToken(LoginExternalProvider loginExternal, CancellationToken cancellationToken = default)
        {
            if (loginExternal == null) 
                throw new ArgumentNullException(nameof(loginExternal));
            
            var externalLoginOption = this.Options.ExternalLogins
                .FirstOrDefault(x => x.Name == loginExternal.LoginProvider);

            if (externalLoginOption == null)
                throw new NullReferenceException(nameof(externalLoginOption));
            
            switch (loginExternal.LoginProvider)
            {
                case "Facebook":
                    using (var client = new HttpClient())
                    {
                        const string HOST = "https://graph.facebook.com";

                        var url = $"{HOST}/debug_token?input_token={loginExternal.AccessToken}&access_token={externalLoginOption.Id}|{externalLoginOption.Secret}";
                        var response = await client.GetAsync(url, cancellationToken);

                        if (!response.IsSuccessStatusCode)
                            return false;
                        
                        var content = await response.Content.ReadAsStringAsync();
                        var validation = JsonConvert.DeserializeObject<dynamic>(content);

                        if (!(bool)validation.data.is_valid)
                            return false;

                        if (validation.data.app_id != externalLoginOption.Id)
                            return false;

                        if (validation.data.user_id != loginExternal.ProviderKey)
                            return false;

                        return true;
                    }
                    
                case "Google":
                    var settings = new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[]
                        {
                            externalLoginOption.Id
                        }
                    };

                    var payload = await GoogleJsonWebSignature
                        .ValidateAsync(loginExternal.AccessToken, settings);

                    return payload.Subject == loginExternal.ProviderKey;

                default:
                    throw new NotSupportedException(loginExternal.LoginProvider);
            }
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

            if (ConfigManager.HasDbContext)
            {
                var result = await this.SignInManager
                    .PasswordSignInAsync(login.Username, login.Password, login.IsRememerMe, this.Options.Lockout.AllowedForNewUsers);

                if (result.Succeeded)
                {
                    var appId = login.AppId ?? IdentityManager.DEFAULT_APP_ID;

                    var identityUser = await this.UserManager
                        .FindByNameAsync(login.Username);

                    return await this.GenerateJwtToken(identityUser, appId, cancellationToken);
                }

                if (result.IsLockedOut)
                    throw new UnauthorizedLockedOutException();

                if (result.RequiresTwoFactor)
                    throw new UnauthorizedTwoFactorRequiredException();
            }
            else
            {
                return await this.SignInAdminAsync(login, cancellationToken);
            }

            throw new UnauthorizedException();
        }

        /// <summary>
        /// Signs in the admin user statically.
        /// The login is transient, no Identity backing store is used.
        /// </summary>
        /// <param name="login">The <see cref="Login"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> SignInAdminAsync(Login login, CancellationToken cancellationToken = new CancellationToken())
        {
            if (login == null) 
                throw new ArgumentNullException(nameof(login));

            if (login.Username == this.Options.User.AdminUsername && login.Password == this.Options.User.AdminPassword)
            {
                var tokenData = new AccessTokenData
                {
                    UserId = Guid.Empty.ToString(),
                    UserName = this.Options.User.AdminUsername,
                    UserEmail = this.Options.User.AdminEmailAddress,
                    Claims = new[]
                    {
                        new Claim(ClaimTypes.Role, BuiltInUserRoles.Administrator)
                    }
                };

                return await this.GenerateJwtToken(tokenData, cancellationToken);
            }            

            throw new UnauthorizedException();
        }

        /// <summary>
        /// Refresh the login of a user.
        /// </summary>
        /// <param name="loginRefresh">The <see cref="LoginRefresh"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> SignInRefreshAsync(LoginRefresh loginRefresh, CancellationToken cancellationToken = default)
        {
            if (loginRefresh == null) 
                throw new ArgumentNullException(nameof(loginRefresh));

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true, 
                    ValidateLifetime = false, 
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = this.Options.Jwt.Issuer,
                    ValidAudience = this.Options.Jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Options.Jwt.SecretKey)),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(loginRefresh.Token, validationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new InvalidOperationException();

                var identityUser = await this.UserManager
                    .FindByNameAsync(principal.Identity.Name);

                var appClaim = principal.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypesExtended.AppId);

                if (appClaim == null)
                    throw new NullReferenceException(nameof(appClaim));

                var identityUserToken = this.DbContext
                    .Set<IdentityUserTokenExpiry<string>>()
                    .Where(x => x.UserId == identityUser.Id && x.Name == appClaim.Value)
                    .AsNoTracking()
                    .FirstOrDefault();

                if (identityUserToken == null)
                    throw new NullReferenceException(nameof(identityUserToken));

                if (identityUserToken.Value != loginRefresh.RefreshToken)
                    throw new InvalidOperationException($"identityUserToken.Value ({identityUserToken.Value}) != loginRefresh.RefreshToken ({loginRefresh.RefreshToken})");

                if (identityUserToken.ExpireAt <= DateTimeOffset.UtcNow)
                    throw new InvalidOperationException("identityUserToken.ExpireAt <= DateTimeOffset.UtcNow");

                return await this.GenerateJwtToken(identityUser, identityUserToken.Name, cancellationToken);
            }
            catch (Exception ex)
            {
                this.UserManager.Logger.LogWarning(ex, ex.Message);

                throw new UnauthorizedException();
            }
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

            if (identityUser == null)
                return null;

            var success = await this.ValidateExternalProviderAccessToken(loginExternal, cancellationToken);
            
            if (!success)
                throw new UnauthorizedException();

            var appId = loginExternal.AppId ?? IdentityManager.DEFAULT_APP_ID;

            await this.SignInManager
                .SignInAsync(identityUser, loginExternal.IsRememerMe);

            return await this.GenerateJwtToken(identityUser, appId, cancellationToken);
        }

        /// <summary>
        /// Signs in a user, from external login.
        /// The login is transient, no Identity backing store is used.
        /// The login relies on the external login provider being valid.
        /// </summary>
        /// <param name="loginExternalTransient">The <see cref="LoginExternal"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        public virtual async Task<AccessToken> SignInExternalTransientAsync(LoginExternalTransient loginExternalTransient, CancellationToken cancellationToken = default)
        {
            if (loginExternalTransient == null) 
                throw new ArgumentNullException(nameof(loginExternalTransient));

            var externalLoginData = await this.GetExternalProviderInfoAsync(loginExternalTransient, cancellationToken);
            
            if (externalLoginData == null)
                throw new UnauthorizedException();

            var tokenData = new AccessTokenData
            {
                UserId = externalLoginData.Id,
                UserName = externalLoginData.Name,
                UserEmail = externalLoginData.Email,
                Claims = loginExternalTransient.Roles
                    .Select(x => new Claim(ClaimTypes.Role, x))
            };

            return await this.GenerateJwtToken(tokenData, cancellationToken);
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
        public virtual async Task<ExternalLoginData> SignInExternalChallangeCallbackAsync(CancellationToken cancellationToken = default)
        {
            var externalLoginInfo = await this.SignInManager
                .GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
                throw new UnauthorizedException();

            var result = await this.SignInManager
                .ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, false, true);

            if (!result.Succeeded)
                throw new UnauthorizedException();
            
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
        /// Logs out a user.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task SignOutAsync(CancellationToken cancellationToken = default)
        {
            var appId = this.SignInManager.Context
                .GetJwtAppId();

            var username = this.SignInManager.Context
                .GetJwtUserName();

            var identityUser = await this.UserManager
                .FindByNameAsync(username);

            await this.UserManager
                .RemoveAuthenticationTokenAsync(identityUser, JwtBearerDefaults.AuthenticationScheme, appId);

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
            {
                var invalidEmail = new IdentityErrorDescriber().InvalidEmail(resetPassword.EmailAddress);
                throw new TranslationException(invalidEmail.Description);
            }

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

            user.EmailConfirmed = false;
 
            this.DbContext
                .Update(user);

            await this.DbContext
                .SaveChangesAsync(cancellationToken);
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
            {
                var invalidEmail = new IdentityErrorDescriber().InvalidEmail(confirmEmail.EmailAddress);
                throw new TranslationException(invalidEmail.Description);
            }

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
            {
                var invalidEmail = new IdentityErrorDescriber().InvalidEmail(emailAddress);
                throw new TranslationException(invalidEmail.Description);
            }

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
            {
                var invalidEmail = new IdentityErrorDescriber().InvalidEmail(emailAddress);
                throw new TranslationException(invalidEmail.Description);
            }

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
            {
                var invalidEmail = new IdentityErrorDescriber().InvalidEmail(emailAddress);
                throw new TranslationException(invalidEmail.Description);
            }

            var userNew = await this.UserManager
                .FindByEmailAsync(newEmailAddress);

            if (userNew != null)
            {
                var duplicateEmail = new IdentityErrorDescriber().DuplicateEmail(newEmailAddress);
                throw new TranslationException(duplicateEmail.Description);
            }

            var token = await this.UserManager
                .GenerateChangeEmailTokenAsync(user, newEmailAddress);

            return new ChangeEmailToken
            {
                Token = token,
                EmailAddress = emailAddress,
                NewEmailAddress = newEmailAddress
            };
        }

        private async Task<AccessToken> GenerateJwtToken(AccessTokenData tokenData, CancellationToken cancellationToken = default)
        {
            if (tokenData == null) 
                throw new ArgumentNullException(nameof(tokenData));

            var claims = new Collection<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, tokenData.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, tokenData.UserId),
                    new Claim(JwtRegisteredClaimNames.Email, tokenData.UserEmail),
                    new Claim(ClaimTypes.Name, tokenData.UserName),
                    new Claim(ClaimTypes.NameIdentifier, tokenData.UserId),
                    new Claim(ClaimTypesExtended.AppId, tokenData.AppId)
                }
                .Union(tokenData.Claims)
                .Distinct();

            return await Task.Factory
                .StartNew(() =>
                {
                    var notBeforeAt = DateTime.UtcNow;
                    var expireAt = DateTime.UtcNow.AddHours(this.Options.Jwt.ExpirationInHours);
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Options.Jwt.SecretKey));
                    var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                    var securityToken = new JwtSecurityToken(this.Options.Jwt.Issuer, this.Options.Jwt.Issuer, claims, notBeforeAt, expireAt, signingCredentials);
                    var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            
                    return new AccessToken
                    {
                        AppId = tokenData.AppId,
                        Token = token,
                        ExpireAt = expireAt
                    };
                }, cancellationToken);
        }
        private async Task<AccessToken> GenerateJwtToken(IdentityUser identityUser, string appId, CancellationToken cancellationToken = default)
        {
            if (identityUser == null)
                throw new ArgumentNullException(nameof(identityUser));

            if (appId == null)
                throw new ArgumentNullException(nameof(appId));

            var roles = await this.UserManager
                .GetRolesAsync(identityUser);
            
            var userClaims = await this.UserManager
                .GetClaimsAsync(identityUser);
            
            var roleClaims = roles
                .Select(y => new Claim(ClaimTypes.Role, y));

            var claims = userClaims
                .Union(roleClaims);

            var tokenData = new AccessTokenData
            {
                AppId = appId,
                UserId = identityUser.Id,
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                Claims = claims
            };

            var token = await this.GenerateJwtToken(tokenData, cancellationToken);
            var refreshToken = await this.GenerateJwtRefreshToken(identityUser, appId);

            token.RefreshToken = refreshToken;

            return token;
        }
        private async Task<RefreshToken> GenerateJwtRefreshToken(IdentityUser identityUser, string appId)
        {
            if (appId == null)
                return null;

            var token = StringExtensions.GetRandomToken();

            var removeResult = await this.UserManager
                .RemoveAuthenticationTokenAsync(identityUser, JwtBearerDefaults.AuthenticationScheme, appId);

            if (!removeResult.Succeeded)
                this.ThrowIdentityExceptions(removeResult.Errors);

            var identityUserToken = new IdentityUserTokenExpiry<string>
            {
                UserId = identityUser.Id,
                Name = appId,
                Value = token,
                LoginProvider = JwtBearerDefaults.AuthenticationScheme,
                ExpireAt = DateTimeOffset.UtcNow.AddHours(this.Options.Jwt.RefreshExpirationInHours)
            };

            await this.DbContext
                .AddAsync(identityUserToken);

            await this.DbContext
                .SaveChangesAsync();

            return new RefreshToken
            {
                Token = token,
                ExpireAt = identityUserToken.ExpireAt
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