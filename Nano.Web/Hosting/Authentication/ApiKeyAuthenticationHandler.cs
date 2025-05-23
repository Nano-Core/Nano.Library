﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.Security;
using Nano.Web.Hosting.Authentication.Const;

namespace Nano.Web.Hosting.Authentication;

/// <summary>
/// Api Key Authentication Handler.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string API_KEY_HEADER_NAME = "x-api-key";

    private readonly DefaultIdentityManager identityManager; // TODO: Make base

    /// <inheritdoc />
    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, DefaultIdentityManager identityManager)
        : base(options, logger, encoder)
    {
        this.identityManager = identityManager ?? throw new ArgumentNullException(nameof(identityManager));
    }

    /// <inheritdoc />
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var success = this.Request.Headers
            .TryGetValue(API_KEY_HEADER_NAME, out var apiKeyHeaderValues);

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

        var identityApiKey = await this.identityManager
            .ValidateApiKeyAsync(apiKey);

        if (identityApiKey == null)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        var identityUser = await this.identityManager
            .GetIdentityUserOrDefaultAsync(identityApiKey.IdentityUserId);

        if (identityUser == null)
        {
            return AuthenticateResult.Fail("User not found");
        }

        var transientClaims = new Dictionary<string, string>
        {
            { ApiKeyClaimTypes.UserId, identityUser.Id.ToString() },
            { ApiKeyClaimTypes.UserEmail, identityUser.Email },
            { ApiKeyClaimTypes.ApiKeyId, identityApiKey.Id.ToString() },
            { ApiKeyClaimTypes.ApiKeyName, identityApiKey.Name }
        };

        var claims = await this.identityManager
            .GetAllClaims(identityUser, transientClaims: transientClaims);

        var identity = new ClaimsIdentity(claims, nameof(ApiKeyAuthenticationHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}