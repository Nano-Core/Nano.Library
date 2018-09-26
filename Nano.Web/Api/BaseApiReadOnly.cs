using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.Web.Api.Requests;

namespace Nano.Web.Api
{
    /// <inheritdoc />
    public abstract class BaseApiReadOnly : BaseApi
    {
        /// <inheritdoc />
        protected BaseApiReadOnly(ApiOptions apiOptions)
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
        /// Query.
        /// Invokes the 'query' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="QueryRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The matching entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> Query<TEntity>(QueryRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<QueryRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Details.
        /// Invokes the 'details' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DetailsRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The matching entities.</returns>
        public virtual async Task<TEntity> Details<TEntity>(DetailsRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<DetailsRequest, TEntity>(request, cancellationToken);
        }
    }
}