using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Nano.App.Api.Config;

namespace Nano.App.Api.Extensions;

internal static class KestrelServerOptionsExtensions
{
    internal static void ConfigurePorts(this KestrelServerOptions kestrel, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(kestrel);
        ArgumentNullException.ThrowIfNull(apiOptions);

        kestrel.AddServerHeader = false;
        kestrel.Limits.MaxRequestBodySize = apiOptions.Hosting.MultipartLimits?.MaxUploadBytes;
        kestrel.Limits.KeepAliveTimeout = apiOptions.Hosting.MultipartLimits?.KeepAliveTimeout ?? kestrel.Limits.KeepAliveTimeout;

        foreach (var port in apiOptions.Hosting.Http.Ports)
        {
            kestrel
                .ListenAnyIP(port, listen =>
                {
                    listen.Protocols = HttpProtocols.Http1;
                });
        }

        if (apiOptions.Hosting.Https != null)
        {
            foreach (var port in apiOptions.Hosting.Https.Ports)
            {
                kestrel
                    .ListenAnyIP(port, listen =>
                    {
                        listen.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;

                        if (File.Exists(apiOptions.Hosting.Https.Certificate.Path))
                        {
                            listen
                                .UseHttps(apiOptions.Hosting.Https.Certificate.Path, apiOptions.Hosting.Https.Certificate.Password);
                        }
                    });
            }
        }
    }
}