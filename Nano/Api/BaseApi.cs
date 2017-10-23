using System;
using System.Threading.Tasks;
using Nano.Api.Requests;
using Nano.Api.Responses;
using Nano.App.Models.Interfaces;

namespace Nano.Api
{
    /// <summary>
    /// Api (facade).
    /// </summary>
    public abstract class BaseApi
    {
        /// <summary>
        /// Get.
        /// </summary>
        public virtual async Task<GetResponse<TEntity>> Get<TEntity>(GetRequest request)
            where TEntity : IEntity
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await HttpFacade<GetRequest, GetResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Query.
        /// </summary>
        public virtual async Task<QueryResponse<TEntity>> Query<TEntity>(QueryRequest request)
            where TEntity : IEntityCreatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await HttpFacade<QueryRequest, QueryResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Create.
        /// </summary>
        public virtual async Task<CreateResponse<TEntity>> Create<TEntity>(CreateRequest<TEntity> request)
            where TEntity : IEntityCreatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await HttpFacade<CreateRequest<TEntity>, CreateResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Update.
        /// </summary>
        public virtual async Task<UpdateResponse<TEntity>> Update<TEntity>(UpdateRequest<TEntity> request)
            where TEntity : IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await HttpFacade<UpdateRequest<TEntity>, UpdateResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Delete.
        /// </summary>
        public virtual async Task<DeleteResponse> Update<TEntity>(DeleteRequest<TEntity> request)
            where TEntity : IEntityDeletable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await HttpFacade<DeleteRequest<TEntity>, DeleteResponse>.instance.QueryAsync(request);
        }
    }
}