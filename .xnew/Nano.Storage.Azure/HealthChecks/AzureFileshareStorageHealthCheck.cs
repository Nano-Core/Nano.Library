using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.Storage.Abstractions.Config;

namespace Nano.Storage.Azure.HealthChecks;

/// <summary>
/// Azure File-Share Storage Health-Check.
/// </summary>
public class AzureFileshareStorageHealthCheck : IHealthCheck
{
    private readonly StorageOptions options;
    private readonly ShareClientOptions clientOptions;
    
    private static readonly ConcurrentDictionary<string, ShareClient> clientsHolder = new();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">The <see cref="StorageOptions"/>.</param>
    /// <param name="clientOptions">the <see cref="ShareClientOptions"/>.</param>
    public AzureFileshareStorageHealthCheck(StorageOptions options, ShareClientOptions clientOptions = null)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.clientOptions = clientOptions;
    }

    /// <summary>
    /// Check Health Async.
    /// </summary>
    /// <param name="context">The <see cref="HealthCheckContext"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="HealthCheckResult"/>.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        try
        {
            var client = this.GetClient();

            var shareExists = await client
                .ExistsAsync(cancellationToken);

            return shareExists
                ? HealthCheckResult.Healthy()
                : new HealthCheckResult(context.Registration.FailureStatus, $"Storage '{client.Name}' not found");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }


    private ShareClient GetClient()
    {
        var exists = clientsHolder
            .TryGetValue(this.options.Connectionstring, out var client);

        if (exists)
        {
            return client;
        }

        client = new ShareClient(this.options.Connectionstring, this.options.ShareName, this.clientOptions);

        clientsHolder
            .TryAdd(this.options.Connectionstring, client);

        return client;
    }
}