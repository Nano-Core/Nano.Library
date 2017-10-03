using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.Data;
using Nano.Data.Interfaces;
using Nano.Data.Providers;
using Nano.Data.Providers.Interfaces;
using Nano.Eventing.Providers.Interfaces;
using Nano.Logging;
using Nano.Logging.Providers.Interfaces;
using Serilog;

namespace Nano.App.Config.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds logging provider of type <typeparamref name="TProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <typeparamref name="TProvider"/> type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddLogging<TProvider>(this IServiceCollection services)
            where TProvider : ILoggingProvider, new()
        {
            var options = services
                .BuildServiceProvider()
                .GetService<LoggingOptions>();

            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Sink<TProvider>()
                .MinimumLevel.Is(options.LogLevel);

            foreach (var @override in options.LogLevelOverrides)
            {
                loggerConfiguration
                    .MinimumLevel.Override(@override.Namespace, @override.LogLevel);
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            return services;
        }

        /// <summary>
        /// Adds eventing provider of type <typeparamref name="TProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <typeparamref name="TProvider"/> type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddEventing<TProvider>(this IServiceCollection services)
            where TProvider : class, IEventingProvider, new()
        {
            return services
                .AddSingleton<IEventingProvider, TProvider>();
        }

        /// <summary>
        /// Adds data provider for <see cref="IDbContext"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <see cref="IDataProvider"/> implementation.</typeparam>
        /// <typeparam name="TContext">The <see cref="IDbContext"/> implementation.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddDataContext<TProvider, TContext>(this IServiceCollection services)
            where TProvider : class, IDataProvider, new()
            where TContext : DbContext, IDbContext
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<IDbContext, TContext>()
                .AddSingleton<IDataProvider, TProvider>()
                .AddDbContext<TContext>((provider, builder) =>
                {
                    var type = typeof(TProvider);
                    var options = provider.GetRequiredService<DataOptions>();
                    var batchSize = options.BatchSize;
                    var connectionString = options.ConnectionString;

                    if (type == typeof(MySqlDataProvider))
                    {
                        builder.UseMySql(connectionString, x => x.MaxBatchSize(batchSize));
                    }
                    else if (type == typeof(SqlServerDataProvider))
                    {
                        builder.UseSqlServer(connectionString, x => x.MaxBatchSize(batchSize));
                    }
                    else 
                        throw new NotSupportedException("Data provider not supported.");
                });

            return services;
        }

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
                .AddConfigOptions<AppOptions>(configuration, "App", out _);

            return services;
        }

        /// <summary>
        /// Adds <see cref="IConfiguration"/> options to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddConfig(this IServiceCollection services, out IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            const string NAME = "appsettings";

            var path = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddEnvironmentVariables()
                .AddJsonFile($"{NAME}.json", false, true)
                .AddJsonFile($"{NAME}.{environment}.json", true)
                .Build();

            services
                .AddOptions()
                .AddSingleton(configuration);

            return services;
        }

        /// <summary>
        /// Adds a options <see cref="IConfigurationSection"/> as <see cref="IOptions{TOptions}"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TOption">The option implementation type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <param name="name">The name of the <see cref="IConfigurationSection"/>.</param>
        /// <param name="options">The <typeparamref name="TOption"/> instance.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddConfigOptions<TOption>(this IServiceCollection services, IConfiguration configuration, string name, out TOption options)
            where TOption : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var section = configuration.GetSection(name);

            if (section == null)
                throw new NullReferenceException("section");

            options = section.Get<TOption>();

            if (options == null)
                throw new NullReferenceException("options");

            services
                .AddSingleton(options)
                .Configure<TOption>(section);

            return services;
        }
    }
}