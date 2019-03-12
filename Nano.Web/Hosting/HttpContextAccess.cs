using System;
using Microsoft.AspNetCore.Http;

namespace Nano.Web.Hosting
{
    /// <summary>
    /// Http Context Access (Static)
    /// </summary>
    internal static class HttpContextAccess
    {
        private static IHttpContextAccessor accessor;
 
        /// <summary>
        /// Current.
        /// The current <see cref="HttpContext"/>, fetched through the <see cref="IHttpContextAccessor"/>.
        /// </summary>
        internal static HttpContext Current => HttpContextAccess.accessor.HttpContext;

        /// <summary>
        /// Configure.
        /// </summary>
        /// <param name="httpContextAccessor">The <inheritdoc cref="IHttpContextAccessor"/>.</param>
        internal static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccess.accessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
    }
}