using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nano.App;

/// <inheritdoc />
public class DefaultApplication : BaseApplication
{
    /// <inheritdoc />
    public DefaultApplication(IConfiguration configuration)
        : base(configuration)
    {

    }

    /// <inheritdoc />
    public override void Configure(IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var hostingEnvironment = services.GetService<IHostEnvironment>();
        var applicationLifetime = services.GetService<IHostApplicationLifetime>();

        this.Configure(applicationBuilder, hostingEnvironment, applicationLifetime);
    }

    /// <inheritdoc />
    public override void Configure(IApplicationBuilder applicationBuilder, IHostEnvironment hostingEnvironment, IHostApplicationLifetime applicationLifetime)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (hostingEnvironment == null)
            throw new ArgumentNullException(nameof(hostingEnvironment));

        if (applicationLifetime == null)
            throw new ArgumentNullException(nameof(applicationLifetime));
    }
}