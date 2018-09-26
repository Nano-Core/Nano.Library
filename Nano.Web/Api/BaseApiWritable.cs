using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.Models.Interfaces;
using Nano.Web.Api.Requests;

namespace Nano.Web.Api
{
    /// <inheritdoc />
    public abstract class BaseApiWritable : BaseApiReadOnly
    {
        /// <inheritdoc />
        protected BaseApiWritable(ApiOptions apiOptions)
            : base(apiOptions)
        {

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
    }
}