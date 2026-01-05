using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Config;
using Nano.App.ApiClient.Requests;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nano.App.ApiClient;

/// <summary>
/// Default Identity Api.
/// </summary>
public class DefaultIdentityApi<TUser> : BaseIdentityApi<TUser, Guid>
    where TUser : class, IEntityUser<Guid>
{
    /// <inheritdoc />
    public DefaultIdentityApi(IOptionsMonitor<ApiOptions> apiOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiOptions, httpClient, httpContextAccessor)
    {
    }

    /// <inheritdoc />
    public override Task<TEntity> DetailsAsync<TEntity>(DetailsRequest<Guid> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return base.DetailsAsync<TEntity>(request, cancellationToken);
    }

    /// <inheritdoc />
    public override Task<IEnumerable<TEntity>> DetailsManyAsync<TEntity>(DetailsManyRequest<Guid> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return base.DetailsManyAsync<TEntity>(request, cancellationToken);
    }

    /// <inheritdoc />
    public override Task DeleteAsync<TEntity>(DeleteRequest<Guid> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return base.DeleteAsync<TEntity>(request, cancellationToken);
    }

    /// <inheritdoc />
    public override Task DeleteManyAsync<TEntity>(DeleteManyRequest<Guid> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return base.DeleteManyAsync<TEntity>(request, cancellationToken);
    }
}