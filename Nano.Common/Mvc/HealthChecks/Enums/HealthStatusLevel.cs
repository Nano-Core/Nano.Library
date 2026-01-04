namespace Nano.Common.Mvc.HealthChecks.Enums;

/// <summary>
/// Health Status Level.
/// </summary>
public enum HealthStatusLevel
{
    /// <summary>
    /// Indicates that the health check determined that the component was unhealthy, or an unhandled
    /// exception was thrown while executing the health check.
    /// </summary>
    Unhealthy,

    /// <summary>
    /// Indicates that the health check determined that the component was in a degraded state.
    /// </summary>
    Degraded,
    
    /// <summary>
    /// Indicates that the health check determined that the component was healthy.
    /// </summary>
    Healthy
}