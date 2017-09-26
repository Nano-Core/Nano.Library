using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nano.App.Config.Extensions;
using Nano.App.Config.Options;
using Nano.App.Data.Extensions;
using Nano.App.Eventing.Extensions;
using Nano.App.Hosting.Extensions;
using Nano.App.Logging.Extensions;
using Nano.App.Logging.Middleware.Interfaces;
using Nano.App.Security.Extensions;

namespace Nano.App
{
    // TODO
    //Change tracking / onChanged events? for RabbitMQ and other triggers for calculating stats
    //Versioning / api-explorer (https://dotnetcoretutorials.com/2017/01/17/api-versioning-asp-net-core/)
    //RabbitMQ setup
    // .UseUrls("http://localhost") TODO: investigate if usable
    // application parts for views, somehow make standard views for Index, Create, Edit, Delete traversing all properties in html. Without using css and (js)



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
        public virtual void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, IApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
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
            var options = services.GetRequiredService<RootOptions>();
            var dbContext = services.GetRequiredService<DbContext>();

            if (options.Hosting.EnableSession)
                applicationBuilder.UseSession();

            if (options.Hosting.EnableGzipCompression)
                applicationBuilder.UseResponseCompression();

            if (options.Hosting.EnableRequestLocalization)
                applicationBuilder.UseRequestLocalization();

            if (options.Logging.IncludeHttpContext)
                applicationBuilder.UseMiddleware<IHttpContextLoggingMiddleware>();

            applicationBuilder
                .UseStaticFiles()
                .UseAuthentication()
                .UseForwardedHeaders()
                .UseMvc(x =>
                {
                    x.MapRoute("default", "api/" + options.AppName + "/{controller=Home}/{action=Index}/{id?}");
                });

            if (hostingEnvironment.IsDevelopment())
            {
                applicationBuilder
                    .UseBrowserLink()
                    .UseDatabaseErrorPage()
                    .UseDeveloperExceptionPage();

                dbContext.Database
                    .EnsureDeleted();
            }
            else
            {
                applicationBuilder
                    .UseExceptionHandler("/Home/Error")
                    .UseStatusCodePagesWithRedirects("/Home/Error/{0}");
            }

            dbContext.Database
                .EnsureCreatedAsync()
                .ContinueWith(x => dbContext.AddIsolationLevel(options.Data.IsolationLevel))
                .ContinueWith(x => dbContext.Database.MigrateAsync())
                .Wait();
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/>, ready to <see cref="IWebHostBuilder.Build()"/> and <see cref="WebHostExtensions.Run(IWebHost)"/>.
        /// </summary>
        /// <typeparam name="TApplication">The type containing method for application start-up.</typeparam>
        /// <param name="captureErrors">Whether startup errors should be captured in the configuration settings of the web host. When enabled, startup exceptions will be caught and an error page will be returned. If disabled, startup exceptions will be propagated.</param>
        /// <returns></returns>
        public static IWebHostBuilder GetWebHostBuilder<TApplication>(bool captureErrors = false)
            where TApplication : class
        {
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
                    x.AddSecurity(configuration);
                    x.AddEventing(configuration);
                    x.AddDataContext(configuration);
                })
                .UseStartup<TApplication>();
        }
    }
}