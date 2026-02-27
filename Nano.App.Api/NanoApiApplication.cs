using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.App.Abstractions;
using Nano.App.Api.Config;
using Nano.App.Api.Extensions;
using Nano.App.Extensions;

namespace Nano.App.Api;

/// <summary>
/// Represents a Nano API application.
/// </summary>
/// <remarks>Documentation: <see href="https://github.com/Nano-Core/Nano.Library/tree/master/Nano.App.Api">Nano Api Application</see></remarks>
public class NanoApiApplication : BaseNanoApplication<IApiApplication, WebApplication, WebApplicationBuilder>, IApiApplication
{
    /// <summary>
    /// Initializes an instance of <see cref="NanoApiApplication"/>.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/>.</param>
    protected NanoApiApplication(WebApplicationBuilder builder)
        : base(builder)
    {
    }

    /// <summary>
    /// Creates and configures the API application with default Nano services, middleware, and web options.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A configured <see cref="IApplication"/> instance.</returns>
    public static IApiApplication ConfigureApp(params string[] args)
    {
        var builder = CreateWebBuilder(args);

        builder.Services
            .AddNanoApp<ApiOptions>(builder.Configuration, out var options);

        builder.Services
            .ConfigureNanoApiServices(options);

        builder.WebHost
            .ConfigureWebHost(options);

        return new NanoApiApplication(builder);
    }

    /// <inheritdoc />
    public virtual IApiApplication ConfigureServices(Action<IServiceCollection> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        configure(this.applicationBuilder.Services);

        return this;
    }

    /// <inheritdoc />
    public virtual IApiApplication Build(Action<IApplicationBuilder>? applicationBuilderAction = null)
    {
        this.application = this.applicationBuilder
            .Build();

        var options = this.application.Services
            .GetRequiredService<IOptionsMonitor<ApiOptions>>();

        this.application
            .ConfigureNanoApiApplication(options.CurrentValue);

        applicationBuilderAction?
            .Invoke(this.application);

        return this;
    }
}