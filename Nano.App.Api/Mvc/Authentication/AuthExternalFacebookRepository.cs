using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Api.Config;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Newtonsoft.Json;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc />
public class AuthExternalFacebookRepository(FacebookOptions options, HttpClient httpClient)
    : IAuthExternalFacebookRepository
{
    private readonly FacebookOptions options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly HttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public virtual async Task<ExternalLogInData> Authenticate(ExternalLoginProviderFacebook provider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        switch (provider)
        {
            case ExternalLoginProviderImplicit implicitLogin:
            {
                const string HOST = "https://graph.facebook.com";
                const string FIELDS = "id,name,address,email,birthday";

                var debugTokenResponse = await httpClient
                    .GetAsync($"{HOST}/debug_token?input_token={implicitLogin.AccessToken}&access_token={options.AppId}|{options.AppSecret}", cancellationToken);

                debugTokenResponse
                    .EnsureSuccessStatusCode();

                var debugToken = await debugTokenResponse.Content
                    .ReadAsStringAsync(cancellationToken);

                var validation = JsonConvert.DeserializeObject<dynamic>(debugToken);

                if (validation == null)
                {
                    throw new NullReferenceException(nameof(validation));
                }

                if (!(bool)validation.data.is_valid)
                {
                    throw new UnauthorizedException("!validation.data.is_valid");
                }

                if (validation.data.app_id != options.AppId)
                {
                    throw new UnauthorizedException("validation.data.app_id != externalLoginOption.Id");
                }

                using var userResponse = await httpClient
                    .GetAsync($"{HOST}/{validation.data.user_id}/?fields={FIELDS}&access_token={implicitLogin.AccessToken}", cancellationToken);

                userResponse
                    .EnsureSuccessStatusCode();

                var user = await userResponse.Content
                    .ReadAsStringAsync(cancellationToken);

                var externalLoginData = JsonConvert.DeserializeObject<ExternalLogInData>(user);

                externalLoginData?.ExternalToken = new ExternalLoginTokenData
                {
                    Name = ExternalLogInProviderNames.FACEBOOK,
                    Token = implicitLogin.AccessToken
                };

                return externalLoginData ?? throw new UnauthorizedException();
            }

            default:
                throw new NotSupportedException(provider.GetType().Name);
        }
    }

    /// <inheritdoc />
    public virtual async Task<ExternalLoginTokenData> AuthenticateRefresh(LogInExternalRefreshFacebook logInExternalRefresh, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logInExternalRefresh);

        await Task.CompletedTask;

        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.httpClient
            .Dispose();
    }
}