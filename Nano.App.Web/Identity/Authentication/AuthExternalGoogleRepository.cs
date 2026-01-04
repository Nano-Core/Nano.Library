using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Nano.App.Web.Config;
using Nano.Common.Config;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Web.Identity.Authentication;

/// <inheritdoc />
public class AuthExternalGoogleRepository : IAuthExternalGoogleRepository
{
    private readonly GoogleOptions options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public AuthExternalGoogleRepository(GoogleOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual async Task<ExternalLogInData> Authenticate(ExternalLoginProviderGoogle provider, CancellationToken cancellationToken = default)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

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
                        Name = "Google",
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
        if (logInExternalRefresh == null)
            throw new ArgumentNullException(nameof(logInExternalRefresh));

        await Task.CompletedTask;

        throw new NotImplementedException();
    }
}