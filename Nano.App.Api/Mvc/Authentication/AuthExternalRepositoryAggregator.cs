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
    public Task<ExternalAuthenticationData> AuthenticateAsync<TFlow>(BaseExternalProvider<TFlow> provider, CancellationToken cancellationToken = default)
        where TFlow : BaseAuthFlow
    {
        ArgumentNullException.ThrowIfNull(provider);

        var externalRepository = this.GetRepository(provider.Name);

        return externalRepository
            .AuthenticateAsync(provider, cancellationToken) ?? throw new UnauthorizedException();
    }

    /// <inheritdoc />
    public Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(BaseExternalProvider provider, string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(refreshToken);

        var externalRepository = this.GetRepository(provider.Name);

        return externalRepository
            .AuthenticateRefreshAsync(refreshToken, cancellationToken) ?? throw new UnauthorizedException();
    }


    private IAuthExternalRepository GetRepository(string providerName)
    {
        ArgumentNullException.ThrowIfNull(providerName);

        var externalRepository = this.repositories
            .FirstOrDefault(x => x.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

        if (externalRepository == null)
        {
            throw new NotFoundException(nameof(externalRepository));
        }

        return externalRepository;
    }
}