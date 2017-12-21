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
        /// Use Memory Cache.
        /// </summary>
        public virtual bool UseMemoryCache { get; set; } = true;

        /// <summary>
        /// Connection String.
        /// </summary>
        public virtual string ConnectionString { get; set; }
    }
}