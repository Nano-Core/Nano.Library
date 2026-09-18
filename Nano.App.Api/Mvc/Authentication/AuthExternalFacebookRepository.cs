using Nano.App.Api.Config;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Api.Mvc.Authentication.Abstractions;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc cref="BaseAuthExternalRepository{TFlow}" />
public class AuthExternalFacebookRepository(FacebookOptions options, HttpClient httpClient)
    : BaseAuthExternalRepository<ImplicitFlow>(BuiltInExternalLogInProviderNames.FACEBOOK), IBuiltInAuthExternalRepository
{
    private readonly FacebookOptions options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly HttpClient httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <inheritdoc />
    public override async Task<ExternalAuthenticationData> AuthenticateAsync(ImplicitFlow flow, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(flow);

        const string HOST = "https://graph.facebook.com/v21.0";
        const string FIELDS = "id,name,email";

        var debugTokenResponse = await httpClient
            .GetAsync($"{HOST}/debug_token?input_token={flow.AccessToken}&access_token={options.AppId}|{options.AppSecret}", cancellationToken);

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
            .GetAsync($"{HOST}/{validation.data.user_id}/?fields={FIELDS}&access_token={flow.AccessToken}", cancellationToken);

        userResponse
            .EnsureSuccessStatusCode();

        var user = await userResponse.Content
            .ReadAsStringAsync(cancellationToken);

        var userData = JsonConvert.DeserializeObject<JObject>(user);

        if (userData == null)
        {
            throw new NullReferenceException(nameof(userData));
        }

        var id = userData["id"]?.ToString();

        if (id == null)
        {
            throw new NullReferenceException(nameof(id));
        }

        var name = userData["name"]?.ToString();

        if (name == null)
        {
            throw new NullReferenceException(nameof(name));
        }

        var email = userData["email"]?.ToString();

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
                Name = BuiltInExternalLogInProviderNames.FACEBOOK,
                Token = flow.AccessToken
            }
        };
    }

    /// <inheritdoc />
    public override async Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        await Task.CompletedTask;

        throw new UnauthorizedException();
    }
}