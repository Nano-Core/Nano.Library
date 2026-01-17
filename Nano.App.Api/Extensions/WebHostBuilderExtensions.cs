using System;
using Microsoft.AspNetCore.Hosting;
using Nano.App.Api.Config;

namespace Nano.App.Api.Extensions;

internal static class WebHostBuilderExtensions
{
    internal static IWebHostBuilder UseNanoKestrel(this IWebHostBuilder builder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        builder
            .UseKestrel(x =>
            {
                x.AddServerHeader = false;
                x.Limits.MaxRequestBodySize = null;
                x.Limits.MaxResponseBufferSize = null;

                x.ConfigurePorts(apiOptions);
            });

        return builder;
    }
}