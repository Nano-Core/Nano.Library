using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Mvc.HealthChecks.Extensions;
using Nano.Storage.Abstractions;
using Nano.Storage.Abstractions.Config;
using Nano.Storage.LocalShare.HealthChecks;

namespace Nano.Storage.LocalShare;

/// <summary>
/// Storage provider for local file system–backed storage.
/// </summary>
/// <remarks>
///     <para>
///         This provider targets storage that is already accessible to the application
///         via the local file system, such as host directories or Docker volume mounts.
///     </para>
///     <para>
///         The provider does not perform any mounting, drive mapping, or path resolution.
///         Infrastructure concerns (e.g. Docker volumes, bind mounts, or OS-level shares) must be configured externally.
///     </para>
///     <para>
///         When health checks are enabled via <see cref="StorageOptions.HealthCheck"/>,
///         a lightweight file system health check is registered to verify that the configured storage path exists and is accessible.
///     </para>
/// </remarks>
public sealed class LocalFileShareProvider : IStorageProvider
{
    /// <summary>
    /// Configures services required for local file system storage.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which storage-related servicesand optional health checks are added.</param>
    /// <param name="options">The <see cref="StorageOptions"/> used to control provider behavior.</param>
    /// <remarks>
    ///     <para>
    ///         If <see cref="StorageOptions.HealthCheck"/> is <c>null</c>, no health-check services are registered
    ///         and the method returns without side effects.
    ///     </para>
    ///     <para>
    ///         This provider assumes that the root storage path has already been made
    ///         available to the application (for example via Docker volume mounts or host file system configuration).
    ///     </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="options"/> is <c>null</c>.</exception>
    public static void Configure(IServiceCollection services, StorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        if (options.HealthCheck == null)
        {
            return;
        }

        var failureStatus = options.HealthCheck.UnhealthyStatus
            .GetHealthStatus();

        services
            .AddHealthChecks()
            .AddCheck<LocalFileShareHealthCheck>("local-fileshare", failureStatus);
    }
}