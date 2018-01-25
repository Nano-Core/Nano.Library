using System;
using System.Globalization;
using System.IO;
using System.Linq;
using EasyNetQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Extensions;
using Nano.App.Extensions.Middleware;
using Nano.App.Interfaces;
using Nano.Data;
using Nano.Data.Attributes;

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

            var options = applicationBuilder.ApplicationServices.GetRequiredService<AppOptions>();
            var dbContext = applicationBuilder.ApplicationServices.GetRequiredService<BaseDbContext>();
            
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
                    x.DocumentTitle($"{options.Name} Docs v{options.Version}");

                    x.RoutePrefix = "docs";
                    x.SwaggerEndpoint($"/docs/{options.Version}/swagger.json", $"{options.Name} v{options.Version}");
                })
                .UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(options.Cultures.Default),
                    SupportedCultures = options.Cultures.Supported.Select(x => new CultureInfo(x)).ToArray(),
                    SupportedUICultures = options.Cultures.Supported.Select(x => new CultureInfo(x)).ToArray()
                })
               .UseExceptionHandler("/Home/Error");

            dbContext.Database
                .EnsureCreatedAsync()
                .ContinueWith(x => dbContext.Database.MigrateAsync())
                .ContinueWith(x =>
                {
                    AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(y => y.GetTypes())
                        .Where(y => y.GetAttributes<DataImportAttribute>().Any())
                        .ToList()
                        .ForEach(async y =>
                        {
                            var attribute = y.GetAttribute<DataImportAttribute>();

                            await dbContext
                                .AddRangeAsync(attribute.Uri, y);
                        });
                })
                .Wait();
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/>, ready to <see cref="IWebHostBuilder.Build()"/> and <see cref="WebHostExtensions.Run(IWebHost)"/>.
        /// </summary>
        /// <typeparam name="TApplication">The type containing method for application start-up.</typeparam>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder ConfigureApp<TApplication>(params string[] args)
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
