using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Api.Config;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nano.App.Api.Mvc.Authentication;

// BUG: 000: Look into response from Authenticate / Authenticate Refresh, we need to support more and custom data return. Maybe we can return claims.
// Also look at direct methods if we need to add claims for something if we change the response
// BASICALLY, CAN WE RETURN MORE DATA AS CLAIMS AND THEN THEY SHOULD BE ADDED TO SIGNUP / LOGON ???????

/// <inheritdoc />
public class AuthExternalMicrosoftRepository(MicrosoftOptions options, HttpClient httpClient)
    : BaseAuthExternalRepository<ExternalProviderMicrosoft>(ExternalLogInProviderNames.MICROSOFT)
{
    private readonly MicrosoftOptions options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly HttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public override Task<ExternalLogInData> AuthenticateAsync(ExternalProviderMicrosoft provider, ImplicitFlow auth, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override async Task<ExternalLogInData> AuthenticateAsync(ExternalProviderMicrosoft provider, AuthCodeFlow auth, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var tokenHandler = new JwtSecurityTokenHandler();

        string accessToken;
        string? refreshToken;

        using var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.RequestUri = new Uri($"https://login.microsoftonline.com/{this.options.TenantId}/oauth2/v2.0/token");

        using var formContent = new MultipartFormDataContent();

        formContent.Add(new StringContent(this.options.ClientId), "client_id");
        formContent.Add(new StringContent(this.options.ClientSecret), "client_secret");
        formContent.Add(new StringContent("authorization_code"), "grant_type");
        formContent.Add(new StringContent(auth.Code), "code");
        formContent.Add(new StringContent(auth.CodeVerifier), "code_verifier");
        formContent.Add(new StringContent(auth.RedirectUri), "redirect_uri");
        formContent.Add(new StringContent(this.options.Scopes.Aggregate(string.Empty, (current, x) => current + $"{x} ")), "scope");

        httpRequestMessage.Content = formContent;

        var httpResponse = await httpClient
            .SendAsync(httpRequestMessage, cancellationToken);

        var stringContent = await httpResponse.Content
            .ReadAsStringAsync(cancellationToken);

        var content = JsonConvert.DeserializeObject<JObject>(stringContent);

        var error = content?["error"]?.ToString();
        var errorDescription = content?["error"]?.ToString() ?? "Unknown";

        if (error != null)
        {
            throw new InvalidOperationException($"{error}: {errorDescription}");
        }

        accessToken = content?["access_token"]?.ToString() ?? throw new NullReferenceException(nameof(accessToken));
        refreshToken = content["refresh_token"]?.ToString();


        var jwtToken = tokenHandler
            .ReadJwtToken(accessToken);

        var id = jwtToken?.Payload
            .Where(x => x.Key == "oid").Select(x => x.Value?.ToString())
            .FirstOrDefault();

        if (id == null)
        {
            throw new NullReferenceException(nameof(id));
        }

        var name = jwtToken?.Payload
            .Where(x => x.Key == "name").Select(x => x.Value?.ToString())
            .FirstOrDefault();

        if (name == null)
        {
            throw new NullReferenceException(nameof(id));
        }

        var email = jwtToken?.Payload
            .Where(x => x.Key == "upn").Select(x => x.Value?.ToString())
            .FirstOrDefault();

        if (email == null)
        {
            throw new NullReferenceException(nameof(id));
        }

        return new ExternalLogInData
        {
            Id = id,
            Name = name,
            Email = email,
            Username = email,
            ExternalToken =
            {
                Name = ExternalLogInProviderNames.MICROSOFT,
                Token = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    /// <inheritdoc />
    public override async Task<ExternalLoginTokenData> AuthenticateRefreshAsync(ExternalProviderMicrosoft provider, string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(refreshToken);

        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.RequestUri = new Uri($"https://login.microsoftonline.com/{this.options.TenantId}/oauth2/v2.0/token");

        using var formContent = new MultipartFormDataContent();

        formContent.Add(new StringContent(this.options.ClientId), "client_id");
        formContent.Add(new StringContent(this.options.ClientSecret), "client_secret");
        formContent.Add(new StringContent("refresh_token"), "grant_type");
        formContent.Add(new StringContent(refreshToken), "refresh_token");
        formContent.Add(new StringContent(this.options.Scopes.Aggregate(string.Empty, (current, x) => current + $"{x} ")), "scope");

        httpRequestMessage.Content = formContent;

        var httpResponse = await this.httpClient
            .SendAsync(httpRequestMessage, cancellationToken);

        var stringContent = await httpResponse.Content
            .ReadAsStringAsync(cancellationToken);

        var content = JsonConvert.DeserializeObject<JObject>(stringContent);

        var error = content?["error"]?.ToString();
        var errorDescription = content?["error"]?.ToString() ?? "Unknown";

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

        return new ExternalLoginTokenData
        {
            Name = provider.Name,
            Token = accessToken,
            RefreshToken = refreshTokenNew
        };
    }
}