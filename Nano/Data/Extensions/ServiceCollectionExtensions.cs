using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Services;
using Nano.App.Services.Interfaces;
using Nano.Config.Extensions;

namespace Nano.Data.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="DataOptions"/> options to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddScoped<IService, DefaultService>()
                .AddScoped<IServiceSpatial, DefaultServiceSpatial>()
                .AddConfigOptions<DataOptions>(configuration, DataOptions.SectionName, out var options);

            if (options.UseMemoryCache)
                services.AddDistributedMemoryCache();

            return services;
        }
    }
}