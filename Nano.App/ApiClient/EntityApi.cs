using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Nano.App.ApiClient.Requests;
using Nano.Data.Abstractions.Exceptions;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient;

/// <summary>
/// 
/// </summary>
public sealed class EntityApi<TIdentity>(ApiClient api)
    where TIdentity : IEquatable<TIdentity>
{
    private readonly ApiClient api = api ?? throw new ArgumentNullException(nameof(api));


    #region Read

    /// <summary>
    /// Invokes the 'index' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="IndexRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The entities.</returns>
    public async Task<IEnumerable<TEntity>> IndexAsync<TEntity>(IndexRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(request);

        return await this.api
            .InvokeAsync<IndexRequest, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
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

        return this.api
            .InvokeAsync<DetailsRequest<TIdentity>, TEntity>(request, cancellationToken);
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

        return await this.api
            .InvokeAsync<DetailsManyRequest<TIdentity>, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
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

        return await this.api
            .InvokeAsync<QueryRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken) ?? [];
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

        return this.api
            .InvokeAsync<QueryFirstRequest<TCriteria>, TEntity>(request, cancellationToken);
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

        var response = await this.api
            .InvokeAsync<TEntity, QueryCountRequest<TCriteria>, string>(request, cancellationToken);

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

    #endregion


    #region Create

    /// <summary>
    /// Invokes the 'create' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
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
    /// Invokes the 'create' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entity">The entity of type <see cref="IEntityCreatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The created entity.</returns>
    public Task<TEntity> CreateAsync<TEntity>(IEntityCreatable entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.CreateAsync<TEntity>(new CreateRequest
        {
            Entity = entity
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'create/get' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateOrGetRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The created entity.</returns>
    public async Task<TEntity> CreateOrGetAsync<TEntity>(CreateOrGetRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityCreated = await this.api
            .InvokeAsync<CreateOrGetRequest, TEntity>(request, cancellationToken);

        return entityCreated ?? throw new NotFoundException(nameof(entityCreated));
    }

    /// <summary>
    /// Invokes the 'create/get' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entity">The entity of type <see cref="IEntityCreatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The created entity.</returns>
    public Task<TEntity> CreateOrGetAsync<TEntity>(IEntityCreatable entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.CreateOrGetAsync<TEntity>(new CreateOrGetRequest
        {
            Entity = entity
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'create/reload' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateAndGetRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
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
    /// Invokes the 'create/reload' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entity">The entity of type <see cref="IEntityCreatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The created entity.</returns>
    public Task<TEntity?> CreateAndGetAsync<TEntity>(IEntityCreatable entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.CreateAndGetAsync<TEntity>(new CreateAndGetRequest
        {
            Entity = entity
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'create/many' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateManyRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public async Task CreateManyAsync<TEntity>(CreateManyRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<CreateManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'create/many' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entities">The entities of type <see cref="IEntityCreatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public Task CreateManyAsync<TEntity>(IEnumerable<IEntityCreatable> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(entities);

        return this.CreateManyAsync<TEntity>(new CreateManyRequest
        {
            Entities = entities
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'create/many' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="CreateManyBulkRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public Task CreateManyBulkAsync<TEntity>(CreateManyBulkRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.api
            .InvokeAsync<CreateManyBulkRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'create/many' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entities">The entities of type <see cref="IEntityCreatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public Task CreateManyBulkAsync<TEntity>(IEnumerable<IEntityCreatable> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(entities);

        return this.CreateManyBulkAsync<TEntity>(new CreateManyBulkRequest
        {
            Entities = entities
        }, cancellationToken);
    }

    #endregion


    #region Edit

    /// <summary>
    /// Invokes the 'edit' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entity.</returns>
    public async Task<TEntity?> EditAsync<TEntity>(EditRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityEdited = await this.api
            .InvokeAsync<EditRequest, TEntity>(request, cancellationToken);

        return entityEdited ?? throw new NotFoundException(nameof(entityEdited));
    }

    /// <summary>
    /// Invokes the 'edit' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entity">The entity of type <see cref="IEntityUpdatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entity.</returns>
    public Task<TEntity?> EditAsync<TEntity>(IEntityUpdatable entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.EditAsync<TEntity>(new EditRequest
        {
            Entity = entity
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'edit/get' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditAndGetRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entity.</returns>
    public async Task<TEntity?> EditAndGetAsync<TEntity>(EditAndGetRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(request);

        var entityEdited = await this.api
            .InvokeAsync<EditAndGetRequest, TEntity>(request, cancellationToken);

        return entityEdited ?? throw new NotFoundException(nameof(entityEdited));
    }

    /// <summary>
    /// Invokes the 'edit/get' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entity">The entity of type <see cref="IEntityUpdatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entity.</returns>
    public Task<TEntity?> EditGetAsync<TEntity>(IEntityUpdatable entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable, IEntityIdentity<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(entity);

        return this.EditAndGetAsync<TEntity>(new EditAndGetRequest
        {
            Entity = entity
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'edit/many' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditManyRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public async Task EditManyAsync<TEntity>(EditManyRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<EditManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'edit/many' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entities">The entities of type <see cref="IEntityUpdatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public Task EditManyAsync<TEntity>(IEnumerable<IEntityUpdatable> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(entities);

        return this.EditManyAsync<TEntity>(new EditManyRequest
        {
            Entities = entities
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'edit/many/bulk' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="EditManyBulkRequest"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public Task EditManyBulkAsync<TEntity>(EditManyBulkRequest request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.api
            .InvokeAsync<EditManyBulkRequest, IEnumerable<TEntity>>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'edit/many/bulk' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entities">The entities of type <see cref="IEntityUpdatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public Task EditManyBulkAsync<TEntity>(IEnumerable<IEntityUpdatable> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(entities);

        return this.EditManyBulkAsync<TEntity>(new EditManyBulkRequest
        {
            Entities = entities
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'edit/query' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="EditQueryRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public async Task EditQueryAsync<TEntity, TCriteria>(EditQueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<EditQueryRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'edit/query' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="criteria">The criteria used to select entities to update.</param>
    /// <param name="propertyUpdates">A dictionary of property names and their new values to update on the selected entities.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public Task EditQueryAsync<TEntity, TCriteria>(TCriteria criteria, IDictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);
        ArgumentNullException.ThrowIfNull(propertyUpdates);

        return this.EditQueryAsync<TEntity, TCriteria>(new EditQueryRequest<TCriteria>
        {
            Query =
            {
                Criteria = criteria,
                PropertyUpdates = propertyUpdates
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'edit/query/bulk' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="EditQueryRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public async Task EditQueryBulkAsync<TEntity, TCriteria>(EditQueryBulkRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<EditQueryBulkRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'edit/query/bulk' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="criteria">The criteria used to select entities to update.</param>
    /// <param name="propertyUpdates">A dictionary of property names and their new values to update on the selected entities.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public Task EditQueryBulkAsync<TEntity, TCriteria>(TCriteria criteria, IDictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);
        ArgumentNullException.ThrowIfNull(propertyUpdates);

        return this.EditQueryBulkAsync<TEntity, TCriteria>(new EditQueryBulkRequest<TCriteria>
        {
            Query =
            {
                Criteria = criteria,
                PropertyUpdates = propertyUpdates
            }
        }, cancellationToken);
    }

    #endregion


    #region Delete

    /// <summary>
    /// Invokes the 'delete' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DeleteRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public async Task DeleteAsync<TEntity>(DeleteRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<DeleteRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'delete' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public Task DeleteAsync<TEntity>(TIdentity id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        return this.DeleteAsync<TEntity>(new DeleteRequest<TIdentity>
        {
            Id = id
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'delete/many' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DeleteManyRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public async Task DeleteManyAsync<TEntity>(DeleteManyRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<DeleteManyRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'delete/many' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The ids.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
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
    /// Invokes the 'delete/many/bulk' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="request">The <see cref="DeleteManyBulkRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public async Task DeleteManyBulkAsync<TEntity>(DeleteManyBulkRequest<TIdentity> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<DeleteManyBulkRequest<TIdentity>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'delete/many/bulk' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="ids">The ids.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
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
    /// Invokes the 'delete/query' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="DeleteQueryRequest{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public async Task DeleteQueryAsync<TEntity, TCriteria>(DeleteQueryRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<DeleteQueryRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'delete/query' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="criteria">The criteria used to select entities to update.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public Task DeleteQueryAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);

        return this.DeleteQueryAsync<TEntity, TCriteria>(new DeleteQueryRequest<TCriteria>
        {
            Criteria = criteria
        }, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'delete/query/bulk' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="request">The <see cref="DeleteQueryBulkRequest{TIdentity}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public async Task DeleteQueryBulkAsync<TEntity, TCriteria>(DeleteQueryBulkRequest<TCriteria> request, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(request);

        await this.api
            .InvokeAsync<DeleteQueryBulkRequest<TCriteria>, TEntity>(request, cancellationToken);
    }

    /// <summary>
    /// Invokes the 'delete/query/bulk' endpoint of the <typeparamref name="TEntity"/> in the api.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TCriteria">The criteria type</typeparam>
    /// <param name="criteria">The criteria used to select entities to update.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>Nothing.</returns>
    public Task DeleteQueryBulkAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TIdentity>, IEntityDeletable
        where TCriteria : IQueryCriteria, new()
    {
        ArgumentNullException.ThrowIfNull(criteria);

        return this.DeleteQueryBulkAsync<TEntity, TCriteria>(new DeleteQueryBulkRequest<TCriteria>
        {
            Criteria = criteria
        }, cancellationToken);
    }

    #endregion
}