using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nano.Storage.Providers.Azure.HealthChecks;

/// <summary>
/// Azure File-Share Storage Health-Check.
/// </summary>
public class AzureFileshareStorageHealthCheck : IHealthCheck
{
    private readonly string shareName;
    private readonly string connectionString;
    private readonly ShareClientOptions clientOptions;
    
    private static readonly ConcurrentDictionary<string, ShareClient> clientsHolder = new();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="connectionString">The storage connection-string.</param>
    /// <param name="shareName">The share name.</param>
    /// <param name="clientOptions">the <see cref="ShareClientOptions"/>.</param>
    public AzureFileshareStorageHealthCheck(string connectionString, string shareName = default, ShareClientOptions clientOptions = null)
    {
        this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        this.shareName = shareName;
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
            var shareExists = await client.ExistsAsync(cancellationToken);

            return shareExists
                ? HealthCheckResult.Healthy()
                : new HealthCheckResult(context.Registration.FailureStatus, $"Storage '{shareName}' not found");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }

    private ShareClient GetClient()
    {
        var exists = clientsHolder.TryGetValue(this.connectionString, out var client);

        if (exists)
        {
            return client;
        }

        client = new ShareClient(this.connectionString, this.shareName, this.clientOptions);

        clientsHolder
            .TryAdd(this.connectionString, client);

        return client;
    }
}