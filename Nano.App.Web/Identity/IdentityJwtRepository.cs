using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Nano.App.Web.Identity.Abstractions;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Models;
using Nano.Data.Extensions;
using Nano.Security.Exceptions;
using IdentityOptions = Nano.Web.IdentityOptions;

namespace Nano.App.Web.Identity;

/// <summary>
/// 
/// </summary>
public class IdentityJwtRepository<TIdentity> : IIdentityJwtRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    private const string DEFAULT_APP_ID = "Default";

    /// <summary>
    /// 
    /// </summary>
    protected virtual IdentityOptions Options { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public IdentityJwtRepository(IdentityOptions options)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual Task<JwtToken> GenerateJwtToken(GenerateJwtToken<TIdentity> generateJwtToken, CancellationToken cancellationToken = default)
    {
        if (generateJwtToken == null)
            throw new ArgumentNullException(nameof(generateJwtToken));

        if (generateJwtToken == null) 
            throw new ArgumentNullException(nameof(generateJwtToken));

        generateJwtToken.AppId ??= DEFAULT_APP_ID;

        var accessTokenData = new AccessTokenData<TIdentity>
        {
            AppId = generateJwtToken.AppId,
            UserId = generateJwtToken.UserId,
            Username = generateJwtToken.UserName,
            UserEmail = generateJwtToken.Email,
            ExternalToken = new ExternalLoginTokenData
            {
                Name = generateJwtToken.ExternalToken.Name,
                Token = generateJwtToken.ExternalToken.Token,
                RefreshToken = generateJwtToken.ExternalToken.RefreshToken
            },
            Claims = generateJwtToken.Claims
        };

        return this.GenerateJwtToken(accessTokenData);
    }

    /// <inheritdoc />
    public virtual async Task<JwtToken> GenerateJwtToken(AccessTokenData<TIdentity> accessTokenData)
    {
        if (accessTokenData == null)
            throw new ArgumentNullException(nameof(accessTokenData));

        await Task.CompletedTask;

        var claims = new Collection<Claim>
            {
                new(ClaimTypesExtended.AppId, accessTokenData.AppId),
                new(JwtRegisteredClaimNames.Jti, accessTokenData.Id),
                new(JwtRegisteredClaimNames.Sub, accessTokenData.UserId.ToString() ?? string.Empty),
                new(JwtRegisteredClaimNames.Name, accessTokenData.Username),
                new(JwtRegisteredClaimNames.Email, accessTokenData.UserEmail),
                new(ClaimTypesExtended.ExternalProviderName, accessTokenData.ExternalToken?.Name ?? string.Empty),
                new(ClaimTypesExtended.ExternalProviderToken, accessTokenData.ExternalToken?.Token ?? string.Empty),
                new(ClaimTypesExtended.ExternalProviderRefreshToken, accessTokenData.ExternalToken?.RefreshToken ?? string.Empty)
            }
            .Union(accessTokenData.Claims)
            .Distinct();

        var notBeforeAt = DateTimeOffset.UtcNow;
        var expireAt = DateTimeOffset.UtcNow.AddMinutes(this.Options.Authentication.Jwt.ExpirationInMinutes);

        var rsaSecurityKey = this.Options.Authentication.Jwt.PrivateKey
            .CreateRsaSecurityKey();

        var signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha512);
        var securityToken = new JwtSecurityToken(this.Options.Authentication.Jwt.Issuer, this.Options.Authentication.Jwt.Audience, claims, notBeforeAt.DateTime, expireAt.DateTime, signingCredentials);

        var token = new JwtSecurityTokenHandler()
            .WriteToken(securityToken);

        return new JwtToken
        {
            AppId = accessTokenData.AppId ?? DEFAULT_APP_ID,
            UserId = accessTokenData.UserId.ToString(),
            Token = token,
            ExpireAt = expireAt
        };
    }

    /// <inheritdoc />
    public virtual async Task<JwtToken> GenerateJwtTokenByRefreshAsync(IdentityUser<TIdentity> identityUser, LogInRefresh logInRefresh, IEnumerable<Claim> claims, RefreshToken refreshToken = null, CancellationToken cancellationToken = default)
    {
        if (identityUser == null) 
            throw new ArgumentNullException(nameof(identityUser));
        
        if (logInRefresh == null)
            throw new ArgumentNullException(nameof(logInRefresh));

        var rsaSecurityKey = this.Options.Authentication.Jwt.PublicKey
            .CreateRsaSecurityKey();

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
            throw new UnauthorizedAccessException("The jwt token is invalid.");
        }

        if (identityUser == null)
        {
            throw new UnauthorizedException($"The user: '{identityUser.UserName}' was not found or is deactivated.");
        }

        var appClaim = principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypesExtended.AppId);

        var externalProviderName = principal.Claims
            .Where(x => x.Type == ClaimTypesExtended.ExternalProviderName)
            .Select(x => x.Value)
            .FirstOrDefault();

        var externalProviderRefreshToken = principal.Claims
            .Where(x => x.Type == ClaimTypesExtended.ExternalProviderRefreshToken)
            .Select(x => x.Value)
            .FirstOrDefault();

        var externalProviderData = await this.RefreshExternalProviderTokenOrDefault(externalProviderName, externalProviderRefreshToken, cancellationToken);

        var appId = appClaim?.Value ?? DEFAULT_APP_ID;

        var jwtToken = await this.GenerateJwtToken(new AccessTokenData<TIdentity>
        {
            AppId = appId,
            UserId = identityUser.Id,
            Username = identityUser.UserName,
            UserEmail = identityUser.Email,
            ExternalToken = new ExternalLoginTokenData
            {
                Name = externalProviderData.Name,
                RefreshToken = externalProviderData.RefreshToken,
                Token = externalProviderData.Token
            },
            Claims = claims
        });

        return jwtToken;
    }


    private async Task<ExternalLoginTokenData> RefreshExternalProviderTokenOrDefault(string name = null, string externalRefreshToken = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(name))
        {
            return new ExternalLoginTokenData();
        }

        if (string.IsNullOrEmpty(externalRefreshToken))
        {
            return new ExternalLoginTokenData();
        }

        return name switch
        {
            "Facebook" => await this.RefreshExternalProviderTokenFacebook(name, externalRefreshToken, cancellationToken),
            "Google" => await this.RefreshExternalProviderTokenGoogle(name, externalRefreshToken, cancellationToken),
            "Microsoft" => await this.RefreshExternalProviderTokenMicrosoft(name, externalRefreshToken, cancellationToken),
            _ => throw new NotSupportedException($"The external provider: {name} is not supported.")
        };
    }
    private async Task<ExternalLoginTokenData> RefreshExternalProviderTokenGoogle(string name, string externalRefreshToken = null, CancellationToken cancellationToken = default)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (externalRefreshToken == null)
            throw new ArgumentNullException(nameof(externalRefreshToken));

        await Task.CompletedTask;

        throw new NotImplementedException();
    }
    private async Task<ExternalLoginTokenData> RefreshExternalProviderTokenFacebook(string name, string externalRefreshToken = null, CancellationToken cancellationToken = default)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (externalRefreshToken == null)
            throw new ArgumentNullException(nameof(externalRefreshToken));

        await Task.CompletedTask;

        throw new NotImplementedException();
    }
    private async Task<ExternalLoginTokenData> RefreshExternalProviderTokenMicrosoft(string name, string externalRefreshToken = null, CancellationToken cancellationToken = default)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (externalRefreshToken == null)
            throw new ArgumentNullException(nameof(externalRefreshToken));

        if (string.IsNullOrEmpty(externalRefreshToken))
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
                    formContent.Add(new StringContent(externalRefreshToken), "refresh_token");
                    formContent.Add(new StringContent(externalLoginOptions.Scopes.Aggregate(string.Empty, (current, x) => current + $"{x} ")), "scope");

                    httpRequestMessage.Content = formContent;

                    var httpResponse = await httpClient
                        .SendAsync(httpRequestMessage, cancellationToken);

                    var stringContent = await httpResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    var content = JsonSerializer.Deserialize<JsonObject>(stringContent); // TODO: Microsoft serialization. change

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
                        Name = name,
                        Token = accessToken,
                        RefreshToken = refreshToken
                    };
                }
            }
        }
    }
}