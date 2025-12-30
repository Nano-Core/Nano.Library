using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Common.Config.Extensions;
using Nano.Storage.Abstractions;
using Nano.Storage.Abstractions.Config;
using Nano.Storage.Azure.HealthChecks.Extensions;

namespace Nano.Storage.Azure;

/// <summary>
/// Azure Fileshare Provider.
/// </summary>
public class AzureFileshareProvider : IStorageProvider
{
    /// <inheritdoc />
    public virtual void Configure(IServiceCollection services, StorageOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (!options.UseHealthCheck)
        {
            return;
        }

        var failureStatus = options.UnhealthyStatus
            .GetHealthStatus();

        services
            .AddHealthChecks()
            .AddAzureFileshareStorage(failureStatus);
    }
}