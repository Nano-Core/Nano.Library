using Nano.Data.Abstractions.Identity.Authentication.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Abstractions.Identity.Authentication;

/// <summary>
/// Provides a centralized mechanism for authenticating users through external identity providers.
/// Resolves the appropriate external authentication repository based on the specified provider and delegates authentication and token refresh operations.
/// </summary>
public interface IAuthExternalRepositoryAggregator
{
    /// <summary>
    /// Retrieves all configured external authentication provider schemes available for sign-in.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collectionof <see cref="ExternalLoginProvider"/> representing the available external authentication providers.</returns>
    ExternalLoginProvider[] GetExternalProviderSchemes(CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user using the specified external login provider.
    /// The appropriate external authentication repository is resolved dynamically based on the provider type.
    /// </summary>
    /// <param name="provider">The external login provider data containing the information required to authenticate the user.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests (optional).</param>
    /// <returns>An <see cref="ExternalAuthenticationData"/> instance containing the authenticated user information and associated external login details.</returns>
    public Task<ExternalAuthenticationData> AuthenticateAsync<TFlow>(BaseExternalProvider<TFlow> provider, CancellationToken cancellationToken = default)
        where TFlow : BaseAuthFlow;

    /// <summary>
    /// Refreshes an existing external authentication session using the specified provider refresh data.
    /// The appropriate external authentication repository is resolved dynamically based on the provider type.
    /// </summary>
    /// <param name="provider">The external provider refresh data containing the information required to refresh the authentication session.</param>
    /// <param name="refreshToken">The external refresh token used to retrieve a new JWT access token.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests (optional).</param>
    /// <returns>An <see cref="ExternalAuthenticationToken"/> instance containing the refreshed authentication tokens.</returns>
    public Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(BaseExternalProvider provider, string refreshToken, CancellationToken cancellationToken = default);
}