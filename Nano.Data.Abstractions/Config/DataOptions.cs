using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nano.Data.Abstractions.Config.Enums;
using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Config;

/// <summary>
/// Data Options.
/// </summary>
public class DataOptions
{
    /// <summary>
    /// Section Name.
    /// </summary>
    public static string SectionName => "Data";

    /// <summary>
    /// Batch Size.
    /// </summary>
    [Required]
    public virtual int BatchSize { get; set; } = 25;

    /// <summary>
    /// Bulik Batch Size.
    /// </summary>
    [Required]
    public virtual int BulkBatchSize { get; set; } = 500;

    /// <summary>
    /// Bulk Batch Delay.
    /// </summary>
    [Required]
    public virtual int BulkBatchDelay { get; set; } = 1000;

    /// <summary>
    /// Query Retry Count.
    /// </summary>
    [Required]
    public virtual int QueryRetryCount { get; set; } = 0;

    /// <summary>
    /// Query Include Depth.
    /// </summary>
    [Required]
    public virtual int QueryIncludeDepth { get; set; } = 4;

    /// <summary>
    /// Use Audit.
    /// </summary>
    [Required]
    public virtual bool UseAudit { get; set; } = false;

    /// <summary>
    /// Use Auto Save.
    /// </summary>
    [Required]
    public virtual bool UseAutoSave { get; set; } = false;

    /// <summary>
    /// Use Lazy Loading.
    /// </summary>
    [Required]
    public virtual bool UseLazyLoading { get; set; } = true;

    /// <summary>
    /// Use Soft Deletetion.
    /// </summary>
    [Required]
    public virtual bool UseSoftDeletetion { get; set; } = true;

    /// <summary>
    /// Use Create Database.
    /// </summary>
    [Required]
    public virtual bool UseCreateDatabase { get; set; } = false;

    /// <summary>
    /// Use Migrate Database.
    /// </summary>
    [Required]
    public virtual bool UseMigrateDatabase { get; set; } = true;

    /// <summary>
    /// Use Sensitive Data Logging.
    /// </summary>
    [Required]
    public virtual bool UseSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Use Connection Pooling.
    /// </summary>
    [Required]
    public virtual bool UseConnectionPooling { get; set; } = false;

    /// <summary>
    /// Use Query Split Behavior.
    /// </summary>
    [Required]
    public virtual QuerySplitBehavior UseQuerySplittingBehavior { get; set; } = QuerySplitBehavior.SingleQuery;

    /// <summary>
    /// Use Health Check.
    /// </summary>
    [Required]
    public virtual bool UseHealthCheck { get; set; } = true;

    /// <summary>
    /// Unhealthy Status.
    /// </summary>
    [Required]
    public virtual HealthStatus UnhealthyStatus { get; set; } = HealthStatus.Unhealthy;

    /// <summary>
    /// Default Collation.
    /// </summary>
    public virtual string DefaultCollation { get; set; } = null;

    /// <summary>
    /// Connection String.
    /// </summary>
    [Required]
    public virtual string ConnectionString { get; set; } = null;

    /// <summary>
    /// Cache.
    /// </summary>
    public virtual CacheOptions Cache { get; set; }

    /// <summary>
    /// Identity.
    /// </summary>
    public virtual IdentityOptions Identity { get; set; }
}