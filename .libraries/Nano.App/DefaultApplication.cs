using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.Web.Middleware;

namespace Nano.App
{
    /// <inheritdoc />
    public class DefaultApplication : BaseApplication<IConfiguration>
    {
        /// <inheritdoc />
        public DefaultApplication(IConfiguration configuration)
            : base(configuration)
        {

        }

        /// <inheritdoc />
        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

        }

        /// <inheritdoc />
        public override void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, IApplicationLifetime applicationLifetime)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            if (applicationLifetime == null)
                throw new ArgumentNullException(nameof(applicationLifetime));

            var services = applicationBuilder.ApplicationServices;
            var dbContext = services.GetService<DbContext>();

            dbContext?.Database
                .EnsureCreatedAsync()
                .ContinueWith(x => dbContext.Database.MigrateAsync())
                .Wait();

            var appOptions = services.GetRequiredService<AppOptions>();
            var version = appOptions.Version;
            var basePath = $"{appOptions.Hosting.Path}/{appOptions.Name}";

            applicationBuilder
                .UseStaticFiles()
                .UseForwardedHeaders()
                .UseMvc(x =>
                {
                    x.MapRoute("default", basePath + "/{controller=Home}/{action=Index}/{id?}");
                })
                .UseExceptionHandler($"/{basePath}/Home/Error")
                .UseStatusCodePagesWithRedirects(basePath + "/Home/Error/{0}");

            if (appOptions.Switches.EnableSession)
                applicationBuilder.UseSession();

            if (appOptions.Switches.EnableDocumentation)
            {
                applicationBuilder
                    .UseSwagger(x =>
                    {
                        x.RouteTemplate = basePath + "/docs/{documentName}/swagger.json";
                    })
                    .UseSwaggerUI(x =>
                    {
                        x.ShowRequestHeaders();
                        x.SwaggerEndpoint($"{basePath}/docs/{version}/swagger.json", $"Api {version}");
                    });
            }

            if (appOptions.Switches.EnableGzipCompression)
                applicationBuilder.UseResponseCompression();

            if (appOptions.Switches.EnableHttpContextLocalization)
            {
                var defaultCulture = new RequestCulture(appOptions.Cultures.Default);
                var supportedCultures = appOptions.Cultures.Supported.Select(x => new CultureInfo(x)).ToArray();

                applicationBuilder
                    .UseRequestLocalization(new RequestLocalizationOptions
                    {
                        SupportedCultures = supportedCultures,
                        SupportedUICultures = supportedCultures,
                        DefaultRequestCulture = defaultCulture
                    });
            }

            if (appOptions.Switches.EnableHttpContextIdentifier)
                applicationBuilder.UseMiddleware<HttpContextIdentifierMiddleware>();

            applicationBuilder
                .UseMiddleware<HttpContextContentTypeMiddleware>()
                .UseMiddleware<HttpContextExceptionMiddleware>();

            if (appOptions.Switches.EnableHttpContextLogging)
                applicationBuilder.UseMiddleware<HttpContextLoggingMiddleware>();
        }
    }
}