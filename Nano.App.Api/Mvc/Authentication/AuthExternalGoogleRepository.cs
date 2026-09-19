using Google.Apis.Auth;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc cref="BaseAuthExternalRepository{TFlow}" />
public class AuthExternalGoogleRepository(GoogleOptions options, HttpClient httpClient)
    : BaseAuthExternalRepository<AuthCodeFlow>(BuiltInExternalLogInProviderNames.GOOGLE), IBuiltInAuthExternalRepository
{
    private readonly GoogleOptions options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly HttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public override async Task<ExternalAuthenticationData> AuthenticateAsync(AuthCodeFlow flow, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(flow);

        using var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.RequestUri = new Uri("https://oauth2.googleapis.com/token");

        httpRequestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = this.options.ClientId,
            ["client_secret"] = this.options.ClientSecret,
            ["grant_type"] = "authorization_code",
            ["code"] = flow.Code,
            ["code_verifier"] = flow.CodeVerifier,
            ["redirect_uri"] = flow.RedirectUri
        });

        var httpResponse = await this.httpClient
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

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience =
            [
                this.options.ClientId
            ]
        };

        var payload = await GoogleJsonWebSignature
            .ValidateAsync(idToken, settings);

        return new ExternalAuthenticationData
        {
            Id = payload.Subject,
            Name = payload.Name,
            EmailAddress = payload.Email,
            Username = payload.Email,
            ExternalToken = new ExternalAuthenticationToken
            {
                Name = BuiltInExternalLogInProviderNames.GOOGLE,
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
        httpRequestMessage.RequestUri = new Uri("https://oauth2.googleapis.com/token");

        httpRequestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = this.options.ClientId,
            ["client_secret"] = this.options.ClientSecret,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken
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

        // Google does not rotate refresh tokens - a refresh grant response omits refresh_token, so the
        // original one (still valid) is carried forward instead of being dropped.
        var refreshTokenNew = content?["refresh_token"]?.ToString() ?? refreshToken;

        return new ExternalAuthenticationToken
        {
            Name = BuiltInExternalLogInProviderNames.GOOGLE,
            Token = accessToken,
            RefreshToken = refreshTokenNew
        };
    }
}