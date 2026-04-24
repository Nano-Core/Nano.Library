using Nano.App.Api.Config;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using Newtonsoft.Json;
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

        const string HOST = "https://graph.facebook.com";
        const string FIELDS = "id,name,address,email,birthday";

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

        var externalLoginData = JsonConvert.DeserializeObject<ExternalAuthenticationData>(user);

        externalLoginData?.ExternalToken = new ExternalAuthenticationToken
        {
            Name = BuiltInExternalLogInProviderNames.FACEBOOK,
            Token = flow.AccessToken
        };

        return externalLoginData ?? throw new UnauthorizedException();
    }

    /// <inheritdoc />
    public override async Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        await Task.CompletedTask;

        throw new UnauthorizedException();
    }
}