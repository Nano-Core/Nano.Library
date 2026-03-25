using Google.Apis.Auth;
using Nano.App.Api.Config;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc />
public class AuthExternalGoogleRepository(GoogleOptions options)
    : BaseAuthExternalRepository<ExternalProviderGoogle>(ExternalLogInProviderNames.GOOGLE)
{
    private readonly GoogleOptions options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public override Task<ExternalLogInData> AuthenticateAsync(ExternalProviderGoogle provider, AuthCodeFlow auth, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override async Task<ExternalLogInData> AuthenticateAsync(ExternalProviderGoogle provider, ImplicitFlow auth, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

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
            .ValidateAsync(auth.AccessToken, settings);

        return new ExternalLogInData
        {
            Id = payload.Subject,
            Name = payload.Name,
            Email = payload.Email,
            Username = payload.Email,
            ExternalToken =
            {
                Name = ExternalLogInProviderNames.GOOGLE,
                Token = auth.AccessToken
            }
        };
    }

    /// <inheritdoc />
    public override async Task<ExternalLoginTokenData> AuthenticateRefreshAsync(ExternalProviderGoogle provider, string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(refreshToken);

        await Task.CompletedTask;

        throw new NotImplementedException();
    }
}