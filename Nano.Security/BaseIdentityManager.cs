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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Nano.Config;
using Nano.Models.Exceptions;
using Nano.Security.Const;
using Nano.Security.Data.Models;
using Nano.Security.Exceptions;
using Nano.Security.Extensions;
using Nano.Security.Models;
using Newtonsoft.Json;
using Serilog;

namespace Nano.Security
{
    /// <summary>
    /// Base Identity Manager.
    /// </summary>
    public abstract class BaseIdentityManager
    {
        internal const string DEFAULT_APP_ID = "Default";

        /// <summary>
        /// Options.
        /// </summary>
        protected virtual SecurityOptions Options { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">The <see cref="SecurityOptions"/>.</param>
        protected BaseIdentityManager(SecurityOptions options)
        {
            this.Options = options ?? throw new ArgumentNullException(nameof(options));

        }

        /// <summary>
        /// Signs in the admin user statically.
        /// The login is transient, no Identity store is used.
        /// </summary>
        /// <param name="login">The <see cref="Login"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> SignInAdminTransientAsync(Login login, CancellationToken cancellationToken = default)
        {
            if (login == null)
                throw new ArgumentNullException(nameof(login));

            if (string.IsNullOrEmpty(this.Options.User.AdminEmailAddress) || string.IsNullOrEmpty(this.Options.User.AdminEmailAddress))
            {
                throw new UnauthorizedException();
            }

            if (login.Username == this.Options.User.AdminEmailAddress && login.Password == this.Options.User.AdminPassword)
            {
                var tokenData = new AccessTokenData
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = this.Options.User.AdminEmailAddress,
                    UserEmail = this.Options.User.AdminEmailAddress,
                    Claims = new[]
                    {
                        new Claim(ClaimTypes.Role, BuiltInUserRoles.ADMINISTRATOR)
                    }
                };

                return await this.GenerateJwtToken(tokenData, cancellationToken);
            }

            throw new UnauthorizedException();
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

            var externalLoginData = await this.ValidateExternalProviderAccessToken(loginExternalTransient, cancellationToken);

            if (externalLoginData == null)
                throw new UnauthorizedException();

            var claims = loginExternalTransient.Claims
                .Select(x => new Claim(x.Key, x.Value));

            var roleClaims = loginExternalTransient.Roles
                .Select(x => new Claim(ClaimTypes.Role, x));

            var tokenData = new AccessTokenData
            {
                AppId = loginExternalTransient.AppId ?? BaseIdentityManager.DEFAULT_APP_ID,
                UserId = externalLoginData.Id,
                UserName = externalLoginData.Name,
                UserEmail = externalLoginData.Email,
                Claims = claims
                    .Union(roleClaims)
            };

            return await this.GenerateJwtToken(tokenData, cancellationToken);
        }

        /// <summary>
        /// Generate Jwt Token
        /// </summary>
        /// <param name="tokenData">The <see cref="AccessTokenData"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        protected async Task<AccessToken> GenerateJwtToken(AccessTokenData tokenData, CancellationToken cancellationToken = default)
        {
            if (tokenData == null)
                throw new ArgumentNullException(nameof(tokenData));

            var appId = tokenData.AppId ?? BaseIdentityManager.DEFAULT_APP_ID;

            var claims = new Collection<Claim>
                {
                    new(ClaimTypesExtended.AppId, appId),
                    new(JwtRegisteredClaimNames.Jti, tokenData.Id),
                    new(JwtRegisteredClaimNames.Sub, tokenData.UserId),
                    new(JwtRegisteredClaimNames.Name, tokenData.UserName),
                    new(JwtRegisteredClaimNames.Email, tokenData.UserEmail)
                }
                .Union(tokenData.Claims)
                .Distinct();

            return await Task
                .Run(() =>
                {
                    var notBeforeAt = DateTime.UtcNow;
                    var expireAt = DateTime.UtcNow.AddHours(this.Options.Jwt.ExpirationInHours);
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Options.Jwt.SecretKey));
                    var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                    var securityToken = new JwtSecurityToken(this.Options.Jwt.Issuer, this.Options.Jwt.Issuer, claims, notBeforeAt, expireAt, signingCredentials);
                    var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

                    return new AccessToken
                    {
                        AppId = appId,
                        UserId = tokenData.UserId,
                        Token = token,
                        ExpireAt = expireAt
                    };
                }, cancellationToken);
        }

        /// <summary>
        /// Validate External Provider Access Token.
        /// </summary>
        /// <param name="loginExternal">The <see cref="LoginExternal"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ExternalLoginData"/>.</returns>
        protected async Task<ExternalLoginData> ValidateExternalProviderAccessToken(LoginExternalProvider loginExternal, CancellationToken cancellationToken = default)
        {
            if (loginExternal == null)
                throw new ArgumentNullException(nameof(loginExternal));

            var externalLoginOption = this.Options.ExternalLogins
                .FirstOrDefault(x => x.Name == loginExternal.LoginProvider);

            if (externalLoginOption == null)
                throw new NullReferenceException(nameof(externalLoginOption));

            try
            {
                switch (loginExternal.LoginProvider)
                {
                    case "Facebook":
                        using (var client = new HttpClient())
                        {
                            const string HOST = "https://graph.facebook.com";
                            const string FIELDS = "id,name,address,email,birthday";

                            var debugTokenResponse = await client
                                .GetAsync($"{HOST}/debug_token?input_token={loginExternal.AccessToken}&access_token={externalLoginOption.Id}|{externalLoginOption.Secret}", cancellationToken);

                            debugTokenResponse
                                .EnsureSuccessStatusCode();

                            var debugToken = await debugTokenResponse.Content
                                .ReadAsStringAsync(cancellationToken);

                            var validation = JsonConvert.DeserializeObject<dynamic>(debugToken);

                            if (validation == null)
                                throw new NullReferenceException(nameof(validation));

                            if (!(bool)validation.data.is_valid)
                                throw new InvalidOperationException("!validation.data.is_valid");

                            if (validation.data.app_id != externalLoginOption.Id)
                                throw new InvalidOperationException("validation.data.app_id != externalLoginOption.Id");

                            using var userResponse = await client
                                .GetAsync($"{HOST}/{validation.data.user_id}/?fields={FIELDS}&access_token={loginExternal.AccessToken}", cancellationToken);

                            userResponse
                                .EnsureSuccessStatusCode();

                            var user = await userResponse.Content
                                .ReadAsStringAsync(cancellationToken);

                            return JsonConvert.DeserializeObject<ExternalLoginData>(user);
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

                        return new ExternalLoginData
                        {
                            Id = payload.Subject,
                            Name = payload.Name,
                            Email = payload.Email
                        };

                    case "Microsoft":
                        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>("https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
                        var config = await configManager
                            .GetConfigurationAsync(cancellationToken);

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var validationParameters = new TokenValidationParameters
                        {
                            ValidAudience = externalLoginOption.Id,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(externalLoginOption.Secret)),
                            IssuerSigningKeys = config.SigningKeys,
                            ClockSkew = TimeSpan.FromMinutes(5),
                            ValidateIssuerSigningKey = true,
                            ValidateLifetime = true,
                            ValidateAudience = true,
                            ValidateIssuer = false
                        };

                        tokenHandler
                            .ValidateToken(loginExternal.AccessToken, validationParameters, out _);

                        var jwtToken = tokenHandler
                            .ReadJwtToken(loginExternal.AccessToken);

                        var id = jwtToken?.Payload.Where(x => x.Key == "oid").Select(x => x.Value?.ToString()).FirstOrDefault();
                        var name = jwtToken?.Payload.Where(x => x.Key == "name").Select(x => x.Value?.ToString()).FirstOrDefault();
                        var email = jwtToken?.Payload.Where(x => x.Key == "email").Select(x => x.Value?.ToString()).FirstOrDefault();

                        return new ExternalLoginData
                        {
                            Id = id,
                            Name = name,
                            Email = email
                        };

                    default:
                        throw new NotSupportedException(loginExternal.LoginProvider);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);

                throw new UnauthorizedException();
            }
        }
    }

    /// <summary>
    /// Base Identity Manager.
    /// </summary>
    public class BaseIdentityManager<TIdentity> : BaseIdentityManager
        where TIdentity : IEquatable<TIdentity>
    {
        /// <summary>
        /// Db Context.
        /// </summary>
        protected virtual DbContext DbContext { get; }

        /// <summary>
        /// User Manager.
        /// </summary>
        protected virtual UserManager<IdentityUser<TIdentity>> UserManager { get; }

        /// <summary>
        /// Role Manager.
        /// </summary>
        protected virtual RoleManager<IdentityRole<TIdentity>> RoleManager { get; }

        /// <summary>
        /// Sign In Manager.
        /// </summary>
        protected virtual SignInManager<IdentityUser<TIdentity>> SignInManager { get; }

        /// <summary>
        /// The user authenticates and on success recieves a jwt token for use with auhtorization.
        /// </summary>
        /// <param name="dbContext">The <see cref="SignInManager{T}"/>.</param>
        /// <param name="signInManager">The <see cref="SignInManager{T}"/>.</param>
        /// <param name="userManager">The <see cref="UserManager{T}"/>.</param>
        /// <param name="roleManager">The <see cref="RoleManager{T}"/></param>
        /// <param name="options">The <see cref="SecurityOptions"/>.</param>
        protected BaseIdentityManager(DbContext dbContext, SignInManager<IdentityUser<TIdentity>> signInManager, RoleManager<IdentityRole<TIdentity>> roleManager, UserManager<IdentityUser<TIdentity>> userManager, SecurityOptions options)
            : base(options)
        {
            this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
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
                    var appId = login.AppId ?? BaseIdentityManager.DEFAULT_APP_ID;

                    var identityUser = await this.UserManager
                        .FindByNameAsync(login.Username);

                    return await this.GenerateJwtToken(identityUser, appId, login.IsRefreshable, login.TransientClaims, cancellationToken);
                }

                if (result.IsLockedOut)
                    throw new UnauthorizedLockedOutException();

                if (result.RequiresTwoFactor)
                    throw new UnauthorizedTwoFactorRequiredException();
            }
            else
            {
                return await this.SignInAdminTransientAsync(login, cancellationToken);
            }

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

            var externalLoginData = await this.ValidateExternalProviderAccessToken(loginExternal, cancellationToken);

            if (externalLoginData == null)
                throw new UnauthorizedException();

            var identityUser = await this.UserManager
                .FindByLoginAsync(loginExternal.LoginProvider, externalLoginData.Id);

            if (identityUser == null)
                return null;

            var appId = loginExternal.AppId ?? BaseIdentityManager.DEFAULT_APP_ID;

            await this.SignInManager
                .SignInAsync(identityUser, loginExternal.IsRememerMe);

            return await this.GenerateJwtToken(identityUser, appId, loginExternal.IsRefreshable, loginExternal.TransientClaims, cancellationToken);
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

                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new InvalidOperationException();

                var subClaim = principal.Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

                if (subClaim == null)
                    throw new NullReferenceException(nameof(subClaim));

                var identityUser = await this.UserManager
                    .FindByIdAsync(subClaim.Value);

                if (identityUser == null)
                    throw new NullReferenceException(nameof(identityUser));

                var appClaim = principal.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypesExtended.AppId);

                if (appClaim == null)
                    throw new NullReferenceException(nameof(appClaim));

                var identityUserToken = this.DbContext
                    .Set<IdentityUserTokenExpiry<TIdentity>>()
                    .Where(x => x.UserId.Equals(identityUser.Id) && x.Name == appClaim.Value)
                    .AsNoTracking()
                    .FirstOrDefault();

                if (identityUserToken == null)
                    throw new NullReferenceException(nameof(identityUserToken));

                if (identityUserToken.Value != loginRefresh.RefreshToken)
                    throw new InvalidOperationException($"identityUserToken.Value ({identityUserToken.Value}) != loginRefresh.RefreshToken ({loginRefresh.RefreshToken})");

                if (identityUserToken.ExpireAt <= DateTimeOffset.UtcNow)
                    throw new InvalidOperationException("identityUserToken.ExpireAt <= DateTimeOffset.UtcNow");

                return await this.GenerateJwtToken(identityUser, identityUserToken.Name, true, loginRefresh.TransientClaims, cancellationToken);
            }
            catch (Exception ex)
            {
                this.UserManager.Logger.LogError(ex, ex.Message);

                throw new UnauthorizedException();
            }
        }

        /// <summary>
        /// Logs out a user.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task SignOutAsync(CancellationToken cancellationToken = default)
        {
            var username = this.SignInManager.Context
                .GetJwtUserName();

            var user = await this.UserManager
                .FindByNameAsync(username);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            await this.SignInManager
                .SignOutAsync();
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="signUp">The <see cref="SignUp"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IdentityUser"/>.</returns>
        public virtual async Task<IdentityUser<TIdentity>> SignUpAsync(SignUp signUp, CancellationToken cancellationToken = default)
        {
            if (signUp == null)
                throw new ArgumentNullException(nameof(signUp));

            var user = new IdentityUser<TIdentity>
            {
                Email = signUp.EmailAddress,
                UserName = signUp.Username
            };

            var result = await this.UserManager
                .CreateAsync(user, signUp.Password);

            if (!result.Succeeded)
                this.ThrowIdentityExceptions(result.Errors);

            await this.AssignSignUpRolesAndClaims(signUp, user);

            return user;
        }

        /// <summary>
        /// Registers a new user using an external login provider.
        /// </summary>
        /// <param name="signUpExternal">The <see cref="SignUpExternal"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IdentityUser"/>.</returns>
        public virtual async Task<IdentityUser<TIdentity>> SignUpExternalAsync(SignUpExternal signUpExternal, CancellationToken cancellationToken = default)
        {
            if (signUpExternal == null)
                throw new ArgumentNullException(nameof(signUpExternal));

            var externalLoginData = await this.ValidateExternalProviderAccessToken(signUpExternal.ExternalLogin, cancellationToken);

            if (externalLoginData == null)
                throw new UnauthorizedException();

            var user = new IdentityUser<TIdentity>
            {
                Email = externalLoginData.Email,
                UserName = externalLoginData.Email
            };

            var createResult = await this.UserManager
                .CreateAsync(user);

            if (!createResult.Succeeded)
            {
                this.ThrowIdentityExceptions(createResult.Errors);
            }

            var userLoginInfo = new UserLoginInfo(signUpExternal.ExternalLogin.LoginProvider, externalLoginData.Id, signUpExternal.ExternalLogin.LoginProvider);

            var addLoginResult = await this.UserManager
                .AddLoginAsync(user, userLoginInfo);

            if (!addLoginResult.Succeeded)
            {
                this.ThrowIdentityExceptions(addLoginResult.Errors);
            }

            await this.AssignSignUpRolesAndClaims(signUpExternal, user);

            await this.SignInManager
                .SignInAsync(user, signUpExternal.ExternalLogin.IsRememerMe);

            return user;
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

            var user = await this.UserManager
                .FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .RemoveLoginAsync(user, externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);

            if (result.Succeeded)
            {
                await this.SignInManager
                    .RefreshSignInAsync(user);
            }
            else
            {
                this.ThrowIdentityExceptions(result.Errors);
            }
        }

        /// <summary>
        /// Sets a emailAddress for a user.
        /// </summary>
        /// <param name="setUsername">The <see cref="SetUsername{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task SetUsernameAsync(SetUsername<TIdentity> setUsername, CancellationToken cancellationToken = default)
        {
            if (setUsername == null)
                throw new ArgumentNullException(nameof(setUsername));

            var user = await this.UserManager
                .FindByIdAsync(setUsername.UserId.ToString());

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .SetUserNameAsync(user, setUsername.NewUsername);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }
        }

        /// <summary>
        /// Sets a password for a user.
        /// </summary>
        /// <param name="setPassword">The <see cref="SetPassword{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task SetPasswordAsync(SetPassword<TIdentity> setPassword, CancellationToken cancellationToken = default)
        {
            if (setPassword == null)
                throw new ArgumentNullException(nameof(setPassword));

            var user = await this.UserManager
                .FindByIdAsync(setPassword.UserId.ToString());

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var hasPassword = await this.UserManager
                .HasPasswordAsync(user);

            if (hasPassword)
                throw new UnauthorizedSetPasswordException();

            var result = await this.UserManager
                .AddPasswordAsync(user, setPassword.NewPassword);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }

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
                var invalidEmailAddress = new IdentityErrorDescriber().InvalidEmail(resetPassword.EmailAddress);
                throw new TranslationException(invalidEmailAddress.Description);
            }

            var result = await this.UserManager
                .ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }
        }

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="changePassword">The <see cref="ChangePassword{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task ChangePasswordAsync(ChangePassword<TIdentity> changePassword, CancellationToken cancellationToken = default)
        {
            if (changePassword == null)
                throw new ArgumentNullException(nameof(changePassword));

            var user = await this.UserManager
                .FindByIdAsync(changePassword.UserId.ToString());

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .ChangePasswordAsync(user, changePassword.OldPassword, changePassword.NewPassword);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }

            await this.SignInManager
                .RefreshSignInAsync(user);
        }

        /// <summary>
        /// Changes the email address of a user.
        /// </summary>
        /// <param name="changeEmail">The <see cref="ChangeEmail{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task ChangeEmailAsync(ChangeEmail<TIdentity> changeEmail, CancellationToken cancellationToken = default)
        {
            if (changeEmail == null)
                throw new ArgumentNullException(nameof(changeEmail));

            var user = await this.UserManager
                .FindByIdAsync(changeEmail.UserId.ToString());

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .ChangeEmailAsync(user, changeEmail.NewEmailAddress, changeEmail.Token);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }

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
                var invalidEmailAddress = new IdentityErrorDescriber().InvalidEmail(confirmEmail.EmailAddress);
                throw new TranslationException(invalidEmailAddress.Description);
            }

            var result = await this.UserManager
                .ConfirmEmailAsync(user, confirmEmail.Token);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }
        }

        /// <summary>
        /// Changes the phone numberof a user.
        /// </summary>
        /// <param name="changePhoneNumber">The <see cref="ChangePhoneNumber{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task ChangePhoneNumberAsync(ChangePhoneNumber<TIdentity> changePhoneNumber, CancellationToken cancellationToken = default)
        {
            if (changePhoneNumber == null)
                throw new ArgumentNullException(nameof(changePhoneNumber));

            var user = await this.UserManager
                .FindByIdAsync(changePhoneNumber.UserId.ToString());

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .ChangePhoneNumberAsync(user, changePhoneNumber.NewPhoneNumber, changePhoneNumber.Token);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }

            user.PhoneNumberConfirmed = false;

            this.DbContext
                .Update(user);

            await this.DbContext
                .SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Confirms the phone number of a user.
        /// </summary>
        /// <param name="confirmPhoneNumber">The <see cref="ConfirmPhoneNumber"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task ConfirmPhoneNumberAsync(ConfirmPhoneNumber confirmPhoneNumber, CancellationToken cancellationToken = default)
        {
            if (confirmPhoneNumber == null)
                throw new ArgumentNullException(nameof(confirmPhoneNumber));

            var user = await this.UserManager
                .FindByPhoneNumberAsync<IdentityUser<TIdentity>, TIdentity>(confirmPhoneNumber.PhoneNumber);

            if (user == null)
            {
                var invalidPhoneNumber = new IdentityErrorDescriber().InvalidPhoneNumber(confirmPhoneNumber.PhoneNumber);
                throw new TranslationException(invalidPhoneNumber.Description);
            }

            var result = await this.UserManager
                .ConfirmPhoneNumberAsync<IdentityUser<TIdentity>, TIdentity>(user, confirmPhoneNumber.Token);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }
        }

        /// <summary>
        /// Generates an reset password token for a user.
        /// </summary>
        /// <param name="emailAddress">The emailAddress.</param>
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
                var invalidEmailAddress = new IdentityErrorDescriber().InvalidEmail(emailAddress);
                throw new TranslationException(invalidEmailAddress.Description);
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
                var invalidEmailAddress = new IdentityErrorDescriber().InvalidEmail(emailAddress);
                throw new TranslationException(invalidEmailAddress.Description);
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
        /// <param name="emailAddress">The emailAddress.</param>
        /// <param name="newEmailAddress">The new email address.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ChangeEmailToken"/>.</returns>
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
                var invalidEmailAddress = new IdentityErrorDescriber().InvalidEmail(emailAddress);
                throw new TranslationException(invalidEmailAddress.Description);
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

        /// <summary>
        /// Generates an confirm phone number token for a user.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ResetPasswordToken"/>.</returns>
        public virtual async Task<ConfirmPhoneNumberToken> GenerateConfirmPhoneNumberTokenAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            if (phoneNumber == null)
                throw new ArgumentNullException(nameof(phoneNumber));

            var user = await this.UserManager
                .FindByPhoneNumberAsync<IdentityUser<TIdentity>, TIdentity>(phoneNumber);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var token = await this.UserManager
                .GeneratePhoneNumberConfirmationTokenAsync<IdentityUser<TIdentity>, TIdentity>(user);

            return new ConfirmPhoneNumberToken
            {
                Token = token,
                PhoneNumber = phoneNumber
            };
        }

        /// <summary>
        /// Generates an change phone number token for a user.
        /// </summary>
        /// <param name="phoneNumber">The user id.</param>
        /// <param name="newPhoneNumber">The new phone number.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="ResetPasswordToken"/>.</returns>
        public virtual async Task<ChangePhoneNumberToken> GenerateChangePhoneNumberTokenAsync(string phoneNumber, string newPhoneNumber, CancellationToken cancellationToken = default)
        {
            if (phoneNumber == null)
                throw new ArgumentNullException(nameof(phoneNumber));

            if (newPhoneNumber == null)
                throw new ArgumentNullException(nameof(newPhoneNumber));

            var user = await this.UserManager
                .FindByPhoneNumberAsync<IdentityUser<TIdentity>, TIdentity>(phoneNumber);

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var userNew = await this.UserManager
                .FindByPhoneNumberAsync<IdentityUser<TIdentity>, TIdentity>(phoneNumber);

            if (userNew != null)
            {
                var duplicatePhoneNumber = new IdentityErrorDescriber().DuplicatePhoneNumber(newPhoneNumber);
                throw new TranslationException(duplicatePhoneNumber.Description);
            }

            var token = await this.UserManager
                .GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber);

            return new ChangePhoneNumberToken
            {
                Token = token,
                PhoneNumber = phoneNumber,
                NewPhoneNumber = newPhoneNumber
            };
        }

        /// <summary>
        /// Gets all the <see cref="IdentityRole{TIdentity}"/>'s.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IdentityRole{TIdentity}"/>'s.</returns>
        public virtual async Task<IEnumerable<IdentityRole<TIdentity>>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                return this.RoleManager.Roles
                    .OrderBy(x => x.Name);
            }, cancellationToken);
        }

        /// <summary>
        /// Creates a <see cref="IdentityRole{TIdentity}"/>.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IdentityRole{TIdentity}"/>.</returns>
        public virtual async Task<IdentityRole<TIdentity>> CreateRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            if (roleName == null)
                throw new ArgumentNullException(nameof(roleName));

            var identityRole = new IdentityRole<TIdentity>(roleName);

            var result = await this.RoleManager
                .CreateAsync(identityRole);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }

            return identityRole;
        }

        /// <summary>
        /// Deletes a <see cref="IdentityRole{TIdentity}"/>.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default)
        {
            if (roleName == null)
                throw new ArgumentNullException(nameof(roleName));

            var role = await this.RoleManager
                .FindByNameAsync(roleName);

            if (role == null)
                throw new NullReferenceException(nameof(role));

            var result = await this.RoleManager
                .DeleteAsync(role);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }
        }

        /// <summary>
        /// Gets the roles of a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The role names.</returns>
        public virtual async Task<IEnumerable<string>> GetUserRolesAsync(TIdentity userId, CancellationToken cancellationToken = default)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            var user = await this.UserManager
                .FindByIdAsync(userId.ToString());

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var roles = await this.UserManager
                .GetRolesAsync(user);

            return roles;
        }

        /// <summary>
        /// Assign a role to a user.
        /// </summary>
        /// <param name="assignRole">The <see cref="AssignRole{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task AssignUserRoleAsync(AssignRole<TIdentity> assignRole, CancellationToken cancellationToken = default)
        {
            if (assignRole == null)
                throw new ArgumentNullException(nameof(assignRole));

            var user = await this.UserManager
                .FindByIdAsync(assignRole.UserId.ToString());

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .AddToRoleAsync(user, assignRole.RoleName);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }
        }

        /// <summary>
        /// Removes a role from a user.
        /// </summary>
        /// <param name="removeRole">The <see cref="RemoveRole{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task RemoveUserRoleAsync(RemoveRole<TIdentity> removeRole, CancellationToken cancellationToken = default)
        {
            if (removeRole == null)
                throw new ArgumentNullException(nameof(removeRole));

            var user = await this.UserManager
                .FindByIdAsync(removeRole.UserId.ToString());

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var result = await this.UserManager
                .RemoveFromRoleAsync(user, removeRole.RoleName);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }
        }

        /// <summary>
        /// Gets the <see cref="Claim"/> of a user.
        /// </summary>
        /// <param name="getClaim">The <see cref="GetClaim{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="Claim"/>.</returns>
        public virtual async Task<Claim> GetUserClaimAsync(GetClaim<TIdentity> getClaim, CancellationToken cancellationToken = default)
        {
            if (getClaim == null)
                throw new ArgumentNullException(nameof(getClaim));

            var claims = await this.GetUserClaimsAsync(getClaim.Id, cancellationToken);

            return claims
                .FirstOrDefault(x => x.Type == getClaim.ClaimType);
        }

        /// <summary>
        /// Gets the <see cref="Claim"/>'s of a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="Claim"/>'s.</returns>
        public virtual async Task<IEnumerable<Claim>> GetUserClaimsAsync(TIdentity userId, CancellationToken cancellationToken = default)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            var user = await this.UserManager
                .FindByIdAsync(userId.ToString());

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var claims = await this.UserManager
                .GetClaimsAsync(user);

            return claims;
        }

        /// <summary>
        /// Assigns a <see cref="IdentityUserClaim{TIdentity}"/> to a user.
        /// </summary>
        /// <param name="assignClaim">The <see cref="AssignClaim{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
        public virtual async Task<IdentityUserClaim<TIdentity>> AssignUserClaimAsync(AssignClaim<TIdentity> assignClaim, CancellationToken cancellationToken = default)
        {
            if (assignClaim == null)
                throw new ArgumentNullException(nameof(assignClaim));

            var user = await this.UserManager
                .FindByIdAsync(assignClaim.Id.ToString());

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var userClaim = new IdentityUserClaim<TIdentity>
            {
                ClaimType = assignClaim.ClaimType, 
                ClaimValue = assignClaim.ClaimValue
            };

            var claim = userClaim
                .ToClaim();

            var result = await this.UserManager
                .AddClaimAsync(user, claim);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }

            return userClaim;
        }

        /// <summary>
        /// Removes a <see cref="IdentityUserClaim{TIdentity}"/> from a user.
        /// </summary>
        /// <param name="removeClaim">The <see cref="RemoveClaim{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task RemoveUserClaimAsync(RemoveClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default)
        {
            if (removeClaim == null)
                throw new ArgumentNullException(nameof(removeClaim));

            var user = await this.UserManager
                .FindByIdAsync(removeClaim.Id.ToString());

            if (user == null)
                throw new NullReferenceException(nameof(user));

            var claims = await this.UserManager
                .GetClaimsAsync(user);

            var claim = claims
                .FirstOrDefault(x => x.Type == removeClaim.ClaimType);

            if (claim == null)
                throw new NullReferenceException(nameof(claim));

            var result = await this.UserManager
                .RemoveClaimAsync(user, claim);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }
        }

        /// <summary>
        /// Gets the <see cref="Claim"/> of a role.
        /// </summary>
        /// <param name="getClaim">The <see cref="GetClaim{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="Claim"/>.</returns>
        public virtual async Task<Claim> GetRoleClaimAsync(GetClaim<TIdentity> getClaim, CancellationToken cancellationToken = default)
        {
            if (getClaim == null)
                throw new ArgumentNullException(nameof(getClaim));

            var claims = await this.GetRoleClaimsAsync(getClaim.Id, cancellationToken);

            return claims
                .FirstOrDefault(x => x.Type == getClaim.ClaimType);
        }

        /// <summary>
        /// Gets the <see cref="Claim"/>'s of a role.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="Claim"/>'s.</returns>
        public virtual async Task<IEnumerable<Claim>> GetRoleClaimsAsync(TIdentity roleId, CancellationToken cancellationToken = default)
        {
            if (roleId == null)
                throw new ArgumentNullException(nameof(roleId));

            var role = await this.RoleManager
                .FindByIdAsync(roleId.ToString());

            if (role == null)
                throw new NullReferenceException(nameof(role));

            var claims = await this.RoleManager
                .GetClaimsAsync(role);

            return claims;
        }

        /// <summary>
        /// Assigns a <see cref="IdentityRoleClaim{TIdentity}"/> to a role.
        /// </summary>
        /// <param name="assignClaim">The <see cref="AssignClaim{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
        public virtual async Task<IdentityRoleClaim<TIdentity>> AssignRoleClaimAsync(AssignClaim<TIdentity> assignClaim, CancellationToken cancellationToken = default)
        {
            if (assignClaim == null)
                throw new ArgumentNullException(nameof(assignClaim));

            var role = await this.RoleManager
                .FindByIdAsync(assignClaim.Id.ToString());

            if (role == null)
                throw new NullReferenceException(nameof(role));

            var roleClaim = new IdentityRoleClaim<TIdentity>
            {
                ClaimType = assignClaim.ClaimType,
                ClaimValue = assignClaim.ClaimValue
            };

            var claim = roleClaim
                .ToClaim();

            var result = await this.RoleManager
                .AddClaimAsync(role, claim);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }

            return roleClaim;
        }

        /// <summary>
        /// Removes a <see cref="IdentityRoleClaim{TIdentity}"/> from a role.
        /// </summary>
        /// <param name="removeClaim">The <see cref="RemoveClaim{TIdentity}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task RemoveRoleClaimAsync(RemoveClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default)
        {
            if (removeClaim == null)
                throw new ArgumentNullException(nameof(removeClaim));

            var role = await this.RoleManager
                .FindByIdAsync(removeClaim.Id.ToString());

            if (role == null)
                throw new NullReferenceException(nameof(role));

            var claims = await this.RoleManager
                .GetClaimsAsync(role);

            var claim = claims
                .FirstOrDefault(x => x.Type == removeClaim.ClaimType);

            if (claim == null)
                throw new NullReferenceException(nameof(claim));

            var result = await this.RoleManager
                .RemoveClaimAsync(role, claim);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }
        }

        /// <summary>
        /// Deletes the <see cref="IdentityUser"/>.
        /// </summary>
        /// <param name="identityUser">The <see cref="IdentityUser"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void.</returns>
        public virtual async Task DeleteIdentityUser(IdentityUser<TIdentity> identityUser, CancellationToken cancellationToken = default)
        {
            if (identityUser == null)
                throw new ArgumentNullException(nameof(identityUser));

            var result = await this.UserManager
                .DeleteAsync(identityUser);

            if (!result.Succeeded)
            {
                this.ThrowIdentityExceptions(result.Errors);
            }
        }

        private async Task AssignSignUpRolesAndClaims(BaseSignUp signUp, IdentityUser<TIdentity> identityUser)
        {
            if (signUp == null)
                throw new ArgumentNullException(nameof(signUp));

            if (identityUser == null)
                throw new ArgumentNullException(nameof(identityUser));

            var roles = signUp.Roles
                .Union(this.Options.User.DefaultRoles)
                .Distinct();

            var roleAssignResult = await this.UserManager
                .AddToRolesAsync(identityUser, roles);

            if (!roleAssignResult.Succeeded)
                this.ThrowIdentityExceptions(roleAssignResult.Errors);

            if (signUp.Claims.Any())
            {
                var claims = signUp.Claims
                    .Select(x => new Claim(x.Key, x.Value));

                var claimAssignResult = await this.UserManager
                    .AddClaimsAsync(identityUser, claims);

                if (!claimAssignResult.Succeeded)
                    this.ThrowIdentityExceptions(claimAssignResult.Errors);
            }
        }
        private async Task<AccessToken> GenerateJwtToken(IdentityUser<TIdentity> identityUser, string appId, bool isRefreshable, IDictionary<string, string> transientClaims, CancellationToken cancellationToken = default)
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
                .Union(roleClaims)
                .Union(transientClaims.Select(x => new Claim(x.Key, x.Value)));

            var tokenData = new AccessTokenData
            {
                AppId = appId,
                UserId = identityUser.Id.ToString(),
                UserName = identityUser.UserName,
                UserEmail = identityUser.Email,
                Claims = claims
            };

            var token = await this.GenerateJwtToken(tokenData, cancellationToken);

            if (isRefreshable)
            {
                var refreshToken = await this.GenerateJwtRefreshToken(identityUser, appId);

                token.RefreshToken = refreshToken;
            }

            return token;
        }
        private async Task<RefreshToken> GenerateJwtRefreshToken(IdentityUser<TIdentity> identityUser, string appId)
        {
            if (appId == null)
                return null;

            var token = StringExtensions.GetRandomToken();

            var removeResult = await this.UserManager
                .RemoveAuthenticationTokenAsync(identityUser, JwtBearerDefaults.AuthenticationScheme, appId);

            if (!removeResult.Succeeded)
            {
                this.ThrowIdentityExceptions(removeResult.Errors);
            }

            var identityUserToken = new IdentityUserTokenExpiry<TIdentity>
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