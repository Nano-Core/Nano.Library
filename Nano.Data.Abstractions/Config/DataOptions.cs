using Nano.Data.Abstractions.Config.Enums;
using System.ComponentModel.DataAnnotations;
using Nano.Common.Config;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Options for configuring the data access layer and DbContext behavior.
/// </summary>
public class DataOptions
{
    /// <summary>
    /// Gets the configuration section name for data options.
    /// </summary>
    internal static string SectionName => "Data";

    /// <summary>
    /// Gets or sets the maximum batch size for queries.
    /// Defaults to <c>25</c>.
    /// </summary>
    [Required]
    public virtual int BatchSize { get; set; } = 25;

    /// <summary>
    /// Gets or sets the maximum batch size for bulk operations.
    /// Defaults to <c>500</c>.
    /// </summary>
    [Required]
    public virtual int BulkBatchSize { get; set; } = 500;

    /// <summary>
    /// Gets or sets the delay (in milliseconds) between bulk batches.
    /// Defaults to <c>1000</c> ms.
    /// </summary>
    [Required]
    public virtual int BulkBatchDelay { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the number of times a query will retry on failure.
    /// Defaults to <c>0</c>.
    /// </summary>
    [Required]
    public virtual int QueryRetryCount { get; set; } = 0;

    /// <summary>
    /// Gets or sets a value indicating whether lazy loading is enabled.
    /// Defaults to <c>true</c>.
    /// </summary>
    [Required]
    public virtual bool UseLazyLoading { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether soft deletion is enabled.
    /// Defaults to <c>true</c>.
    /// </summary>
    [Required]
    public virtual bool UseSoftDeletetion { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the database should be created automatically.
    /// Defaults to <c>false</c>.
    /// </summary>
    [Required]
    public virtual bool UseCreateDatabase { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether database migrations should be applied automatically.
    /// Defaults to <c>true</c>.
    /// </summary>
    [Required]
    public virtual bool UseMigrateDatabase { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether sensitive data logging is enabled.
    /// Defaults to <c>false</c>.
    /// </summary>
    [Required]
    public virtual bool UseSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether auditing is enabled.
    /// Defaults to <c>false</c>.
    /// </summary>
    public virtual bool UseAudit { get; set; } = false;

    /// <summary>
    /// Gets or sets the query splitting behavior for EF Core queries.
    /// Defaults to <see cref="QuerySplitBehavior.SingleQuery"/>.
    /// </summary>
    [Required]
    public virtual QuerySplitBehavior QuerySplittingBehavior { get; set; } = QuerySplitBehavior.SingleQuery;

    /// <summary>
    /// Gets or sets the default collation for the database.
    /// </summary>
    public virtual string? DefaultCollation { get; set; }

    /// <summary>
    /// Gets or sets the connection string for the database.
    /// This value is required and cannot be <c>null</c>.
    /// </summary>
    [Required]
    public virtual string ConnectionString { get; set; } = null!;

    /// <summary>
    /// Gets or sets the repository configuration options.
    /// </summary>
    public virtual RepositoryOptions Repository { get; set; } = new();

    /// <summary>
    /// Gets or sets the identity configuration options.
    /// </summary>
    public virtual IdentityOptions? Identity { get; set; }

    /// <summary>
    /// Gets or sets the connection pool configuration options.
    /// </summary>
    public virtual ConnectionPoolOptions? ConnectionPool { get; set; }

    /// <summary>
    /// Gets or sets the options for configuring health checks.
    /// </summary>
    public virtual HealthCheckOptions? HealthCheck { get; set; }
}