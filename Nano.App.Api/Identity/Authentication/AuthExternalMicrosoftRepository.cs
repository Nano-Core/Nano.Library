using Nano.App.Api.Config;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Nano.Data.Abstractions.Identity.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Consts;

namespace Nano.App.Api.Identity.Authentication;

/// <inheritdoc />
public class AuthExternalMicrosoftRepository : IAuthExternalMicrosoftRepository
{
    private readonly MicrosoftOptions options;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="httpClient"></param>
    public AuthExternalMicrosoftRepository(MicrosoftOptions options, HttpClient httpClient)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <inheritdoc />
    public virtual async Task<ExternalLogInData> Authenticate(ExternalLoginProviderMicrosoft provider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var tokenHandler = new JwtSecurityTokenHandler();

        string accessToken;
        string? refreshToken;

        switch (provider)
        {
            case ExternalLoginProviderAuthCode authCodeLogin:
            {
                using var httpRequestMessage = new HttpRequestMessage();

                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.RequestUri = new Uri($"https://login.microsoftonline.com/{options.TenantId}/oauth2/v2.0/token");

                using var formContent = new MultipartFormDataContent();

                formContent.Add(new StringContent(options.ClientId), "client_id");
                formContent.Add(new StringContent(options.ClientSecret), "client_secret");
                formContent.Add(new StringContent("authorization_code"), "grant_type");
                formContent.Add(new StringContent(authCodeLogin.Code), "code");
                formContent.Add(new StringContent(authCodeLogin.CodeVerifier), "code_verifier");
                formContent.Add(new StringContent(authCodeLogin.RedirectUri), "redirect_uri");
                formContent.Add(new StringContent(options.Scopes.Aggregate(string.Empty, (current, x) => current + $"{x} ")), "scope");

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

                break;
            }

            default:
                throw new NotSupportedException(provider.GetType().Name);
        }

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
            ExternalToken =
            {
                Name = ExternalLogInProviderNames.MICROSOFT,
                Token = accessToken,
                RefreshToken = refreshToken
            }
        };
    }

    /// <inheritdoc />
    public virtual async Task<ExternalLoginTokenData> AuthenticateRefresh(LogInExternalRefreshMicrosoft logInExternalRefresh, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logInExternalRefresh);

        var httpRequestMessage = new HttpRequestMessage();

        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.RequestUri = new Uri($"https://login.microsoftonline.com/{this.options.TenantId}/oauth2/v2.0/token");

        using var formContent = new MultipartFormDataContent();

        formContent.Add(new StringContent(this.options.ClientId), "client_id");
        formContent.Add(new StringContent(this.options.ClientSecret), "client_secret");
        formContent.Add(new StringContent("refresh_token"), "grant_type");
        formContent.Add(new StringContent(logInExternalRefresh.RefreshToken), "refresh_token");
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

        var refreshToken = content?["refresh_token"]?.ToString();

        return new ExternalLoginTokenData
        {
            Name = logInExternalRefresh.ProviderName,
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }
}