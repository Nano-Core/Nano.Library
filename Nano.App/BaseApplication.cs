using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Interfaces;

namespace Nano.App;

/// <summary>
/// Base Application (abstract).
/// </summary>
public abstract class BaseApplication : IApplication
{
    /// <summary>
    /// Configuration.
    /// </summary>
    protected virtual IConfiguration Configuration { get; }

    /// <summary>
    /// Constructor.
    /// Accepting an instance of <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    protected BaseApplication(IConfiguration configuration)
    {
        this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc />
    public virtual IServiceProvider ConfigureServices(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var provider = services
            .BuildServiceProvider();

        return provider;
    }

    /// <inheritdoc />
    public abstract void Configure(IApplicationBuilder applicationBuilder);

    /// <inheritdoc />
    public abstract void Configure(IApplicationBuilder applicationBuilder, IHostEnvironment hostingEnvironment, IHostApplicationLifetime applicationLifetime);
}