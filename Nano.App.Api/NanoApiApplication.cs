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
public class NanoApiApplication : BaseNanoApplication<WebApplication, WebApplicationBuilder>, INanoApplication
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
    /// <returns>The current <see cref="INanoApplication"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configure"/> is null.</exception>
    public virtual INanoApplication ConfigureServices(Action<IServiceCollection> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        configure(this.applicationBuilder.Services);

        return this;
    }

    /// <summary>
    /// Creates and configures the API application with default Nano services, middleware, and web options.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A configured <see cref="INanoApplication"/> instance.</returns>
    public static INanoApplication ConfigureApp(params string[] args)
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
            .AddNanoVersioning(options.Version)
            .AddNanoIdentityAuthentication(options.Identity?.Authentication)
            .AddNanoIdentityAuthorization()
            .AddNanoRequestLocalization(options.Localization)
            .AddNanoRequestTimeZone(options.TimeZone)
            .AddNanoVirusScan(options.VirusScan)
            .AddNanoResponseCompression(options.ResponseCompression)
            .AddNanoRequestIdentifier()
            .AddNanoFormOptions(options.Hosting.MultipartLimits)
            .AddNanoHttpsRedirection(options.Hosting.Http, options.Hosting.Https)
            .AddNanoMvc()
            .AddNanoDocumentation(options.Documentation)
            .AddNanoHealthChecking(options.Hosting.Http.Ports.FirstOrDefault(), options.HealthCheck);

        applicationBuilder.WebHost
            .UseNanoKestrel(options)
            .CaptureStartupErrors(true)
            .UseShutdownTimeout(TimeSpan.FromSeconds(options.ShutdownTimeout));

        return new NanoApiApplication(applicationBuilder);
    }

    /// <summary>
    /// Builds the API application, registers middleware, routing, and health checks.
    /// </summary>
    /// <returns>The current <see cref="INanoApplication"/> instance.</returns>
    public INanoApplication Build()
    {
        this.application = this.applicationBuilder
            .Build();

        var options = this.application.Services
            .GetRequiredService<IOptionsMonitor<ApiOptions>>();

        this.application
            .UseNanoExceptionHandling()
            .UseNanoHttpsRedirection(options.CurrentValue.Hosting.Http, options.CurrentValue.Hosting.Https)
            .UseNanoHttpXForwardedHeaders(options.CurrentValue.HttpPolicyHeaders.ForwardedHeaders)
            .UseNanoHttpXRobotsTagHeaders(options.CurrentValue.HttpPolicyHeaders.Robots)
            .UseNanoHttpXFrameOptionsPolicyHeader(options.CurrentValue.HttpPolicyHeaders.FrameOptions)
            .UseNanoHttpXXssProtectionPolicyHeader(options.CurrentValue.HttpPolicyHeaders.XssProtection)
            .UseNanoHttpContentTypeOptionsPolicyHeader(options.CurrentValue.HttpPolicyHeaders.ContentType)
            .UseNanoHttpReferrerPolicyHeader(options.CurrentValue.HttpPolicyHeaders.ReferrerPolicy)
            .UseNanoHttpStrictTransportSecurityPolicyHeader(options.CurrentValue.HttpPolicyHeaders.Hsts)
            .UseNanoHttpContentSecurityPolicyHeader(options.CurrentValue.HttpPolicyHeaders.Csp)
            .UseStaticFiles()
            .UseCookiePolicy()
            .UseRouting()
            .UseNanoHttpCorsPolicy(options.CurrentValue.HttpPolicyHeaders.Cors)
            .UseAuthentication()
            .UseAuthorization()
            .UseNanoSession(options.CurrentValue.Session)
            .UseNanoRequestIdentifier()
            .UseNanoRequestVirusScan(options.CurrentValue.VirusScan)
            .UseNanoRequestLocalization(options.CurrentValue.Localization)
            .UseNanoRequestTimeZone(options.CurrentValue.TimeZone)
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


    private static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var wwwroot = Path.Combine(root, "wwwroot");
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environments.Development;

        var entryAssembly = Assembly.GetEntryAssembly();
        var config = ConfigManager.BuildConfiguration(environment, entryAssembly, args);
        var applicationName = entryAssembly?.GetName().Name;

        var options = new WebApplicationOptions
        {
            Args = args,
            ApplicationName = applicationName,
            EnvironmentName = environment,
            ContentRootPath = root,
            WebRootPath = wwwroot
        };

        var builder = WebApplication
            .CreateBuilder(options);

        builder.Configuration
            .AddConfiguration(config);

        return builder;
    }
}
