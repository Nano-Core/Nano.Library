using System;
using Microsoft.Extensions.DependencyInjection;
using Nano.Models.Extensions;
using Nano.Storage.Abstractions;
using Nano.Storage.Abstractions.Config;
using Nano.Storage.Azure.HealthChecks.Extensions;

namespace Nano.Storage.Azure;

/// <summary>
/// Azure Fileshare Provider.
/// </summary>
public class AzureFileshareProvider : IStorageProvider
{
    private readonly StorageOptions options;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="StorageOptions"/>.</param>
    public AzureFileshareProvider(StorageOptions options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public virtual IStorageProvider Configure(IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (!this.options.UseHealthCheck)
        {
            return this;
        }

        if (this.options.Connectionstring == null)
        {
            throw new NullReferenceException(nameof(options.Connectionstring));
        }

        var healtStatus = this.options.UnhealthyStatus
            .GetHealthStatus();

        services
            .AddHealthChecks()
            .AddAzureFileshareStorage(failureStatus: healtStatus);

        return this;
    }
}