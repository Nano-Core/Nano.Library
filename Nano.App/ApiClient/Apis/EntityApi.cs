using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Requests;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Apis;

/// <summary>
/// 
/// </summary>
public sealed class EntityApi<TIdentity>(ApiClient api)
    where TIdentity : IEquatable<TIdentity>
{
    private readonly ApiClient api = api ?? throw new ArgumentNullException(nameof(api));


    #region Read

    /// <summary>
    /// Executes <c>index</c> for <typeparamref name="TEntity"/> to retrieve a collection of entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The index request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of entities.</returns>
    public async Task<IEnumerable<TEntity>> IndexAsync<TEntity>(IndexRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.api
            .InvokeAsync<IndexRequest, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>details</c> for <typeparamref name="TEntity"/> to retrieve a single entity by request.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The details request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching entity, or <c>null</c> if not found.</returns>
    public Task<TEntity?> DetailsAsync<TEntity>(DetailsRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.api
            .InvokeAsync<DetailsRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>details</c> for <typeparamref name="TEntity"/> to retrieve a single entity by identifier.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching entity, or <c>null</c> if not found.</returns>
    public Task<TEntity?> GetAsync<TEntity>(TIdentity id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsAsync<TEntity>(new DetailsRequest<TIdentity>
        {
            Id = id
        }, cancellationToken);
    }

    /// <summary>
    /// Executes <c>details</c> for <typeparamref name="TEntity"/> to retrieve a single entity with include depth.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The entity identifier.</param>
    /// <param name="includeDepth">The include depth level.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching entity, or <c>null</c> if not found.</returns>
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
    /// Executes <c>details/many</c> for <typeparamref name="TEntity"/> to retrieve multiple entities by request.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The details-many request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of entities.</returns>
    public async Task<IEnumerable<TEntity>> DetailsManyAsync<TEntity>(DetailsManyRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.api
            .InvokeAsync<DetailsManyRequest<TIdentity>, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>details/many</c> for <typeparamref name="TEntity"/> to retrieve multiple entities by identifiers.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The entity identifiers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of entities.</returns>
    public Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(ICollection<TIdentity> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>
    {
        return this.DetailsManyAsync<TEntity>(new DetailsManyRequest<TIdentity>
        {
            Ids = ids
        }, cancellationToken);
    }

    /// <summary>
    /// Executes <c>details/many</c> for <typeparamref name="TEntity"/> to retrieve multiple entities by identifiers with include depth.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The entity identifiers.</param>
    /// <param name="includeDepth">The include depth level.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of entities.</returns>
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
    /// Executes <c>query</c> for <typeparamref name="TEntity"/> to retrieve matching entities by request.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The query criteria type.</typeparam>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of matching entities.</returns>
    public async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TCriteria>(QueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.api
            .InvokeAsync<QueryRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
    }

    /// <summary>
    /// Executes <c>query/first</c> for <typeparamref name="TEntity"/> to retrieve the first matching entity by request.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The query criteria type.</typeparam>
    /// <param name="request">The query-first request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The first matching entity, or <c>null</c> if none found.</returns>
    public Task<TEntity?> QueryFirstAsync<TEntity, TCriteria>(QueryFirstRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.api
            .InvokeAsync<QueryFirstRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Executes <c>query/count</c> for <typeparamref name="TEntity"/> to count matching entities by request.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The query criteria type.</typeparam>
    /// <param name="request">The query count request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of matching entities.</returns>
    public async Task<int> QueryCountAsync<TEntity, TCriteria>(QueryCountRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await this.api
            .InvokeAsync<TEntity, QueryCountRequest<TCriteria>, string>(request, cancellationToken);

        int.TryParse(response, out var count);

        return count;
    }

    #endregion


    #region Create

    /// <summary>
    /// Invokes the 'create' endpoint of the entity in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The create request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created entity.</returns>
    public async Task<TEntity> CreateAsync<TEntity>(CreateRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityCreated = await this.api
            .InvokeAsync<CreateRequest, TEntity>(request, cancellationToken);

        return entityCreated ?? throw new NotFoundException(nameof(entityCreated));
    }

    /// <summary>
    /// Invokes the 'create/edit' endpoint of the entity in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The create or edit request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created or edited entity.</returns>
    public async Task<TEntity> CreateOrEditAsync<TEntity>(CreateOrEditRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatableAndUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityCreated = await this.api
            .InvokeAsync<CreateOrEditRequest, TEntity>(request, cancellationToken);

        return entityCreated ?? throw new NotFoundException(nameof(entityCreated));
    }

    /// <summary>
    /// Invokes the 'create/get' endpoint of the entity in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The create-or-get request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created or existing entity.</returns>
    public async Task<TEntity> CreateOrGetAsync<TEntity>(CreateOrGetRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityCreated = await this.api
            .InvokeAsync<CreateOrGetRequest, TEntity>(request, cancellationToken);

        return entityCreated ?? throw new NotFoundException(nameof(entityCreated));
    }

    /// <summary>
    /// Invokes the 'create/reload' endpoint of the entity in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The create-and-get request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created entity.</returns>
    public async Task<TEntity?> CreateAndGetAsync<TEntity>(CreateAndGetRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityCreated = await this.api
            .InvokeAsync<CreateAndGetRequest, TEntity>(request, cancellationToken);

        return entityCreated ?? throw new NotFoundException(nameof(entityCreated));
    }

    /// <summary>
    /// Invokes the 'create/many' endpoint of the entity in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The bulk create request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Operation result.</returns>
    public async Task CreateManyAsync<TEntity>(CreateManyRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<CreateManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'create/many' bulk endpoint of the entity in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The bulk request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Operation result.</returns>
    public Task CreateManyBulkAsync<TEntity>(CreateManyBulkRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.api
            .InvokeAsync<CreateManyBulkRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    #endregion


    #region Edit

    /// <summary>
    /// Updates an entity via the 'edit' endpoint.
    /// Route: edit
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The edit request containing the updated entity data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated entity instance.</returns>
    public async Task<TEntity?> EditAsync<TEntity>(EditRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityEdited = await this.api
            .InvokeAsync<EditRequest, TEntity>(request, cancellationToken);

        return entityEdited ?? throw new NotFoundException(nameof(entityEdited));
    }

    /// <summary>
    /// Updates and returns an entity via the 'edit/get' endpoint.
    /// Route: edit/get
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The edit-and-get request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated entity instance.</returns>
    public async Task<TEntity?> EditAndGetAsync<TEntity>(EditAndGetRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityEdited = await this.api
            .InvokeAsync<EditAndGetRequest, TEntity>(request, cancellationToken);

        return entityEdited ?? throw new NotFoundException(nameof(entityEdited));
    }

    /// <summary>
    /// Updates multiple entities via the 'edit/many' endpoint.
    /// Route: edit/many
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The batch edit request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the operation.</returns>
    public async Task EditManyAsync<TEntity>(EditManyRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<EditManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Bulk updates multiple entities via the 'edit/many/bulk' endpoint.
    /// Route: edit/many/bulk
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The bulk edit request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the operation.</returns>
    public Task EditManyBulkAsync<TEntity>(EditManyBulkRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.api
            .InvokeAsync<EditManyBulkRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Updates entities matching a query via the 'edit/query' endpoint.
    /// Route: edit/query
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The query criteria type.</typeparam>
    /// <param name="request">The query-based edit request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the operation.</returns>
    public async Task EditQueryAsync<TEntity, TCriteria>(EditQueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<EditQueryRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Bulk (batch) updates entities matching a query via the 'edit/query/bulk' endpoint.
    /// Route: edit/query/bulk
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The query criteria type.</typeparam>
    /// <param name="request">The bulk query edit request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the operation.</returns>
    public async Task EditQueryBulkAsync<TEntity, TCriteria>(EditQueryBulkRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<EditQueryBulkRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    #endregion


    #region Delete

    /// <summary>
    /// Deletes a single entity using the 'delete' endpoint.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The request containing the entity identity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteAsync<TEntity>(DeleteRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<DeleteRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Deletes a single entity by identifier using the 'delete' endpoint.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteAsync<TEntity>(TIdentity id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        return this.DeleteAsync<TEntity>(new DeleteRequest<TIdentity>
        {
            Id = id
        }, cancellationToken);
    }

    /// <summary>
    /// Deletes multiple entities using the 'delete/many' endpoint.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The request containing entity identifiers.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteManyAsync<TEntity>(DeleteManyRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<DeleteManyRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Deletes multiple entities by identifiers using the 'delete/many' endpoint.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The entity identifiers.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteManyAsync<TEntity>(IEnumerable<TIdentity> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(ids);

        return this.DeleteManyAsync<TEntity>(new DeleteManyRequest<TIdentity>
        {
            Ids = ids
        }, cancellationToken);
    }

    /// <summary>
    /// Deletes multiple entities in bulk using the 'delete/many/bulk' endpoint.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The bulk delete request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteManyBulkAsync<TEntity>(DeleteManyBulkRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<DeleteManyBulkRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Deletes multiple entities in bulk by identifiers using the 'delete/many/bulk' endpoint.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The entity identifiers.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteManyBulkAsync<TEntity>(IEnumerable<TIdentity> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(ids);

        return this.DeleteManyBulkAsync<TEntity>(new DeleteManyBulkRequest<TIdentity>
        {
            Ids = ids
        }, cancellationToken);
    }

    /// <summary>
    /// Deletes entities matching a query using the 'delete/query' endpoint.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type.</typeparam>
    /// <param name="request">The delete query request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteQueryAsync<TEntity, TCriteria>(DeleteQueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<DeleteQueryRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Deletes (batch) entities in bulk using a query via the 'delete/query/bulk' endpoint.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type.</typeparam>
    /// <param name="request">The bulk delete query request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteQueryBulkAsync<TEntity, TCriteria>(DeleteQueryBulkRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<DeleteQueryBulkRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    #endregion
}