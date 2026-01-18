using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.Storage.Abstractions;

namespace Nano.Storage.LocalShare.HealthChecks;

/// <summary>
/// Health check that validates access to a local file system–backed storage location.
/// </summary>
/// <remarks>
///     <para>
///         The check verifies that the root storage directory provided by
///         <see cref="IPathProvider"/> exists and is writable by the application.
///     </para>
///     <para>
///         This health check does not perform any mounting or configuration of the underlying file system.
///         It assumes that the storage path has already been made available via host
///         or container configuration (for example, Docker volume mounts).
///     </para>
/// </remarks>
public sealed class LocalFileShareHealthCheck : IHealthCheck
{
    private readonly IPathProvider pathProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalFileShareHealthCheck"/> class.
    /// </summary>
    /// <param name="pathProvider">The <see cref="IPathProvider"/> used to resolve the root directory of theconfigured storage location.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pathProvider"/> is <c>null</c>.</exception>
    public LocalFileShareHealthCheck(IPathProvider pathProvider)
    {
        this.pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
    }

    /// <summary>
    /// Performs a health check of the local file share.
    /// </summary>
    /// <param name="context">The context in which the health check is executed.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the health check operation.</param>
    /// <returns>A <see cref="HealthCheckResult"/> indicating whether the local file share is accessible and writable.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var root = pathProvider.Root;

            if (!Directory.Exists(root))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"Directory '{root}' does not exist."));
            }

            var testFile = Path.Combine(root, ".healthcheck");

            File.WriteAllText(testFile, "ok");
            File.Delete(testFile);

            return Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Local file share is not accessible.", ex));
        }
    }
}