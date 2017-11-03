using Microsoft.Extensions.Configuration;

namespace Nano.Config
{ 
    /// <summary>
    /// Data Options.
    /// Populated from "Data" <see cref="IConfigurationSection"/>.
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