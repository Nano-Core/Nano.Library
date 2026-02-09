using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.Common.Mvc.HealthChecks.Enums;

namespace Nano.Common.Mvc.HealthChecks.Extensions;

/// <summary>
/// Extension methods for <see cref="HealthStatusLevel"/> to map to <see cref="HealthStatus"/>.
/// </summary>
public static class HealthStatusLevelExtensions
{
    /// <summary>
    /// Maps a <see cref="HealthStatusLevel"/> value to a corresponding <see cref="HealthStatus"/> value.
    /// </summary>
    /// <param name="healthStatusLevel">The <see cref="HealthStatusLevel"/> to convert.</param>
    /// <returns>The corresponding <see cref="HealthStatus"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="healthStatusLevel"/> has a value not defined in <see cref="HealthStatusLevel"/>.</exception>
    public static HealthStatus GetHealthStatus(this HealthStatusLevel healthStatusLevel)
    {
        return healthStatusLevel switch
        {
            HealthStatusLevel.Healthy => HealthStatus.Healthy,
            HealthStatusLevel.Degraded => HealthStatus.Degraded,
            HealthStatusLevel.Unhealthy => HealthStatus.Unhealthy,
            _ => throw new ArgumentOutOfRangeException(nameof(healthStatusLevel), healthStatusLevel, "Argument out of range.")
        };
    }
}