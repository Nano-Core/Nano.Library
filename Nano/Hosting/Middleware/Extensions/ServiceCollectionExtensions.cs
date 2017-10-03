using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Hosting.Middleware.Interfaces;

namespace Nano.Hosting.Middleware.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="IHttpContextExtensionMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpContextExtensions(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<IHttpContextExtensionMiddleware, HttpContextExtensionMiddleware>();
        }

        /// <summary>
        /// Adds <see cref="IHttpRequestIdentifierMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpRequestIdentifier(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<IHttpRequestIdentifierMiddleware, HttpRequestIdentifierMiddleware>();
        }

        /// <summary>
        /// Adds <see cref="IHttpRequestContentTypeMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpRequestContentTypes(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<IHttpRequestContentTypeMiddleware, HttpRequestContentTypeMiddleware>()
                .AddMvc(x =>
                {
                    x.ReturnHttpNotAcceptable = true;
                    x.RespectBrowserAcceptHeader = true;
                    x.FormatterMappings.SetMediaTypeMappingForFormat("html", "text/html");
                    x.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/json");
                    x.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/javascript");
                    x.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
                })
                .AddXmlSerializerFormatters()
                .AddXmlDataContractSerializerFormatters();

            return services;
        }
    }
}