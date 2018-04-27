using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.Config.Extensions;

namespace Nano.App.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="AppOptions"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddApp(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddConfigOptions<AppOptions>(configuration, AppOptions.SectionName, out var _);

            return services;
        }

        /// <summary>
        /// Outputs the <see cref="IServiceCollection"/> to log.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection LogServices(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var logger = services.BuildServiceProvider().GetService<ILogger>();

            var servicesStr = services
                .OrderBy(x => x.ServiceType.FullName)
                .Aggregate(string.Empty, (current, service) => current + $"{service.ServiceType.FullName} => {service.ImplementationType?.FullName ?? "None"} ({service.Lifetime}){Environment.NewLine}");

            logger.LogDebug($"Total services registered: {services.Count}");
            logger.LogDebug(servicesStr);

            return services;
        }
    }
}