using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.Config.Extensions;
using Nano.Console.Workers;
using Nano.Models.Extensions;
using Nano.Repository;
using Nano.Repository.Interfaces;

namespace Nano.Console.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="ConsoleOptions"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddConsole(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            
            services
                .AddConfigOptions<ConsoleOptions>(configuration, ConsoleOptions.SectionName, out _);

            services
                .AddScoped<IRepository, DefaultRepository>()
                .AddScoped<IRepositorySpatial, DefaultRepositorySpatial>();

            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    !x.IsAbstract &&
                    x.IsTypeOf(typeof(BaseWorker)))
                .ToList()
                .ForEach(x =>
                {
                    services
                        .AddSingleton(typeof(IHostedService), x);
                });

            return services;
        }
    }
}