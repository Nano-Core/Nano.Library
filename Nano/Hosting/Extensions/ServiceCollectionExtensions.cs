using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Config;
using Nano.App.Config.Extensions;
using Nano.Hosting.Middleware.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace Nano.Hosting.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="HostingOptions"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHosting(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddVersioning()
                .AddHttpRequestContentTypes()
                .AddConfigOptions<HostingOptions>(configuration, "Hosting", out var options)
                .AddMvc()
                    .AddControllersAsServices()
                    .AddViewComponentsAsServices()
                    .AddApplicationPart(Assembly.GetExecutingAssembly());

            if (options.EnableSession)
                services.AddSession();

            if (options.EnableDocumentation)
                services.AddDocumentation(configuration);

            if (options.EnableGzipCompression)
                services.AddGzipCompression();

            if (options.EnableHttpRequestLocalization)
                services.AddLocalizations();

            if (options.EnableHttpRequestIdentifier)
                services.AddHttpRequestIdentifier();

            if (options.EnableHttpContextExtension)
                services.AddHttpContextExtensions();

            return services;
        }

        /// <summary>
        /// Adds api versioningto the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddApiVersioning(x =>
                {
                    x.ReportApiVersions = true;
                    x.DefaultApiVersion = new ApiVersion(1, 0);
                    x.AssumeDefaultVersionWhenUnspecified = true;
                    x.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
                });
        }

        /// <summary>
        /// Adds Swagger generated documentation to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddDocumentation(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var options = configuration.GetSection("App").Get<AppOptions>();

            return services
                .AddSwaggerGen(x =>
                {
                    x.SwaggerDoc(options.Version, new Info
                    {
                        Title = options.Name,
                        Version = options.Version,
                        Description = options.Description
                    });
                });
        }

        /// <summary>
        /// Adds Request and View localiztion to the <see cref="IMvcBuilder"/> and <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddLocalizations(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // TODO: LOCALIZATION: Implement ressource storage (file, text translations?).
            services
                .AddLocalization()
                .AddMvc()
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization();

            return services;
        }

        /// <summary>
        /// Adds <see cref="GzipCompressionProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddGzipCompression(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddResponseCompression(y => y.Providers.Add<GzipCompressionProvider>());

            return services;
        }
    }
}