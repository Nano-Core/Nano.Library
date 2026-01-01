using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.Common.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.Data.Identity.Authentication;

// TODO: Api-Key Authentication across services? 

/// <summary>
/// Api Key Authentication Handler.
/// </summary>
public class ApiKeyAuthenticationHandler<TIdentity> : AuthenticationHandler<AuthenticationSchemeOptions>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IIdentityRepository<TIdentity> identityRepository;

    /// <inheritdoc />
    public ApiKeyAuthenticationHandler(ILoggerFactory loggerFactory, IOptionsMonitor<AuthenticationSchemeOptions> options, UrlEncoder encoder, IIdentityRepository<TIdentity> identityManager)
        : base(options, loggerFactory, encoder)
    {
        this.identityRepository = identityManager ?? throw new ArgumentNullException(nameof(identityManager));
    }

    /// <inheritdoc />
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var success = this.Request.Headers
            .TryGetValue(ApiKeyHeaderNames.X_API_KEY, out var apiKeyHeaderValues);

        if (!success)
        {
            return AuthenticateResult.NoResult();
        }

        var apiKey = apiKeyHeaderValues
            .FirstOrDefault();

        if (apiKey == null)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        var identityApiKey = await this.identityRepository
            .ValidateApiKeyAsync(apiKey);

        if (identityApiKey == null)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        var identityUser = await this.identityRepository
            .GetIdentityUserOrDefaultAsync(identityApiKey.IdentityUserId);

        if (identityUser == null)
        {
            return AuthenticateResult.Fail("User not found");
        }

        var transientClaims = new Dictionary<string, string>
        {
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
            { JwtRegisteredClaimNames.Sub, identityUser.Id.ToString() },
            { JwtRegisteredClaimNames.Name, identityUser.UserName },
            { JwtRegisteredClaimNames.Email, identityUser.Email },
            { ClaimTypesExtended.ApiKeyId, identityApiKey.Id.ToString() },
            { ClaimTypesExtended.ApiKeyName, identityApiKey.Name }
        };

        var claims = await this.identityRepository
            .GetAllClaims(identityUser, transientClaims: transientClaims);

        var identity = new ClaimsIdentity(claims, nameof(ApiKeyAuthenticationHandler<TIdentity>));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}