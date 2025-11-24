using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.Common.Extensions;
using Nano.Storage.Abstractions;
using Nano.Storage.Abstractions.Config;
using Nano.Storage.Azure.HealthChecks.Extensions;

namespace Nano.Storage.Azure;

/// <summary>
/// Azure Fileshare Provider.
/// </summary>
public class AzureFileshareProvider : IStorageProvider
{
    private readonly IOptionsMonitor<StorageOptions> options;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="IOptionsMonitor{StorageOptions}"/>.</param>
    public AzureFileshareProvider(IOptionsMonitor<StorageOptions> options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual IStorageProvider Configure(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (this.options.CurrentValue.UseHealthCheck)
        {
            var failureStatus = this.options.CurrentValue.UnhealthyStatus
                .GetHealthStatus();

            services
                .AddHealthChecks()
                .AddAzureFileshareStorage(failureStatus);
        }

        return this;
    }
}