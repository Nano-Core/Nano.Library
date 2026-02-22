using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Nano.App.Api.Config;

namespace Nano.App.Api.Extensions;

internal static class ConfigureWebHostBuilderExtensions
{
    internal static void ConfigureWebHost(this ConfigureWebHostBuilder applicationBuilder, ApiOptions options)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(options);

        applicationBuilder
            .UseNanoKestrel(options)
            .CaptureStartupErrors(true)
            .UseShutdownTimeout(TimeSpan.FromSeconds(options.ShutdownTimeout));
    }
}