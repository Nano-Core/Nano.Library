using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Requests;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Models;

namespace Nano.App.ApiClient.Apis;

/// <summary>
/// 
/// </summary>
public sealed class AuditApi<TIdentity>(ApiClient api)
    where TIdentity : IEquatable<TIdentity>
{
    private readonly ApiClient api = api ?? throw new ArgumentNullException(nameof(api));

    /// <summary>
    /// Invokes the 'index' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <param name="request">The <see cref="IndexRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The entities.</returns>
    public async Task<IEnumerable<AuditEntry<TIdentity>>> IndexAsync(IndexRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = ControllerRoutes.AUDIT;

        return await this.api
            .InvokeAsync<IndexRequest, IEnumerable<AuditEntry<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Invokes the 'details' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <param name="request">The <see cref="DetailsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public Task<AuditEntry<TIdentity>?> DetailsAsync(DetailsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = ControllerRoutes.AUDIT;

        return this.api
            .InvokeAsync<DetailsRequest<TIdentity>, AuditEntry<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'details' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public Task<AuditEntry<TIdentity>?> GetAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        return this.DetailsAsync(new DetailsRequest<TIdentity>
        {
            Id = id
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'details' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public Task<AuditEntry<TIdentity>?> GetAsync(TIdentity id, int includeDepth, CancellationToken cancellationToken = default)
    {
        return this.DetailsAsync(new DetailsRequest<TIdentity>
        {
            Id = id,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'details/many' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <param name="request">The <see cref="DetailsManyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public async Task<IEnumerable<AuditEntry<TIdentity>>> DetailsManyAsync(DetailsManyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = ControllerRoutes.AUDIT;

        return await this.api
            .InvokeAsync<DetailsManyRequest<TIdentity>, IEnumerable<AuditEntry<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Invokes the 'details/many' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <param name="ids">The ids.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public Task<IEnumerable<AuditEntry<TIdentity>>> GetManyAsync(ICollection<TIdentity> ids, CancellationToken cancellationToken = default)
    {
        return this.DetailsManyAsync(new DetailsManyRequest<TIdentity>
        {
            Ids = ids
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'details/many' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <param name="ids">The ids.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public Task<IEnumerable<AuditEntry<TIdentity>>> GetManyAsync(ICollection<TIdentity> ids, int includeDepth, CancellationToken cancellationToken = default)
    {
        return this.DetailsManyAsync(new DetailsManyRequest<TIdentity>
        {
            Ids = ids,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'query' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public async Task<IEnumerable<AuditEntry<TIdentity>>> QueryAsync<TCriteria>(QueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = ControllerRoutes.AUDIT;

        return await this.api
            .InvokeAsync<QueryRequest<TCriteria>, IEnumerable<AuditEntry<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Invokes the 'query' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="query">The query with criteria of type <typeparamref name="TCriteria"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public Task<IEnumerable<AuditEntry<TIdentity>>> QueryAsync<TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(query);

        return this.QueryAsync(new QueryRequest<TCriteria>
        {
            Query = query
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'query' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="query">The query with criteria of type <typeparamref name="TCriteria"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public Task<IEnumerable<AuditEntry<TIdentity>>> QueryAsync<TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(query);

        return this.QueryAsync(new QueryRequest<TCriteria>
        {
            Query = query,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/first' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryFirstRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The first match entity.</returns>
    public Task<AuditEntry<TIdentity>?> QueryFirstAsync<TCriteria>(QueryFirstRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = ControllerRoutes.AUDIT;

        return this.api
            .InvokeAsync<QueryFirstRequest<TCriteria>, AuditEntry<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/first' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="query">The query with criteria of type <typeparamref name="TCriteria"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The first match entity.</returns>
    public Task<AuditEntry<TIdentity>?> QueryFirstAsync<TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(query);

        return this.QueryFirstAsync(new QueryFirstRequest<TCriteria>
        {
            Query = query
        }, cancellationToken);
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/first' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="query">The query with criteria of type <typeparamref name="TCriteria"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The first match entity.</returns>
    public Task<AuditEntry<TIdentity>?> QueryFirstAsync<TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(query);

        return this.QueryFirstAsync(new QueryFirstRequest<TCriteria>
        {
            Query = query,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'query/count' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryCountRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The count of matching entities.</returns>
    public async Task<int> QueryCountAsync<TCriteria>(QueryCountRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = ControllerRoutes.AUDIT;

        var response = await this.api
            .InvokeAsync<AuditEntry<TIdentity>, QueryCountRequest<TCriteria>, string>(request, cancellationToken);

        int.TryParse(response, out var count);

        return count;
    }

    /// <summary>
    /// Invokes the 'query/count' endpoint of the <see cref="AuditEntry{TIdentity}"/> in the api.
    /// </summary>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="criteria">The criteria of type <typeparamref name="TCriteria"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The count of matching entities.</returns>
    public Task<int> QueryCountAsync<TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);

        return this.QueryCountAsync(new QueryCountRequest<TCriteria>
        {
            Criteria = criteria
        }, cancellationToken);
    }
}