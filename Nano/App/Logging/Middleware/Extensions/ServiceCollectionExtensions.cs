using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Logging.Middleware.Interfaces;

namespace Nano.App.Logging.Middleware.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="IHttpContextLoggingMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddHttpContext(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<IHttpContextLoggingMiddleware, HttpContextLoggingMiddleware>();
        }
    }
}