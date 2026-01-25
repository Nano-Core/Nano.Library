using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Nano.App.Api.Config;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Api.Identity.Authentication;

/// <inheritdoc />
public class AuthExternalGoogleRepository(GoogleOptions options) : IAuthExternalGoogleRepository
{
    private readonly GoogleOptions options = options ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public virtual async Task<ExternalLogInData> Authenticate(ExternalLoginProviderGoogle provider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        switch (provider)
        {
            case ExternalLoginProviderImplicit implicitLogin:
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience =
                    [
                        options.ClientId
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
                        Name = ExternalLogInProviderNames.GOOGLE,
                        Token = implicitLogin.AccessToken
                    }
                };

            default:
                throw new NotSupportedException(provider.GetType().Name);
        }
    }

    /// <inheritdoc />
    public virtual async Task<ExternalLoginTokenData> AuthenticateRefresh(LogInExternalRefreshGoogle logInExternalRefresh, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logInExternalRefresh);

        await Task.CompletedTask;

        throw new NotImplementedException();
    }
}