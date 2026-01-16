using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Abstractions;

namespace Nano.App;

// BUG: 000: Add TenantId to IdentityUserEx (nullable)
// BUG: 000: How can I force providers to be parameterless constructor

// BUG: Should we make a BackgroundJob option, like we did in ....
// and auto resolve just one of the consumers dependencies using IServiceScopeFactory and resolving each of that dependencies

/// <summary>
/// Base Application (abstract).
/// </summary>
public abstract class BaseApplication<THost, THostBuilder> : IApplication
    where THost : class, IHost
    where THostBuilder : IHostApplicationBuilder
{
    /// <summary>
    /// 
    /// </summary>
    protected THost application = null!;

    /// <summary>
    /// 
    /// </summary>
    protected THostBuilder applicationBuilder;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    protected BaseApplication(THostBuilder builder)
    {
        this.applicationBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    /// <summary>
    /// Allows consumers to register application services.
    /// </summary>
    public virtual IApplication ConfigureServices(Action<IServiceCollection> configure)
    {
        configure(applicationBuilder.Services);

        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract IApplication Build();

    /// <summary>
    /// 
    /// </summary>
    public virtual void Run()
    {
        if (this.application == null)
        {
            throw new InvalidOperationException("No Application has been configured and Build.");
        }

        this.application
            .Run();
    }
}