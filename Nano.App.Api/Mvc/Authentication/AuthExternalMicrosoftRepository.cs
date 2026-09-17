using Nano.App.Api.Config;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Api.Mvc.Authentication.Abstractions;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc cref="BaseAuthExternalRepository{TFlow}" />
public class AuthExternalMicrosoftRepository(MicrosoftOptions options, HttpClient httpClient)
    : BaseAuthExternalRepository<AuthCodeFlow>(BuiltInExternalLogInProviderNames.MICROSOFT), IBuiltInAuthExternalRepository
{
    private readonly MicrosoftOptions options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly HttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public override async Task<ExternalAuthenticationData> AuthenticateAsync(AuthCodeFlow flow, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(flow);

        var tokenHandler = new JwtSecurityTokenHandler();

        using var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.RequestUri = new Uri($"https://login.microsoftonline.com/{this.options.TenantId}/oauth2/v2.0/token");

        httpRequestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = this.options.ClientId,
            ["client_secret"] = this.options.ClientSecret,
            ["grant_type"] = "authorization_code",
            ["code"] = flow.Code,
            ["code_verifier"] = flow.CodeVerifier,
            ["redirect_uri"] = flow.RedirectUri,
            ["scope"] = string.Join(" ", this.options.Scopes)
        });

        var httpResponse = await httpClient
            .SendAsync(httpRequestMessage, cancellationToken);

        var stringContent = await httpResponse.Content
            .ReadAsStringAsync(cancellationToken);

        var content = JsonConvert.DeserializeObject<JObject>(stringContent);

        if (content == null)
        {
            throw new InvalidOperationException("Token endpoint returned invalid JSON.");
        }

        var error = content["error"]?.ToString();
        var errorDescription = content["error_description"]?.ToString() ?? "Unknown";

        if (error != null)
        {
            throw new InvalidOperationException($"{error}: {errorDescription}");
        }

        var accessToken = content["access_token"]?.ToString();

        if (accessToken == null)
        {
            throw new NullReferenceException(nameof(accessToken));
        }

        var refreshToken = content["refresh_token"]?.ToString();

        var idToken = content["id_token"]?.ToString();

        if (idToken == null)
        {
            throw new NullReferenceException(nameof(idToken));
        }

        var jwtToken = tokenHandler
            .ReadJwtToken(idToken);

        var id = jwtToken.Payload
            .Where(x => x.Key == "oid")
            .Select(x => x.Value?.ToString())
            .FirstOrDefault();

        if (id == null)
        {
            throw new NullReferenceException(nameof(id));
        }

        var name = jwtToken.Payload
            .Where(x => x.Key == "name")
            .Select(x => x.Value?.ToString())
            .FirstOrDefault();

        if (name == null)
        {
            throw new NullReferenceException(nameof(name));
        }

        var email = jwtToken.Payload
            .Where(x => x.Key is "email" or "preferred_username")
            .Select(x => x.Value?.ToString())
            .FirstOrDefault();

        if (email == null)
        {
            throw new NullReferenceException(nameof(email));
        }

        return new ExternalAuthenticationData
        {
            Id = id,
            Name = name,
            EmailAddress = email,
            Username = email,
            ExternalToken = new ExternalAuthenticationToken
            {
                Name = BuiltInExternalLogInProviderNames.MICROSOFT,
                Token = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    /// <inheritdoc />
    public override async Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        using var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.RequestUri = new Uri($"https://login.microsoftonline.com/{this.options.TenantId}/oauth2/v2.0/token");

        httpRequestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = this.options.ClientId,
            ["client_secret"] = this.options.ClientSecret,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken,
            ["scope"] = string.Join(" ", this.options.Scopes)
        });

        var httpResponse = await this.httpClient
            .SendAsync(httpRequestMessage, cancellationToken);

        var stringContent = await httpResponse.Content
            .ReadAsStringAsync(cancellationToken);

        var content = JsonConvert.DeserializeObject<JObject>(stringContent);

        var error = content?["error"]?.ToString();
        var errorDescription = content?["error_description"]?.ToString() ?? "Unknown";

        if (error != null)
        {
            throw new UnauthorizedException($"{error}: {errorDescription}");
        }

        var accessToken = content?["access_token"]?.ToString();

        if (accessToken == null)
        {
            throw new NullReferenceException(nameof(accessToken));
        }

        var refreshTokenNew = content?["refresh_token"]?.ToString();

        return new ExternalAuthenticationToken
        {
            Name = BuiltInExternalLogInProviderNames.MICROSOFT,
            Token = accessToken,
            RefreshToken = refreshTokenNew
        };
    }
}