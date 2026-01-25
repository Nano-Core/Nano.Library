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
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Consts;

namespace Nano.Data.Identity.Authentication;

/// <summary>
/// Handles API key authentication for requests.
/// Validates the API key provided in the request headers and creates a <see cref="ClaimsPrincipal"/>
/// if the API key and associated user are valid.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key, e.g., <see cref="Guid"/> or <see cref="string"/>.</typeparam>
public class ApiKeyAuthenticationHandler<TIdentity> : AuthenticationHandler<AuthenticationSchemeOptions>
    where TIdentity : IEquatable<TIdentity>
{
    private readonly IIdentityRepository<TIdentity> identityRepository;

    /// <summary>
    /// Initializes a new instance of <see cref="ApiKeyAuthenticationHandler{TIdentity}"/>.
    /// </summary>
    /// <param name="loggerFactory">The logger factory used to create loggers.</param>
    /// <param name="options">The authentication scheme options.</param>
    /// <param name="encoder">The URL encoder.</param>
    /// <param name="identityManager">The identity repository used to validate API keys and retrieve users.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="identityManager"/> is null.</exception>
    public ApiKeyAuthenticationHandler(ILoggerFactory loggerFactory, IOptionsMonitor<AuthenticationSchemeOptions> options, UrlEncoder encoder, IIdentityRepository<TIdentity> identityManager)
        : base(options, loggerFactory, encoder)
    {
        this.identityRepository = identityManager ?? throw new ArgumentNullException(nameof(identityManager));
    }

    /// <summary>
    /// Handles the authentication process for an incoming HTTP request.
    /// </summary>
    /// <returns>
    /// An <see cref="AuthenticateResult"/> indicating success, failure, or no result.
    /// <list type="bullet">
    /// <item>If the API key header is missing, returns <see cref="AuthenticateResult.NoResult"/>.</item>
    /// <item>If the API key is invalid, returns <see cref="AuthenticateResult.Fail(string)"/>.</item>
    /// <item>If the associated user is not found, returns <see cref="AuthenticateResult.Fail(string)"/>.</item>
    /// <item>If the API key and user are valid, returns <see cref="AuthenticateResult.Success"/> with a <see cref="ClaimsPrincipal"/>.</item>
    /// </list>
    /// </returns>
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
            { JwtRegisteredClaimNames.Sub, identityUser.Id.ToString() ?? "" },
            { JwtRegisteredClaimNames.Name, identityUser.UserName ?? "" },
            { JwtRegisteredClaimNames.Email, identityUser.Email ?? "" },
            { ClaimTypesExtended.ApiKeyId, identityApiKey.Id.ToString() ?? "" },
            { ClaimTypesExtended.ApiKeyName, identityApiKey.Name }
        };

        var claims = await this.identityRepository
            .GetAllClaims(identityUser, transientClaims: transientClaims);

        var identity = new ClaimsIdentity(claims, nameof(ApiKeyAuthenticationHandler<>));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}