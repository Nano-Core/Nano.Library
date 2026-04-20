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
/// Client for interacting with audit endpoints (route: <c>audit/*</c>).
/// Provides querying and retrieval of <see cref="AuditEntry{TIdentity}"/> resources.
/// </summary>
public sealed class AuditApi<TIdentity>(ApiClient api)
    where TIdentity : IEquatable<TIdentity>
{
    private readonly ApiClient api = api ?? throw new ArgumentNullException(nameof(api));

    /// <summary>
    /// Executes <c>audit/index</c> to retrieve a paged or filtered set of audit entries.
    /// </summary>
    /// <param name="request">The index request configuration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of matching audit entries.</returns>
    public async Task<IEnumerable<AuditEntry<TIdentity>>> IndexAsync(IndexRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = ControllerRoutes.AUDIT;

        return await this.api
            .InvokeAsync<IndexRequest, IEnumerable<AuditEntry<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>audit/details</c> to retrieve a single audit entry.
    /// </summary>
    /// <param name="request">The details request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching audit entry, or <c>null</c> if not found.</returns>
    public Task<AuditEntry<TIdentity>?> DetailsAsync(DetailsRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = ControllerRoutes.AUDIT;

        return this.api
            .InvokeAsync<DetailsRequest<TIdentity>, AuditEntry<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>audit/details</c> to retrieve a single audit entry by id.
    /// </summary>
    /// <param name="id">The identifier of the audit entry.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching audit entry, or <c>null</c> if not found.</returns>
    public Task<AuditEntry<TIdentity>?> GetAsync(TIdentity id, CancellationToken cancellationToken = default)
    {
        return this.DetailsAsync(new DetailsRequest<TIdentity>
        {
            Id = id
        }, cancellationToken);
    }

    /// <summary>
    /// Executes <c>audit/details</c> to retrieve a single audit entry by id with related data.
    /// </summary>
    /// <param name="id">The identifier of the audit entry.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching audit entry, or <c>null</c> if not found.</returns>
    public Task<AuditEntry<TIdentity>?> GetAsync(TIdentity id, int includeDepth, CancellationToken cancellationToken = default)
    {
        return this.DetailsAsync(new DetailsRequest<TIdentity>
        {
            Id = id,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Executes <c>audit/details/many</c> to retrieve multiple audit entries.
    /// </summary>
    /// <param name="request">The request containing identifiers and options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of matching audit entries.</returns>
    public async Task<IEnumerable<AuditEntry<TIdentity>>> DetailsManyAsync(DetailsManyRequest<TIdentity> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = ControllerRoutes.AUDIT;

        return await this.api
            .InvokeAsync<DetailsManyRequest<TIdentity>, IEnumerable<AuditEntry<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>audit/details/many</c> to retrieve multiple audit entries by ids.
    /// </summary>
    /// <param name="ids">The identifiers to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of matching audit entries.</returns>
    public Task<IEnumerable<AuditEntry<TIdentity>>> GetManyAsync(ICollection<TIdentity> ids, CancellationToken cancellationToken = default)
    {
        return this.DetailsManyAsync(new DetailsManyRequest<TIdentity>
        {
            Ids = ids
        }, cancellationToken);
    }

    /// <summary>
    /// Executes <c>audit/details/many</c> to retrieve multiple audit entries with related data.
    /// </summary>
    /// <param name="ids">The identifiers to retrieve.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of matching audit entries.</returns>
    public Task<IEnumerable<AuditEntry<TIdentity>>> GetManyAsync(ICollection<TIdentity> ids, int includeDepth, CancellationToken cancellationToken = default)
    {
        return this.DetailsManyAsync(new DetailsManyRequest<TIdentity>
        {
            Ids = ids,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Executes <c>audit/query</c> to retrieve audit entries matching criteria.
    /// </summary>
    /// <typeparam name="TCriteria">The criteria type.</typeparam>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of matching audit entries.</returns>
    public async Task<IEnumerable<AuditEntry<TIdentity>>> QueryAsync<TCriteria>(QueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = ControllerRoutes.AUDIT;

        return await this.api
            .InvokeAsync<QueryRequest<TCriteria>, IEnumerable<AuditEntry<TIdentity>>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>audit/query</c> using a query abstraction.
    /// </summary>
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
    /// Executes <c>audit/query</c> using a query abstraction with related data.
    /// </summary>
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
    /// Executes <c>audit/query/first</c> to retrieve the first matching audit entry.
    /// </summary>
    public Task<AuditEntry<TIdentity>?> QueryFirstAsync<TCriteria>(QueryFirstRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Controller = ControllerRoutes.AUDIT;

        return this.api
            .InvokeAsync<QueryFirstRequest<TCriteria>, AuditEntry<TIdentity>>(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>audit/query/first</c> using a query abstraction.
    /// </summary>
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
    /// Executes <c>audit/query/first</c> using a query abstraction with related data.
    /// </summary>
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
    /// Executes <c>audit/query/count</c> to count matching audit entries.
    /// </summary>
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
    /// Executes <c>audit/query/count</c> using criteria.
    /// </summary>
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