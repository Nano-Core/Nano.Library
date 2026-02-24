using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.App.Abstractions;
using Nano.App.Api;
using Nano.App.Api.Config;
using Nano.App.Api.Extensions;
using Nano.App.Config;
using Nano.App.Config.Extensions;
using Nano.App.Extensions;
using Nano.App.Web.Config;
using Nano.App.Web.Extensions;
using System;

namespace Nano.App.Web;

/// <summary>
/// Represents a Nano Web application.
/// </summary>
/// <remarks>Documentation: <see href="https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Web">Nano Web Application</see></remarks>
public class NanoWebApplication<TRoot> : NanoApiApplication
{
    /// <summary>
    /// Initializes an instance of <see cref="NanoWebApplication{TRoot}"/>.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/>.</param>
    protected NanoWebApplication(WebApplicationBuilder builder)
        : base(builder)
    {
    }

    /// <summary>
    /// Creates and configures the Web application with default Nano services, middleware, and web options.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A configured <see cref="IApplication"/> instance.</returns>
    public new static IApplication ConfigureApp(params string[] args)
    {
        var builder = BaseNanoApplication.CreateWebBuilder(args);

        builder.Services
            .AddNanoApp<WebOptions>(builder.Configuration, out var options)
            .AddNanoConfigSection<ApiOptions>(builder.Configuration, BaseAppOptions.SectionName, out _);

        builder.Services
            .ConfigureNanoWebServices(options);

        builder.WebHost
            .ConfigureWebHost(options);

        return new NanoWebApplication<TRoot>(builder);
    }

    /// <summary>
    /// Builds the Web application, registers middleware, routing, and health checks.
    /// </summary>
    /// <param name="applicationBuilderAction">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The current <see cref="IApplication"/> instance.</returns>
    public override IApplication Build(Action<IApplicationBuilder>? applicationBuilderAction = null)
    {
        this.application = this.applicationBuilder
            .Build();

        var options = this.application.Services
            .GetRequiredService<IOptionsMonitor<WebOptions>>();

        this.application
            .ConfigureNanoWebApplication<TRoot>(options.CurrentValue);

        applicationBuilderAction?
            .Invoke(this.application);

        return this;
    }
}
