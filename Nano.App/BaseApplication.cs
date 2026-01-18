using Microsoft.Extensions.Hosting;
using System;

namespace Nano.App;

/// <summary>
/// Base Application (abstract).
/// </summary>
public abstract class BaseApplication<THost, THostBuilder>
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