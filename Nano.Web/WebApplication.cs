using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App;
using Nano.App.Extensions;
using Nano.App.Interfaces;
using Nano.Config;
using Nano.Config.Extensions;
using Nano.Data.Extensions;
using Nano.Eventing.Extensions;
using Nano.Logging.Extensions;
using Nano.Security.Extensions;
using Nano.Web.Hosting.Extensions;
using Nano.Web.Hosting.Middleware;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Nano.Web
{
    /// <inheritdoc />
    public class WebApplication : DefaultApplication
    {
        /// <inheritdoc />
        public WebApplication(IConfiguration configuration)
            : base(configuration)
        {

        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="hostingEnvironment">The <see cref="IHostingEnvironment"/>.</param>
        /// <param name="applicationLifetime">The <see cref="IApplicationLifetime"/>.</param>
        public override void Configure(IApplicationBuilder applicationBuilder, IHostingEnvironment hostingEnvironment, IApplicationLifetime applicationLifetime)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            if (applicationLifetime == null)
                throw new ArgumentNullException(nameof(applicationLifetime));

            base.Configure(applicationBuilder, hostingEnvironment, applicationLifetime);

            applicationBuilder.UseHttpsRedirection();

            this.ConfigureHosting(applicationBuilder);
            this.ConfigureLocalization(applicationBuilder);
            this.ConfigureDocumentation(applicationBuilder);
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/>, ready to <see cref="IWebHostBuilder.Build()"/> and <see cref="WebHostExtensions.Run(IWebHost)"/>.
        /// The application startup implementation is defaulted to <see cref="WebApplication"/>.
        /// </summary>
        /// <param name="args">The command-line args, if any.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder ConfigureApp(params string[] args)
        {
            return WebApplication
                .ConfigureApp<WebApplication>(args);
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
            var applicationKey = Assembly.GetEntryAssembly().FullName;

            var webOptions = config.GetSection(WebOptions.SectionName).Get<WebOptions>() ?? new WebOptions();

            return new WebHostBuilder()
                .UseKestrel(x =>
                {
                    webOptions.Hosting.Ports
                        .ToList()
                        .ForEach(y =>
                        {
                            x.ListenAnyIP(y, z =>
                            {
                                z.Protocols = HttpProtocols.Http1AndHttp2;
                            });
                        });

                    var certificate = webOptions.Hosting.Certificate;

                    webOptions.Hosting.PortsHttps
                        .ToList()
                        .ForEach(y =>
                        {
                            x.ListenAnyIP(y, z =>
                            {
                                z.Protocols = HttpProtocols.Http1AndHttp2;
                                z.UseHttps(certificate.Path, certificate.Password);
                            });
                        });
                })
                .UseContentRoot(root)
                .UseConfiguration(config)
                .UseEnvironment(environment)
                .ConfigureServices(x =>
                {
                    x.AddSingleton<IApplication, TApplication>();

                    x.AddApp(config);
                    x.AddData(config);
                    x.AddConfig(config);
                    x.AddLogging(config);
                    x.AddSecurity(config);
                    x.AddEventing(config);
                    x.AddWeb(config);
                })
                .UseStartup<TApplication>()
                .UseSetting(WebHostDefaults.ApplicationKey, applicationKey)
                .CaptureStartupErrors(true)
                .UseShutdownTimeout(shutdownTimeout);
        }

        private void ConfigureHosting(IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null) 
                throw new ArgumentNullException(nameof(applicationBuilder));

            applicationBuilder
                .UseSession()
                .UseStaticFiles()
                .UseHttpsRewrite()
                .UseCookiePolicy()
                .UseAuthentication()
                .UseForwardedHeaders()
                .UseResponseCompression()
                .UseHttpsRedirection()
                .UseHttpsRedirectionValidation()
                .UseMiddleware<ExceptionHandlingMiddleware>()
                .UseMiddleware<HttpRequestUserMiddleware>()
                .UseMiddleware<HttpRequestIdentifierMiddleware>()
                .UseMiddleware<HttpRequestOptionsMiddleware>()
                .UseNoCache()
                .UseRobotsTag()
                .UseDownloadOptions()
                .UseContentTypeOptions()
                .UseStrictTransportSecurity()
                .UseReferrerPolicies()
                .UseXFrameOptionsPolicies()
                .UseXXssProtectionPolicies()
                .UseCors(x =>
                {
                    x.AllowAnyOrigin();
                    x.AllowAnyHeader();
                    x.AllowAnyMethod();
                    x.AllowCredentials();
                })
                .UseMvc(x =>
                {
                    x.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                });
        }
        private void ConfigureLocalization(IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var appOptions = services.GetService<AppOptions>() ?? new AppOptions();

            applicationBuilder
                .UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(appOptions.Cultures.Default),
                    SupportedCultures = appOptions.Cultures.Supported.Select(x => new CultureInfo(x)).ToArray(),
                    SupportedUICultures = appOptions.Cultures.Supported.Select(x => new CultureInfo(x)).ToArray()
                });
        }
        private void ConfigureDocumentation(IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var services = applicationBuilder.ApplicationServices;
            var appOptions = services.GetService<AppOptions>() ?? new AppOptions();

            applicationBuilder
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
                });
        }
    }
}