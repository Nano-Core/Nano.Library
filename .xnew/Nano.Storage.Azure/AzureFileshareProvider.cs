using Microsoft.Extensions.DependencyInjection;
using Nano.Models.Extensions;
using Nano.Storage.Providers.Azure.HealthChecks.Extensions;
using System;
using Nano.Storage.Interfaces;

namespace Nano.Storage.Providers.Azure;

/// <summary>
/// Azure Fileshare Provider.
/// </summary>
public class AzureFileshareProvider : IStorageProvider
{
    /// <summary>
    /// Options.
    /// </summary>
    protected virtual StorageOptions Options { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="StorageOptions"/>.</param>
    public AzureFileshareProvider(StorageOptions options)
    {
        this.Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual IStorageProvider Configure(IServiceCollection services, StorageOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null) 
            throw new ArgumentNullException(nameof(options));

        if (options.UseHealthCheck)
        {
            if (options.ConnectionString != null)
            {
                var healtStatus = options.UnhealthyStatus
                    .GetHealthStatus();

                services
                    .AddHealthChecks()
                    .AddAzureFileshareStorage(options.ConnectionString, options.ShareName, failureStatus: healtStatus);
            }
        }

        return this;
    }
}