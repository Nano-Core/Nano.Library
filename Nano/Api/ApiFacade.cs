using System;
using System.Threading.Tasks;
using Nano.Api.Requests;
using Nano.Api.Requests.Interfaces;
using Nano.Api.Responses;
using Nano.Models.Interfaces;

namespace Nano.Api
{
    /// <summary>
    /// Api (facade).
    /// </summary>
    public sealed class ApiFacade
    {
        /// <summary>
        /// Get.
        /// </summary>
        public async Task<GetResponse<TEntity>> Get<TEntity>(GetRequest request)
            where TEntity : IEntity
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await HttpFacade<GetRequest, GetResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Query.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntity"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <returns>A <see cref="Task{T}"/>.</returns>
        public async Task<QueryResponse<TEntity>> Query<TEntity>(QueryRequest request)
            where TEntity : IEntity
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await HttpFacade<QueryRequest, QueryResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Create.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntityCreatable"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <returns>A <see cref="Task{T}"/>.</returns>
        public async Task<CreateResponse<TEntity>> Create<TEntity>(CreateRequest<TEntity> request)
            where TEntity : IEntityCreatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await HttpFacade<CreateRequest<TEntity>, CreateResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntityUpdatable"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <returns>A <see cref="Task{T}"/>.</returns>
        public async Task<UpdateResponse<TEntity>> Update<TEntity>(UpdateRequest<TEntity> request)
            where TEntity : IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await HttpFacade<UpdateRequest<TEntity>, UpdateResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Delete.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntityDeletable"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <returns>A <see cref="Task{T}"/>.</returns>
        public async Task<DeleteResponse> Delete<TEntity>(DeleteRequest<TEntity> request)
            where TEntity : IEntityDeletable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await HttpFacade<DeleteRequest<TEntity>, DeleteResponse>.instance.QueryAsync(request);
        }
    }
}