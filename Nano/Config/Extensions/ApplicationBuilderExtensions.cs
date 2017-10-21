using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nano.App;
using Nano.Data.Interfaces;
using Nano.Hosting;
using Nano.Hosting.Middleware;

namespace Nano.Config.Extensions
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

            builder
                .UseStaticFiles()
                .UseForwardedHeaders()
                .UseMvc(x =>
                {
                    x.MapRoute("default", "api/" + appOptions.Name + "/{controller=Home}/{action=Index}/{id?}");
                })
                .UseExceptionHandler("/api/" + appOptions.Name + "/Home/Error")
                .UseStatusCodePagesWithRedirects("/api/" + appOptions.Name + "/Home/Error/{0}");


            if (hostingOptions.EnableSession)
                builder.UseSession();

            if (hostingOptions.EnableDocumentation)
            {
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

            if (hostingOptions.EnableHttpContextExtension)
                builder.UseMiddleware<HttpContextLoggingMiddleware>();

            if (hostingOptions.EnableHttpRequestIdentifier)
                builder.UseMiddleware<HttpRequestIdentifierMiddleware>();

            builder
                .UseMiddleware<HttpRequestContentTypeMiddleware>();

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