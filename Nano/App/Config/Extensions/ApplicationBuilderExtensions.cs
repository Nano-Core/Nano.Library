using System;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Interfaces;
using Nano.Hosting;
using Nano.Hosting.Middleware.Interfaces;

namespace Nano.App.Config.Extensions
{
    /// <summary>
    /// Application Builder Extensions.
    /// </summary>
    internal static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Registers all hosting middleware components.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseHosting(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var services = builder.ApplicationServices;
            var hostingOptions = services.GetRequiredService<HostingOptions>();

            if (hostingOptions.EnableSession)
                builder.UseSession();

            if (hostingOptions.EnableDocumentation)
            {
                var appOptions = services.GetRequiredService<AppOptions>();

                builder
                    .UseSwagger(x =>
                    {
                        x.RouteTemplate = "api-docs/" + appOptions.Name + "/{documentName}/swagger.json";
                    })
                    .UseSwaggerUI(x =>
                    {
                        x.ShowRequestHeaders();
                        x.SwaggerEndpoint($"/api-docs/{appOptions.Name}/{appOptions.Version}/swagger.json", $"Api {appOptions.Version}");
                    });
            }

            if (hostingOptions.EnableGzipCompression)
                builder.UseResponseCompression();

            if (hostingOptions.EnableHttpRequestLocalization)
            {
                var defaultCulture = new RequestCulture("en-US");
                var supportedCultures = new[] { new CultureInfo("en-US") };

                builder
                    .UseRequestLocalization(new RequestLocalizationOptions
                    {
                        SupportedCultures = supportedCultures,
                        SupportedUICultures = supportedCultures,
                        DefaultRequestCulture = defaultCulture
                    });
            }

            if (hostingOptions.EnableHttpContextExtension)
                builder.UseMiddleware<IHttpContextExtensionMiddleware>();

            if (hostingOptions.EnableHttpRequestIdentifier)
                builder.UseMiddleware<IHttpRequestIdentifierMiddleware>();

            builder
                .UseMiddleware<IHttpRequestContentTypeMiddleware>();

            return builder;
        }

        /// <summary>
        /// Registers data context, and build database.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
        internal static IApplicationBuilder UseDataContext(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var services = builder.ApplicationServices;
            var dbContext = services.GetRequiredService<IDbContext>();

            dbContext.Database
                .EnsureCreatedAsync()
                .ContinueWith(x => dbContext.Database.MigrateAsync())
                .Wait();

            return builder;
        }
    }
}