using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc />
public class AuthExternalRepositoryAggregator(IEnumerable<IAuthExternalRepository> repositories)
    : IAuthExternalRepositoryAggregator
{
    private readonly IEnumerable<IAuthExternalRepository> repositories = repositories ?? throw new ArgumentNullException(nameof(repositories));

    /// <inheritdoc />
    public Task<ExternalLogInData> AuthenticateAsync(BaseExternalProvider provider, BaseAuthFlow auth, CancellationToken cancellationToken = default)
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
            AuthCodeFlow authCode => externalRepository.AuthenticateAsync(provider, authCode, cancellationToken),
            ImplicitFlow implicitFlow => externalRepository.AuthenticateAsync(provider, implicitFlow, cancellationToken),
            _ => throw new UnauthorizedException()
        };
    }

    /// <inheritdoc />
    public Task<ExternalLoginTokenData> AuthenticateRefreshAsync(BaseExternalProvider provider, string refreshToken, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var externalRepository = this.repositories
            .FirstOrDefault(x => x.ProviderName.Equals(provider.Name, StringComparison.OrdinalIgnoreCase));

        if (externalRepository == null)
        {
            throw new NotFoundException(nameof(externalRepository));
        }

        return externalRepository
            .AuthenticateRefreshAsync(provider, refreshToken, cancellationToken);
    }
}