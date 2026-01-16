using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Nano.Storage.Abstractions.Config;

namespace Nano.Storage.Azure.HealthChecks;

/// <summary>
/// Performs a health check against an Azure File Share to verify its availability.
/// </summary>
/// <remarks>
///     The health check verifies that the configured Azure File Share exists and is reachable. Configuration is resolved from <see cref="StorageOptions"/>
///     using <see cref="IOptionsMonitor{TOptions}"/>. A cached <see cref="ShareClient"/> instance is reused across executions.
/// </remarks>
public class AzureFileshareStorageHealthCheck : IHealthCheck
{
    private readonly IOptionsMonitor<StorageOptions> options;
    private readonly ShareClientOptions? clientOptions;

    private static readonly ConcurrentDictionary<string, ShareClient> clientsHolder = new();

    /// <summary>
    /// Initializes a new instance of <see cref="AzureFileshareStorageHealthCheck"/>.
    /// </summary>
    /// <param name="options">A non-null <see cref="IOptionsMonitor{StorageOptions}"/> providing the connectionstring and share name for the Azure File Share.</param>
    /// <param name="clientOptions">Optional <see cref="ShareClientOptions"/> used to configure the underlying <see cref="ShareClient"/>. If <c>null</c>, default client options are used.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c>.</exception>
    public AzureFileshareStorageHealthCheck(IOptionsMonitor<StorageOptions> options, ShareClientOptions? clientOptions = null)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.clientOptions = clientOptions;
    }

    /// <summary>
    /// Executes the health check asynchronously.
    /// </summary>
    /// <param name="context">The <see cref="HealthCheckContext"/> containing registration and failure status information.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the health check operation.</param>
    /// <returns>
    ///     A <see cref="HealthCheckResult"/> indicating whether the Azure File Share exists. Returns <see cref="HealthCheckResult.Healthy"/> when the share is found;
    ///     otherwise, returns a result with the configured failure status.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <c>null</c>.</exception>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

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
        var connectionString = this.GetConnectionString();

        AzureFileshareStorageHealthCheck.clientsHolder
            .TryGetValue(connectionString, out var client);

        if (client != null)
        {
            return client;
        }

        client = new ShareClient(connectionString, this.options.CurrentValue.ShareName, this.clientOptions);

        AzureFileshareStorageHealthCheck.clientsHolder
            .TryAdd(connectionString, client);

        return client;
    }
    private string GetConnectionString()
    {
        return $"DefaultEndpointsProtocol=https;AccountName={this.options.CurrentValue.AccountName};AccountKey={this.options.CurrentValue.AccountKey};EndpointSuffix=core.windows.net";
    }
}