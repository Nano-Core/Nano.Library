using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Abstractions;
using Nano.App.Extensions;
using Nano.App.Web.Config;
using Nano.App.Web.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
using Nano.App.Config;
using Nano.App.Web.Identity;
using Nano.App.Web.Identity.Abstractions;
using Nano.Common.Config;
using Nano.Common.Config.Extensions;

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
            .UseNanoExceptionHandling()
            .UseNanoHttpCorsPolicy(webOptions.CurrentValue)
            .UseNanoHttpXForwardedHeaders(webOptions.CurrentValue)
            .UseNanoHttpXRobotsTagHeaders(webOptions.CurrentValue)
            .UseNanoHttpXFrameOptionsPolicyHeader(webOptions.CurrentValue)
            .UseNanoHttpXXssProtectionPolicyHeader(webOptions.CurrentValue)
            .UseNanoHttpContentTypeOptionsPolicyHeader(webOptions.CurrentValue)
            .UseNanoHttpReferrerPolicyHeader(webOptions.CurrentValue)
            .UseNanoHttpStrictTransportSecurityPolicyHeader(webOptions.CurrentValue)
            .UseNanoHttpContentSecurityPolicyHeader(webOptions.CurrentValue)
            .UseStaticFiles()
            .UseCookiePolicy()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseNanoSession(webOptions.CurrentValue)
            .UseNanoRequestOptions()
            .UseNanoRequestIdentifier()
            .UseNanoRequestVirusScan(webOptions.CurrentValue)
            .UseNanoRequestLocalization(webOptions.CurrentValue)
            .UseNanoRequestTimeZone()
            .UseNanoResponseCompression(webOptions.CurrentValue)
            .UseNanoResponseCaching(webOptions.CurrentValue)
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
            .UseNanoDocumentataion(webOptions.CurrentValue)
            .UseNanoHealthChecks(webOptions.CurrentValue);

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

        var tempWebOptions = config
            .GetSection(BaseAppOptions.SectionName)
            .Get<WebOptions>();

        if (tempWebOptions == null)
        {
            throw new NullReferenceException(nameof(tempWebOptions));
        }

        var shutdownTimeout = TimeSpan.FromSeconds(tempWebOptions.ShutdownTimeout);

        return new WebHostBuilder()
            .UseEnvironment(environment)
            .UseConfiguration(config)
            .UseKestrel(x =>
            {
                x.AddServerHeader = false;

                x.Limits.MaxRequestBodySize = null;
                x.Limits.MaxResponseBufferSize = null;

                tempWebOptions.Hosting.Ports
                    .ToList()
                    .ForEach(y =>
                    {
                        x.ListenAnyIP(y, z =>
                        {
                            z.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                        });
                    });

                tempWebOptions.Hosting.PortsHttps
                    .ToList()
                    .ForEach(y =>
                    {
                        x.ListenAnyIP(y, z =>
                        {
                            z.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;

                            if (tempWebOptions.Hosting.Certificate != null)
                            {
                                if (File.Exists(tempWebOptions.Hosting.Certificate.Path))
                                {
                                    z.UseHttps(tempWebOptions.Hosting.Certificate.Path, tempWebOptions.Hosting.Certificate.Password);
                                }
                            }
                        });
                    });
            })
            .UseContentRoot(root)
            .ConfigureServices(x =>
            {
                x.AddNanoApp<TApplication>(config);

                x.AddConfigSection<WebOptions>(BaseAppOptions.SectionName, out var webOptions);

                x.AddNanoExceptionHandling(webOptions);
                x.AddNanoCors(webOptions);
                x.AddNanoForwardedHeaders(webOptions);
                x.AddNanoHsts(webOptions);
                x.AddNanoCookies(webOptions);
                x.AddNanoSession(webOptions);
                x.AddNanoResponseCaching(webOptions);
                x.AddNanoVersioning(webOptions);
                x.AddNanoIdentityAuthenticationAndAuthorization(webOptions.Identity.Authentication);
                x.AddNanoRequestLocalization(webOptions);
                x.AddNanoRequestTimeZone(webOptions);
                x.AddNanoVirusScan(webOptions);
                x.AddNanoResponseCompression(webOptions);
                x.AddNanoRequestOptions(webOptions);
                x.AddNanoRequestIdentifier(webOptions);
                x.AddNanoFormOptions(webOptions);
                x.AddNanoMvc(webOptions);
                x.AddNanoDocumentation(webOptions);
                x.AddNanoHealthChecking(webOptions);

                x.AddScoped<IIdentityJwtRepository, IdentityJwtRepository>();
                x.AddScoped<IAuthRepository, DefaultAuthRepository>();
                x.AddScoped<IAuthTransientRepository, DefaultAuthTransientRepository>();
            })
            .UseStartup<TApplication>()
            .UseSetting(WebHostDefaults.ApplicationKey, applicationKey)
            .CaptureStartupErrors(true)
            .UseShutdownTimeout(shutdownTimeout);
    }
}