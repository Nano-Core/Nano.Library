using System;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.TimeZone
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services to the <see cref="IServiceCollection"/>, required for request timezone support
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="defaultTimeZone">The name of the time zone that should be used as default.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRequestTimeZone(this IServiceCollection services, string defaultTimeZone)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddSingleton(x => new RequestTimeZoneOptions
                {
                    DefaultRequestTimeZone = new RequestTimeZone(defaultTimeZone)
                })
                .AddSingleton<RequestTimeZoneMiddleware>();

            return services;
        }
    }
}