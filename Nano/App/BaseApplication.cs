using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.App.Config.Extensions;
using Nano.App.Config.Options;
using Nano.App.Data.Extensions;
using Nano.App.Eventing.Extensions;
using Nano.App.Hosting.Extensions;
using Nano.App.Hosting.Middleware.Interfaces;
using Nano.App.Logging.Extensions;

namespace Nano.App
{
    // TODO: RabbitMQ setup (Entity Change tracking events).

    // COSMETIC: Add ServiceOptions (IOtions<T>).
    // COSMETIC: Generic / Inherited Views (interating properites to display fields (editable readonly).
    // COSMETIC: HTTP Patch method, for parsing partial models.
    // COSMETIC: OWIN (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/owin)

    // BUG: OrderBy possiblity in queries (and also default to KEY somehow, EF warns)
    // BUG: Issue with Swagger not able to generate documentation from generic operations (path/action needs to be unique, and it can't see that in generic base class controller it seems)

    /// <summary>
    /// Base Application (abstract).
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IConfiguration"/>.</typeparam>
    public abstract class BaseApplication<T>
        where T : IConfiguration
    {
        /// <summary>
        /// Configuration.
        /// The <see cref="IConfiguration"/> instance of type <typeparamref name="T"/>.
        /// </summary>
        public virtual T Configuration { get; protected set; }

        /// <summary>
        /// Constructor. 
        /// Accepting a <typeparamref name="T"/> instance, initializing <see cref="Configuration"/>.
        /// </summary>
        /// <param name="configuration">The instance of <typeparamref name="T"/>.</param>
        protected BaseApplication(T configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.Configuration = configuration;
        }

        /// <summary>
        /// Configure Services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="hostingEnvironment">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="applicationLifetime">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public virtual void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment,
            IApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            if (applicationLifetime == null)
                throw new ArgumentNullException(nameof(applicationLifetime));

            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            var services = applicationBuilder.ApplicationServices;
            var dbContext = services.GetRequiredService<DbContext>();
            var rootOptions = services.GetRequiredService<RootOptions>();

            if (rootOptions.Hosting.EnableSession)
                applicationBuilder.UseSession();

            if (rootOptions.Hosting.EnableGzipCompression)
                applicationBuilder.UseResponseCompression();

            if (rootOptions.Hosting.EnableRequestLocalization)
            {
                var supportedCultures = new[] {new CultureInfo("en-US")};

                applicationBuilder
                    .UseRequestLocalization(new RequestLocalizationOptions
                    {
                        SupportedCultures = supportedCultures,
                        SupportedUICultures = supportedCultures,
                        DefaultRequestCulture = new RequestCulture("en-US")
                    });
            }

            if (rootOptions.Logging.IncludeHttpContext)
                applicationBuilder.UseMiddleware<IHttpContextMiddleware>();

            if (rootOptions.Hosting.EnableRequestIdentifier)
                applicationBuilder.UseMiddleware<IHttpRequestIdentifierMiddleware>();

            if (rootOptions.Logging.IncludeHttpContext)
                applicationBuilder.UseMiddleware<IHttpRequestContentTypeMiddleware>();

            if (rootOptions.Hosting.EnableDocumentation)
                applicationBuilder
                    .UseSwagger(x =>
                        x.RouteTemplate = "api-docs/" + rootOptions.AppName.ToLower() + "/{documentName}/swagger.json")
                    .UseSwaggerUI(x =>
                    {
                        x.ShowRequestHeaders();
                        x.SwaggerEndpoint(
                            $"/api-docs/{rootOptions.AppName.ToLower()}/{rootOptions.AppVersion}/swagger.json",
                            $"Api {rootOptions.AppVersion}");
                    });

            applicationBuilder
                .UseStaticFiles()
                // TODO: SECURITY: .UseAuthentication()
                .UseForwardedHeaders()
                .UseMvc(x =>
                {
                    x.MapRoute("default", "api/" + rootOptions.AppName + "/{controller=Home}/{action=Index}/{id?}");
                });


            applicationBuilder
                .UseExceptionHandler("/api/" + rootOptions.AppName + "/Home/Error")
                .UseStatusCodePagesWithRedirects("/api/" + rootOptions.AppName + "/Home/Error/{0}");

            if (hostingEnvironment.IsDevelopment())
            {
                Thread.Sleep(5000); // BUG: Fix docker delay for MySQL

                dbContext.Database
                    .EnsureDeleted();
            }

            dbContext.Database
                .EnsureCreatedAsync()
                .ContinueWith(x => dbContext.AddIsolationLevel(rootOptions.Data.IsolationLevel))
                .ContinueWith(x => dbContext.Database.MigrateAsync())
                .Wait();
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/>, ready to <see cref="IWebHostBuilder.Build()"/> and <see cref="WebHostExtensions.Run(IWebHost)"/>.
        /// </summary>
        /// <typeparam name="TApplication">The type containing method for application start-up.</typeparam>
        /// <param name="captureErrors">Whether startup errors should be captured in the configuration settings of the web host. When enabled, startup exceptions will be caught and an error page will be returned. If disabled, startup exceptions will be propagated.</param>
        /// <returns></returns>
        public static IWebHostBuilder ConfigureApp<TApplication>(bool captureErrors = false)
            where TApplication : class
        {
            const string URL = "http://*:80";
            const string NAME = "appsettings";

            var path = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddEnvironmentVariables()
                .AddJsonFile($"{NAME}.json", false, true)
                .AddJsonFile($"{NAME}.{environment}.json", true)
                .Build();

            return new WebHostBuilder()
                .UseKestrel()
                .UseUrls(URL)
                .UseContentRoot(path)
                .UseEnvironment(environment)
                .UseConfiguration(configuration)
                .UseShutdownTimeout(TimeSpan.FromSeconds(10))
                .CaptureStartupErrors(captureErrors)
                .ConfigureServices(x =>
                {
                    x.AddConfig(configuration);
                    x.AddHosting(configuration);
                    x.AddLogging(configuration);
                    // TODO: SECURITY: x.AddSecurity(configuration);
                    x.AddEventing(configuration);
                    x.AddDataContext(configuration);
                })
                .UseStartup<TApplication>();
        }
    }
}