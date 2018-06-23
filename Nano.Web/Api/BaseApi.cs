using System;
using System.Threading.Tasks;
using Nano.Models.Interfaces;
using Nano.Web.Api.Requests;
using Nano.Web.Api.Requests.Interfaces;
using Nano.Web.Api.Responses;

namespace Nano.Web.Api
{
    // TODO: Generic Api-Client

    /// <summary>
    /// Api (facade).
    /// </summary>
    public abstract class BaseApi
    {
        /// <summary>
        /// Api Connect.
        /// </summary>
        protected virtual ApiConnect ApiConnect { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="apiConnect">The <see cref="ApiConnect"/>.</param>
        protected BaseApi(ApiConnect apiConnect)
        {
            if (apiConnect == null)
                throw new ArgumentNullException(nameof(apiConnect));

            this.ApiConnect = apiConnect;
        }

        /// <summary>
        /// Get.
        /// </summary>
        public virtual async Task<GetResponse<TEntity>> Get<TEntity>(GetRequest request)
            where TEntity : IEntity
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Connect = this.ApiConnect;
            request.Connect.Action = "details";

            return await HttpFacade<GetRequest, GetResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Get All.
        /// </summary>
        public virtual async Task<GetAllResponse<TEntity>> GetAll<TEntity>(GetAllRequest request)
            where TEntity : IEntity
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Connect = this.ApiConnect;
            request.Connect.Action = "index";

            return await HttpFacade<GetAllRequest, GetAllResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Query.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntity"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <returns>A <see cref="Task{T}"/>.</returns>
        public virtual async Task<QueryResponse<TEntity>> Query<TEntity>(QueryRequest request)
            where TEntity : IEntity
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Connect = this.ApiConnect;
            request.Connect.Action = "query";

            return await HttpFacade<QueryRequest, QueryResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Create.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntityCreatable"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <returns>A <see cref="Task{T}"/>.</returns>
        public virtual async Task<CreateResponse<TEntity>> Create<TEntity>(CreateRequest<TEntity> request)
            where TEntity : IEntityCreatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Connect = this.ApiConnect;
            request.Connect.Action = "create";

            return await HttpFacade<CreateRequest<TEntity>, CreateResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntityUpdatable"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <returns>A <see cref="Task{T}"/>.</returns>
        public virtual async Task<UpdateResponse<TEntity>> Update<TEntity>(UpdateRequest<TEntity> request)
            where TEntity : IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Connect = this.ApiConnect;
            request.Connect.Action = "edit";

            return await HttpFacade<UpdateRequest<TEntity>, UpdateResponse<TEntity>>.instance.QueryAsync(request);
        }

        /// <summary>
        /// Delete.
        /// </summary>
        /// <typeparam name="TEntity">The type of <see cref="IEntityDeletable"/>.</typeparam>
        /// <param name="request">The <see cref="IRequest"/>.</param>
        /// <returns>A <see cref="Task{T}"/>.</returns>
        public virtual async Task<DeleteResponse> Delete<TEntity>(DeleteRequest<TEntity> request)
            where TEntity : IEntityDeletable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.Connect = this.ApiConnect;
            request.Connect.Action = "delete";

            return await HttpFacade<DeleteRequest<TEntity>, DeleteResponse>.instance.QueryAsync(request);
        }
    }
}