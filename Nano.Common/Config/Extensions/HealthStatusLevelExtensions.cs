using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.Common.Config.Enums;

namespace Nano.Common.Config.Extensions;

/// <summary>
/// Health Status Level Extensions.
/// </summary>
public static class HealthStatusLevelExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="healthStatusLevel"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static HealthStatus GetHealthStatus(this HealthStatusLevel healthStatusLevel)
    {
        return healthStatusLevel switch
        {
            HealthStatusLevel.Healthy => HealthStatus.Healthy,
            HealthStatusLevel.Degraded => HealthStatus.Degraded,
            HealthStatusLevel.Unhealthy => HealthStatus.Unhealthy,
            _ => throw new ArgumentOutOfRangeException(nameof(healthStatusLevel), healthStatusLevel, null)
        };
    }
}