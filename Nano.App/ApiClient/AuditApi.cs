using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Requests;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient;

/// <summary>
/// 
/// </summary>
public sealed class AuditApi(ApiClient api)
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

        return await this.api
            .InvokeAsync<IndexRequest, IEnumerable<AuditEntry<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Invokes the 'details' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DetailsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public Task<TEntity?> DetailsAsync<TEntity>(DetailsRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<DetailsRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'details' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public Task<TEntity?> GetAsync<TEntity>(TIdentity id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsAsync<TEntity>(new DetailsRequest<TIdentity>
        {
            Id = id
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'details' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public Task<TEntity?> GetAsync<TEntity>(TIdentity id, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsAsync<TEntity>(new DetailsRequest<TIdentity>
        {
            Id = id,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'details/many' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DetailsManyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public async Task<IEnumerable<TEntity>> DetailsManyAsync<TEntity>(DetailsManyRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.InvokeAsync<DetailsManyRequest<TIdentity>, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Invokes the 'details/many' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The ids.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(ICollection<TIdentity> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsManyAsync<TEntity>(new DetailsManyRequest<TIdentity>
        {
            Ids = ids
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'details/many' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The ids.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(ICollection<TIdentity> ids, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsManyAsync<TEntity>(new DetailsManyRequest<TIdentity>
        {
            Ids = ids,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'query' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TCriteria>(QueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.InvokeAsync<QueryRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Invokes the 'query' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="query">The query with criteria of type <typeparamref name="TCriteria"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public Task<IEnumerable<TEntity>> QueryAsync<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(query);

        return this.QueryAsync<TEntity, TCriteria>(new QueryRequest<TCriteria>
        {
            Query = query
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'query' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="query">The query with criteria of type <typeparamref name="TCriteria"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public Task<IEnumerable<TEntity>> QueryAsync<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(query);

        return this.QueryAsync<TEntity, TCriteria>(new QueryRequest<TCriteria>
        {
            Query = query,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/first' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryFirstRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The first match entity.</returns>
    public Task<TEntity?> QueryFirstAsync<TEntity, TCriteria>(QueryFirstRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<QueryFirstRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/first' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="query">The query with criteria of type <typeparamref name="TCriteria"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The first match entity.</returns>
    public Task<TEntity?> QueryFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(query);

        return this.QueryFirstAsync<TEntity, TCriteria>(new QueryFirstRequest<TCriteria>
        {
            Query = query
        }, cancellationToken);
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/first' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="query">The query with criteria of type <typeparamref name="TCriteria"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The first match entity.</returns>
    public Task<TEntity?> QueryFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(query);

        return this.QueryFirstAsync<TEntity, TCriteria>(new QueryFirstRequest<TCriteria>
        {
            Query = query,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'query/count' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryCountRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The count of matching entities.</returns>
    public async Task<int> QueryCountAsync<TEntity, TCriteria>(QueryCountRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await this.InvokeAsync<TEntity, QueryCountRequest<TCriteria>, string>(request, cancellationToken);

        int.TryParse(response, out var count);

        return count;
    }

    /// <summary>
    /// Invokes the 'query/count' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="criteria">The criteria of type <typeparamref name="TCriteria"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The count of matching entities.</returns>
    public Task<int> QueryCountAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);

        return this.QueryCountAsync<TEntity, TCriteria>(new QueryCountRequest<TCriteria>
        {
            Criteria = criteria
        }, cancellationToken);
    }
}