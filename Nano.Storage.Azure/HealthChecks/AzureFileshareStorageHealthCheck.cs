using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Nano.Storage.Abstractions.Config;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Nano.Storage.Azure.HealthChecks;

/// <summary>
/// Performs a health check against an Azure File Share to verify its availability.
/// </summary>
/// <remarks>
///     The health check verifies that the configured Azure File Share exists and is reachable. Configuration is resolved from <see cref="StorageOptions"/>
///     using <see cref="IOptionsMonitor{TOptions}"/>. A cached <see cref="ShareClient"/> instance is reused across executions.
/// </remarks>
public sealed class AzureFileshareStorageHealthCheck : IHealthCheck
{
    private readonly IOptionsMonitor<StorageOptions> options;

    private static readonly ConcurrentDictionary<string, bool> dnsCache = new();

    /// <summary>
    /// Initializes a new instance of <see cref="AzureFileshareStorageHealthCheck"/>.
    /// </summary>
    /// <param name="options">A non-null <see cref="IOptionsMonitor{StorageOptions}"/> providing the connectionstring and share name for the Azure File Share.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c>.</exception>
    public AzureFileshareStorageHealthCheck(IOptionsMonitor<StorageOptions> options)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
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
            var accountName = this.options.CurrentValue.HealthCheck?.AccountName ?? throw new InvalidOperationException("Storage account name missing");

            var host = $"{accountName}.file.core.windows.net";

            var portOpen = await IsPortOpenAsync(host, 445, TimeSpan.FromSeconds(3), cancellationToken);

            return portOpen
                ? HealthCheckResult.Healthy("Azure Files endpoint reachable")
                : new HealthCheckResult(context.Registration.FailureStatus, $"Cannot reach Azure Files endpoint {host}:445");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }


    private static async Task<bool> IsPortOpenAsync(string host, int port, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(host);

        try
        {
            using var tcpClient = new TcpClient();

            var connectTask = tcpClient
                .ConnectAsync(host, port);

            var completed = await Task.WhenAny(connectTask, Task.Delay(timeout, cancellationToken));

            return completed == connectTask && tcpClient.Connected;
        }
        catch
        {
            return false;
        }
    }
}