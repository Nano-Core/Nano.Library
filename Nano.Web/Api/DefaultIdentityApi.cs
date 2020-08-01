using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nano.Models.Interfaces;
using Nano.Web.Api.Requests;
using Nano.Web.Api.Requests.Identity;

namespace Nano.Web.Api
{
    /// <summary>
    /// Default Identity Api.
    /// </summary>
    public class DefaultIdentityApi<TUser> : BaseIdentityApi<TUser, Guid>
        where TUser : IEntityUser<Guid>
    {
        /// <inheritdoc />
        public DefaultIdentityApi(ApiOptions apiOptions)
            : base(apiOptions)
        {

        }

        /// <inheritdoc />
        public override async Task<TUser> SignUpAsync(SignUpRequest<TUser, Guid> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await base.SignUpAsync(request, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<TUser> SignUpExternalAsync(SignUpExternalRequest<TUser, Guid> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await base.SignUpExternalAsync(request, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<TEntity> DetailsAsync<TEntity>(DetailsRequest<Guid> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await base.DetailsAsync<TEntity>(request, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<TEntity>> DetailsManyAsync<TEntity>(DetailsManyRequest<Guid> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await base.DetailsManyAsync<TEntity>(request, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task DeleteAsync<TEntity>(DeleteRequest<Guid> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await base.DeleteAsync<TEntity>(request, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task DeleteManyAsync<TEntity>(DeleteManyRequest<Guid> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await base.DeleteManyAsync<TEntity>(request, cancellationToken);
        }
    }
}