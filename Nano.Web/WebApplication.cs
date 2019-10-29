using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App;
using Nano.App.Extensions;
using Nano.App.Interfaces;
using Nano.Config;
using Nano.Config.Extensions;
using Nano.Data.Extensions;
using Nano.Eventing.Extensions;
using Nano.Logging.Extensions;
using Nano.Security.Extensions;
using Nano.Web.Extensions;

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

        /// <inheritdoc />
        public override void Configure(IApplicationBuilder applicationBuilder, IHostEnvironment hostingEnvironment, IHostApplicationLifetime applicationLifetime)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            if (applicationLifetime == null)
                throw new ArgumentNullException(nameof(applicationLifetime));

            base.Configure(applicationBuilder, hostingEnvironment, applicationLifetime);

            applicationBuilder
                .UseExceptionHandling()
                .UseHttpCorsPolicy()
                .UseHttpXForwardedHeaders()
                .UseHttpXRobotsTagHeaders()
                .UseHttpXFrameOptionsPolicyHeader()
                .UseHttpXXssProtectionPolicyHeader()
                .UseHttpReferrerPolicyHeader()
                .UseHttpStrictTransportSecurityPolicyHeader()
                .UseHttpsRedirect()
                .UseStaticFiles()
                .UseCookiePolicy(new CookiePolicyOptions
                {
                    Secure = CookieSecurePolicy.SameAsRequest
                })
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseHttpSession()
                .UseHttpContextAccessor()
                .UseHttpRequestOptions()
                .UseHttpRequestIdentifier()
                .UseHttpLocalization()
                .UseHttpRequestTimeZone()
                .UseHttpResponseCaching()
                .UseHttpResponseCompression()
                .UseEndpoints(x =>
                {
                    x.MapControllers();
                    x.MapDefaultControllerRoute();
                    x.MapRazorPages();
                })
                .Use((context, next) =>
                {
                    if (context.Request.Path == "/signin-facebook")
                    {
                        context.Request.Scheme = "https";
                    }

                    return next();
                })
                .UseHttpDocumentataion()
                .UseHealthChecks();
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
            var applicationKey = Assembly.GetEntryAssembly()?.FullName ?? Assembly.GetExecutingAssembly().FullName;

            var webOptions = config
                .GetSection(WebOptions.SectionName)
                .Get<WebOptions>() ?? new WebOptions();

            return new WebHostBuilder()
                .UseEnvironment(environment)
                .UseConfiguration(config)
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

                                if (certificate.Path != null)
                                {
                                    if (File.Exists(certificate.Path))
                                        z.UseHttps(certificate.Path, certificate.Password);
                                }
                            });
                        });
                })
                .UseContentRoot(root)
                .ConfigureServices(x =>
                {
                    x.AddSingleton(x);
                    x.AddSingleton<IApplication, TApplication>();

                    x.AddApp(config);
                    x.AddConfig(config);
                    x.AddLogging(config);
                    x.AddData(config);
                    x.AddSecurity(config);
                    x.AddEventing(config);
                    x.AddWeb(config);
                })
                .UseStartup<TApplication>()
                .UseSetting(WebHostDefaults.ApplicationKey, applicationKey)
                .CaptureStartupErrors(true)
                .UseShutdownTimeout(shutdownTimeout);
        }
    }
}