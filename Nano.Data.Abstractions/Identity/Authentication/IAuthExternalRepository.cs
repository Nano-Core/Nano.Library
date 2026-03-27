using Nano.Data.Abstractions.Identity.Authentication.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Data.Abstractions.Identity.Authentication;

/// <summary>
/// Defines operations for authenticating users using an external identity provider.
/// Provides methods for handling different authentication flows and refreshing tokens.
/// </summary>
public interface IAuthExternalRepository
{
    /// <summary>
    /// Gets the provider type identifier.
    /// Used to map incoming authentication requests to the appropriate repository implementation.
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Authenticates a user using the provider authentication flow.
    /// </summary>
    /// <param name="provider">The external provider instance.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>An <see cref="ExternalAuthenticationData"/> containing the authenticated user information.</returns>
    Task<ExternalAuthenticationData> AuthenticateAsync(BaseExternalProvider provider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an existing external authentication session using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token issued by the external provider.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>An <see cref="ExternalAuthenticationToken"/> containing the refreshed authentication tokens.</returns>
    Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a strongly-typed contract for authenticating users using a specific external provider.
/// Enables type-safe implementations while maintaining compatibility with <see cref="IAuthExternalRepository"/>.
/// </summary>
/// <typeparam name="TProvider">The type of the external provider, derived from <see cref="BaseExternalProvider"/>.</typeparam>
/// <typeparam name="TFlow">The type of flow of the external provider.</typeparam>
public interface IAuthExternalRepository<in TProvider, TFlow> : IAuthExternalRepository
    where TProvider : BaseExternalProvider<TFlow>
    where TFlow : BaseAuthFlow
{
    /// <summary>
    /// Authenticates a user using the provider authentication flow.
    /// </summary>
    /// <param name="provider">The strongly-typed external provider instance.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>An <see cref="ExternalAuthenticationData"/> containing the authenticated user information.</returns>
    Task<ExternalAuthenticationData> AuthenticateAsync(TProvider provider, CancellationToken cancellationToken = default);
}