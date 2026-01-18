using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.App.StartUp;

namespace Nano.App.Api.Mvc.HealthChecks;

/// <inheritdoc />
public sealed class StartupHealthCheck : IHealthCheck
{
    private readonly StartupTaskContext taskContext;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="taskContext">The <see cref="StartupTaskContext"/>.</param>
    public StartupHealthCheck(StartupTaskContext taskContext)
    {
        this.taskContext = taskContext ?? throw new ArgumentNullException(nameof(taskContext));
    }

    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var result = this.taskContext.IsDone
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy();

        return Task.FromResult(result);
    }
}