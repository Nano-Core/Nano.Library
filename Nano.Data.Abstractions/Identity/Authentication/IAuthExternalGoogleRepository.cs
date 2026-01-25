using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Authentication;

/// <summary>
/// Defines operations for authenticating users using Google as an external identity provider.
/// </summary>
public interface IAuthExternalGoogleRepository
{
    /// <summary>
    /// Authenticates a user using Google as an external login provider.
    /// </summary>
    /// <param name="provider">The Google external login provider data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The external login data.</returns>
    Task<ExternalLogInData> Authenticate(ExternalLoginProviderGoogle provider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an existing Google external authentication session.
    /// </summary>
    /// <param name="logInExternalRefresh">The Google refresh token data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The refreshed external login token data.</returns>
    Task<ExternalLoginTokenData> AuthenticateRefresh(LogInExternalRefreshGoogle logInExternalRefresh, CancellationToken cancellationToken = default);
}