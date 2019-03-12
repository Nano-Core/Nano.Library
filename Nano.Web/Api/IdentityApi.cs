using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Models;
using Nano.Web.Api.Requests.User;

namespace Nano.Web.Api
{
    /// <summary>
    /// Default Identity Api.
    /// </summary>
    public class IdentityApi : DefaultApi
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="apiOptions">The <see cref="ApiOptions"/>.</param>
        public IdentityApi(ApiOptions apiOptions)
            : base(apiOptions)
        {

        }
            
        /// <summary>
        /// Sign Up Async.
        /// </summary>
        /// <param name="request">The <see cref="SingUpRequest{TUser}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The instance of <typeparamref name="TUser"/>.</returns>
        public virtual async Task<TUser> SignUpAsync<TUser>(SingUpRequest<TUser> request, CancellationToken cancellationToken = default)
            where TUser : DefaultEntityUser
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<SingUpRequest<TUser>, TUser>(request, cancellationToken);
        }
    }
}