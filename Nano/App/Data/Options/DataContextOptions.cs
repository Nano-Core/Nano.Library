using Microsoft.Extensions.Configuration;

namespace Nano.App.Data.Options
{
    /// <summary>
    /// Data Options.
    /// Populatd from "Data" <see cref="IConfigurationSection"/>.
    /// </summary>
    public class DataContextOptions
    {
        /// <summary>
        /// Provider.
        /// </summary>
        public virtual string Provider { get; set; } = "SqlServer";
        
        /// <summary>
        /// Batch Size.
        /// </summary>
        public virtual int BatchSize { get; set; } = 25;

        /// <summary>
        /// Use Memory Cache.
        /// </summary>
        public virtual bool UseMemoryCache { get; set; } = true;

        /// <summary>
        /// Isolation Level.
        /// </summary>
        public virtual string IsolationLevel { get; set; } = "READ UNCOMMITTED";

        /// <summary>
        /// Connection String.
        /// </summary>
        public virtual string ConnectionString { get; set; } = null;
    }
}