using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nano.Storage.Providers.Azure.HealthChecks.Extensions;

/// <summary>
/// Health-Check Builder Extensions.
/// </summary>
public static class HealthChecksBuilderExtensions
{
    private const string NAME = "azurefileshare";

    /// <summary>
    /// Add a health check for Azure Fileshare Storage.
    /// </summary>
    /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="connectionString">The Azure Storage connectionstring.</param>
    /// <param name="shareName">The Azure Storage share name. to check if exist. Optional.If <c>null</c> then file-share name check is not executed.</param>
    /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'azurefileshare' will be used for the name.</param>
    /// <param name="failureStatus">The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.</param>
    /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
    /// <param name="timeout">An optional System.TimeSpan representing the timeout of the check.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    public static IHealthChecksBuilder AddAzureFileshareStorage(this IHealthChecksBuilder builder, string connectionString, string shareName = default, string name = NAME, HealthStatus? failureStatus = default, IEnumerable<string> tags = default, TimeSpan? timeout = default)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (connectionString == null)
            throw new ArgumentNullException(nameof(connectionString));

        var storageHealthCheck = new AzureFileshareStorageHealthCheck(connectionString, shareName);

        builder
            .Add(new HealthCheckRegistration(name, _ => storageHealthCheck, failureStatus, tags, timeout));

        return builder;
    }
}