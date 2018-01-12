using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.App.Extensions;
using Nano.App.Interfaces;
using Nano.Web.Middleware;
using Newtonsoft.Json;

namespace Nano.App
{
    /// <summary>
    /// Base Application (abstract).
    /// </summary>
    public abstract class BaseApplication : IApplication
    {
        /// <summary>
        /// Configuration.
        /// </summary>
        protected virtual IConfiguration Configuration { get; set; }

        /// <summary>
        /// Constructor. 
        /// Accepting an instance of <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        protected BaseApplication(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.Configuration = configuration;
        }

        /// <inheritdoc />
        public virtual void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var logger = services.BuildServiceProvider().GetRequiredService<ILogger>();
            var dependencies = services
                .Select(x => new
                {
                    Service = x.ServiceType.FullName,
                    Implementation = x.ImplementationType?.FullName,
                    LifeCycle = x.Lifetime.ToString()
                })
                .Distinct();

            logger.LogDebug(JsonConvert.SerializeObject(dependencies));
        }

        /// <inheritdoc />
        public virtual void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, IApplicationLifetime applicationLifetime)
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

            if (basePath.EndsWith("/"))
                basePath = basePath.Substring(0, basePath.Length - 1);

            var defaultCulture = new RequestCulture(appOptions.Cultures.Default);
            var supportedCultures = appOptions.Cultures.Supported.Select(x => new CultureInfo(x)).ToArray();

            applicationBuilder
                .UseSession()
                .UseStaticFiles()
                .UseForwardedHeaders()
                .UseResponseCompression()
                .UseMiddleware<HttpRequestIdentifierMiddleware>()
                .UseMiddleware<HttpLoggingContextMiddleware>()
                .UseMiddleware<HttpContentTypeMiddleware>()
                .UseMiddleware<HttpExceptionHandlingMiddleware>()
                .UseSwagger(x =>
                {
                    x.RouteTemplate = basePath + "/docs/{documentName}/swagger.json";
                })
                .UseSwaggerUI(x =>
                {
                    x.ShowRequestHeaders();
                    x.SwaggerEndpoint($"{basePath}/docs/{version}/swagger.json", $"Api {version}");
                })
                .UseRequestLocalization(new RequestLocalizationOptions
                {
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures,
                    DefaultRequestCulture = defaultCulture
                })
                .UseMvc(x =>
                {
                    x.MapRoute("default", basePath + "/{controller=Home}/{action=Index}/{id?}");
                })
                .UseExceptionHandler($"/{basePath}/Home/Error")
                .UseStatusCodePagesWithRedirects(basePath + "/Home/Error/{0}");
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/>, ready to <see cref="IWebHostBuilder.Build()"/> and <see cref="WebHostExtensions.Run(IWebHost)"/>.
        /// </summary>
        /// <typeparam name="TApplication">The type containing method for application start-up.</typeparam>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder ConfigureApp<TApplication>()
            where TApplication : class, IApplication
        {
            const string NAME = "appsettings";

            var path = Directory.GetCurrentDirectory();
            var shutdownTimeout = TimeSpan.FromSeconds(10);
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile($"{NAME}.json", false, true)
                .AddJsonFile($"{NAME}.{environment}.json", true)
                .AddEnvironmentVariables()
                .Build();

            var options = configuration.GetSection(AppOptions.SectionName).Get<AppOptions>() ?? new AppOptions();
            var urls = options.Hosting.Ports.Select(x => $"http://*:{x}").Distinct().ToArray();

            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(path)
                .UseEnvironment(environment)
                .UseConfiguration(configuration)
                .UseShutdownTimeout(shutdownTimeout)
                .UseUrls(urls)
                .CaptureStartupErrors(true)
                .ConfigureServices(x =>
                {
                    x.AddApp(configuration);
                    x.AddData(configuration);
                    x.AddConfig(configuration);
                    x.AddLogging(configuration);
                    x.AddEventing(configuration);

                    x.AddSingleton<IApplication, TApplication>();
                })
                .UseStartup<TApplication>();
        }
    }
}
