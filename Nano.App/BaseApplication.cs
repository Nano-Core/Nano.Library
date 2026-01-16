using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Abstractions;

namespace Nano.App;

// BUG: Add TenantId to IdentityUserEx (nullable)

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
    protected THost? application;

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