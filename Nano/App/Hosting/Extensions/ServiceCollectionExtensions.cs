using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Hosting.Options;

namespace Nano.App.Hosting.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds hosting to the <see cref="IServiceCollection"/>.
        /// Configures <see cref="HostingOptions"/> for the passed <paramref name="configuration"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddHosting(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var section = configuration.GetSection("Hosting");
            var options = section?.Get<HostingOptions>() ?? new HostingOptions();
            var assemblyPart = Assembly.GetAssembly(typeof(BaseApplication<>));

            services
                .AddMvc()
                .AddControllersAsServices()
                .AddViewComponentsAsServices()
                .AddApplicationPart(assemblyPart);

            services
                .AddSingleton(options)
                .Configure<HostingOptions>(section);

            if (options.EnableSession)
                services.AddSession();

            if (options.EnableGzipCompression)
                services.AddGzipCompression();

            return services;
        }

        /// <summary>
        /// Adds <see cref="GzipCompressionProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddGzipCompression(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddResponseCompression(y => y.Providers.Add<GzipCompressionProvider>());
        }
    }
}