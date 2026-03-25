using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Authentication;

/// <summary>
/// Provides a centralized mechanism for authenticating users through external identity providers.
/// Resolves the appropriate external authentication repository based on the specified provider and delegates authentication and token refresh operations.
/// </summary>
public interface IAuthExternalRepositoryAggregator
{
    /// <summary>
    /// Authenticates a user using the specified external login provider.
    /// The appropriate external authentication repository is resolved dynamically based on the provider type.
    /// </summary>
    /// <param name="provider">The external login provider data containing the information required to authenticate the user.</param>
    /// <param name="auth"></param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests (optional).</param>
    /// <returns>An <see cref="ExternalLogInData"/> instance containing the authenticated user information and associated external login details.</returns>
    public Task<ExternalLogInData> AuthenticateAsync(BaseExternalProvider provider, BaseAuthFlow auth, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an existing external authentication session using the specified provider refresh data.
    /// The appropriate external authentication repository is resolved dynamically based on the provider type.
    /// </summary>
    /// <param name="provider">The external provider refresh data containing the information required to refresh the authentication session.</param>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests (optional).</param>
    /// <returns>An <see cref="ExternalLoginTokenData"/> instance containing the refreshed authentication tokens.</returns>
    public Task<ExternalLoginTokenData> AuthenticateRefreshAsync(BaseExternalProvider provider, string refreshToken, CancellationToken cancellationToken = default);
}