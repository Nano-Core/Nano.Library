using Google.Apis.Auth;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc cref="BaseAuthExternalRepository{TFlow}" />
public class AuthExternalGoogleRepository(GoogleOptions options)
    : BaseAuthExternalRepository<ImplicitFlow>(BuiltInExternalLogInProviderNames.GOOGLE), IBuiltInAuthExternalRepository
{
    private readonly GoogleOptions options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public override async Task<ExternalAuthenticationData> AuthenticateAsync(ImplicitFlow flow, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(flow);

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience =
            [
                options.ClientId
            ]
        };

        var payload = await GoogleJsonWebSignature
            .ValidateAsync(flow.AccessToken, settings);

        return new ExternalAuthenticationData
        {
            Id = payload.Subject,
            Name = payload.Name,
            EmailAddress = payload.Email,
            Username = payload.Email,
            ExternalToken =
            {
                Name = BuiltInExternalLogInProviderNames.GOOGLE,
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