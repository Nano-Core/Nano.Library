using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc />
public abstract class BaseAuthExternalRepository<TProvider, TFlow> : IAuthExternalRepository
    where TProvider : BaseExternalProvider<TFlow>
    where TFlow : BaseAuthFlow
{
    /// <summary>
    /// Gets the unique name of the external provider.
    /// Used to resolve the correct repository in the aggregator.
    /// </summary>
    public string ProviderName { get; } = "Custom"; // new TProvider().Name; // BUG: still ugly

    // BUG: We should have TFlow on "provider", but its not on the interface.

    /// <inheritdoc />
    public virtual Task<ExternalAuthenticationData> AuthenticateAsync(BaseExternalProvider provider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var a = typeof(TProvider);

        if (provider is not TProvider typedProvider)
        {
            throw new UnauthorizedException($"Invalid provider type for {this.ProviderName}");
        }

        return this.AuthenticateAsync(typedProvider, cancellationToken);
    }

    /// <summary>
    /// Authenticates a user using the strongly-typed external provider.
    /// Implementations must provide the actual authentication logic.
    /// </summary>
    /// <param name="provider">The strongly-typed external login provider data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The authenticated external login data.</returns>
    public abstract Task<ExternalAuthenticationData> AuthenticateAsync(TProvider provider, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an existing external authentication session using the strongly-typed refresh data.
    /// Implementations must provide the actual token refresh logic.
    /// </summary>
    /// <param name="refreshToken">The current refresh token.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The refreshed external login token data.</returns>
    public abstract Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
}