using Microsoft.Extensions.Configuration;

namespace Nano.Hosting
{
    /// <summary>
    /// Hosting Options.
    /// Populated from "Hosting" <see cref="IConfigurationSection"/>.
    /// </summary>
    public class HostingOptions
    {
        /// <summary>
        /// Section Name.
        /// </summary>
        internal static string SectionName => "Hosting";

        /// <summary>
        /// Path.
        /// </summary>
        public virtual string Path { get; set; } = "app";

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
        /// Enable Http Context Extension.
        /// </summary>
        public virtual bool EnableHttpContextLogging { get; set; } = true;

        /// <summary>
        /// Enable Http Request Identifier.
        /// </summary>
        public virtual bool EnableHttpContextIdentifier { get; set; } = true;

        /// <summary>
        /// Enable Http Request Localization.
        /// </summary>
        public virtual bool EnableHttpContextLocalization { get; set; } = true;
    }
}