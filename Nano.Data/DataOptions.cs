namespace Nano.Data
{ 
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
        /// Bulik Batch Delay.
        /// </summary>
        public virtual int BulkBatchDelay { get; set; } = 1000;

        /// <summary>
        /// Use Audit.
        /// </summary>
        public virtual bool UseAudit { get; set; } = true;

        /// <summary>
        /// Use Lazy Loading.
        /// </summary>
        public virtual bool UseLazyLoading { get; set; } = true;
        
        /// <summary>
        /// Use Memory Cache.
        /// </summary>
        public virtual bool UseMemoryCache { get; set; } = true;

        /// <summary>
        /// Use Soft Deletetion.
        /// </summary>
        public virtual bool UseSoftDeletetion { get; set; } = true;

        /// <summary>
        /// Use Create Database.
        /// </summary>
        public virtual bool UseCreateDatabase { get; set; } = true;

        /// <summary>
        /// Connection String.
        /// </summary>
        public virtual string ConnectionString { get; set; }
    }
}