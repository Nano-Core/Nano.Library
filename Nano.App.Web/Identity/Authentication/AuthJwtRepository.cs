using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Nano.App.Web.Config;
using Nano.Common.Config;
using Nano.Data.Abstractions.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Consts;
using Nano.Data.Abstractions.Identity.Exceptions;

namespace Nano.App.Web.Identity.Authentication;

/// <inheritdoc />
public class AuthJwtRepository : IAuthJwtRepository
{
    /// <summary>
    /// 
    /// </summary>
    protected virtual JwtAuthenticationOptions Options { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AuthJwtRepository(JwtAuthenticationOptions options)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual AccessToken GenerateJwtToken(GenerateJwtToken generateJwtToken)
    {
        if (generateJwtToken == null)
            throw new ArgumentNullException(nameof(generateJwtToken));

        var claims = new Collection<Claim>
            {
                new(ClaimTypesExtended.AppId, generateJwtToken.AppId ?? IdentityDefaults.DEFAULT_APP_ID),
                new(JwtRegisteredClaimNames.Jti, generateJwtToken.Id),
                new(JwtRegisteredClaimNames.Sub, generateJwtToken.UserId),
                new(JwtRegisteredClaimNames.Name, generateJwtToken.UserName),
                new(JwtRegisteredClaimNames.Email, generateJwtToken.UserEmail),
                new(ClaimTypesExtended.ExternalProviderName, generateJwtToken.ExternalToken?.Name ?? string.Empty),
                new(ClaimTypesExtended.ExternalProviderToken, generateJwtToken.ExternalToken?.Token ?? string.Empty),
                new(ClaimTypesExtended.ExternalProviderRefreshToken, generateJwtToken.ExternalToken?.RefreshToken ?? string.Empty)
            }
            .Union(generateJwtToken.Claims)
            .Distinct();

        var notBeforeAt = DateTimeOffset.UtcNow;
        var expireAt = DateTimeOffset.UtcNow.AddMinutes(this.Options.ExpirationInMinutes);

        var rsaSecurityKey = this.CreateRsaSecurityKey();
        var signingCredentials = new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha512);
        var securityToken = new JwtSecurityToken(this.Options.Issuer, this.Options.Audience, claims, notBeforeAt.DateTime, expireAt.DateTime, signingCredentials);

        var token = new JwtSecurityTokenHandler()
            .WriteToken(securityToken);

        return new AccessToken
        {
            AppId = generateJwtToken.AppId ?? IdentityDefaults.DEFAULT_APP_ID,
            UserId = generateJwtToken.UserId,
            Token = token,
            ExpireAt = expireAt
        };
    }

    /// <inheritdoc />
    public virtual void ValidateRefreshToken(string refreshToken)
    {
        var rsaSecurityKey = this.CreateRsaSecurityKey();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = this.Options.Issuer,
            ValidAudience = this.Options.Audience,
            IssuerSigningKey = rsaSecurityKey,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        var securityTokenHandler = new JwtSecurityTokenHandler();

        securityTokenHandler
            .ValidateToken(refreshToken, validationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha512, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new UnauthorizedException("The jwt token is invalid.");
        }
    }


    private RsaSecurityKey CreateRsaSecurityKey()
    {
        var base64 = Convert.FromBase64String(this.Options.PrivateKey);

        var rsaAlgorithm = RSA.Create();

        rsaAlgorithm
            .ImportRSAPublicKey(base64, out _);

        return new RsaSecurityKey(rsaAlgorithm);
    }
}