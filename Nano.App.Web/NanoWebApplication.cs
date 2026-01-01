using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Nano.App.Abstractions;
using Nano.App.Extensions;
using Nano.App.Web.Config;
using Nano.App.Web.Extensions;
using Nano.App.Web.Identity.Extensions;
using System;
using System.IO;
using System.Reflection;
using Nano.Common.Config;
using Nano.Data.Abstractions.Eventing.Extensions;
using Nano.Data.Abstractions.Extensions;
using Nano.Eventing.Abstractions.Extensions;

namespace Nano.App.Web;

/// <summary>
/// 
/// </summary>
public sealed class NanoWebApplication : BaseApplication<WebApplication, WebApplicationBuilder>
{
    private NanoWebApplication(WebApplicationBuilder builder)
        : base(builder)
    {
    }

    /// <summary>
    /// Entry point used by consumers.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static NanoWebApplication ConfigureApp(params string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Development;
        var applicationName = Assembly.GetEntryAssembly()?.GetName().Name;
        var config = ConfigManager.BuildConfiguration(args);

        var applicationOptions = new WebApplicationOptions
        {
            Args = args,
            ApplicationName = applicationName,
            EnvironmentName = environment,
            ContentRootPath = root
        };

        var applicationBuilder = WebApplication.CreateBuilder(applicationOptions);

        applicationBuilder.Configuration
            .AddConfiguration(config);

        applicationBuilder.Services
            .AddNanoApp<WebOptions>(config, out var webOptions)
            .AddNanoExceptionHandling(webOptions)
            .AddNanoCors(webOptions)
            .AddNanoForwardedHeaders(webOptions)
            .AddNanoHsts(webOptions)
            .AddNanoCookies(webOptions)
            .AddNanoSession(webOptions)
            .AddNanoResponseCaching(webOptions)
            .AddNanoVersioning(webOptions)
            .AddNanoIdentityAuthentication(webOptions.Identity.Authentication)
            .AddNanoIdentityAuthorization()
            .AddNanoRequestLocalization(webOptions)
            .AddNanoRequestTimeZone(webOptions)
            .AddNanoVirusScan(webOptions)
            .AddNanoResponseCompression(webOptions)
            .AddNanoRequestOptions(webOptions)
            .AddNanoRequestIdentifier(webOptions)
            .AddNanoFormOptions(webOptions)
            .AddNanoMvc(webOptions)
            .AddNanoDocumentation(webOptions)
            .AddNanoHealthChecking(webOptions);

        applicationBuilder.WebHost
            .UseKestrel(x =>
            {
                x.AddServerHeader = false;
                x.Limits.MaxRequestBodySize = null;
                x.Limits.MaxResponseBufferSize = null;

                ConfigurePorts(x, webOptions);
            })
            .CaptureStartupErrors(true)
            .UseShutdownTimeout(TimeSpan.FromSeconds(webOptions.ShutdownTimeout));

        return new NanoWebApplication(applicationBuilder);
    }

    /// <inheritdoc />
    public override IApplication Build()
    {
        this.application = this.applicationBuilder
            .Build();

        var webOptions = this.application.Services
            .GetRequiredService<IOptionsMonitor<WebOptions>>();

        this.application
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
            .UseNanoDocumentataion(webOptions.CurrentValue, this.application.Environment.EnvironmentName)
            .UseNanoHealthChecks(webOptions.CurrentValue, this.application.Environment.EnvironmentName);

        this.application
            .UseEventHandlers()
            .UseEntityEventHandlers()
            .UseNanoDbMigrations();

        return this;
    }


    private static void ConfigurePorts(KestrelServerOptions kestrel, WebOptions webOptions)
    {
        if (kestrel == null)
            throw new ArgumentNullException(nameof(kestrel));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        foreach (var port in webOptions.Hosting.Ports)
        {
            kestrel.ListenAnyIP(port, listen =>
            {
                listen.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
            });
        }

        foreach (var port in webOptions.Hosting.PortsHttps)
        {
            kestrel.ListenAnyIP(port, listen =>
            {
                listen.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;

                var cert = webOptions.Hosting.Certificate;
                if (cert != null && File.Exists(cert.Path))
                {
                    listen.UseHttps(cert.Path, cert.Password);
                }
            });
        }
    }
}