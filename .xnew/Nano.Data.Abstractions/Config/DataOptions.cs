using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nano.Data;

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
    public virtual int BatchSize { get; set; } = 25;

    /// <summary>
    /// Bulik Batch Size.
    /// </summary>
    public virtual int BulkBatchSize { get; set; } = 500;

    /// <summary>
    /// Bulk Batch Delay.
    /// </summary>
    public virtual int BulkBatchDelay { get; set; } = 1000;

    /// <summary>
    /// Query Retry Count.
    /// </summary>
    public virtual int QueryRetryCount { get; set; } = 0;

    /// <summary>
    /// Query Include Depth.
    /// </summary>
    public virtual int QueryIncludeDepth { get; set; } = 4;

    /// <summary>
    /// Use Audit.
    /// </summary>
    public virtual bool UseAudit { get; set; } = false;

    /// <summary>
    /// Use Auto Save.
    /// </summary>
    public virtual bool UseAutoSave { get; set; } = false;

    /// <summary>
    /// Use Lazy Loading.
    /// </summary>
    public virtual bool UseLazyLoading { get; set; } = true;

    /// <summary>
    /// Use Memory Cache.
    /// </summary>
    public virtual bool UseMemoryCache { get; set; } = false;

    /// <summary>
    /// Use Soft Deletetion.
    /// </summary>
    public virtual bool UseSoftDeletetion { get; set; } = true;

    /// <summary>
    /// Use Create Database.
    /// </summary>
    public virtual bool UseCreateDatabase { get; set; } = false;

    /// <summary>
    /// Use Migrate Database.
    /// </summary>
    public virtual bool UseMigrateDatabase { get; set; } = true;

    /// <summary>
    /// Use Sensitive Data Logging.
    /// </summary>
    public virtual bool UseSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Use Connection Pooling.
    /// </summary>
    public virtual bool UseConnectionPooling { get; set; } = false;

    /// <summary>
    /// Use Query Split Behavior.
    /// </summary>
    public virtual QuerySplitBehavior UseQuerySplittingBehavior { get; set; } = QuerySplitBehavior.SingleQuery;

    /// <summary>
    /// Use Health Check.
    /// </summary>
    public virtual bool UseHealthCheck { get; set; } = true;

    /// <summary>
    /// Unhealthy Status.
    /// </summary>
    public virtual HealthStatus UnhealthyStatus { get; set; } = HealthStatus.Unhealthy;

    /// <summary>
    /// Default Collation.
    /// </summary>
    public virtual string DefaultCollation { get; set; } = null;

    /// <summary>
    /// Connection String.
    /// </summary>
    public virtual string ConnectionString { get; set; } = null;

    /// <summary>
    /// Identity.
    /// </summary>
    public virtual IdentityOptions Identity { get; set; } = new();

    /// <summary>
    /// Cache.
    /// </summary>
    public virtual MemoryCacheOptions Cache { get; set; } = new();
}