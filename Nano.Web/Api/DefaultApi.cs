using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nano.Web.Api.Requests;

namespace Nano.Web.Api;

/// <summary>
/// Default Api.
/// </summary>
public class DefaultApi : BaseApi<Guid>
{
    /// <inheritdoc />
    public DefaultApi(HttpClient httpClient, ApiOptions apiOptions)
        : base(httpClient, apiOptions)
    {

    }

    /// <inheritdoc />
    public override async Task<TEntity> DetailsAsync<TEntity>(DetailsRequest<Guid> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return await base.DetailsAsync<TEntity>(request, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<TEntity>> DetailsManyAsync<TEntity>(DetailsManyRequest<Guid> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return await base.DetailsManyAsync<TEntity>(request, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task DeleteAsync<TEntity>(DeleteRequest<Guid> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        await base.DeleteAsync<TEntity>(request, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task DeleteManyAsync<TEntity>(DeleteManyRequest<Guid> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        await base.DeleteManyAsync<TEntity>(request, cancellationToken);
    }
}