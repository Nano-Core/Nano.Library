using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Http;
using Nano.App.ApiClient.Config;
using Nano.App.ApiClient.Requests;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.App.ApiClient;

/// <inheritdoc />
public abstract class BaseEntityApi : BaseEntityApi<Guid>
{
    /// <inheritdoc />
    protected BaseEntityApi(ApiClientOptions apiClientOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiClientOptions, httpClient, httpContextAccessor)
    {
    }
}

/// <summary>
/// Represents a base entity api.
/// </summary>
/// <typeparam name="TIdentity">The identity type.</typeparam>
public abstract class BaseEntityApi<TIdentity> : BaseApi
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    protected BaseEntityApi(ApiClientOptions apiClientOptions, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        : base(apiClientOptions, httpClient, httpContextAccessor)
    {
    }

    /// <summary>
    /// Index.
    /// Invokes the 'index' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="IndexRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> IndexAsync<TEntity>(IndexRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.InvokeAsync<IndexRequest, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Get.
    /// Invokes the 'details' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public virtual Task<TEntity?> GetAsync<TEntity>(TIdentity id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsAsync<TEntity>(new DetailsRequest<TIdentity>
        {
            Id = id
        }, cancellationToken);
    }

    /// <summary>
    /// Get.
    /// Invokes the 'details' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public virtual Task<TEntity?> GetAsync<TEntity>(TIdentity id, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsAsync<TEntity>(new DetailsRequest<TIdentity>
        {
            Id = id,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Get many.
    /// Invokes the 'details/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The ids.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(ICollection<TIdentity> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsManyAsync<TEntity>(new DetailsManyRequest<TIdentity>
        {
            Ids = ids
        }, cancellationToken);
    }

    /// <summary>
    /// Get many.
    /// Invokes the 'details/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The ids.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(ICollection<TIdentity> ids, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsManyAsync<TEntity>(new DetailsManyRequest<TIdentity>
        {
            Ids = ids,
            IncludeDepth = includeDepth
        }, cancellationToken);
    }

    /// <summary>
    /// Details.
    /// Invokes the 'details' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DetailsRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entity.</returns>
    public virtual Task<TEntity?> DetailsAsync<TEntity>(DetailsRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<DetailsRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Details Many.
    /// Invokes the 'details' endpoint of the api, with multiple id's.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DetailsManyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> DetailsManyAsync<TEntity>(DetailsManyRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.InvokeAsync<DetailsManyRequest<TIdentity>, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The matching entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TCriteria>(QueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.InvokeAsync<QueryRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/first' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryFirstRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The first match entity.</returns>
    public virtual Task<TEntity?> QueryFirstAsync<TEntity, TCriteria>(QueryFirstRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<QueryFirstRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Query.
    /// Invokes the 'query/count' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="QueryCountRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The count of matching entities.</returns>
    public virtual async Task<int> QueryCountAsync<TEntity, TCriteria>(QueryCountRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await this.InvokeAsync<TEntity, QueryCountRequest<TCriteria>, string>(request, cancellationToken);

        int.TryParse(response, out var count);

        return count;
    }

    /// <summary>
    /// Create.
    /// Invokes the 'create' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The created entity.</returns>
    public virtual async Task<TEntity> CreateAsync<TEntity>(CreateRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityCreated = await this.InvokeAsync<CreateRequest, TEntity>(request, cancellationToken);

        return entityCreated ?? throw new NotFoundException(nameof(entityCreated));
    }

    /// <summary>
    /// Create And Get.
    /// Invokes the 'create/get' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateAndGetRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The created entity.</returns>
    public virtual async Task<TEntity?> CreateAndGetAsync<TEntity>(CreateAndGetRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityCreated = await this.InvokeAsync<CreateAndGetRequest, TEntity>(request, cancellationToken);

        return entityCreated ?? throw new NotFoundException(nameof(entityCreated));
    }

    /// <summary>
    /// Create Many.
    /// Invokes the 'create/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateManyRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task CreateManyAsync<TEntity>(CreateManyRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<CreateManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Create Many Bulk.
    /// Invokes the 'create/many/bulk' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateManyBulkRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual Task CreateManyBulkAsync<TEntity>(CreateManyBulkRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<CreateManyBulkRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Edit.
    /// Invokes the 'edit' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entity.</returns>
    public virtual Task<TEntity?> EditAsync<TEntity>(EditRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<EditRequest, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Edit And Get.
    /// Invokes the 'edit/get' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditAndGetRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entity.</returns>
    public virtual Task<TEntity?> EditAndGetAsync<TEntity>(EditAndGetRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<EditAndGetRequest, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Edit Many.
    /// Invokes the 'Edit/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditManyRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task EditManyAsync<TEntity>(EditManyRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<EditManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Edit Many Bulk.
    /// Invokes the 'Edit/many/bulk' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditManyBulkRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual Task EditManyBulkAsync<TEntity>(EditManyBulkRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.InvokeAsync<EditManyBulkRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Edit Query.
    /// Invokes the 'edit/query' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="EditQueryRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task EditQueryAsync<TEntity, TCriteria>(EditQueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<EditQueryRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Delete.
    /// Invokes the 'delete' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DeleteRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task DeleteAsync<TEntity>(DeleteRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<DeleteRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Delete Many.
    /// Invokes the 'delete/many' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DeleteManyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task DeleteManyAsync<TEntity>(DeleteManyRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<DeleteManyRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Delete Many Bulk.
    /// Invokes the 'delete/many/bulk' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DeleteManyBulkRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task DeleteManyBulkAsync<TEntity>(DeleteManyBulkRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<DeleteManyBulkRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Delete Query.
    /// Invokes the 'delete/query' endpoint of the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="DeleteQueryRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public virtual async Task DeleteQueryAsync<TEntity, TCriteria>(DeleteQueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.InvokeAsync<DeleteQueryRequest<TCriteria>, TEntity>(request, cancellationToken);
    }
}