using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nano.App.Startup;
using Nano.App.Startup.Tasks;
using Nano.Config.Extensions;
using Nano.Models.Extensions;

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
                .AddConfigOptions<AppOptions>(configuration, AppOptions.SectionName, out _);

            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    !x.IsAbstract &&
                    x.IsTypeDef(typeof(BaseStartupTask)))
                .ToList()
                .ForEach(x =>
                {
                    services
                        .AddSingleton(typeof(IHostedService), x);
                });

            services
                .AddSingleton<StartupTaskContext>()
                .AddHostedService<InitializeApplicationStartupTask>();

            return services;
        }

        /// <summary>
        /// Gets a colletion of dependencies registered in the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="isUnique">Bool indicating if only unique service dependencies should be returned.</param>
        /// <returns>The <see cref="IEnumerator{T}"/> of <see cref="ServiceDescriptor"/> items.</returns>
        internal static IEnumerable<ServiceDescriptor> GetServices(this IServiceCollection services, bool isUnique = true)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var list = new List<ServiceDescriptor>();

            services
                .OrderBy(x => x.ServiceType.FullName)
                .ToList()
                .ForEach(x =>
                {
                    if (isUnique)
                    {
                        var exists = list.Any(y =>
                            x.ServiceType == y.ServiceType &&
                            x.ImplementationType == y.ImplementationType &&
                            x.Lifetime == y.Lifetime);

                        if (!exists)
                            list.Add(x);
                    }
                    else
                        list.Add(x);
                });

            return list;
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

            var list = services.GetServices().ToList();
            var logger = services.BuildServiceProvider().GetService<ILogger>();

            var servicesStr = list
                .Aggregate(string.Empty, (x, y) => 
                    x + $"{y.ServiceType.GetFriendlyName() } => {y.ImplementationType?.GetFriendlyName() ?? "None"} ({y.Lifetime}){Environment.NewLine}");

            logger.LogDebug($"Total services registered: {list.Count}");
            logger.LogDebug(servicesStr);

            return services;
        }
    }
}