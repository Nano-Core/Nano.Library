using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Nano.App.Abstractions;
using Nano.App.Api.Config;
using Nano.App.Api.Extensions;
using Nano.App.Api.Identity.Authentication.Extensions;
using Nano.App.Extensions;
using Nano.Common.Config;
using Nano.Data.Abstractions.Eventing.Extensions;
using Nano.Data.Abstractions.Extensions;
using Nano.Eventing.Abstractions.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Nano.App.Api;

/// <summary>
/// 
/// </summary>
/// <remarks>Documentation: https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api</remarks>
public sealed class NanoApiApplication : BaseApplication<WebApplication, WebApplicationBuilder>, IApplication
{
    private NanoApiApplication(WebApplicationBuilder builder)
        : base(builder)
    {
    }

    /// <summary>
    /// Allows consumers to register application services.
    /// </summary>
    public IApplication ConfigureServices(Action<IServiceCollection> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        configure(applicationBuilder.Services);

        return this;
    }

    /// <summary>
    /// Entry point used by consumers.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IApplication ConfigureApp(params string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Development;
        var entryAssembly = Assembly.GetEntryAssembly();
        var config = ConfigManager.BuildConfiguration(environment, entryAssembly, args);
        var applicationName = entryAssembly?.GetName().Name;

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
            .AddNanoApp<ApiOptions>(config, out var webOptions)
            .AddNanoExceptionHandling()
            .AddNanoCors(webOptions.HttpPolicyHeaders.Cors)
            .AddNanoForwardedHeaders(webOptions.HttpPolicyHeaders.ForwardedHeaders)
            .AddNanoHsts(webOptions.HttpPolicyHeaders.Hsts)
            .AddNanoCookies()
            .AddNanoSession(webOptions.Session)
            .AddNanoResponseCaching(webOptions.ResponseCache)
            .AddNanoVersioning(webOptions.Version, webOptions.Documentation?.UseDefaultVersion)
            .AddNanoIdentityAuthentication(webOptions.Identity?.Authentication)
            .AddNanoIdentityAuthorization()
            .AddNanoRequestLocalization()
            .AddNanoRequestTimeZone(webOptions.DefaultTimeZone)
            .AddNanoVirusScan(webOptions.VirusScan)
            .AddNanoResponseCompression(webOptions.ResponseCompression)
            .AddNanoRequestOptions()
            .AddNanoRequestIdentifier()
            .AddNanoFormOptions(webOptions.Hosting.MultipartLimits)
            .AddNanoMvc()
            .AddNanoDocumentation(webOptions.Documentation)
            .AddNanoHealthChecking(webOptions.Hosting.Ports.FirstOrDefault(), webOptions.HealthCheck);

        applicationBuilder.WebHost
            .UseNanoKestrel(webOptions)
            .CaptureStartupErrors(true)
            .UseShutdownTimeout(TimeSpan.FromSeconds(webOptions.ShutdownTimeout));

        return new NanoApiApplication(applicationBuilder);
    }

    /// <inheritdoc />
    public IApplication Build()
    {
        this.application = this.applicationBuilder
            .Build();

        var options = this.application.Services
            .GetRequiredService<IOptionsMonitor<ApiOptions>>();

        this.application
            .UseNanoExceptionHandling()
            .UseNanoHttpCorsPolicy(options.CurrentValue.HttpPolicyHeaders.Cors)
            .UseNanoHttpXForwardedHeaders(options.CurrentValue.HttpPolicyHeaders.ForwardedHeaders)
            .UseNanoHttpXRobotsTagHeaders(options.CurrentValue.HttpPolicyHeaders.Robots)
            .UseNanoHttpXFrameOptionsPolicyHeader(options.CurrentValue.HttpPolicyHeaders.XFrameOptions)
            .UseNanoHttpXXssProtectionPolicyHeader(options.CurrentValue.HttpPolicyHeaders.XXssProtection)
            .UseNanoHttpContentTypeOptionsPolicyHeader(options.CurrentValue.HttpPolicyHeaders.ContentType)
            .UseNanoHttpReferrerPolicyHeader(options.CurrentValue.HttpPolicyHeaders.ReferrerPolicy)
            .UseNanoHttpStrictTransportSecurityPolicyHeader(options.CurrentValue.HttpPolicyHeaders.Hsts)
            .UseNanoHttpContentSecurityPolicyHeader(options.CurrentValue.HttpPolicyHeaders.Csp)
            .UseStaticFiles()
            .UseCookiePolicy()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseNanoSession(options.CurrentValue.Session)
            .UseNanoRequestOptions()
            .UseNanoRequestIdentifier()
            .UseNanoRequestVirusScan(options.CurrentValue.VirusScan)
            .UseNanoRequestLocalization(options.CurrentValue)
            .UseNanoRequestTimeZone()
            .UseNanoResponseCompression(options.CurrentValue.ResponseCompression)
            .UseNanoResponseCaching(options.CurrentValue.ResponseCache)
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
            .UseNanoDocumentataion(this.application.Environment, options.CurrentValue.Version, options.CurrentValue.Documentation)
            .UseNanoHealthChecks(this.application.Environment, options.CurrentValue.HealthCheck);

        this.application
            .UseEventHandlers()
            .UseEntityEventHandlers()
            .UseNanoDbMigrations();

        return this;
    }
}