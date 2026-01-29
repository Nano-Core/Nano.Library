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
/// Represents a Nano web API application.
/// </summary>
/// <remarks>Documentation: <see href="https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api">Nano Api Application</see></remarks>
public class NanoApiApplication : BaseApplication<WebApplication, WebApplicationBuilder>, IApplication
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    protected NanoApiApplication(WebApplicationBuilder builder)
        : base(builder)
    {
    }

    /// <summary>
    /// Allows consumers to register services for the API application.
    /// </summary>
    /// <param name="configure">A delegate to configure <see cref="IServiceCollection"/>.</param>
    /// <returns>The current <see cref="IApplication"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configure"/> is null.</exception>
    public virtual IApplication ConfigureServices(Action<IServiceCollection> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        configure(this.applicationBuilder.Services);

        return this;
    }

    /// <summary>
    /// Creates and configures the API application with default Nano services, middleware, and web options.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A configured <see cref="IApplication"/> instance.</returns>
    public static IApplication ConfigureApp(params string[] args)
    {
        var applicationBuilder = CreateBuilder(args);

        applicationBuilder.Services
            .AddNanoApp<ApiOptions>(applicationBuilder.Configuration, out var options)
            .AddNanoExceptionHandling()
            .AddNanoCors(options.HttpPolicyHeaders.Cors)
            .AddNanoForwardedHeaders(options.HttpPolicyHeaders.ForwardedHeaders)
            .AddNanoHsts(options.HttpPolicyHeaders.Hsts)
            .AddNanoCookies()
            .AddNanoSession(options.Session)
            .AddNanoResponseCaching(options.ResponseCache)
            .AddNanoVersioning(options.Version, options.Documentation?.UseDefaultVersion)
            .AddNanoIdentityAuthentication(options.Identity?.Authentication)
            .AddNanoIdentityAuthorization()
            .AddNanoRequestLocalization()
            .AddNanoRequestTimeZone(options.DefaultTimeZone)
            .AddNanoVirusScan(options.VirusScan)
            .AddNanoResponseCompression(options.ResponseCompression)
            .AddNanoRequestOptions()
            .AddNanoRequestIdentifier()
            .AddNanoFormOptions(options.Hosting.MultipartLimits)
            .AddNanoMvc()
            .AddNanoDocumentation(options.Documentation)
            .AddNanoHealthChecking(options.Hosting.Ports.FirstOrDefault(), options.HealthCheck);

        applicationBuilder.WebHost
            .UseNanoKestrel(options)
            .CaptureStartupErrors(true)
            .UseShutdownTimeout(TimeSpan.FromSeconds(options.ShutdownTimeout));

        return new NanoApiApplication(applicationBuilder);
    }

    /// <summary>
    /// Builds the API application, registers middleware, routing, and health checks.
    /// </summary>
    /// <returns>The current <see cref="IApplication"/> instance.</returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Development;

        var entryAssembly = Assembly.GetEntryAssembly();
        var config = ConfigManager.BuildConfiguration(environment, entryAssembly, args);
        var applicationName = entryAssembly?.GetName().Name;

        var options = new WebApplicationOptions
        {
            Args = args,
            ApplicationName = applicationName,
            EnvironmentName = environment,
            ContentRootPath = root
        };

        var builder = WebApplication
            .CreateBuilder(options);

        builder.Configuration
            .AddConfiguration(config);

        return builder;
    }
}
