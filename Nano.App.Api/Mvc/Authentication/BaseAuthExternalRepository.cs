using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.Api.Mvc.Authentication;

/// <inheritdoc />
public abstract class BaseAuthExternalRepository<TFlow>(string providerName) : IAuthExternalRepository<TFlow>
    where TFlow : BaseAuthFlow
{
    /// <inheritdoc />
    public string ProviderName { get; } = providerName;

    /// <inheritdoc />
    public virtual Task<ExternalAuthenticationData> AuthenticateAsync(BaseAuthFlow flow, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(flow);

        if (flow is not TFlow typedFlow)
        {
            throw new NotFoundException(nameof(flow));
        }

        return this.AuthenticateAsync(typedFlow, cancellationToken);
    }

    /// <inheritdoc />
    public abstract Task<ExternalAuthenticationData> AuthenticateAsync(TFlow flow, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<ExternalAuthenticationToken> AuthenticateRefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
}