using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Nano.Common.Exceptions;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity.Models;
using IdentityOptions = Nano.Web.IdentityOptions;

namespace Nano.App.Web.Identity;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public class BaseBaseIdentityAuthRepository<TIdentity>
    where TIdentity : IEquatable<TIdentity> 
{
    /// <summary>
    /// Options.
    /// </summary>
    protected virtual IdentityOptions Options { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    protected BaseBaseIdentityAuthRepository(IdentityOptions options)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    /// <param name="logInExternalProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="UnauthorizedException"></exception>
    public virtual async Task<ExternalLogInData> GetExternalProviderLogInData<TProvider>(TProvider logInExternalProvider, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider
    {
        if (logInExternalProvider == null)
        {
            throw new ArgumentNullException(nameof(logInExternalProvider));
        }

        return logInExternalProvider.Name switch
        {
            "Google" => await this.GetExternalProviderLoginDataGoogle(logInExternalProvider, this.Options.Authentication.Jwt?.ExternalLogins?.Google),
            "Facebook" => await this.GetExternalProviderLoginDataFacebook(logInExternalProvider, this.Options.Authentication.Jwt?.ExternalLogins?.Facebook, cancellationToken),
            "Microsoft" => await this.GetExternalProviderLoginDataMicrosoft(logInExternalProvider, this.Options.Authentication.Jwt?.ExternalLogins?.Microsoft, cancellationToken),
            _ => throw new NotSupportedException(logInExternalProvider.Name)
        };
    }


    private async Task<ExternalLogInData> GetExternalProviderLoginDataGoogle<TProvider>(TProvider logInExternalProvider, GoogleOptions externalLoginOptions)
        where TProvider : BaseLogInExternalProvider
    {
        if (logInExternalProvider == null)
        {
            throw new ArgumentNullException(nameof(logInExternalProvider));
        }

        if (externalLoginOptions == null)
        {
            throw new ArgumentNullException(nameof(externalLoginOptions));
        }

        switch (logInExternalProvider)
        {
            case ExternalLoginProviderImplicit implicitLogin:
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience =
                    [
                        externalLoginOptions.ClientId
                    ]
                };

                var payload = await GoogleJsonWebSignature
                    .ValidateAsync(implicitLogin.AccessToken, settings);

                return new ExternalLogInData
                {
                    Id = payload.Subject,
                    Name = payload.Name,
                    Email = payload.Email,
                    ExternalToken =
                    {
                        Name = "Google",
                        Token = implicitLogin.AccessToken
                    }
                };

            default:
                throw new NotSupportedException(logInExternalProvider.GetType().Name);
        }
    }
    private async Task<ExternalLogInData> GetExternalProviderLoginDataFacebook<TProvider>(TProvider logInExternalProvider, FacebookOptions externalLoginOptions, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider
    {
        if (logInExternalProvider == null)
        {
            throw new ArgumentNullException(nameof(logInExternalProvider));
        }

        if (externalLoginOptions == null)
        {
            throw new ArgumentNullException(nameof(externalLoginOptions));
        }

        switch (logInExternalProvider)
        {
            case ExternalLoginProviderImplicit implicitLogin:
                using (var httpClient = new HttpClient())
                {
                    const string HOST = "https://graph.facebook.com";
                    const string FIELDS = "id,name,address,email,birthday";

                    var debugTokenResponse = await httpClient
                        .GetAsync($"{HOST}/debug_token?input_token={implicitLogin.AccessToken}&access_token={externalLoginOptions.AppId}|{externalLoginOptions.AppSecret}", cancellationToken);

                    debugTokenResponse
                        .EnsureSuccessStatusCode();

                    var debugToken = await debugTokenResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    var validation = JsonSerializer.Deserialize<dynamic>(debugToken); // BUG: Uses Microsoft serialization

                    if (validation == null)
                    {
                        throw new NullReferenceException(nameof(validation));
                    }

                    if (!(bool)validation.data.is_valid)
                    {
                        throw new InvalidOperationException("!validation.data.is_valid");
                    }

                    if (validation.data.app_id != externalLoginOptions.AppId)
                    {
                        throw new InvalidOperationException("validation.data.app_id != externalLoginOption.Id");
                    }

                    using var userResponse = await httpClient
                        .GetAsync($"{HOST}/{validation.data.user_id}/?fields={FIELDS}&access_token={implicitLogin.AccessToken}", cancellationToken);

                    userResponse
                        .EnsureSuccessStatusCode();

                    var user = await userResponse.Content
                        .ReadAsStringAsync(cancellationToken);

                    var externalLoginData = JsonSerializer.Deserialize<ExternalLogInData>(user);
                    if (externalLoginData != null)
                    {
                        externalLoginData.ExternalToken = new ExternalLoginTokenData
                        {
                            Name = "Facebook",
                            Token = implicitLogin.AccessToken
                        };
                    }

                    return externalLoginData;
                }

            default:
                throw new NotSupportedException(logInExternalProvider.GetType().Name);
        }
    }
    private async Task<ExternalLogInData> GetExternalProviderLoginDataMicrosoft<TProvider>(TProvider logInExternalProvider, MicrosoftOptions externalLoginOptions, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider
    {
        if (logInExternalProvider == null)
        {
            throw new ArgumentNullException(nameof(logInExternalProvider));
        }

        if (externalLoginOptions == null)
        {
            throw new ArgumentNullException(nameof(externalLoginOptions));
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        string accessToken;
        string refreshToken;

        switch (logInExternalProvider)
        {
            case ExternalLoginProviderAuthCode authCodeLogin:
                using (var httpClient = new HttpClient())
                {
                    var httpRequestMessage = new HttpRequestMessage();

                    httpRequestMessage.Method = HttpMethod.Post;
                    httpRequestMessage.RequestUri = new Uri($"https://login.microsoftonline.com/{externalLoginOptions.TenantId}/oauth2/v2.0/token");

                    using var formContent = new MultipartFormDataContent();
                    {
                        formContent.Add(new StringContent(externalLoginOptions.ClientId), "client_id");
                        formContent.Add(new StringContent(externalLoginOptions.ClientSecret), "client_secret");
                        formContent.Add(new StringContent("authorization_code"), "grant_type");
                        formContent.Add(new StringContent(authCodeLogin.Code), "code");
                        formContent.Add(new StringContent(authCodeLogin.CodeVerifier), "code_verifier");
                        formContent.Add(new StringContent(authCodeLogin.RedirectUri), "redirect_uri");
                        formContent.Add(new StringContent(externalLoginOptions.Scopes.Aggregate(string.Empty, (current, x) => current + $"{x} ")), "scope");

                        httpRequestMessage.Content = formContent;

                        var httpResponse = await httpClient
                            .SendAsync(httpRequestMessage, cancellationToken);

                        var stringContent = await httpResponse.Content
                            .ReadAsStringAsync(cancellationToken);

                        var content = JsonSerializer.Deserialize<JsonObject>(stringContent);

                        var error = (string)content?["error"];
                        var errorDescription = (string)content?["error"];

                        if (error != null)
                        {
                            throw new InvalidOperationException($"{error}: {errorDescription}");
                        }

                        accessToken = (string)content?["access_token"];
                        refreshToken = (string)content?["refresh_token"];
                    }
                }
                break;

            default:
                throw new NotSupportedException(logInExternalProvider.GetType().Name);
        }

        var jwtToken = tokenHandler
            .ReadJwtToken(accessToken);

        var id = jwtToken?.Payload.Where(x => x.Key == "oid").Select(x => x.Value?.ToString()).FirstOrDefault();
        var name = jwtToken?.Payload.Where(x => x.Key == "name").Select(x => x.Value?.ToString()).FirstOrDefault();
        var email = jwtToken?.Payload.Where(x => x.Key == "upn").Select(x => x.Value?.ToString()).FirstOrDefault();

        return new ExternalLogInData
        {
            Id = id,
            Name = name,
            Email = email,
            ExternalToken =
            {
                Name = "Microsoft",
                Token = accessToken,
                RefreshToken = refreshToken
            }
        };
    }
}