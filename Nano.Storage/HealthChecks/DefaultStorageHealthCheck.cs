using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.Storage.Abstractions;

namespace Nano.Storage.HealthChecks;

/// <summary>
/// Performs a health check against the mounted storage share to verify its availability.
/// </summary>
/// <remarks>
///     The health check verifies that the storage share root is accessible and writable by writing and deleting a probe file,
///     exercising the underlying storage mount end-to-end. The share root is resolved from the injected <see cref="IPathProvider"/>.
///     The probe file name is unique per machine to avoid collisions when multiple replicas share the same storage.
/// </remarks>
public sealed class DefaultStorageHealthCheck : IHealthCheck
{
    private readonly IPathProvider pathProvider;

    private static readonly TimeSpan probeTimeout = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Initializes a new instance of <see cref="DefaultStorageHealthCheck"/>.
    /// </summary>
    /// <param name="pathProvider">A non-null <see cref="IPathProvider"/> providing the root path of the mounted storage share.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pathProvider"/> is <c>null</c>.</exception>
    public DefaultStorageHealthCheck(IPathProvider pathProvider)
    {
        this.pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
    }

    /// <summary>
    /// Executes the health check asynchronously.
    /// </summary>
    /// <param name="context">The <see cref="HealthCheckContext"/> containing registration and failure status information.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the health check operation.</param>
    /// <returns>
    ///     A <see cref="HealthCheckResult"/> indicating whether the storage share root is accessible and writable. Returns <see cref="HealthCheckResult.Healthy"/> when the probe file can be written and deleted;
    ///     otherwise, returns a result with the configured failure status.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <c>null</c>.</exception>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            var root = this.pathProvider.Root;

            await ProbePathAsync(root, probeTimeout, cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, $"Cannot access storage mount {this.pathProvider.Root}", ex);
        }
    }

    private static async Task ProbePathAsync(string path, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(path);

        using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        cancellationTokenSource
            .CancelAfter(timeout);

        var probeTask = Task.Run(() =>
        {
            var probeFileName = Path.GetFileName($".healthcheck-{Environment.MachineName}");

            if (Path.IsPathRooted(probeFileName))
            {
                throw new InvalidOperationException("Probe file name must be a relative file name.");
            }

            var probeFile = Path.Combine(path, probeFileName);

            File.WriteAllText(probeFile, "ok");
            File.Delete(probeFile);
        }, cancellationTokenSource.Token);

        await probeTask
            .WaitAsync(cancellationTokenSource.Token);
    }
}