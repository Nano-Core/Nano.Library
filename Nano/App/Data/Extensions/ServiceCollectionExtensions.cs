using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Data.Options;
using Nano.Services;
using Nano.Services.Interfaces;

namespace Nano.App.Data.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds authentication identity to the <see cref="IServiceCollection"/>.
        /// Configures <see cref="DataContextOptions"/> for the passed <paramref name="configuration"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var section = configuration.GetSection("Data");
            var options = section?.Get<DataContextOptions>() ?? new DataContextOptions();

            services
                .AddSingleton(options)
                .Configure<DataContextOptions>(section);

            if (options.UseMemoryCache)
                services.AddDistributedMemoryCache();

            return services;
        }

        /// <summary>
        /// Adds data provider for <see cref="DbContext"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TContext">The <see cref="DbContext"/> implementation.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddDataContext<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<DbContext, TContext>()
                .AddScoped<IService, DefaultService<TContext>>()
                .AddScoped<IServiceSpatial, DefaultServiceSpatial<TContext>>()
                .AddDbContext<TContext>((provider, builder) =>
                {
                    var options = provider.GetRequiredService<DataContextOptions>();

                    switch (options.Provider)
                    {
                        case "MySql":
                             builder
                                .UseMySql(options.ConnectionString, x => x.MaxBatchSize(options.BatchSize));
                            break;

                        case "SqlServer":
                            builder
                                .UseSqlServer(options.ConnectionString, x => x.MaxBatchSize(options.BatchSize));
                            break;

                        default:
                            throw new NotSupportedException($"The provider: {options.Provider ?? "[NULL]"} is not supported.");
                    }
                });
        }
    }
}