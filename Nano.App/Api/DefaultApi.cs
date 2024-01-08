using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.App.Api.Requests;

namespace Nano.App.Api;

/// <summary>
/// Default Api.
/// </summary>
public class DefaultApi : BaseApi<Guid>
{
    /// <inheritdoc />
    public DefaultApi(ApiOptions apiOptions)
        : base(apiOptions)
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