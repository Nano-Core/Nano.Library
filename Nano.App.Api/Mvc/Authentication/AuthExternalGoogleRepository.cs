using Google.Apis.Auth;
using Nano.App.Api.Config;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Api.Mvc.Authentication.Abstractions;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc cref="BaseAuthExternalRepository{TProvider}" />
public class AuthExternalGoogleRepository(GoogleOptions options)
    : BaseAuthExternalRepository<ExternalProviderGoogle>, IBuiltInAuthExternalRepository
{
    private readonly GoogleOptions options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public override Task<ExternalAuthenticationData> AuthenticateAsync(ExternalProviderGoogle provider, AuthCodeFlow authCodeFlow, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override async Task<ExternalAuthenticationData> AuthenticateAsync(ExternalProviderGoogle provider, ImplicitFlow implicitFlow, CancellationToken cancellationToken = default)
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
            .ValidateAsync(implicitFlow.AccessToken, settings);

        return new ExternalAuthenticationData
        {
            Id = payload.Subject,
            Name = payload.Name,
            EmailAddress = payload.Email,
            Username = payload.Email,
            ExternalToken =
            {
                Name = BuiltInExternalLogInProviderNames.GOOGLE,
                Token = implicitFlow.AccessToken
            }
        };
    }

    /// <inheritdoc />
    public override async Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(ExternalProviderGoogle provider, string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(refreshToken);

        await Task.CompletedTask;

        throw new NotImplementedException();
    }
}