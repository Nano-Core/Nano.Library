using Nano.Common.Config;

namespace Nano.Storage.Abstractions.Config;

/// <summary>
/// Represents configuration options for storage-related health checks.
/// </summary>
/// <remarks>
///     Extends <see cref="HealthCheckOptions"/> with settings required to validate connectivity and availability of storage resources such as Azure File Shares.
/// </remarks>
public class StorageHealthCheckOptions : HealthCheckOptions;