using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Mvc.HealthChecks.Extensions;
using Nano.Storage.Abstractions;
using Nano.Storage.Abstractions.Config;
using Nano.Storage.Azure.HealthChecks.Extensions;

namespace Nano.Storage.Azure;

/// <summary>
/// Nano Storage provider implementation for Azure File Shares.
/// </summary>
/// <remarks>
///     This Nano provider registers Azure File Share–related services based on the supplied <see cref="StorageOptions"/>.
///     When health checks are enabled, it adds an Azure File Share health check to the application's health check pipeline.
/// </remarks>
public class AzureFileshareProvider : IStorageProvider
{
    /// <summary>
    /// Configures services required for Azure File Share storage.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which storage-related services are added.</param>
    /// <param name="options">The non-null <see cref="StorageOptions"/> used to control provider behavior.</param>
    /// <remarks>
    ///     If <see cref="StorageOptions.HealthCheck"/> is <c>null</c>, no health-check services
    ///     are registered and the method returns without side effects.
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
            .AddAzureFileshareStorage(failureStatus);
    }
}