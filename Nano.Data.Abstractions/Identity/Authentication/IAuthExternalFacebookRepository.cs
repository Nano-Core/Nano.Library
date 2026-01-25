using Nano.Data.Abstractions.Identity.Authentication.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Abstractions.Identity.Authentication;

/// <summary>
/// Defines operations for authenticating users using Facebook as an external identity provider.
/// </summary>
public interface IAuthExternalFacebookRepository : IDisposable
{
    /// <summary>
    /// Authenticates a user using Facebook as an external login provider.
    /// </summary>
    /// <param name="provider">The Facebook external login provider data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The external login data.</returns>
    Task<ExternalLogInData> Authenticate(ExternalLoginProviderFacebook provider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an existing Facebook external authentication session.
    /// </summary>
    /// <param name="logInExternalRefresh">The Facebook refresh token data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The refreshed external login token data.</returns>
    Task<ExternalLoginTokenData> AuthenticateRefresh(LogInExternalRefreshFacebook logInExternalRefresh, CancellationToken cancellationToken = default);
}