using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Nano.Models.Interfaces;
using Nano.Web.Api.Requests;

namespace Nano.Web.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultApi : BaseApi
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="apiOptions">The <see cref="Api.ApiOptions"/>.</param>
        public DefaultApi(ApiOptions apiOptions)
            : base(apiOptions)
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
        public virtual async Task<IEnumerable<TEntity>> Index<TEntity>(IndexRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<IndexRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Details.
        /// Invokes the 'details' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DetailsRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The matching entity.</returns>
        public virtual async Task<TEntity> Details<TEntity>(DetailsRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<DetailsRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Details Many.
        /// Invokes the 'details' endpoint of the api, with multiple id's.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DetailsManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The matching entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> Details<TEntity>(DetailsManyRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<DetailsManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Query.
        /// Invokes the 'query' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TCriteria">The criteira type</typeparam>
        /// <param name="request">The <see cref="QueryRequest{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The matching entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> Query<TEntity, TCriteria>(QueryRequest<TCriteria> request, CancellationToken cancellationToken = default) 
            where TCriteria : IQueryCriteria, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<QueryRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Query.
        /// Invokes the 'query/first' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TCriteria">The criteira type</typeparam>
        /// <param name="request">The <see cref="QueryFirstRequest{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The first match entity.</returns>
        public virtual async Task<IEnumerable<TEntity>> QueryFirst<TEntity, TCriteria>(QueryFirstRequest<TCriteria> request, CancellationToken cancellationToken = default) 
            where TCriteria : IQueryCriteria, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<QueryFirstRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Create.
        /// Invokes the 'create' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="CreateRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The created entity.</returns>
        public virtual async Task<TEntity> Create<TEntity>(CreateRequest request, CancellationToken cancellationToken = default)
            where TEntity : IEntityCreatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<CreateRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Create Many.
        /// Invokes the 'create/many' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="CreateManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The created entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> CreateMany<TEntity>(CreateManyRequest request, CancellationToken cancellationToken = default)
            where TEntity : IEntityCreatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<CreateManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Edit.
        /// Invokes the 'edit' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="EditRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The updated entity.</returns>
        public virtual async Task<TEntity> Edit<TEntity>(EditRequest request, CancellationToken cancellationToken = default)
            where TEntity : IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<EditRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Edit Many.
        /// Invokes the 'Edit/many' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="EditManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The updated entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> EditMany<TEntity>(EditManyRequest request, CancellationToken cancellationToken = default)
            where TEntity : IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<EditManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Edit Many.
        /// Invokes the 'Edit/query' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="EditManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The updated entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> EditQuery<TEntity>(EditQueryRequest request, CancellationToken cancellationToken = default)
            where TEntity : IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<EditQueryRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Delete.
        /// Invokes the 'delete' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DeleteRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Nothing.</returns>
        public virtual async Task Delete<TEntity>(DeleteRequest request, CancellationToken cancellationToken = default)
            where TEntity : IEntityDeletable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.Invoke<DeleteRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Delete Many.
        /// Invokes the 'delete/many' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DeleteManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Nothing.</returns>
        public virtual async Task DeleteMany<TEntity>(DeleteManyRequest request, CancellationToken cancellationToken = default)
            where TEntity : IEntityDeletable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.Invoke<DeleteManyRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Delete Query.
        /// Invokes the 'delete/query' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DeleteManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Nothing.</returns>
        public virtual async Task DeleteQuery<TEntity>(DeleteQueryRequest request, CancellationToken cancellationToken = default)
            where TEntity : IEntityDeletable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.Invoke<DeleteQueryRequest, TEntity>(request, cancellationToken);
        }
    }
}