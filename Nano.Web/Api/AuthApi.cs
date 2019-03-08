using System;
using System.Collections.Generic;
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
        /// <param name="request">The <see cref="LogInRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> LogInAsync(LogInRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<LogInRequest, AccessToken>(request, cancellationToken);
        }

        /// <summary>
        /// Log Out Async.
        /// </summary>
        /// <param name="request">The <see cref="LogOutRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void..</returns>
        public virtual async Task LogOutAsync(LogOutRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.CustomAsync(request, cancellationToken);
        }
        
        /// <summary>
        /// Get External Schemes Async.
        /// </summary>
        /// <param name="request">The <see cref="ExternalSchemesRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A collection of <see cref="ExternalScheme"/>'s.</returns>
        public virtual async Task<IEnumerable<ExternalScheme>> GetExternalSchemesAsync(ExternalSchemesRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<ExternalSchemesRequest, IEnumerable<ExternalScheme>>(request, cancellationToken);
        }
    }
}