using System;
using System.Collections.Generic;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Nano.Storage.Abstractions.Config;

namespace Nano.Storage.Azure.HealthChecks.Extensions;

/// <summary>
/// Health-Check Builder Extensions.
/// </summary>
internal static class HealthChecksBuilderExtensions
{
    private const string NAME = "azurefileshare";

    /// <summary>
    /// Add a health check for Azure Fileshare Storage.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="failureStatus">The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.</param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    internal static IHealthChecksBuilder AddAzureFileshareStorage(this IHealthChecksBuilder builder, HealthStatus? failureStatus = default, IEnumerable<string> tags = default, TimeSpan? timeout = default)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

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