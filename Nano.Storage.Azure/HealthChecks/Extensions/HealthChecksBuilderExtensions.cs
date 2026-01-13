using System;
using System.Collections.Generic;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Nano.Storage.Abstractions.Config;

namespace Nano.Storage.Azure.HealthChecks.Extensions;

internal static class HealthChecksBuilderExtensions
{
    private const string NAME = "azurefileshare";

    internal static IHealthChecksBuilder AddAzureFileshareStorage(this IHealthChecksBuilder builder, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null, TimeSpan? timeout = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .Add(new HealthCheckRegistration(NAME, x =>
            {
                var options = x
                    .GetRequiredService<IOptionsMonitor<StorageOptions>>();

                return new AzureFileshareStorageHealthCheck(options, new ShareClientOptions());
            }, failureStatus, tags, timeout));

        return builder;
    }
}