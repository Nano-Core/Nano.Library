using Microsoft.Extensions.Configuration;

namespace Nano.App.Hosting.Options
{
    /// <summary>
    /// Hosting Options.
    /// Populatd from "Hosting" <see cref="IConfigurationSection"/>.
    /// </summary>
    public class HostingOptions
    {
        /// <summary>
        /// Enable Session.
        /// </summary>
        public virtual bool EnableSession { get; set; } = true;

        /// <summary>
        /// Enable Documentation.
        /// </summary>
        public virtual bool EnableDocumentation { get; set; } = true;
        
        /// <summary>
        /// Enable Gzip Compression.
        /// </summary>
        public virtual bool EnableGzipCompression { get; set; } = true;

        /// <summary>
        /// Enable Request Identifier.
        /// </summary>
        public virtual bool EnableRequestIdentifier { get; set; } = true;

        /// <summary>
        /// Enable Request Localization.
        /// </summary>
        public virtual bool EnableRequestLocalization { get; set; } = true;
    }
}