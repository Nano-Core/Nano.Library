using System;
using System.Threading;
using System.Threading.Tasks;
using Nano.Models.Auth;
using Nano.Web.Api.Requests.Auth;

namespace Nano.Web.Api
{
    /// <summary>
    /// Auth Api.
    /// </summary>
    public class AuthApi : BaseApi
    {
        /// <inheritdoc />
        public AuthApi(ApiOptions apiOptions) 
            : base(apiOptions)
        {

        }

        /// <summary>
        /// Log In Async.
        /// </summary>
        /// <param name="request">The <see cref="LoginRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> LogInAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Custom<LoginRequest, AccessToken>(request, cancellationToken);
        }

        /// <summary>
        /// Log Out Async.
        /// </summary>
        /// <param name="request">The <see cref="LoginRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> LogOutAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Custom<LoginRequest, AccessToken>(request, cancellationToken);
        }
    }
}