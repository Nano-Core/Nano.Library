using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Authentication.Extensions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Exceptions;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc />
public class AuthJwtRepository(JwtAuthenticationOptions options) : IAuthJwtRepository
{
    private readonly JwtAuthenticationOptions options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public virtual AccessToken GenerateJwtToken(GenerateJwtToken generateJwtToken)
    {
        ArgumentNullException.ThrowIfNull(generateJwtToken);

        var appId = generateJwtToken.AppId ?? IdentityDefaults.DEFAULT_APP_ID;

        var claims = new Collection<Claim>
            {
                new(ClaimTypesExtended.AppId, appId),
                new(JwtRegisteredClaimNames.Jti, generateJwtToken.Id),
                new(JwtRegisteredClaimNames.Sub, generateJwtToken.UserId ?? string.Empty),
                new(JwtRegisteredClaimNames.Name, generateJwtToken.UserName ?? string.Empty),
                new(JwtRegisteredClaimNames.Email, generateJwtToken.UserEmail ?? string.Empty),
                new(ClaimTypesExtended.ExternalProviderName, generateJwtToken.ExternalToken?.Name ?? string.Empty),
                new(ClaimTypesExtended.ExternalProviderToken, generateJwtToken.ExternalToken?.Token ?? string.Empty),
                new(ClaimTypesExtended.ExternalProviderRefreshToken, generateJwtToken.ExternalToken?.RefreshToken ?? string.Empty)
            }
            .Union(generateJwtToken.Claims)
            .Distinct();

        var notBeforeAt = DateTimeOffset.UtcNow;
        var expireAt = DateTimeOffset.UtcNow.Add(this.options.Expiration);

        var rsaSecurityKey = this.options.PrivateKey?
            .CreatePrivateRsaSecurityKey();

        var signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha512);
        var securityToken = new JwtSecurityToken(this.options.Issuer, this.options.Audience, claims, notBeforeAt.DateTime, expireAt.DateTime, signingCredentials);

        var token = new JwtSecurityTokenHandler()
            .WriteToken(securityToken);

        return new AccessToken
        {
            AppId = appId,
            UserId = generateJwtToken.UserId,
            Token = token,
            ExpireAt = expireAt
        };
    }

    /// <inheritdoc />
    public virtual RefreshToken GenerateJwtRefreshToken()
    {
        var token = GetRandomToken();

        var expireAt = DateTimeOffset.UtcNow.Add(this.options.RefreshExpiration);

        return new RefreshToken
        {
            Token = token,
            ExpireAt = expireAt
        };
    }

    /// <inheritdoc />
    public virtual void ValidateTokenForRefresh(string refreshToken)
    {
        var rsaSecurityKey = this.options.PublicKey
            .CreatePublicRsaSecurityKey();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = this.options.Issuer,
            ValidAudience = this.options.Audience,
            IssuerSigningKey = rsaSecurityKey,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        var securityTokenHandler = new JwtSecurityTokenHandler();

        securityTokenHandler
            .ValidateToken(refreshToken, validationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha512, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedException("Invalid jwt token.");
        }
    }


    private static string GetRandomToken()
    {
        var bytes = new byte[32];

        using var generator = RandomNumberGenerator.Create();

        generator
            .GetBytes(bytes);

        var token = Convert.ToBase64String(bytes);

        return token;
    }
}