using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nano.Common.Config;
using Nano.Data.Abstractions.Identity.Authentication.Abstractions;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Newtonsoft.Json;

namespace Nano.App.Web.Identity.Authentication;

/// <inheritdoc />
public class AuthExternalFacebookRepository : IAuthExternalFacebookRepository
{
    private readonly FacebookOptions options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public AuthExternalFacebookRepository(FacebookOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual async Task<ExternalLogInData> Authenticate(ExternalLoginProviderFacebook provider, CancellationToken cancellationToken = default)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        switch (provider)
        {
            case ExternalLoginProviderImplicit implicitLogin:
                using (var httpClient = new HttpClient())
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
                        throw new InvalidOperationException("!validation.data.is_valid");
                    }

                    if (validation.data.app_id != options.AppId)
                    {
                        throw new InvalidOperationException("validation.data.app_id != externalLoginOption.Id");
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
                        Name = "Facebook",
                        Token = implicitLogin.AccessToken
                    };

                    return externalLoginData;
                }

            default:
                throw new NotSupportedException(provider.GetType().Name);
        }
    }

    /// <inheritdoc />
    public virtual async Task<ExternalLoginTokenData> AuthenticateRefresh(string name, string externalRefreshToken = null, CancellationToken cancellationToken = default)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (externalRefreshToken == null)
            throw new ArgumentNullException(nameof(externalRefreshToken));

        await Task.CompletedTask;

        throw new NotImplementedException();
    }
}