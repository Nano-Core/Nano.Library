using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Authentication;

/// <summary>
/// Defines operations for authenticating users using Microsoft as an external identity provider.
/// </summary>
public interface IAuthExternalMicrosoftRepository
{
    /// <summary>
    /// Authenticates a user using Microsoft as an external login provider.
    /// </summary>
    /// <param name="provider">The Microsoft external login provider data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The external login data.</returns>
    Task<ExternalLogInData> Authenticate(ExternalLoginProviderMicrosoft provider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an existing Microsoft external authentication session.
    /// </summary>
    /// <param name="logInExternalRefresh">The Microsoft refresh token data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The refreshed external login token data.</returns>
    Task<ExternalLoginTokenData> AuthenticateRefresh(LogInExternalRefreshMicrosoft logInExternalRefresh, CancellationToken cancellationToken = default);
}