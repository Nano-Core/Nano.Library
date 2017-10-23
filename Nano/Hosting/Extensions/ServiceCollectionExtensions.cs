using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App;
using Nano.App.Controllers.Criteria.Binders.Providers;
using Nano.Config.Extensions;
using Nano.Hosting.Constants;
using Nano.Hosting.Middleware;
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
                .AddConfigOptions<HostingOptions>(configuration, HostingOptions.SectionName, out var options)
                .AddMvc(x =>
                {
                    x.ModelBinderProviders.Insert(0, new QueryModelBinderProvider());
                })
                .AddControllersAsServices()
                .AddViewComponentsAsServices()
                .AddApplicationPart(Assembly.GetExecutingAssembly());

            if (options.EnableSession)
                services.AddSession();

            if (options.EnableDocumentation)
                services.AddDocumentation(configuration);

            if (options.EnableGzipCompression)
                services.AddGzipCompression();

            if (options.EnableHttpContextExtension)
                services.AddHttpContextExtensions();

            if (options.EnableHttpRequestIdentifier)
                services.AddHttpRequestIdentifier();

            if (options.EnableHttpRequestLocalization)
                services.AddHttpRequestLocalization();

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
                    x.SwaggerDoc(options.Version.ToString(), new Info
                    {
                        Title = options.Name,
                        Version = options.Version.ToString(),
                        Description = options.Description
                    });
                });
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

        /// <summary>
        /// Adds <see cref="HttpContextLoggingMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpContextExtensions(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<HttpContextLoggingMiddleware>();
        }

        /// <summary>
        /// Adds Request and View localiztion to the <see cref="IMvcBuilder"/> and <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpRequestLocalization(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddLocalization()
                .AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            return services;
        }

        /// <summary>
        /// Adds <see cref="HttpRequestIdentifierMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpRequestIdentifier(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<HttpRequestIdentifierMiddleware>();
        }

        /// <summary>
        /// Adds <see cref="HttpRequestContentTypeMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpRequestContentTypes(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<HttpRequestContentTypeMiddleware>()
                .AddMvc(x =>
                {
                    x.ReturnHttpNotAcceptable = true;
                    x.RespectBrowserAcceptHeader = true;
                    x.FormatterMappings.SetMediaTypeMappingForFormat("xml", HttpContentType.Xml);
                    x.FormatterMappings.SetMediaTypeMappingForFormat("json", HttpContentType.Json);
                    x.FormatterMappings.SetMediaTypeMappingForFormat("json", HttpContentType.JavaScript);
                    x.FormatterMappings.SetMediaTypeMappingForFormat("html", HttpContentType.Html);
                })
                .AddXmlSerializerFormatters()
                .AddXmlDataContractSerializerFormatters();

            return services;
        }
    }
}