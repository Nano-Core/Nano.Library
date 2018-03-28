using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Extensions;
using Nano.App.Extensions.Middleware;
using Nano.App.Interfaces;
using Nano.Config;
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
        protected virtual IConfiguration Configuration { get; }

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
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .BuildServiceProvider();
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
   
            if (appOptions.Hosting.UseSsl)
            {
                applicationBuilder
                    .UseRewriter(new RewriteOptions().AddRedirectToHttps());
            }

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
                    x.EnableFilter();
                    x.EnableDeepLinking();
                    x.EnableValidator(null);
                    x.ShowExtensions();
                    x.DisplayOperationId();
                    x.DisplayRequestDuration();
                    x.MaxDisplayedTags(-1);
                    x.DefaultModelExpandDepth(2);
                    x.DefaultModelsExpandDepth(1);
                    x.DefaultModelRendering(ModelRendering.Example);
                    x.DocExpansion(DocExpansion.None);

                    x.RoutePrefix = "docs";
                    x.DocumentTitle = $"Nano - {appOptions.Name} Docs v{appOptions.Version} ({ConfigManager.Environment})";
                    x.SwaggerEndpoint($"/docs/{appOptions.Version}/swagger.json", $"Nano - {appOptions.Name} v{appOptions.Version} ({ConfigManager.Environment})");
                })
                .UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(appOptions.Cultures.Default),
                    SupportedCultures = appOptions.Cultures.Supported.Select(x => new CultureInfo(x)).ToArray(),
                    SupportedUICultures = appOptions.Cultures.Supported.Select(x => new CultureInfo(x)).ToArray()
                })
               .UseExceptionHandler("/Home/Error");

            var dbContext = applicationBuilder.ApplicationServices.GetService<BaseDbContext>();

            dbContext?
                .EnsureCreatedAsync()
                .ContinueWith(async x =>
                {
                    await dbContext.EnsureMigratedAsync();
                    await dbContext.EnsureImportAsync();
                })
                .Wait();
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
            var root = Directory.GetCurrentDirectory();
            var config = ConfigManager.BuildConfiguration(args);
            var shutdownTimeout = TimeSpan.FromSeconds(10);
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var options = config.GetSection(AppOptions.SectionName).Get<AppOptions>() ?? new AppOptions();
            var urls = options.Hosting.Ports.Select(x => $"http://*:{x}").Distinct().ToArray();

            return new WebHostBuilder()
                .CaptureStartupErrors(true)
                .UseKestrel()
                .UseUrls(urls)
                .UseContentRoot(root)
                .UseEnvironment(environment)
                .UseConfiguration(config)
                .UseShutdownTimeout(shutdownTimeout)
                .ConfigureServices(x =>
                {
                    x.AddSingleton<IApplication, TApplication>();

                    x.AddApp(config);
                    x.AddData(config);
                    x.AddConfig(config);
                    x.AddLogging(config);
                    x.AddEventing(config);
                })
                .UseStartup<TApplication>()
                .UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly().FullName);
        }
    }
}
