using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Nano.Config;
using Nano.Models.Exceptions;
using Nano.Models.Interfaces;
using Nano.Security.Const;
using Nano.Security.Exceptions;
using Nano.Security.Extensions;
using Nano.Security.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Nano.Models.Extensions;
using System.Text.Json.Nodes;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Nano.Models.Data;

namespace Nano.Security;

/// <summary>
/// Base Identity Manager.
/// </summary>
public abstract class BaseIdentityManager
{
    internal const string DEFAULT_APP_ID = "Default";

    /// <summary>
    /// Logger.
    /// </summary>
    protected virtual ILogger Logger { get; }

    /// <summary>
    /// Options.
    /// </summary>
    protected virtual SecurityOptions Options { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="options">The <see cref="SecurityOptions"/>.</param>
    protected BaseIdentityManager(ILogger logger, SecurityOptions options)
    {
        this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Signs in the admin user statically.
    /// The login is transient, no Identity store is used.
    /// </summary>
    /// <param name="logIn">The <see cref="LogIn"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual Task<AccessToken> SignInAdminTransientAsync(LogIn logIn, CancellationToken cancellationToken = default)
    {
        if (logIn == null)
        {
            throw new ArgumentNullException(nameof(logIn));
        }

        if (this.Options.User.AdminEmailAddress == null)
        {
            throw new NullReferenceException(nameof(this.Options.User.AdminEmailAddress));
        }

        if (this.Options.User.AdminPassword == null)
        {
            throw new NullReferenceException(nameof(this.Options.User.AdminPassword));
        }

        if (logIn.Username != this.Options.User.AdminEmailAddress || logIn.Password != this.Options.User.AdminPassword)
        {
            throw new UnauthorizedException($"The user: {logIn.Username} failed with incorrect username or password.");
        }

        var tokenData = new AccessTokenData
        {
            UserId = Guid.NewGuid().ToString(),
            UserName = this.Options.User.AdminEmailAddress,
            UserEmail = this.Options.User.AdminEmailAddress,
            Claims =
            [
                new Claim(ClaimTypes.Role, BuiltInUserRoles.ADMINISTRATOR)
            ]
        };

        var accessToken = this.GenerateJwtToken(tokenData);

        return Task.FromResult(accessToken);
    }

    /// <summary>
    /// Signs in a user, from external login.
    /// The login is transient, no Identity backing store is used.
    /// The login relies on the external login provider being valid.
    /// </summary>
    /// <typeparam name="TProvider">The provider type.</typeparam>
    /// <param name="logInExternalTransient">The <see cref="BaseLogInExternal{T}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual async Task<AccessToken> SignInExternalTransientAsync<TProvider>(BaseLogInExternal<TProvider> logInExternalTransient, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        if (logInExternalTransient == null)
        {
            throw new ArgumentNullException(nameof(logInExternalTransient));
        }

        var externalLoginData = await this.GetExternalProviderLogInData(logInExternalTransient.Provider, cancellationToken);

        return await this.SignInExternalTransientAsync(externalLoginData, logInExternalTransient.TransientRoles, logInExternalTransient.TransientClaims, cancellationToken);
    }

    /// <summary>
    /// Signs in a user, from external login.
    /// The login is transient, no Identity backing store is used.
    /// The login relies on the external login provider being valid.
    /// </summary>
    /// <param name="externalLogInData">The <see cref="ExternalLogInData"/>.</param>
    /// <param name="transientRoles">The roles added to the token.</param>
    /// <param name="transientClaims">The claims added to the token.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual Task<AccessToken> SignInExternalTransientAsync(ExternalLogInData externalLogInData, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default)
    {
        if (externalLogInData == null)
        {
            throw new ArgumentNullException(nameof(externalLogInData));
        }

        var claims = transientClaims?
            .Select(x => new Claim(x.Key, x.Value)) ?? new List<Claim>();

        var roleClaims = transientRoles?
            .Select(x => new Claim(ClaimTypes.Role, x)) ?? new List<Claim>();

        var tokenData = new AccessTokenData
        {
            AppId = BaseIdentityManager.DEFAULT_APP_ID,
            UserId = externalLogInData.Id,
            UserName = externalLogInData.Name,
            UserEmail = externalLogInData.Email,
            ExternalToken = externalLogInData.ExternalToken,
            Claims = claims
                .Union(roleClaims)
        };

        var jwtToken = this.GenerateJwtToken(tokenData);

        return Task.FromResult(jwtToken);
    }

    /// <summary>
    /// Validate External Provider Access Token.
    /// </summary>
    /// <typeparam name="TProvider">The provider type.</typeparam>
    /// <param name="logInExternalProvider">The <see cref="object"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ExternalLogInData"/>.</returns>
    public virtual async Task<ExternalLogInData> GetExternalProviderLogInData<TProvider>(TProvider logInExternalProvider, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider
    {
        if (logInExternalProvider == null)
        {
            throw new ArgumentNullException(nameof(logInExternalProvider));
        }

        try
        {
            return logInExternalProvider.Name switch
            {
                "Google" => await this.GetExternalProviderLoginDataGoogle(logInExternalProvider, this.Options.ExternalLogins.Google),
                "Facebook" => await this.GetExternalProviderLoginDataFacebook(logInExternalProvider, this.Options.ExternalLogins.Facebook, cancellationToken),
                "Microsoft" => await this.GetExternalProviderLoginDataMicrosoft(logInExternalProvider, this.Options.ExternalLogins.Microsoft, cancellationToken),
                _ => throw new NotSupportedException(logInExternalProvider.Name)
            };
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, ex.Message);

            throw new UnauthorizedException();
        }
    }

    /// <summary>
    /// Get Pasword Options.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual Task<SecurityOptions.PasswordOptions> GetPaswordOptionsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(this.Options.Password);
    }

    /// <summary>
    /// Generate Jwt Token
    /// </summary>
    /// <param name="tokenData">The <see cref="AccessTokenData"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    protected AccessToken GenerateJwtToken(AccessTokenData tokenData)
    {
        if (tokenData == null)
        {
            throw new ArgumentNullException(nameof(tokenData));
        }

        if (this.Options.Jwt.PrivateKey == null)
        {
            return null;
        }

        var appId = tokenData.AppId ?? BaseIdentityManager.DEFAULT_APP_ID;

        var claims = new Collection<Claim>
            {
                new(ClaimTypesExtended.AppId, appId),
                new(JwtRegisteredClaimNames.Jti, tokenData.Id),
                new(JwtRegisteredClaimNames.Sub, tokenData.UserId),
                new(JwtRegisteredClaimNames.Name, tokenData.UserName),
                new(JwtRegisteredClaimNames.Email, tokenData.UserEmail),
                new(ClaimTypesExtended.ExternalProviderName, tokenData.ExternalToken.Name ?? string.Empty),
                new(ClaimTypesExtended.ExternalProviderToken, tokenData.ExternalToken.Token ?? string.Empty),
                new(ClaimTypesExtended.ExternalProviderRefreshToken, tokenData.ExternalToken.RefreshToken ?? string.Empty)
            }
            .Union(tokenData.Claims)
            .Distinct();

        var notBeforeAt = DateTimeOffset.UtcNow;
        var expireAt = DateTimeOffset.UtcNow.AddMinutes(this.Options.Jwt.ExpirationInMinutes);

        var securityKey = RSA.Create();
        var privateKey = Convert.FromBase64String(this.Options.Jwt.PrivateKey);

        securityKey
            .ImportRSAPrivateKey(privateKey, bytesRead: out _);

        var rsaSecurityKey = new RsaSecurityKey(securityKey);

        var signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha512);
        var securityToken = new JwtSecurityToken(this.Options.Jwt.Issuer, this.Options.Jwt.Issuer, claims, notBeforeAt.DateTime, expireAt.DateTime, signingCredentials);
        var token = new JwtSecurityTokenHandler()
            .WriteToken(securityToken);

        return new AccessToken
        {
            AppId = appId,
            UserId = tokenData.UserId,
            Token = token,
            ExpireAt = expireAt
        };
    }

    private async Task<ExternalLogInData> GetExternalProviderLoginDataGoogle<TProvider>(TProvider logInExternalProvider, SecurityOptions.ExternalLoginOptions.GoogleOptions externalLoginOptions)
        where TProvider : BaseLogInExternalProvider
    {
        if (logInExternalProvider == null)
        {
            throw new ArgumentNullException(nameof(logInExternalProvider));
        }

        if (externalLoginOptions == null)
        {
            throw new ArgumentNullException(nameof(externalLoginOptions));
        }

        switch (logInExternalProvider)
        {
            case ExternalLoginProviderImplicit implicitLogin:
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience =
                    [
                        externalLoginOptions.ClientId
                    ]
                };

                var payload = await GoogleJsonWebSignature
                    .ValidateAsync(implicitLogin.AccessToken, settings);

                return new ExternalLogInData
                {
                    Id = payload.Subject,
                    Name = payload.Name,
                    Email = payload.Email,
                    ExternalToken =
                    {
                        Name = "Google",
                        Token = implicitLogin.AccessToken
                    }
                };

            default:
                throw new NotSupportedException(logInExternalProvider.GetType().Name);
        }
    }
    private async Task<ExternalLogInData> GetExternalProviderLoginDataFacebook<TProvider>(TProvider logInExternalProvider, SecurityOptions.ExternalLoginOptions.FacebookOptions externalLoginOptions, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider
    {
        if (logInExternalProvider == null)
        {
            throw new ArgumentNullException(nameof(logInExternalProvider));
        }

        if (externalLoginOptions == null)
        {
            throw new ArgumentNullException(nameof(externalLoginOptions));
        }

        switch (logInExternalProvider)
        {
            case ExternalLoginProviderImplicit implicitLogin:
                using (var httpClient = new HttpClient())
                {
                    const string HOST = "https://graph.facebook.com";
                    const string FIELDS = "id,name,address,email,birthday";

                    var debugTokenResponse = await httpClient
                        .GetAsync($"{HOST}/debug_token?input_token={implicitLogin.AccessToken}&access_token={externalLoginOptions.AppId}|{externalLoginOptions.AppSecret}", cancellationToken);

                    debugTokenResponse
                        .EnsureSuccessStatusCode();

                    var debugToken = await debugTokenResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    var validation = JsonSerializer.Deserialize<dynamic>(debugToken);

                    if (validation == null)
                    {
                        throw new NullReferenceException(nameof(validation));
                    }

                    if (!(bool)validation.data.is_valid)
                    {
                        throw new InvalidOperationException("!validation.data.is_valid");
                    }

                    if (validation.data.app_id != externalLoginOptions.AppId)
                    {
                        throw new InvalidOperationException("validation.data.app_id != externalLoginOption.Id");
                    }

                    using var userResponse = await httpClient
                        .GetAsync($"{HOST}/{validation.data.user_id}/?fields={FIELDS}&access_token={implicitLogin.AccessToken}", cancellationToken);

                    userResponse
                        .EnsureSuccessStatusCode();

                    var user = await userResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    var externalLoginData = JsonSerializer.Deserialize<ExternalLogInData>(user);
                    if (externalLoginData != null)
                    {
                        externalLoginData.ExternalToken = new ExternalLoginTokenData
                        {
                            Name = "Facebook",
                            Token = implicitLogin.AccessToken
                        };
                    }

                    return externalLoginData;
                }

            default:
                throw new NotSupportedException(logInExternalProvider.GetType().Name);
        }
    }
    private async Task<ExternalLogInData> GetExternalProviderLoginDataMicrosoft<TProvider>(TProvider logInExternalProvider, SecurityOptions.ExternalLoginOptions.MicrosoftOptions externalLoginOptions, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider
    {
        if (logInExternalProvider == null)
        {
            throw new ArgumentNullException(nameof(logInExternalProvider));
        }

        if (externalLoginOptions == null)
        {
            throw new ArgumentNullException(nameof(externalLoginOptions));
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        string accessToken;
        string refreshToken;

        switch (logInExternalProvider)
        {
            case ExternalLoginProviderAuthCode authCodeLogin:
                using (var httpClient = new HttpClient())
                {
                    var httpRequestMessage = new HttpRequestMessage();

                    httpRequestMessage.Method = HttpMethod.Post;
                    httpRequestMessage.RequestUri = new Uri($"https://login.microsoftonline.com/{externalLoginOptions.TenantId}/oauth2/v2.0/token");

                    using var formContent = new MultipartFormDataContent();
                    {
                        formContent.Add(new StringContent(externalLoginOptions.ClientId), "client_id");
                        formContent.Add(new StringContent(externalLoginOptions.ClientSecret), "client_secret");
                        formContent.Add(new StringContent("authorization_code"), "grant_type");
                        formContent.Add(new StringContent(authCodeLogin.Code), "code");
                        formContent.Add(new StringContent(authCodeLogin.CodeVerifier), "code_verifier");
                        formContent.Add(new StringContent(authCodeLogin.RedirectUri), "redirect_uri");
                        formContent.Add(new StringContent(externalLoginOptions.Scopes.Aggregate(string.Empty, (current, x) => current + $"{x} ")), "scope");

                        httpRequestMessage.Content = formContent;

                        var httpResponse = await httpClient
                            .SendAsync(httpRequestMessage, cancellationToken);

                        var stringContent = await httpResponse.Content
                            .ReadAsStringAsync(cancellationToken);

                        var content = JsonSerializer.Deserialize<JsonObject>(stringContent);

                        var error = (string)content?["error"];
                        var errorDescription = (string)content?["error"];

                        if (error != null)
                        {
                            throw new InvalidOperationException($"{error}: {errorDescription}");
                        }

                        accessToken = (string)content?["access_token"];
                        refreshToken = (string)content?["refresh_token"];
                    }
                }
                break;

            default:
                throw new NotSupportedException(logInExternalProvider.GetType().Name);
        }

        var jwtToken = tokenHandler
            .ReadJwtToken(accessToken);

        var id = jwtToken?.Payload.Where(x => x.Key == "oid").Select(x => x.Value?.ToString()).FirstOrDefault();
        var name = jwtToken?.Payload.Where(x => x.Key == "name").Select(x => x.Value?.ToString()).FirstOrDefault();
        var email = jwtToken?.Payload.Where(x => x.Key == "upn").Select(x => x.Value?.ToString()).FirstOrDefault();

        return new ExternalLogInData
        {
            Id = id,
            Name = name,
            Email = email,
            ExternalToken =
            {
                Name = "Microsoft",
                Token = accessToken,
                RefreshToken = refreshToken
            }
        };
    }
}

/// <summary>
/// Base Identity Manager.
/// </summary>
public abstract class BaseIdentityManager<TIdentity> : BaseIdentityManager
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
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="dbContext">The <see cref="SignInManager{T}"/>.</param>
    /// <param name="signInManager">The <see cref="SignInManager{T}"/>.</param>
    /// <param name="userManager">The <see cref="UserManager{T}"/>.</param>
    /// <param name="roleManager">The <see cref="RoleManager{T}"/></param>
    /// <param name="options">The <see cref="SecurityOptions"/>.</param>
    protected BaseIdentityManager(ILogger logger, DbContext dbContext, SignInManager<IdentityUser<TIdentity>> signInManager, RoleManager<IdentityRole<TIdentity>> roleManager, UserManager<IdentityUser<TIdentity>> userManager, SecurityOptions options)
        : base(logger, options)
    {
        this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        this.RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    /// <summary>
    /// Gets the identity user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUser{TIdentity}"/>.</returns>
    public virtual Task<IdentityUser<TIdentity>> GetIdentityUserAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        var identityUser = this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return identityUser;
    }

    /// <summary>
    /// Gets the identity user or default (null).
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUser{TIdentity}"/>.</returns>
    public virtual async Task<IdentityUser<TIdentity>> GetIdentityUserOrDefaultAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await this.UserManager
                .GetIdentityUserAsync(userId, cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Signs in a user.
    /// </summary>
    /// <param name="logIn">The <see cref="LogIn"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual async Task<AccessToken> SignInAsync(LogIn logIn, CancellationToken cancellationToken = default)
    {
        if (logIn == null)
        {
            throw new ArgumentNullException(nameof(logIn));
        }

        if (!ConfigManager.HasDbContext)
        {
            return await this.SignInAdminTransientAsync(logIn, cancellationToken);
        }

        var result = await this.SignInManager
            .PasswordSignInAsync(logIn.Username, logIn.Password, logIn.IsRememberMe, this.Options.Lockout.AllowedForNewUsers);

        if (result.Succeeded)
        {
            var appId = logIn.AppId ?? BaseIdentityManager.DEFAULT_APP_ID;

            var identityUser = await this.UserManager
                .FindByNameAsync(logIn.Username);

            if (identityUser == null)
            {
                throw new UnauthorizedException($"The user: {logIn.Username} was not found or is deactivated.");
            }

            return await this.GenerateJwtToken(identityUser, appId, logIn.IsRefreshable, null, null, null, logIn.TransientClaims, logIn.TransientRoles, cancellationToken);
        }

        if (result.IsLockedOut)
        {
            throw new UnauthorizedException($"The user: {logIn.Username} is locked out.");
        }

        if (result.IsNotAllowed)
        {
            throw new UnauthorizedException($"The user: {logIn.Username} is not allowed to login.");
        }

        if (result.RequiresTwoFactor)
        {
            throw new UnauthorizedException($"The user: {logIn.Username} requires two-factor authentication.");
        }

        throw new UnauthorizedException($"An unknwon error occured, when trying to login user: {logIn.Username}.");
    }

    /// <summary>
    /// Signs in a user, from external login.
    /// </summary>
    /// <typeparam name="TProvider">The provider type.</typeparam>
    /// <param name="logInExternal">The <see cref="BaseLogInExternal{T}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>the <see cref="AccessToken"/>.</returns>
    public virtual async Task<AccessToken> SignInExternalAsync<TProvider>(BaseLogInExternal<TProvider> logInExternal, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        if (logInExternal == null)
        {
            throw new ArgumentNullException(nameof(logInExternal));
        }

        var externalLoginData = await this.GetExternalProviderLogInData(logInExternal.Provider, cancellationToken);

        var logInExternalData = new LogInExternalDirect
        {
            AppId = logInExternal.AppId,
            IsRefreshable = logInExternal.IsRefreshable,
            IsRememberMe = logInExternal.IsRememberMe,
            TransientRoles = logInExternal.TransientRoles,
            TransientClaims = logInExternal.TransientClaims,
            ExternalLogInData = externalLoginData
        };

        return await this.SignInExternalAsync(logInExternalData, cancellationToken);
    }

    /// <summary>
    /// Signs in a user, from external login.
    /// </summary>
    /// <param name="logInExternalDirect">The <see cref="LogInExternalDirect"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual async Task<AccessToken> SignInExternalAsync(LogInExternalDirect logInExternalDirect, CancellationToken cancellationToken = default)
    {
        if (logInExternalDirect == null)
        {
            throw new UnauthorizedException();
        }

        var identityUser = await this.UserManager
            .FindByLoginAsync(logInExternalDirect.ExternalLogInData.ExternalToken.Name, logInExternalDirect.ExternalLogInData.Id);

        if (identityUser == null)
        {
            throw new UnauthorizedException($"The user: {logInExternalDirect.ExternalLogInData.Email} was not found or is deactivated.");
        }

        var appId = logInExternalDirect.AppId ?? BaseIdentityManager.DEFAULT_APP_ID;

        await this.SignInManager
            .SignInAsync(identityUser, logInExternalDirect.IsRememberMe);

        return await this.GenerateJwtToken(identityUser, appId, logInExternalDirect.IsRefreshable, logInExternalDirect.ExternalLogInData.ExternalToken.Name, logInExternalDirect.ExternalLogInData.ExternalToken.Token, logInExternalDirect.ExternalLogInData.ExternalToken.RefreshToken, logInExternalDirect.TransientClaims, logInExternalDirect.TransientRoles, cancellationToken);
    }

    /// <summary>
    /// Gets all the configured external logins schemes.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ExternalLoginProvider"/>'s.</returns>
    public virtual async Task<IEnumerable<ExternalLoginProvider>> GetExternalProviderSchemesAsync(CancellationToken cancellationToken = default)
    {
        var schemes = await this.SignInManager
            .GetExternalAuthenticationSchemesAsync();

        return schemes
            .Select(x => new ExternalLoginProvider
            {
                Name = x.Name,
                DisplayName = x.DisplayName
            });
    }

    /// <summary>
    /// Refresh the login of a user.
    /// </summary>
    /// <param name="logInRefresh">The <see cref="LogInRefresh"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    public virtual async Task<AccessToken> SignInRefreshAsync(LogInRefresh logInRefresh, CancellationToken cancellationToken = default)
    {
        if (logInRefresh == null)
        {
            throw new ArgumentNullException(nameof(logInRefresh));
        }

        var securityKey = RSA.Create();
        var privateKey = Convert.FromBase64String(this.Options.Jwt.PublicKey);

        securityKey
            .ImportRSAPublicKey(privateKey, bytesRead: out _);

        var rsaSecurityKey = new RsaSecurityKey(securityKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = this.Options.Jwt.Issuer,
            ValidAudience = this.Options.Jwt.Audience,
            IssuerSigningKey = rsaSecurityKey,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        var securityTokenHandler = new JwtSecurityTokenHandler();
        var principal = securityTokenHandler
            .ValidateToken(logInRefresh.Token, validationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha512, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedAccessException("The security token is invalid.");
        }

        var subClaim = principal.Claims
            .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

        if (subClaim == null)
        {
            throw new UnauthorizedAccessException("Claim 'Sub' was not found in the jwt-token.");
        }

        var identityUser = await this.UserManager
            .FindByIdAsync(subClaim.Value);

        if (identityUser == null)
        {
            var username = principal.Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name);

            throw new UnauthorizedException($"The user: {username} was not found or is deactivated.");
        }

        var appClaim = principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypesExtended.AppId);

        var appName = appClaim?.Value ?? BaseIdentityManager.DEFAULT_APP_ID;

        var identityUserToken = this.DbContext
            .Set<IdentityUserTokenExpiry<TIdentity>>()
            .Where(x => x.UserId.Equals(identityUser.Id) && x.Name == appName)
            .AsNoTracking()
            .FirstOrDefault();

        if (identityUserToken == null)
        {
            throw new UnauthorizedException($"The refresh token of user: {identityUser.UserName} could not be found.");
        }

        if (identityUserToken.Value != logInRefresh.RefreshToken)
        {
            throw new UnauthorizedException($"The refresh token of user: {identityUser.UserName} is invalid.");
        }

        if (identityUserToken.ExpireAt <= DateTimeOffset.UtcNow)
        {
            throw new UnauthorizedException($"The refresh token of user: {identityUser.UserName} is expired.");
        }

        var externalProviderName = principal.Claims
            .Where(x => x.Type == ClaimTypesExtended.ExternalProviderName)
            .Select(x => x.Value)
            .FirstOrDefault();

        var externalProviderRefreshToken = principal.Claims
            .Where(x => x.Type == ClaimTypesExtended.ExternalProviderRefreshToken)
            .Select(x => x.Value)
            .FirstOrDefault();

        var externalProviderData = await this.RefreshExternalProviderTokenOrDefault(externalProviderName, externalProviderRefreshToken, cancellationToken);

        return await this.GenerateJwtToken(identityUser, identityUserToken.Name, true, externalProviderData.Name, externalProviderData.Token, externalProviderData.RefreshToken, logInRefresh.TransientClaims, logInRefresh.TransientRoles, cancellationToken);
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

        var userId = this.SignInManager.Context
            .GetJwtUserId();

        if (userId == null)
        {
            return;
        }

        var dbSet = this.DbContext
            .Set<IdentityUserTokenExpiry<TIdentity>>();

        var refreshTokens = dbSet
            .Where(x => x.UserId.Equals(userId.Value) && x.Name == appId);

        if (refreshTokens.Any())
        {
            foreach (var identityUserTokenExpiry in refreshTokens)
            {
                this.DbContext
                    .Remove(identityUserTokenExpiry);
            }

            await this.DbContext
                .SaveChangesAsync(cancellationToken);
        }

        await this.SignInManager
            .SignOutAsync();
    }

    /// <summary>
    /// Is Email Address Taken.
    /// </summary>
    /// <param name="emailAddress">The email address.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IsEmailAddressTaken"/>.</returns>
    public virtual async Task<IsEmailAddressTaken> IsEmailAddressTakenAsync(string emailAddress, CancellationToken cancellationToken = default)
    {
        if (emailAddress == null)
            throw new ArgumentNullException(nameof(emailAddress));

        var existingIdentityUser = await this.UserManager
            .FindByEmailAsync(emailAddress);

        return new IsEmailAddressTaken
        {
            IsTaken = existingIdentityUser != null
        };
    }

    /// <summary>
    /// Is Phone Number Taken.
    /// </summary>
    /// <param name="phoneNumber">The phone number.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IsPhoneNumberTaken"/>.</returns>
    public virtual async Task<IsPhoneNumberTaken> IsPhoneNumberTakenAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        if (phoneNumber == null)
            throw new ArgumentNullException(nameof(phoneNumber));

        var existingIdentityUser = await this.UserManager
            .FindByPhoneNumberAsync<IdentityUser<TIdentity>, TIdentity>(phoneNumber);

        return new IsPhoneNumberTaken
        {
            IsTaken = existingIdentityUser != null
        };
    }

    /// <summary>
    /// Sign-Up a new user.
    /// </summary>
    /// <param name="signUp">The <see cref="SignUp"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The user.</returns>
    public virtual async Task<IdentityUser<TIdentity>> SignUpAsync(SignUp signUp, CancellationToken cancellationToken = default)
    {
        if (signUp == null)
        {
            throw new ArgumentNullException(nameof(signUp));
        }

        var identityUser = new IdentityUser<TIdentity>
        {
            Email = signUp.EmailAddress,
            UserName = signUp.Username,
            PhoneNumber = signUp.PhoneNumber
        };

        IdentityResult createResult;
        try
        {
            createResult = await this.UserManager
                .CreateAsync(identityUser, signUp.Password);
        }
        catch (DbUpdateException ex)
        {
            const string MESSAGE = "IX___EFAuthUser_PhoneNumber";

            if (ex.Message.Contains(MESSAGE) || ex.InnerException != null && ex.InnerException.Message.Contains(MESSAGE))
            {
                this.ThrowIdentityExceptions([new IdentityErrorDescriber().DuplicatePhoneNumber(signUp.PhoneNumber)]);
            }

            throw;
        }

        if (!createResult.Succeeded)
        {
            this.ThrowIdentityExceptions(createResult.Errors);
        }

        await this.AssignSignUpRolesAndClaims(identityUser, signUp.Roles, signUp.Claims);

        return identityUser;
    }

    /// <summary>
    /// Sign-Up a new user.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <param name="signUp">The <see cref="SignUp"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The user.</returns>
    public virtual async Task<TUser> SignUpAsync<TUser>(SignUp<TUser, TIdentity> signUp, CancellationToken cancellationToken = default) 
        where TUser : IEntityUser<TIdentity>
    {
        if (signUp == null)
        {
            throw new ArgumentNullException(nameof(signUp));
        }

        var identityUser = await this.SignUpAsync(signUp as SignUp, cancellationToken);
        
        var user = await this.CreateUser(signUp.User, identityUser, cancellationToken);

        return user;
    }

    /// <summary>
    /// Sign-Up a new user using an external login provider.
    /// </summary>
    /// <typeparam name="TProvider">The provider type.</typeparam>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <param name="signUpExternal">The <see cref="BaseSignUpExternal{TProvider, TUser, TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The user.</returns>
    public virtual async Task<IdentityUser<TIdentity>> SignUpExternalAsync<TProvider, TUser>(BaseSignUpExternal<TProvider, TUser, TIdentity> signUpExternal, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
        where TUser : IEntityUser<TIdentity>, new()
    {
        if (signUpExternal == null)
        {
            throw new ArgumentNullException(nameof(signUpExternal));
        }

        var externalLoginData = await this.GetExternalProviderLogInData(signUpExternal.Provider, cancellationToken);

        return await this.SignUpExternalAsync(externalLoginData, signUpExternal.Roles, signUpExternal.Claims, cancellationToken);
    }

    /// <summary>
    /// Sign-Up a new user using an external login provider data.
    /// </summary>
    /// <param name="externalLogInData">The <see cref="ExternalLogInData"/>.</param>
    /// <param name="roles">The roles.</param>
    /// <param name="claims">The claims.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The user.</returns>
    public virtual async Task<IdentityUser<TIdentity>> SignUpExternalAsync(ExternalLogInData externalLogInData, IEnumerable<string> roles = null, IDictionary<string, string> claims = null, CancellationToken cancellationToken = default)
    {
        if (externalLogInData == null)
        {
            throw new ArgumentNullException(nameof(externalLogInData));
        }

        var identityUser = await this.UserManager
            .FindByNameAsync(externalLogInData.Email);

        if (identityUser == null)
        {
            identityUser = new IdentityUser<TIdentity>
            {
                Email = externalLogInData.Email,
                UserName = externalLogInData.Email
            };

            var createResult = await this.UserManager
                .CreateAsync(identityUser);

            if (!createResult.Succeeded)
            {
                this.ThrowIdentityExceptions(createResult.Errors);
            }

            await this.AssignSignUpRolesAndClaims(identityUser, roles, claims);
        }

        var userLoginInfo = new UserLoginInfo(externalLogInData.ExternalToken.Name, externalLogInData.Id, externalLogInData.ExternalToken.Name);

        var addLoginResult = await this.UserManager
            .AddLoginAsync(identityUser, userLoginInfo);

        if (!addLoginResult.Succeeded)
        {
            this.ThrowIdentityExceptions(addLoginResult.Errors);
        }

        await this.SignInManager
            .SignInAsync(identityUser, false);

        return identityUser;
    }

    /// <summary>
    /// Gets external login of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="providerName">The provider name.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ExternalLogin"/>.</returns>
    public virtual async Task<ExternalLogin> GetUserExternalLoginAsync(TIdentity userId, string providerName, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return await this.GetUserExternalLoginAsync(identityUser, providerName, cancellationToken);
    }

    /// <summary>
    /// Gets external logins of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="providerName">The provider name.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ExternalLogin"/>.</returns>
    public virtual async Task<ExternalLogin> GetUserExternalLoginAsync(IdentityUser<TIdentity> identityUser, string providerName, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        var externalLogins = await this.GetUserExternalLoginsAsync(identityUser, cancellationToken);

        return externalLogins
            .FirstOrDefault(x => x.Provider.Name == providerName);
    }

    /// <summary>
    /// Gets external logins of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ExternalLogin"/>.</returns>
    public virtual async Task<IEnumerable<ExternalLogin>> GetUserExternalLoginsAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return await this.GetUserExternalLoginsAsync(identityUser, cancellationToken);
    }

    /// <summary>
    /// Gets external logins of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The collection of <see cref="ExternalLogin"/>.</returns>
    public virtual async Task<IEnumerable<ExternalLogin>> GetUserExternalLoginsAsync(IdentityUser<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        var externalLogins = await this.UserManager
            .GetLoginsAsync(identityUser);

        return externalLogins
            .Select(x => new ExternalLogin
            {
                Key = x.ProviderKey,
                Provider =
                {
                    Name = x.LoginProvider,
                    DisplayName = x.ProviderDisplayName
                }
            });
    }

    /// <summary>
    /// Add the extenral login of a user.
    /// </summary>
    /// <typeparam name="TProvider">The provider type.</typeparam>
    /// <param name="addExternalLogin">The <see cref="BaseAddExternalLogin{TProvider, TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ExternalLogin"/>.</returns>
    public virtual async Task<ExternalLogin> AddExternalLoginAsync<TProvider>(BaseAddExternalLogin<TProvider, TIdentity> addExternalLogin, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider, new()
    {
        if (addExternalLogin == null)
        {
            throw new ArgumentNullException(nameof(addExternalLogin));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(addExternalLogin.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }
        
        var externalProviderLogInData = await this.GetExternalProviderLogInData(addExternalLogin.Provider, cancellationToken);
        var userLoginInfo = new UserLoginInfo(externalProviderLogInData.ExternalToken.Name, externalProviderLogInData.Id, externalProviderLogInData.ExternalToken.Name);

        var addLoginResult = await this.UserManager
            .AddLoginAsync(identityUser, userLoginInfo);

        if (!addLoginResult.Succeeded)
        {
            this.ThrowIdentityExceptions(addLoginResult.Errors);
        }

        return new ExternalLogin
        {
            Key = userLoginInfo.ProviderKey,
            Provider =
            {
                Name = userLoginInfo.LoginProvider,
                DisplayName = userLoginInfo.ProviderDisplayName
            }
        };
    }

    /// <summary>
    /// Removes the extenral login of a user.
    /// </summary>
    /// <param name="removeExternalLogin">The <see cref="RemoveExternalLogin{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task RemoveExternalLoginAsync(RemoveExternalLogin<TIdentity> removeExternalLogin, CancellationToken cancellationToken = default)
    {
        if (removeExternalLogin == null)
        {
            throw new ArgumentNullException(nameof(removeExternalLogin));
        }
        
        var identityUser = await this.UserManager
            .GetIdentityUserAsync(removeExternalLogin.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .RemoveLoginAsync(identityUser, removeExternalLogin.ProviderName, removeExternalLogin.ProviderKey);

        if (result.Succeeded)
        {
            await this.SignInManager
                .RefreshSignInAsync(identityUser);
        }
        else
        {
            this.ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <summary>
    /// Gets api key of a user.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>>.</returns>
    public virtual Task<IdentityApiKey<TIdentity>> GetApiKeyAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        return this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// Gets the api keys of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IEnumerable{IdentityApiKey}"/>>.</returns>
    public virtual async Task<IEnumerable<IdentityApiKey<TIdentity>>> GetApiKeysAsync(TIdentity userId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;

        return this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .Where(x => x.IdentityUserId.Equals(userId));
    }

    /// <summary>
    /// Create Api Key.
    /// </summary>
    /// <param name="createApiKey">The create the key.</param>
    /// <param name="apiKey">The generated api key.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual IdentityApiKey<TIdentity> CreateApiKeyAsync(CreateApiKey<TIdentity> createApiKey, out string apiKey)
    {
        if (createApiKey == null)
        {
            throw new ArgumentNullException(nameof(createApiKey));
        }

        if (this.Options.ApiKey.Secret == null)
        {
            throw new NullReferenceException(nameof(this.Options.ApiKey.Secret));
        }

        var identityUser = this.UserManager.Users
            .SingleOrDefault(x => x.Id.Equals(createApiKey.UserId));

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        apiKey = PasswordGenerator.Generate(new PasswordOptions { RequiredLength = 48 });

        var base64Hash = apiKey
            .HmacEncrypt(this.Options.ApiKey.Secret);

        var identityApiKey = new IdentityApiKey<TIdentity>
        {
            IdentityUserId = identityUser.Id,
            Name = createApiKey.Name,
            Hash = base64Hash
        };

        var createdEntry = this.DbContext
            .Add(identityApiKey);

        this.DbContext
            .SaveChanges();

        return createdEntry.Entity;
    }

    /// <summary>
    /// Validate Api Key.
    /// </summary>
    /// <param name="apiKey">The api key.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public async Task<IdentityApiKey<TIdentity>> ValidateApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        if (apiKey == null)
        {
            throw new ArgumentNullException(nameof(apiKey));
        }

        if (this.Options.ApiKey.Secret == null)
        {
            throw new NullReferenceException(nameof(this.Options.ApiKey.Secret));
        }

        var now = DateTimeOffset.UtcNow;
        var base64Hash = apiKey.HmacEncrypt(this.Options.ApiKey.Secret);

        var identityApiKey = await this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Hash == base64Hash && (x.RevokedAt == null || x.RevokedAt > now), cancellationToken);

        if (identityApiKey == null)
        {
            return null;
        }

        var isValid = CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(base64Hash), Encoding.UTF8.GetBytes(identityApiKey.Hash));

        if (!isValid)
        {
            return null;
        }

        return identityApiKey;
    }

    /// <summary>
    /// Edit Api Key.
    /// </summary>
    /// <param name="editApiKey">The update api key.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual async Task<IdentityApiKey<TIdentity>> EditApiKeyAsync(EditApiKey<TIdentity> editApiKey, CancellationToken cancellationToken = default)
    {
        if (editApiKey == null)
        {
            throw new ArgumentNullException(nameof(editApiKey));
        }

        var identityApiKey = await this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Id.Equals(editApiKey.Id), cancellationToken);

        identityApiKey.Name = editApiKey.Name;

        this.DbContext
            .Update(identityApiKey);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return identityApiKey;
    }

    /// <summary>
    /// Revoke Api Key.
    /// </summary>
    /// <param name="revokeApiKey">The revoke api key.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityApiKey{TIdentity}"/>.</returns>
    public virtual async Task<IdentityApiKey<TIdentity>> RevokeApiKeyAsync(RevokeApiKey<TIdentity> revokeApiKey, CancellationToken cancellationToken = default)
    {
        if (revokeApiKey == null)
        {
            throw new ArgumentNullException(nameof(revokeApiKey));
        }

        var identityApiKey = await this.DbContext
            .Set<IdentityApiKey<TIdentity>>()
            .FirstOrDefaultAsync(x => x.Id.Equals(revokeApiKey.Id) && x.RevokedAt == null, cancellationToken);

        identityApiKey.RevokedAt = revokeApiKey.RevokeAt ?? DateTimeOffset.UtcNow;

        this.DbContext
            .Update(identityApiKey);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return identityApiKey;
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
        {
            throw new ArgumentNullException(nameof(setUsername));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(setUsername.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .SetUserNameAsync(identityUser, setUsername.NewUsername);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }

        await this.DbContext
            .SaveChangesAsync(cancellationToken);
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
        {
            throw new ArgumentNullException(nameof(setPassword));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(setPassword.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .AddPasswordAsync(identityUser, setPassword.NewPassword);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <summary>
    /// Resets the password of a user.
    /// </summary>
    /// <param name="resetPassword">The <see cref="ResetPassword{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task ResetPasswordAsync(ResetPassword<TIdentity> resetPassword, CancellationToken cancellationToken = default)
    {
        if (resetPassword == null)
        {
            throw new ArgumentNullException(nameof(resetPassword));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(resetPassword.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new UnauthorizedException($"The user: {resetPassword.UserId} was not found or is deactivated.");
        }

        var result = await this.UserManager
            .ResetPasswordAsync(identityUser, resetPassword.Token, resetPassword.Password);

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
        {
            throw new ArgumentNullException(nameof(changePassword));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(changePassword.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .ChangePasswordAsync(identityUser, changePassword.OldPassword, changePassword.NewPassword);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }

        await this.SignInManager
            .RefreshSignInAsync(identityUser);
    }

    /// <summary>
    /// Changes the email address of a user.
    /// </summary>
    /// <param name="changeEmail">The <see cref="ChangeEmail{TIdentity}"/>.</param>
    /// <param name="setUsername">Set username.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task ChangeEmailAsync(ChangeEmail<TIdentity> changeEmail, bool setUsername, CancellationToken cancellationToken = default)
    {
        if (changeEmail == null)
        {
            throw new ArgumentNullException(nameof(changeEmail));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(changeEmail.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var identityUserChangeData = this.DbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(changeEmail.UserId));

        if (identityUserChangeData == null)
        {
            throw new NullReferenceException(nameof(identityUserChangeData));
        }

        if (identityUserChangeData.NewEmail == null)
        {
            throw new NullReferenceException(nameof(identityUserChangeData.NewEmail));
        }

        var result = await this.UserManager
            .ChangeEmailAsync(identityUser, identityUserChangeData.NewEmail, changeEmail.Token);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }

        identityUserChangeData.NewEmail = null;

        this.DbContext
            .Update(identityUserChangeData);

        if (setUsername)
        {
            await this.SetUsernameAsync(new SetUsername<TIdentity>
            {
                UserId = identityUser.Id,
                NewUsername = identityUserChangeData.NewEmail
            }, cancellationToken);
        }

        await this.DbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Confirms the email of a user.
    /// </summary>
    /// <param name="confirmEmail">The <see cref="ConfirmEmail{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task ConfirmEmailAsync(ConfirmEmail<TIdentity> confirmEmail, CancellationToken cancellationToken = default)
    {
        if (confirmEmail == null)
        {
            throw new ArgumentNullException(nameof(confirmEmail));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(confirmEmail.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .ConfirmEmailAsync(identityUser, confirmEmail.Token);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <summary>
    /// Changes the phone number of a user.
    /// </summary>
    /// <param name="changePhoneNumber">The <see cref="ChangePhoneNumber{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task ChangePhoneNumberAsync(ChangePhoneNumber<TIdentity> changePhoneNumber, CancellationToken cancellationToken = default)
    {
        if (changePhoneNumber == null)
        {
            throw new ArgumentNullException(nameof(changePhoneNumber));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(changePhoneNumber.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var identityUserChangeData = this.DbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(changePhoneNumber.UserId));

        if (identityUserChangeData == null)
        {
            throw new NullReferenceException(nameof(identityUserChangeData));
        }

        if (identityUserChangeData.NewPhoneNumber == null)
        {
            throw new NullReferenceException(nameof(identityUserChangeData.NewPhoneNumber));
        }

        var result = await this.UserManager
            .ChangePhoneNumberAsync(identityUser, identityUserChangeData.NewPhoneNumber, changePhoneNumber.Token);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }

        identityUserChangeData.NewPhoneNumber = null;

        this.DbContext
            .Update(identityUserChangeData);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Confirms the phone number of a user.
    /// </summary>
    /// <param name="confirmPhoneNumber">The <see cref="ConfirmPhoneNumber{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task ConfirmPhoneNumberAsync(ConfirmPhoneNumber<TIdentity> confirmPhoneNumber, CancellationToken cancellationToken = default)
    {
        if (confirmPhoneNumber == null)
        {
            throw new ArgumentNullException(nameof(confirmPhoneNumber));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(confirmPhoneNumber.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .ConfirmPhoneNumberAsync<IdentityUser<TIdentity>, TIdentity>(identityUser, confirmPhoneNumber.Token);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <summary>
    /// Verifies the custom token of a user.
    /// </summary>
    /// <param name="customToken">The <see cref="CustomPurposeToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task VerifyCustomTokenAsync(CustomPurposeToken<TIdentity> customToken, CancellationToken cancellationToken = default)
    {
        if (customToken == null)
        {
            throw new ArgumentNullException(nameof(customToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(customToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var success = await this.UserManager
            .VerifyUserTokenAsync(identityUser, CustomTokenOptions.CUSTOM_TOKEN_PROVIDER, customToken.Purpose, customToken.Token);

        if (!success)
        {
            this.ThrowIdentityExceptions([new IdentityError { Description = "Invalid Token." }]);
        }
    }

    /// <summary>
    /// Generates a reset password token for a user.
    /// </summary>
    /// <param name="generateResetPasswordToken">The <see cref="GenerateResetPasswordToken"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ResetPasswordToken{TIdentity}"/>.</returns>
    public virtual async Task<ResetPasswordToken<TIdentity>> GenerateResetPasswordTokenAsync(GenerateResetPasswordToken generateResetPasswordToken, CancellationToken cancellationToken = default)
    {
        if (generateResetPasswordToken == null)
        {
            throw new ArgumentNullException(nameof(generateResetPasswordToken));
        }

        var identityUser = await this.UserManager
            .FindByNameAsync(generateResetPasswordToken.Username);

        if (identityUser == null)
        {
            throw new UnauthorizedException($"The user: {generateResetPasswordToken.Username} was not found or is deactivated.");
        }

        var token = await this.UserManager
            .GeneratePasswordResetTokenAsync(identityUser);

        return new ResetPasswordToken<TIdentity>
        {
            Token = token,
            UserId = identityUser.Id
        };
    }

    /// <summary>
    /// Generates an email confirmation token for a user.
    /// </summary>
    /// <param name="generateConfirmEmailToken">The <see cref="GenerateConfirmEmailToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmEmailToken{TIdentity}"/>.</returns>
    public virtual async Task<ConfirmEmailToken<TIdentity>> GenerateConfirmEmailTokenAsync(GenerateConfirmEmailToken<TIdentity> generateConfirmEmailToken, CancellationToken cancellationToken = default)
    {
        if (generateConfirmEmailToken == null)
        {
            throw new ArgumentNullException(nameof(generateConfirmEmailToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(generateConfirmEmailToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var token = await this.UserManager
            .GenerateEmailConfirmationTokenAsync(identityUser);

        return new ConfirmEmailToken<TIdentity>
        {
            UserId = generateConfirmEmailToken.UserId,
            Token = token
        };
    }

    /// <summary>
    /// Generates an change email token for a user.
    /// </summary>
    /// <param name="generateChangeEmailToken">The <see cref="GenerateChangeEmailToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangeEmailToken{TIdentity}"/>.</returns>
    public virtual async Task<ChangeEmailToken<TIdentity>> GenerateChangeEmailTokenAsync(GenerateChangeEmailToken<TIdentity> generateChangeEmailToken, CancellationToken cancellationToken = default)
    {
        if (generateChangeEmailToken == null)
        {
            throw new ArgumentNullException(nameof(generateChangeEmailToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(generateChangeEmailToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        if (this.Options.User.IsUniqueEmailAddressRequired)
        {
            var existingUser = await this.UserManager
                .FindByEmailAsync(generateChangeEmailToken.NewEmailAddress);

            if (existingUser != null)
            {
                var duplicateEmail = new IdentityErrorDescriber().DuplicateEmail(generateChangeEmailToken.NewEmailAddress);

                throw new TranslationException(duplicateEmail.Description);
            }
        }

        var identityUserChangeData = this.DbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(identityUser.Id));

        if (identityUserChangeData == null)
        {
            this.DbContext
                .Add(new IdentityUserChangeData<TIdentity>
                {
                    IdentityUserId = identityUser.Id,
                    NewEmail = generateChangeEmailToken.NewEmailAddress
                });
        }
        else
        {
            identityUserChangeData.NewEmail = generateChangeEmailToken.NewEmailAddress;

            this.DbContext
                .Update(identityUserChangeData);
        }

        var token = await this.UserManager
            .GenerateChangeEmailTokenAsync(identityUser, generateChangeEmailToken.NewEmailAddress);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return new ChangeEmailToken<TIdentity>
        {
            UserId = generateChangeEmailToken.UserId,
            Token = token,
            NewEmailAddress = generateChangeEmailToken.NewEmailAddress
        };
    }

    /// <summary>
    /// Generates a confirm phone number token for a user.
    /// </summary>
    /// <param name="generateConfirmPhoneToken">The <see cref="GenerateConfirmPhoneToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ConfirmPhoneNumberToken{TIdentity}"/>.</returns>
    public virtual async Task<ConfirmPhoneNumberToken<TIdentity>> GenerateConfirmPhoneNumberTokenAsync(GenerateConfirmPhoneToken<TIdentity> generateConfirmPhoneToken, CancellationToken cancellationToken = default)
    {
        if (generateConfirmPhoneToken == null)
        {
            throw new ArgumentNullException(nameof(generateConfirmPhoneToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(generateConfirmPhoneToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var token = await this.UserManager
            .GeneratePhoneNumberConfirmationTokenAsync<IdentityUser<TIdentity>, TIdentity>(identityUser);

        return new ConfirmPhoneNumberToken<TIdentity>
        {
            UserId = generateConfirmPhoneToken.UserId,
            Token = token
        };
    }

    /// <summary>
    /// Generates a change phone number token for a user.
    /// </summary>
    /// <param name="generateChangePhoneToken">The <see cref="GenerateChangePhoneToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="ChangePhoneNumberToken{TIdentity}"/>.</returns>
    public virtual async Task<ChangePhoneNumberToken<TIdentity>> GenerateChangePhoneNumberTokenAsync(GenerateChangePhoneToken<TIdentity> generateChangePhoneToken, CancellationToken cancellationToken = default)
    {
        if (generateChangePhoneToken == null)
        {
            throw new ArgumentNullException(nameof(generateChangePhoneToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(generateChangePhoneToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var existingUser = await this.UserManager
            .GetIdentityUserAsync(generateChangePhoneToken.UserId, cancellationToken);

        if (existingUser != null)
        {
            var duplicatePhoneNumber = new IdentityErrorDescriber().DuplicatePhoneNumber(generateChangePhoneToken.NewPhoneNumber);

            throw new TranslationException(duplicatePhoneNumber.Description);
        }

        var identityUserChangeData = this.DbContext
            .Set<IdentityUserChangeData<TIdentity>>()
            .FirstOrDefault(x => x.IdentityUserId.Equals(identityUser.Id));

        if (identityUserChangeData == null)
        {
            this.DbContext
                .Add(new IdentityUserChangeData<TIdentity>
                {
                    IdentityUserId = identityUser.Id,
                    NewPhoneNumber = generateChangePhoneToken.NewPhoneNumber
                });
        }
        else
        {
            identityUserChangeData.NewPhoneNumber = generateChangePhoneToken.NewPhoneNumber;

            this.DbContext
                .Update(identityUserChangeData);
        }

        var token = await this.UserManager
            .GenerateChangePhoneNumberTokenAsync(identityUser, generateChangePhoneToken.NewPhoneNumber);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return new ChangePhoneNumberToken<TIdentity>
        {
            UserId = generateChangePhoneToken.UserId,
            Token = token,
            NewPhoneNumber = generateChangePhoneToken.NewPhoneNumber
        };
    }

    /// <summary>
    /// Generates a custom token for a user.
    /// </summary>
    /// <param name="generateCustomPurposeToken">The <see cref="GenerateCustomPurposeToken{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="CustomPurposeToken{TIdentity}"/>.</returns>
    public virtual async Task<CustomPurposeToken<TIdentity>> GenerateCustomTokenAsync(GenerateCustomPurposeToken<TIdentity> generateCustomPurposeToken, CancellationToken cancellationToken = default)
    {
        if (generateCustomPurposeToken == null)
        {
            throw new ArgumentNullException(nameof(generateCustomPurposeToken));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(generateCustomPurposeToken.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var token = await this.UserManager
            .GenerateUserTokenAsync(identityUser, CustomTokenOptions.CUSTOM_TOKEN_PROVIDER, generateCustomPurposeToken.Purpose);

        return new CustomPurposeToken<TIdentity>
        {
            UserId = generateCustomPurposeToken.UserId,
            Token = token,
            Purpose = generateCustomPurposeToken.Purpose
        };
    }

    /// <summary>
    /// Gets all the <see cref="IdentityRole{TIdentity}"/>'s.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRole{TIdentity}"/>'s.</returns>
    public virtual async Task<IEnumerable<IdentityRole<TIdentity>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = this.RoleManager.Roles
            .OrderBy(x => x.Name);

        return await Task.FromResult(roles);
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
        {
            throw new ArgumentNullException(nameof(roleName));
        }

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
        {
            throw new ArgumentNullException(nameof(roleName));
        }

        var role = await this.RoleManager
            .FindByNameAsync(roleName);

        if (role == null)
        {
            throw new NullReferenceException(nameof(role));
        }

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
        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return await this.GetUserRolesAsync(identityUser, cancellationToken);
    }

    /// <summary>
    /// Gets the roles of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The role names.</returns>
    public virtual async Task<IEnumerable<string>> GetUserRolesAsync(IdentityUser<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        return await this.UserManager
            .GetRolesAsync(identityUser);
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
        {
            throw new ArgumentNullException(nameof(assignRole));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(assignRole.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .AddToRoleAsync(identityUser, assignRole.RoleName);

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

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(removeRole.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var result = await this.UserManager
            .RemoveFromRoleAsync(identityUser, removeRole.RoleName);

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
        {
            throw new ArgumentNullException(nameof(getClaim));
        }

        var claims = await this.GetUserClaimsAsync(getClaim.UserId, cancellationToken);

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
        var identityUser = await this.UserManager
            .GetIdentityUserAsync(userId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        return await this.GetUserClaimsAsync(identityUser, cancellationToken);
    }

    /// <summary>
    /// Gets the <see cref="Claim"/>'s of a user.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Claim"/>'s.</returns>
    public virtual async Task<IEnumerable<Claim>> GetUserClaimsAsync(IdentityUser<TIdentity> identityUser, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        return await this.UserManager
            .GetClaimsAsync(identityUser);
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
        {
            throw new ArgumentNullException(nameof(assignClaim));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(assignClaim.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var userClaim = new IdentityUserClaim<TIdentity>
        {
            ClaimType = assignClaim.ClaimType,
            ClaimValue = assignClaim.ClaimValue
        };

        var claim = userClaim
            .ToClaim();

        var result = await this.UserManager
            .AddClaimAsync(identityUser, claim);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }

        return userClaim;
    }

    /// <summary>
    /// Replace a <see cref="IdentityUserClaim{TIdentity}"/> to a user.
    /// </summary>
    /// <param name="replaceClaim">The <see cref="ReplaceClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
    public virtual async Task<IdentityUserClaim<TIdentity>> ReplaceUserClaimAsync(ReplaceClaim<TIdentity> replaceClaim, CancellationToken cancellationToken = default)
    {
        if (replaceClaim == null)
            throw new ArgumentNullException(nameof(replaceClaim));

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(replaceClaim.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var existingClaim = (await this.GetUserClaimsAsync(identityUser, cancellationToken))
            .FirstOrDefault(x => x.Type == replaceClaim.ClaimType);

        if (existingClaim == null)
        {
            throw new NullReferenceException(nameof(existingClaim));
        }

        var newClaim = new IdentityUserClaim<TIdentity>
        {
            ClaimType = replaceClaim.ClaimType,
            ClaimValue = replaceClaim.NewClaimValue
        };

        var claim = newClaim
            .ToClaim();

        var result = await this.UserManager
            .ReplaceClaimAsync(identityUser, existingClaim, claim);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }

        return newClaim;
    }

    /// <summary>
    /// Assign or Replace a <see cref="IdentityUserClaim{TIdentity}"/> to a user.
    /// </summary>
    /// <param name="assignOrReplaceClaim">The <see cref="AssignOrReplaceClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUserClaim{TIdentity}"/>.</returns>
    public virtual async Task<IdentityUserClaim<TIdentity>> AssignOrReplaceUserClaimAsync(AssignOrReplaceClaim<TIdentity> assignOrReplaceClaim, CancellationToken cancellationToken = default)
    {
        if (assignOrReplaceClaim == null)
        {
            throw new ArgumentNullException(nameof(assignOrReplaceClaim));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(assignOrReplaceClaim.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var existingClaim = (await this.GetUserClaimsAsync(identityUser, cancellationToken))
            .FirstOrDefault(x => x.Type == assignOrReplaceClaim.ClaimType);

        var newClaim = new IdentityUserClaim<TIdentity>
        {
            ClaimType = assignOrReplaceClaim.ClaimType,
            ClaimValue = assignOrReplaceClaim.ClaimValue
        };

        var claim = newClaim
            .ToClaim();

        var result = existingClaim == null
            ? await this.UserManager
                .AddClaimAsync(identityUser, claim)
            : await this.UserManager
                .ReplaceClaimAsync(identityUser, existingClaim, claim);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }

        return newClaim;
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
        {
            throw new ArgumentNullException(nameof(removeClaim));
        }

        var identityUser = await this.UserManager
            .GetIdentityUserAsync(removeClaim.UserId, cancellationToken);

        if (identityUser == null)
        {
            throw new NullReferenceException(nameof(identityUser));
        }

        var claims = await this.UserManager
            .GetClaimsAsync(identityUser);

        var claim = claims
            .FirstOrDefault(x => x.Type == removeClaim.ClaimType);

        if (claim == null)
        {
            throw new NullReferenceException(nameof(claim));
        }

        var result = await this.UserManager
            .RemoveClaimAsync(identityUser, claim);

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
    public virtual async Task<Claim> GetRoleClaimAsync(GetRoleClaim<TIdentity> getClaim, CancellationToken cancellationToken = default)
    {
        if (getClaim == null)
        {
            throw new ArgumentNullException(nameof(getClaim));
        }

        var claims = await this.GetRoleClaimsAsync(getClaim.RoleId, cancellationToken);

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
        var identityRole = await this.RoleManager
            .GetIdentityRoleAsync(roleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NullReferenceException(nameof(identityRole));
        }

        var claims = await this.RoleManager
            .GetClaimsAsync(identityRole);

        return claims;
    }

    /// <summary>
    /// Assigns a <see cref="IdentityRoleClaim{TIdentity}"/> to a role.
    /// </summary>
    /// <param name="assignClaim">The <see cref="AssignRoleClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
    public virtual async Task<IdentityRoleClaim<TIdentity>> AssignRoleClaimAsync(AssignRoleClaim<TIdentity> assignClaim, CancellationToken cancellationToken = default)
    {
        if (assignClaim == null)
        {
            throw new ArgumentNullException(nameof(assignClaim));
        }

        var identityRole = await this.RoleManager
            .GetIdentityRoleAsync(assignClaim.RoleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NullReferenceException(nameof(identityRole));
        }

        var roleClaim = new IdentityRoleClaim<TIdentity>
        {
            ClaimType = assignClaim.ClaimType,
            ClaimValue = assignClaim.ClaimValue
        };

        var claim = roleClaim
            .ToClaim();

        var result = await this.RoleManager
            .AddClaimAsync(identityRole, claim);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }

        return roleClaim;
    }

    /// <summary>
    /// Replace a <see cref="IdentityRoleClaim{TIdentity}"/> to a role.
    /// </summary>
    /// <param name="replaceClaim">The <see cref="ReplaceClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
    public virtual async Task<IdentityRoleClaim<TIdentity>> ReplaceRoleClaimAsync(ReplaceRoleClaim<TIdentity> replaceClaim, CancellationToken cancellationToken = default)
    {
        if (replaceClaim == null)
        {
            throw new ArgumentNullException(nameof(replaceClaim));
        }

        var identityRole = await this.RoleManager
            .GetIdentityRoleAsync(replaceClaim.RoleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NullReferenceException(nameof(identityRole));
        }

        var existingClaim = (await this.GetRoleClaimsAsync(replaceClaim.RoleId, cancellationToken))
            .FirstOrDefault(x => x.Type == replaceClaim.ClaimType);

        if (existingClaim == null)
        {
            throw new NullReferenceException(nameof(existingClaim));
        }

        var result = await this.RoleManager
            .RemoveClaimAsync(identityRole, existingClaim);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }

        var newClaim = new IdentityRoleClaim<TIdentity>
        {
            ClaimType = replaceClaim.ClaimType,
            ClaimValue = replaceClaim.NewClaimValue
        };

        var claim = newClaim
            .ToClaim();

        result = await this.RoleManager
            .AddClaimAsync(identityRole, claim);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }

        return newClaim;
    }

    /// <summary>
    /// Assign or Replace a <see cref="IdentityRoleClaim{TIdentity}"/> to a role.
    /// </summary>
    /// <param name="assignOrReplaceRoleClaim">The <see cref="AssignOrReplaceRoleClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityRoleClaim{TIdentity}"/>.</returns>
    public virtual async Task<IdentityRoleClaim<TIdentity>> AssignOrReplaceRoleClaimAsync(AssignOrReplaceRoleClaim<TIdentity> assignOrReplaceRoleClaim, CancellationToken cancellationToken = default)
    {
        if (assignOrReplaceRoleClaim == null)
        {
            throw new ArgumentNullException(nameof(assignOrReplaceRoleClaim));
        }

        var identityRole = await this.RoleManager
            .GetIdentityRoleAsync(assignOrReplaceRoleClaim.RoleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NullReferenceException(nameof(identityRole));
        }

        var existingClaim = (await this.GetRoleClaimsAsync(identityRole.Id, cancellationToken))
            .FirstOrDefault(x => x.Type == assignOrReplaceRoleClaim.ClaimType);

        IdentityRoleClaim<TIdentity> newClaim;
        if (existingClaim == null)
        {
            newClaim = await this.AssignRoleClaimAsync(new AssignRoleClaim<TIdentity>
            {
                RoleId = identityRole.Id,
                ClaimType = assignOrReplaceRoleClaim.ClaimType,
                ClaimValue = assignOrReplaceRoleClaim.ClaimValue
            }, cancellationToken);
        }
        else
        {
            newClaim = await this.ReplaceRoleClaimAsync(new ReplaceRoleClaim<TIdentity>
            {
                RoleId = identityRole.Id,
                ClaimType = assignOrReplaceRoleClaim.ClaimType,
                NewClaimValue = assignOrReplaceRoleClaim.ClaimValue
            }, cancellationToken);
        }

        return newClaim;
    }

    /// <summary>
    /// Removes a <see cref="IdentityRoleClaim{TIdentity}"/> from a role.
    /// </summary>
    /// <param name="removeClaim">The <see cref="RemoveRoleClaim{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Void.</returns>
    public virtual async Task RemoveRoleClaimAsync(RemoveRoleClaim<TIdentity> removeClaim, CancellationToken cancellationToken = default)
    {
        if (removeClaim == null)
        {
            throw new ArgumentNullException(nameof(removeClaim));
        }

        var identityRole = await this.RoleManager
            .GetIdentityRoleAsync(removeClaim.RoleId, cancellationToken);

        if (identityRole == null)
        {
            throw new NullReferenceException(nameof(identityRole));
        }

        var claims = await this.RoleManager
            .GetClaimsAsync(identityRole);

        var claim = claims
            .FirstOrDefault(x => x.Type == removeClaim.ClaimType);

        if (claim == null)
        {
            throw new NullReferenceException(nameof(claim));
        }

        var result = await this.RoleManager
            .RemoveClaimAsync(identityRole, claim);

        if (!result.Succeeded)
        {
            this.ThrowIdentityExceptions(result.Errors);
        }
    }

    /// <summary>
    /// Creates a user, and the associated <see cref="IdentityUser{TIdentity}"/>.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <param name="user">The user.</param>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The created user.</returns>
    public virtual async Task<TUser> CreateUser<TUser>(TUser user, IdentityUser<TIdentity> identityUser, CancellationToken cancellationToken = default)
        where TUser : IEntityUser<TIdentity>
    {
        if (identityUser == null)
        {
            throw new ArgumentNullException(nameof(identityUser));
        }

        user.Id = identityUser.Id.Parse<TIdentity>();

        user.IdentityUser = this.DbContext
            .Find<IdentityUser<TIdentity>>(identityUser.Id);

        try
        {
            await this.DbContext
                .AddAsync(user, cancellationToken);

            await this.DbContext
                .SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await this.DeleteIdentityUser(identityUser, cancellationToken);

            throw;
        }

        return user;
    }

    /// <summary>
    /// Activates the user with the passed user id.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <param name="id">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUser{TIdentity}"/>.</returns>
    public virtual async Task<TUser> ActivateIdentityUser<TUser>(TIdentity id, CancellationToken cancellationToken = default)
        where TUser : class, IEntityUser<TIdentity>
    {
        var user = this.DbContext
            .Set<TUser>()
            .IgnoreQueryFilters()
            .FirstOrDefault(x => x.Id.Equals(id));

        if (user == null)
        {
            throw new NullReferenceException(nameof(user));
        }

        user.IsActive = true;

        var entityEntry = this.DbContext
            .Update(user);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return entityEntry.Entity;
    }

    /// <summary>
    /// Deactivates the user with the passed user id.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    /// <param name="id">The user id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="IdentityUser{TIdentity}"/>.</returns>
    public virtual async Task<TUser> DeactivateIdentityUser<TUser>(TIdentity id, CancellationToken cancellationToken = default) 
        where TUser : class, IEntityUser<TIdentity>
    {
        var refreshTokens = this.DbContext
            .Set<IdentityUserTokenExpiry<TIdentity>>()
            .Where(x => x.UserId.Equals(id));

        this.DbContext
            .RemoveRange(refreshTokens);

        await this.SignInManager
            .SignOutAsync();

        var user = this.DbContext
            .Set<TUser>()
            .FirstOrDefault(x => x.Id.Equals(id));

        if (user == null)
        {
            throw new NullReferenceException(nameof(user));
        }

        user.IsActive = false;

        var entityEntry = this.DbContext
            .Update(user);

        await this.DbContext
            .SaveChangesAsync(cancellationToken);

        return entityEntry.Entity;
    }

    /// <summary>
    /// Deletes the <see cref="IdentityUser{TIdentity}"/>.
    /// </summary>
    /// <param name="identityUser">The <see cref="IdentityUser{TIdentity}"/>.</param>
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

        await this.DbContext
            .SaveChangesAsync(cancellationToken);
    }


    internal async Task<IList<Claim>> GetAllClaims(IdentityUser<TIdentity> identityUser, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
            throw new ArgumentNullException(nameof(identityUser));

        transientRoles ??= new List<string>();
        transientClaims ??= new Dictionary<string, string>();

        var userClaims = await this.UserManager
            .GetClaimsAsync(identityUser);

        var query = from userRole in this.DbContext.Set<IdentityUserRole<TIdentity>>()
            join role in this.DbContext.Set<IdentityRole<TIdentity>>() on userRole.RoleId equals role.Id
            where userRole.UserId.Equals(identityUser.Id)
            select new
            {
                role.Id,
                role.Name
            };

        var roles = await query
            .ToListAsync(cancellationToken);

        var roleClaims = await this.DbContext
            .Set<IdentityRoleClaim<TIdentity>>()
            .Where(rc => roles.Select(y => y.Id).Contains(rc.RoleId))
            .Select(c => new Claim(c.ClaimType, c.ClaimValue))
            .ToListAsync(cancellationToken);

        var rolesAsClaims = roles
            .Select(y => new Claim(ClaimTypes.Role, y.Name))
            .Concat(transientRoles.Select(x => new Claim(ClaimTypes.Role, x)));

        var claims = userClaims
            .Union(roleClaims)
            .Union(rolesAsClaims)
            .Union(transientClaims
                .Select(x => new Claim(x.Key, x.Value)))
            .ToList();

        return claims;
    }

    private async Task AssignSignUpRolesAndClaims(IdentityUser<TIdentity> identityUser, IEnumerable<string> roles2 = null, IDictionary<string, string> claims2 = null)
    {
        if (identityUser == null)
            throw new ArgumentNullException(nameof(identityUser));

        var roles = roles2?
            .Union(this.Options.User.DefaultRoles)
            .Distinct()
            .ToList() ?? [];

        if (roles.Any())
        {
            var roleAssignResult = await this.UserManager
                .AddToRolesAsync(identityUser, roles);

            if (!roleAssignResult.Succeeded)
            {
                this.ThrowIdentityExceptions(roleAssignResult.Errors);
            }
        }

        var claims = claims2?
            .Select(x => new Claim(x.Key, x.Value))
            .ToList() ?? [];

        if (claims.Any())
        {
            var claimAssignResult = await this.UserManager
                .AddClaimsAsync(identityUser, claims);

            if (!claimAssignResult.Succeeded)
            {
                this.ThrowIdentityExceptions(claimAssignResult.Errors);
            }
        }
    }
    private async Task<AccessToken> GenerateJwtToken(IdentityUser<TIdentity> identityUser, string appId, bool isRefreshable, string externalProviderName, string externalProviderToken, string externalProviderRefreshToken, IDictionary<string, string> transientClaims, IEnumerable<string> transientRoles, CancellationToken cancellationToken = default)
    {
        if (identityUser == null)
            throw new ArgumentNullException(nameof(identityUser));

        if (appId == null)
            throw new ArgumentNullException(nameof(appId));

        var claims = await this.GetAllClaims(identityUser, transientRoles, transientClaims, cancellationToken);

        var tokenData = new AccessTokenData
        {
            AppId = appId,
            UserId = identityUser.Id.ToString(),
            UserName = identityUser.UserName,
            UserEmail = identityUser.Email,
            ExternalToken =
            {
                Name = externalProviderName,
                Token = externalProviderToken,
                RefreshToken = externalProviderRefreshToken
            },
            Claims = claims
        };

        var token = this.GenerateJwtToken(tokenData);

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

        var token = this.GetRandomToken();

        var removeResult = await this.UserManager
            .RemoveAuthenticationTokenAsync(identityUser, JwtBearerDefaults.AuthenticationScheme, appId);

        if (!removeResult.Succeeded)
        {
            this.ThrowIdentityExceptions(removeResult.Errors);
        }

        var expireAt = DateTimeOffset.UtcNow
            .AddHours(this.Options.Jwt.RefreshExpirationInHours);

        var identityUserToken = new IdentityUserTokenExpiry<TIdentity>
        {
            UserId = identityUser.Id,
            Name = appId,
            Value = token,
            LoginProvider = JwtBearerDefaults.AuthenticationScheme,
            ExpireAt = expireAt
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
    private async Task<ExternalLoginTokenData> RefreshExternalProviderTokenOrDefault(string externalProviderName = null, string externalProviderRefreshToken = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(externalProviderName))
        {
            return new ExternalLoginTokenData();
        }

        if (string.IsNullOrEmpty(externalProviderRefreshToken))
        {
            return new ExternalLoginTokenData();
        }

        try
        {
            return externalProviderName switch
            {
                "Google" => await this.RefreshExternalProviderTokenGoogle(externalProviderName, externalProviderRefreshToken),
                "Facebook" => await this.RefreshExternalProviderTokenFacebook(externalProviderName, externalProviderRefreshToken),
                "Microsoft" => await this.RefreshExternalProviderTokenMicrosoft(externalProviderName, externalProviderRefreshToken, cancellationToken),
                _ => throw new NotSupportedException($"The external provider: {externalProviderName} is not supported.")
            };
        }
        catch (Exception ex)
        {
            this.Logger
                .LogError(ex, ex.Message);

            throw new UnauthorizedException();
        }
    }
    private Task<ExternalLoginTokenData> RefreshExternalProviderTokenGoogle(string externalProviderName, string externalProviderRefreshToken = null)
    {
        if (externalProviderName == null)
            throw new ArgumentNullException(nameof(externalProviderName));

        if (externalProviderRefreshToken == null)
            throw new ArgumentNullException(nameof(externalProviderRefreshToken));

        this.Logger
            .LogInformation($"The external provider: {externalProviderName} does not support refresh token.");

        return Task.FromResult(new ExternalLoginTokenData());
    }
    private Task<ExternalLoginTokenData> RefreshExternalProviderTokenFacebook(string externalProviderName, string externalProviderRefreshToken = null)
    {
        if (externalProviderName == null)
            throw new ArgumentNullException(nameof(externalProviderName));

        if (externalProviderRefreshToken == null)
            throw new ArgumentNullException(nameof(externalProviderRefreshToken));

        this.Logger
            .LogInformation($"The external provider: {externalProviderName} does not support refresh token.");

        return Task.FromResult(new ExternalLoginTokenData());
    }
    private async Task<ExternalLoginTokenData> RefreshExternalProviderTokenMicrosoft(string externalProviderName, string externalProviderRefreshToken = null, CancellationToken cancellationToken = default)
    {
        if (externalProviderName == null)
            throw new ArgumentNullException(nameof(externalProviderName));

        if (externalProviderRefreshToken == null)
            throw new ArgumentNullException(nameof(externalProviderRefreshToken));

        if (string.IsNullOrEmpty(externalProviderRefreshToken))
        {
            return new ExternalLoginTokenData();
        }

        var externalLoginOptions = this.Options.ExternalLogins.Microsoft;

        using var httpClient = new HttpClient();
        {
            var httpRequestMessage = new HttpRequestMessage();
            {
                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.RequestUri = new Uri($"https://login.microsoftonline.com/{externalLoginOptions.TenantId}/oauth2/v2.0/token");

                using var formContent = new MultipartFormDataContent();
                {
                    formContent.Add(new StringContent(externalLoginOptions.ClientId), "client_id");
                    formContent.Add(new StringContent(externalLoginOptions.ClientSecret), "client_secret");
                    formContent.Add(new StringContent("refresh_token"), "grant_type");
                    formContent.Add(new StringContent(externalProviderRefreshToken), "refresh_token");
                    formContent.Add(new StringContent(externalLoginOptions.Scopes.Aggregate(string.Empty, (current, x) => current + $"{x} ")), "scope");

                    httpRequestMessage.Content = formContent;

                    var httpResponse = await httpClient
                        .SendAsync(httpRequestMessage, cancellationToken);

                    var stringContent = await httpResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    var content = JsonSerializer.Deserialize<JsonObject>(stringContent);

                    var error = (string)content?["error"];
                    var errorDescription = (string)content?["error"];

                    if (error != null)
                    {
                        throw new InvalidOperationException($"{error}: {errorDescription}");
                    }

                    var accessToken = (string)content?["access_token"];
                    var refreshToken = (string)content?["refresh_token"];

                    return new ExternalLoginTokenData
                    {
                        Name = externalProviderName,
                        Token = accessToken,
                        RefreshToken = refreshToken
                    };
                }
            }
        }
    }

    private string GetRandomToken()
    {
        var bytes = new byte[32];

        using var generator = RandomNumberGenerator.Create();

        generator
            .GetBytes(bytes);

        var token = Convert.ToBase64String(bytes);

        return token;
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