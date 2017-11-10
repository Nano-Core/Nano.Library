using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Interfaces;
using Nano.Hosting;
using Nano.Hosting.Middleware;

namespace Nano.App.Extensions
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
            var appOptions = services.GetRequiredService<AppOptions>();
            var hostingOptions = services.GetRequiredService<HostingOptions>();
            var basePath = $"{hostingOptions.Path}/{appOptions.Name}";

            builder
                .UseStaticFiles()
                .UseForwardedHeaders()
                .UseMvc(x =>
                {
                    x.MapRoute("default", basePath + "/{controller=Home}/{action=Index}/{id?}");
                })
                .UseExceptionHandler($"/{basePath}/Home/Error")
                .UseStatusCodePagesWithRedirects(basePath + "/Home/Error/{0}");
            
            if (hostingOptions.EnableSession)
                builder.UseSession();

            if (hostingOptions.EnableDocumentation)
            {
                builder
                    .UseSwagger(x =>
                    {
                        x.RouteTemplate = basePath + "/docs/{documentName}/swagger.json";
                    })
                    .UseSwaggerUI(x =>
                    {
                        x.ShowRequestHeaders();
                        x.SwaggerEndpoint($"{basePath}/docs/{appOptions.Version}/swagger.json", $"Api {appOptions.Version}");
                    });
            }

            if (hostingOptions.EnableGzipCompression)
                builder.UseResponseCompression();

            if (hostingOptions.EnableHttpContextLocalization)
            {
                var defaultCulture = new RequestCulture(appOptions.Cultures.Default);
                var supportedCultures = appOptions.Cultures.Supported.Select(x => new CultureInfo(x)).ToArray();

                builder
                    .UseRequestLocalization(new RequestLocalizationOptions
                    {
                        SupportedCultures = supportedCultures,
                        SupportedUICultures = supportedCultures,
                        DefaultRequestCulture = defaultCulture
                    });
            }

            if (hostingOptions.EnableHttpContextIdentifier)
                builder.UseMiddleware<HttpContextIdentifierMiddleware>();

            builder
                .UseMiddleware<HttpContextContentTypeMiddleware>()
                .UseMiddleware<HttpContextExceptionMiddleware>();

            if (hostingOptions.EnableHttpContextLogging)
                builder.UseMiddleware<HttpContextLoggingMiddleware>();

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
            var dbContext = services.GetService<IDbContext>();

            dbContext?.Database
                .EnsureCreatedAsync()
                .ContinueWith(x => dbContext.Database.MigrateAsync())
                .Wait();

            return builder;
        }
    }
}