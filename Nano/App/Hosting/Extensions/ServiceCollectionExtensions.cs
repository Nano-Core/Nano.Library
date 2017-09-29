using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.PlatformAbstractions;
using Nano.App.Config.Options;
using Nano.App.Hosting.Middleware.Extensions;
using Nano.App.Hosting.Options;
using Nano.Controllers;
using Swashbuckle.AspNetCore.Swagger;

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

            var rootOptions = configuration.Get<RootOptions>() ?? new RootOptions();

            var section = configuration.GetSection("Hosting");
            var options = section?.Get<HostingOptions>() ?? new HostingOptions();
            var assemblyPart = Assembly.GetAssembly(typeof(BaseController<,,>));

            if (options.EnableDocumentation)
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(rootOptions.AppVersion, new Info
                    {
                        Title = $"Api - {rootOptions.AppName}",
                        Version = rootOptions.AppVersion,
                        Description = "Api documentation."
                    });

                    var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Nano.xml");
                    c.IncludeXmlComments(filePath);
                });

            services
                .AddSingleton(options)
                .Configure<HostingOptions>(section)
                .Configure<RazorViewEngineOptions>(x =>
                {
                    x.FileProviders.Add(new EmbeddedFileProvider(assemblyPart));
                })
                .AddLocalization() // COSMETIC: Implement ressource storage (file, text translations?).
                .AddMvc(x =>
                {
                    x.ReturnHttpNotAcceptable = true;
                    x.RespectBrowserAcceptHeader = true;
                    x.FormatterMappings.SetMediaTypeMappingForFormat("html", "text/html");
                    x.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/json");
                    x.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/javascript");
                    x.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization()
                .AddControllersAsServices()
                .AddViewComponentsAsServices()
                .AddXmlSerializerFormatters()
                .AddXmlDataContractSerializerFormatters()
                .AddApplicationPart(assemblyPart);

            services.AddApiVersioning(x =>
            {
                x.ReportApiVersions = true;
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.DefaultApiVersion = new ApiVersion(1, 0);
                x.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });
             
            if (options.EnableSession)
                services.AddSession();
            
            if (options.EnableGzipCompression)
                services.AddGzipCompression();

            if (options.EnableRequestLocalization)
                services.AddLocalization();

            if (options.EnableRequestIdentifier)
                services.AddHttpRequestIdentifier();

            services
                .AddHttpRequestContentType();

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