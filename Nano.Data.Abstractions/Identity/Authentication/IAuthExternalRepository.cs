using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Authentication;

/// <summary>
/// Central repository for handling external authentication via Facebook, Google, and Microsoft providers.
/// Defines a generic contract for external authentication via multiple providers.
/// </summary>
public interface IAuthExternalRepository
{
    /// <summary>
    /// Authenticates a user using the specified external login provider.
    /// </summary>
    /// <typeparam name="TProvider">The type of the external login provider.</typeparam>
    /// <param name="provider">The external login provider instance.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The external login data.</returns>
    Task<ExternalLogInData> AuthenticateAsync<TProvider>(TProvider provider, CancellationToken cancellationToken = default)
        where TProvider : BaseLogInExternalProvider;

    /// <summary>
    /// Refreshes an existing external authentication session.
    /// </summary>
    /// <param name="logInExternalRefresh">The refresh token data for the external login.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The refreshed external login token data.</returns>
    Task<ExternalLoginTokenData> AuthenticateRefreshAsync(LogInExternalRefresh logInExternalRefresh, CancellationToken cancellationToken = default);
}