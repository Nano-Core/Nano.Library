using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Nano.Models.Exceptions;
using Nano.Security.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Abstractions.Models;
using Nano.Web.Extensions;
using IdentityOptions = Nano.Web.IdentityOptions;

namespace Nano.Security;

/// <summary>
/// 
/// </summary>
public abstract class BaseIdentityAuthRepository
{
    internal const string DEFAULT_APP_ID = "Default";

    /// <summary>
    /// Logger.
    /// </summary>
    protected virtual ILogger Logger { get; }

    /// <summary>
    /// Options.
    /// </summary>
    protected virtual IdentityOptions Options { get; }

    /// <summary>
    /// Constructor.
    /// </summary>0
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="options">The <see cref="IdentityOptions"/>.</param>
    protected BaseIdentityAuthRepository(ILogger logger, IdentityOptions options)
    {
        this.Logger = logger;
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
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
                "Google" => await this.GetExternalProviderLoginDataGoogle(logInExternalProvider, this.Options.Authentication.Jwt?.ExternalLogins?.Google),
                "Facebook" => await this.GetExternalProviderLoginDataFacebook(logInExternalProvider, this.Options.Authentication.Jwt?.ExternalLogins?.Facebook, cancellationToken),
                "Microsoft" => await this.GetExternalProviderLoginDataMicrosoft(logInExternalProvider, this.Options.Authentication.Jwt?.ExternalLogins?.Microsoft, cancellationToken),
                _ => throw new NotSupportedException(logInExternalProvider.Name)
            };
        }
        catch (Exception ex)
        {
            this.Logger?
                .LogError(ex, ex.Message);

            throw new UnauthorizedException();
        }
    }

    /// <summary>
    /// Generate Jwt Token
    /// </summary>
    /// <param name="tokenData">The <see cref="AccessTokenData"/>.</param>
    /// <returns>The <see cref="AccessToken"/>.</returns>
    protected virtual AccessToken GenerateJwtToken(AccessTokenData tokenData)
    {
        if (tokenData == null)
        {
            throw new ArgumentNullException(nameof(tokenData));
        }

        if (this.Options.Authentication.Jwt.PrivateKey == null)
        {
            return null;
        }

        var appId = tokenData.AppId ?? DEFAULT_APP_ID;

        var claims = new Collection<Claim>
            {
                new(ClaimTypesExtended.AppId, appId),
                new(JwtRegisteredClaimNames.Jti, tokenData.Id),
                new(JwtRegisteredClaimNames.Sub, tokenData.UserId),
                new(JwtRegisteredClaimNames.Name, tokenData.Username),
                new(JwtRegisteredClaimNames.Email, tokenData.UserEmail),
                new(ClaimTypesExtended.ExternalProviderName, tokenData.ExternalToken?.Name ?? string.Empty),
                new(ClaimTypesExtended.ExternalProviderToken, tokenData.ExternalToken?.Token ?? string.Empty),
                new(ClaimTypesExtended.ExternalProviderRefreshToken, tokenData.ExternalToken?.RefreshToken ?? string.Empty)
            }
            .Union(tokenData.Claims)
            .Distinct();

        var notBeforeAt = DateTimeOffset.UtcNow;
        var expireAt = DateTimeOffset.UtcNow.AddMinutes(this.Options.Authentication.Jwt.ExpirationInMinutes);

        var securityKey = RSA.Create();
        var privateKey = Convert.FromBase64String(this.Options.Authentication.Jwt.PrivateKey);

        securityKey
            .ImportRSAPrivateKey(privateKey, bytesRead: out _);

        var rsaSecurityKey = new RsaSecurityKey(securityKey);

        var signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha512);
        var securityToken = new JwtSecurityToken(this.Options.Authentication.Jwt.Issuer, this.Options.Authentication.Jwt.Issuer, claims, notBeforeAt.DateTime, expireAt.DateTime, signingCredentials);

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

    private async Task<ExternalLogInData> GetExternalProviderLoginDataGoogle<TProvider>(TProvider logInExternalProvider, GoogleOptions externalLoginOptions)
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
    private async Task<ExternalLogInData> GetExternalProviderLoginDataFacebook<TProvider>(TProvider logInExternalProvider, FacebookOptions externalLoginOptions, CancellationToken cancellationToken = default)
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
    private async Task<ExternalLogInData> GetExternalProviderLoginDataMicrosoft<TProvider>(TProvider logInExternalProvider, MicrosoftOptions externalLoginOptions, CancellationToken cancellationToken = default)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errors"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="AggregateException"></exception>
    protected void ThrowIdentityExceptions(IEnumerable<IdentityError> errors)
    {
        if (errors == null)
            throw new ArgumentNullException(nameof(errors));

        var exceptions = errors
            .Select(x => new TranslationException(x.Description));

        throw new AggregateException(exceptions);
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public abstract class BaseIdentityAuthRepository<TIdentity> : BaseIdentityAuthRepository, IIdentityAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Db Context.
    /// </summary>
    protected virtual DbContext DbContext { get; }

    /// <summary>
    /// Sign In Manager.
    /// </summary>
    protected virtual SignInManager<IdentityUser<TIdentity>> SignInManager { get; }

    /// <summary>
    /// User Manager.
    /// </summary>
    protected virtual UserManager<IdentityUser<TIdentity>> UserManager { get; }

    /// <summary>
    /// Role Manager.
    /// </summary>
    protected virtual RoleManager<IdentityRole<TIdentity>> RoleManager { get; }

    /// <summary>
    /// The user authenticates and on success recieves a jwt token for use with auhtorization.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    /// <param name="dbContext">The <see cref="SignInManager{T}"/>.</param>
    /// <param name="signInManager">The <see cref="SignInManager{T}"/>.</param>
    /// <param name="userManager">The <see cref="UserManager{T}"/>.</param>
    /// <param name="roleManager">The <see cref="RoleManager{T}"/></param>
    /// <param name="options">The <see cref="IdentityOptions"/>.</param>
    protected BaseIdentityAuthRepository(ILogger logger, DbContext dbContext, SignInManager<IdentityUser<TIdentity>> signInManager, RoleManager<IdentityRole<TIdentity>> roleManager, UserManager<IdentityUser<TIdentity>> userManager, IdentityOptions options)
        : base(logger, options)
    {
        this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        this.UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        this.RoleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
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

        var result = await this.SignInManager
            .PasswordSignInAsync(logIn.Username, logIn.Password, logIn.IsRememberMe, true /* BUG: this is not correct to use here, check other usages: this.Options.Lockout.AllowedForNewUsers*/);

        if (result.Succeeded)
        {
            var appId = logIn.AppId ?? DEFAULT_APP_ID;

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

        var appId = logInExternalDirect.AppId ?? DEFAULT_APP_ID;

        await this.SignInManager
            .SignInAsync(identityUser, logInExternalDirect.IsRememberMe);

        return await this.GenerateJwtToken(identityUser, appId, logInExternalDirect.IsRefreshable, logInExternalDirect.ExternalLogInData.ExternalToken.Name, logInExternalDirect.ExternalLogInData.ExternalToken.Token, logInExternalDirect.ExternalLogInData.ExternalToken.RefreshToken, logInExternalDirect.TransientClaims, logInExternalDirect.TransientRoles, cancellationToken);
    }

    // BUG: SignInExternalRefresh (refresh externally and then make new token if suceededs. Looks like it's already implemented. Check it.

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
        var privateKey = Convert.FromBase64String(this.Options.Authentication.Jwt.PublicKey);

        securityKey
            .ImportRSAPublicKey(privateKey, bytesRead: out _);

        var rsaSecurityKey = new RsaSecurityKey(securityKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = this.Options.Authentication.Jwt.Issuer,
            ValidAudience = this.Options.Authentication.Jwt.Audience,
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

        var appName = appClaim?.Value ?? DEFAULT_APP_ID;

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
            Username = identityUser.UserName,
            UserEmail = identityUser.Email,
            ExternalToken = new ExternalLoginTokenData
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
            .AddHours(this.Options.Authentication.Jwt.RefreshExpirationInHours);

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

        var externalLoginOptions = this.Options.Authentication.Jwt.ExternalLogins.Microsoft;

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

    /// <summary>
    /// Gets all claims of a user.
    /// </summary>
    /// <param name="identityUser"></param>
    /// <param name="transientRoles"></param>
    /// <param name="transientClaims"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<IList<Claim>> GetAllClaims(IdentityUser<TIdentity> identityUser, IEnumerable<string> transientRoles = null, IDictionary<string, string> transientClaims = null, CancellationToken cancellationToken = default)
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

    private string GetRandomToken()
    {
        var bytes = new byte[32];

        using var generator = RandomNumberGenerator.Create();

        generator
            .GetBytes(bytes);

        var token = Convert.ToBase64String(bytes);

        return token;
    }
}