using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc />
public abstract class BaseAuthExternalRepository<TProvider> : IAuthExternalRepository
    where TProvider : BaseExternalProvider, new()
{
    /// <summary>
    /// Gets the unique name of the external provider.
    /// Used to resolve the correct repository in the aggregator.
    /// </summary>
    public string ProviderName { get; } = new TProvider().Name;

    /// <inheritdoc />
    public virtual Task<ExternalAuthenticationData> AuthenticateAsync(BaseExternalProvider provider, ImplicitFlow implicitFlow, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (provider is not TProvider typedProvider)
        {
            throw new UnauthorizedException($"Invalid provider type for {this.ProviderName}");
        }

        return this.AuthenticateAsync(typedProvider, implicitFlow, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<ExternalAuthenticationData> AuthenticateAsync(BaseExternalProvider provider, AuthCodeFlow authCodeFlow, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (provider is not TProvider typedProvider)
        {
            throw new UnauthorizedException($"Invalid provider type for {this.ProviderName}");
        }

        return this.AuthenticateAsync(typedProvider, authCodeFlow, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(BaseExternalProvider provider, string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (provider is not TProvider typedProvider)
        {
            throw new UnauthorizedException($"Invalid provider type for {this.ProviderName}");
        }

        return this.AuthenticateRefreshAsync(typedProvider, refreshToken, cancellationToken);
    }

    /// <summary>
    /// Authenticates a user using the strongly-typed external provider.
    /// Implementations must provide the actual authentication logic.
    /// </summary>
    /// <param name="provider">The strongly-typed external login provider data.</param>
    /// <param name="implicitFlow">The implicit flow authentication data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The authenticated external login data.</returns>
    public abstract Task<ExternalAuthenticationData> AuthenticateAsync(TProvider provider, ImplicitFlow implicitFlow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user using the strongly-typed external provider.
    /// Implementations must provide the actual authentication logic.
    /// </summary>
    /// <param name="provider">The strongly-typed external login provider data.</param>
    /// <param name="authCodeFlow">The authorization code flow authentication data.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The authenticated external login data.</returns>
    public abstract Task<ExternalAuthenticationData> AuthenticateAsync(TProvider provider, AuthCodeFlow authCodeFlow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an existing external authentication session using the strongly-typed refresh data.
    /// Implementations must provide the actual token refresh logic.
    /// </summary>
    /// <param name="provider">The strongly-typed external login refresh data.</param>
    /// <param name="refreshToken">The current refresh token.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The refreshed external login token data.</returns>
    public abstract Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(TProvider provider, string refreshToken, CancellationToken cancellationToken = default);
}