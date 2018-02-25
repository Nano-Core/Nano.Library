using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Extensions;
using Nano.App.Extensions.Middleware;
using Nano.App.Interfaces;
using Nano.Data;

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
        public virtual void Configure(IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var hostingEnvironment = applicationBuilder.ApplicationServices.GetService<IHostingEnvironment>();
            var applicationLifetime = applicationBuilder.ApplicationServices.GetService<IApplicationLifetime>();

            this.Configure(applicationBuilder, hostingEnvironment, applicationLifetime);
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

            var appOptions = applicationBuilder.ApplicationServices.GetService<AppOptions>() ?? new AppOptions();

            applicationBuilder
                .UseSession()
                .UseStaticFiles()
                .UseForwardedHeaders()
                .UseResponseCompression()
                .UseMiddleware<HttpContentTypeMiddleware>()
                .UseMiddleware<HttpRequestIdentifierMiddleware>()
                .UseMiddleware<HttpExceptionHandlingMiddleware>()
                .UseCors(x =>
                {
                    x.AllowAnyOrigin();
                    x.AllowAnyHeader();
                    x.AllowAnyMethod();
                })
                .UseMvc(x =>
                {
                    x.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                })
                .UseSwagger(x =>
                {
                    x.RouteTemplate = "docs/{documentName}/swagger.json";
                })
                .UseSwaggerUI(x =>
                {
                    x.ShowRequestHeaders();
                    x.DocumentTitle($"{appOptions.Name} Docs v{appOptions.Version}");

                    x.RoutePrefix = "docs";
                    x.SwaggerEndpoint($"/docs/{appOptions.Version}/swagger.json", $"{appOptions.Name} v{appOptions.Version}");
                })
                .UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(appOptions.Cultures.Default),
                    SupportedCultures = appOptions.Cultures.Supported.Select(x => new CultureInfo(x)).ToArray(),
                    SupportedUICultures = appOptions.Cultures.Supported.Select(x => new CultureInfo(x)).ToArray()
                })
               .UseExceptionHandler("/Home/Error");

            var baseDbContext = applicationBuilder.ApplicationServices.GetService<BaseDbContext>();

            baseDbContext?
                .CreateDatabaseAsync()
                .ContinueWith(async x =>
                {
                    var success = await x;
                    if (success)
                    {
                        await baseDbContext.MigrateDatabaseAsync();
                    }

                    return success;
                })
                .ContinueWith(async x =>
                {
                    var success = await x.Result;
                    if (success)
                    {
                        await baseDbContext.ImportDatabaseAsync();
                    }

                    return success;
                })
                .Wait();
        }

        /// <inheritdoc />
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .BuildServiceProvider();
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/>, ready to <see cref="IWebHostBuilder.Build()"/> and <see cref="WebHostExtensions.Run(IWebHost)"/>.
        /// The application startup implementation is defaulted to <see cref="DefaultApplication"/>.
        /// </summary>
        /// <param name="args">The command-line args, if any.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder ConfigureApp(params string[] args)
        {
            return BaseApplication
                .ConfigureApp<DefaultApplication>();
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/>, ready to <see cref="IWebHostBuilder.Build()"/> and <see cref="WebHostExtensions.Run(IWebHost)"/>.
        /// The application startup implementation is defined by the generic type parameter <typeparamref name="TApplication"/>.
        /// </summary>
        /// <typeparam name="TApplication">The type containing method for application start-up.</typeparam>
        /// <param name="args">The command-line args, if any.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder ConfigureApp<TApplication>(params string[] args)
            where TApplication : class, IApplication
        {
            const string NAME = "appsettings";

            var path = Directory.GetCurrentDirectory();
            var shutdownTimeout = TimeSpan.FromSeconds(10);
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile($"{NAME}.json", false, true)
                .AddJsonFile($"{NAME}.{environment}.json", true)
                .AddCommandLine(args)
                .AddEnvironmentVariables()
                .Build();

            var options = configuration.GetSection(AppOptions.SectionName).Get<AppOptions>() ?? new AppOptions();
            var urls = options.Hosting.Ports.Select(x => $"http://*:{x}").Distinct().ToArray();

            return new WebHostBuilder()
                .CaptureStartupErrors(true)
                .UseKestrel()
                .UseUrls(urls)
                .UseContentRoot(path)
                .UseEnvironment(environment)
                .UseConfiguration(configuration)
                .UseShutdownTimeout(shutdownTimeout)
                .ConfigureServices(x =>
                {
                    x.AddSingleton<IApplication, TApplication>();

                    x.AddApp(configuration);
                    x.AddData(configuration);
                    x.AddConfig(configuration);
                    x.AddLogging(configuration);
                    x.AddEventing(configuration);
                })
                .UseStartup<TApplication>()
                .UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly().FullName);
        }
    }
}
