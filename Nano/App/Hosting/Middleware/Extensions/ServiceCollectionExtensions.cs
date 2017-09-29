using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Hosting.Middleware.Interfaces;

namespace Nano.App.Hosting.Middleware.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="IHttpContextMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddHttpContext(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<IHttpContextMiddleware, HttpContextMiddleware>();
        }

        /// <summary>
        /// Adds <see cref="IHttpRequestIdentifierMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddHttpRequestIdentifier(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<IHttpRequestIdentifierMiddleware, HttpRequestIdentifierMiddleware>();
        }

        /// <summary>
        /// Adds <see cref="IHttpRequestContentTypeMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddHttpRequestContentType(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<IHttpRequestContentTypeMiddleware, HttpRequestContentTypeMiddleware>();
        }
    }
}