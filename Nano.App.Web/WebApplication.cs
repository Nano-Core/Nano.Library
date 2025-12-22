using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Abstractions;
using Nano.App.Extensions;
using Nano.App.Web.Config;
using Nano.App.Web.Extensions;
using Nano.Common.Config.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
using Nano.App.Config;

namespace Nano.App.Web;

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

        var services = applicationBuilder.ApplicationServices;

        var webOptions = services
            .GetRequiredService<IOptionsMonitor<WebOptions>>();

        applicationBuilder
            .UseExceptionHandling()
            .UseHttpCorsPolicy(webOptions.CurrentValue)
            .UseHttpXForwardedHeaders(webOptions.CurrentValue)
            .UseHttpXRobotsTagHeaders(webOptions.CurrentValue)
            .UseHttpXFrameOptionsPolicyHeader(webOptions.CurrentValue)
            .UseHttpXXssProtectionPolicyHeader(webOptions.CurrentValue)
            .UseXHttpContentTypeOptionsPolicyHeader(webOptions.CurrentValue)
            .UseHttpReferrerPolicyHeader(webOptions.CurrentValue)
            .UseHttpStrictTransportSecurityPolicyHeader(webOptions.CurrentValue)
            .UseHttpContentSecurityPolicyHeader(webOptions.CurrentValue)
            .UseStaticFiles()
            .UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.SameAsRequest
            })
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseHttpSession(webOptions.CurrentValue)
            .UseHttpRequestOptions()
            .UseHttpRequestIdentifier()
            .UseHttpRequestLocalization(webOptions.CurrentValue)
            .UseHttpRequestTimeZone()
            .UseHttpResponseCaching(webOptions.CurrentValue)
            .UseHttpResponseCompression(webOptions.CurrentValue)
            .UseEndpoints(x =>
            {
                x.MapControllers();
                x.MapDefaultControllerRoute();
                x.MapRazorPages();
            })
            .UseHttpRequestVirusScan(webOptions.CurrentValue)
            .Use((context, next) =>
            {
                if (context.Request.Path == "/signin-facebook")
                {
                    context.Request.Scheme = "https";
                }

                return next();
            })
            .UseHttpDocumentataion(webOptions.CurrentValue)
            .UseHttpHealthChecks(webOptions.CurrentValue);

        base.Configure(applicationBuilder, hostingEnvironment, applicationLifetime);
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
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var applicationKey = Assembly.GetEntryAssembly()?.FullName ?? Assembly.GetExecutingAssembly().FullName;

        var webOptions = config
            .GetSection(BaseAppOptions.SectionName)
            .Get<WebOptions>();

        if (webOptions == null)
        {
            throw new NullReferenceException(nameof(webOptions));
        }

        var shutdownTimeout = TimeSpan.FromSeconds(webOptions.ShutdownTimeout);

        return new WebHostBuilder()
            .UseEnvironment(environment)
            .UseConfiguration(config)
            .UseKestrel(x =>
            {
                x.AddServerHeader = false;

                x.Limits.MaxRequestBodySize = null;
                x.Limits.MaxResponseBufferSize = null;

                webOptions.Hosting.Ports
                    .ToList()
                    .ForEach(y =>
                    {
                        x.ListenAnyIP(y, z =>
                        {
                            z.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                        });
                    });

                webOptions.Hosting.PortsHttps
                    .ToList()
                    .ForEach(y =>
                    {
                        x.ListenAnyIP(y, z =>
                        {
                            z.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;

                            if (webOptions.Hosting.Certificate != null)
                            {
                                if (File.Exists(webOptions.Hosting.Certificate.Path))
                                {
                                    z.UseHttps(webOptions.Hosting.Certificate.Path, webOptions.Hosting.Certificate.Password);
                                }
                            }
                        });
                    });
            })
            .UseContentRoot(root)
            .ConfigureServices(x =>
            {
                x.AddApp<TApplication>(config);
                x.AddWeb(config);
            })
            .UseStartup<TApplication>()
            .UseSetting(WebHostDefaults.ApplicationKey, applicationKey)
            .CaptureStartupErrors(true)
            .UseShutdownTimeout(shutdownTimeout);
    }
}