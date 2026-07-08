using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.Storage.Abstractions;

namespace Nano.Storage.HealthChecks.Extensions;

/// <summary>
/// Provides extension methods for registering storage health checks with an <see cref="IHealthChecksBuilder"/>.
/// </summary>
public static class HealthChecksBuilderExtensions
{
    private const string NAME = "fileshare";

    /// <summary>
    /// Adds a health check that verifies the mounted storage share is accessible and writable.
    /// </summary>
    /// <remarks>
    ///     The health check is registered under the name <c>fileshare</c> and resolves the storage root from the <see cref="IPathProvider"/> registered in the service container.
    /// </remarks>
    /// <param name="builder">A non-null <see cref="IHealthChecksBuilder"/> to add the health check to.</param>
    /// <param name="failureStatus">The <see cref="HealthStatus"/> reported when the health check fails. When <c>null</c>, <see cref="HealthStatus.Unhealthy"/> is used.</param>
    /// <param name="tags">An optional collection of tags used to filter health checks.</param>
    /// <param name="timeout">An optional timeout after which the health check is considered failed.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/> to allow chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <c>null</c>.</exception>
    public static IHealthChecksBuilder AddDefaultStorageHealthCheck(this IHealthChecksBuilder builder, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null, TimeSpan? timeout = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .Add(new HealthCheckRegistration(NAME, x =>
            {
                var pathProvider = x
                    .GetRequiredService<IPathProvider>();

                return new DefaultStorageHealthCheck(pathProvider);
            }, failureStatus, tags, timeout));

        return builder;
    }
}