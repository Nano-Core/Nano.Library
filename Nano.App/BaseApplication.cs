using System;
using Microsoft.Extensions.Hosting;

namespace Nano.App;

// BUG: Should we make a BackgroundJob option, like we did in ....
// and auto resolve just one of the consumers dependencies using IServiceScopeFactory and resolving each of that dependencies

// BUG: 000: Add links to documentation and examples readme from triple slash. E.g AddNanoData<>() should link to Nano.Data Readme and related examples / Usages
// BUG: 000: Go through Required / MaxLength / Etc for Entity models and request models - all the way through all layers

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