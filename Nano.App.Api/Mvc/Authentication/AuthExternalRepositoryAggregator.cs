using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc />
public class AuthExternalRepositoryAggregator(IEnumerable<IAuthExternalRepository> repositories)
    : IAuthExternalRepositoryAggregator
{
    private readonly IEnumerable<IAuthExternalRepository> repositories = repositories ?? throw new ArgumentNullException(nameof(repositories));

    /// <inheritdoc />
    public virtual ExternalLoginProvider[] GetExternalProviderSchemes(CancellationToken cancellationToken = default)
    {
        return this.repositories
            .Select(x => new ExternalLoginProvider
            {
                Name = x.ProviderName,
                DisplayName = x.ProviderName
            })
            .ToArray();
    }

    /// <inheritdoc />
    public Task<ExternalAuthenticationData> AuthenticateAsync(BaseExternalProvider provider, BaseAuthFlow auth, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var externalRepository = this.repositories
            .FirstOrDefault(x => x.ProviderName.Equals(provider.Name, StringComparison.OrdinalIgnoreCase));

        if (externalRepository == null)
        {
            throw new NotFoundException(nameof(externalRepository));
        }

        return auth switch
        {
            AuthCodeFlow authCode => externalRepository.AuthenticateAsync(provider, authCode, cancellationToken) ?? throw new UnauthorizedException(),
            ImplicitFlow implicitFlow => externalRepository.AuthenticateAsync(provider, implicitFlow, cancellationToken) ?? throw new UnauthorizedException(),
            _ => throw new UnauthorizedException()
        };
    }

    /// <inheritdoc />
    public Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(BaseExternalProvider provider, string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var externalRepository = this.repositories
            .FirstOrDefault(x => x.ProviderName.Equals(provider.Name, StringComparison.OrdinalIgnoreCase));

        if (externalRepository == null)
        {
            throw new NotFoundException(nameof(externalRepository));
        }

        return externalRepository
            .AuthenticateRefreshAsync(provider, refreshToken, cancellationToken) ?? throw new UnauthorizedException();
    }
}