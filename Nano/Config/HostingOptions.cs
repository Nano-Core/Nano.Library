using Microsoft.Extensions.Configuration;

namespace Nano.Config
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
        public static string SectionName => "Hosting";

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
        public virtual bool EnableHttpContextExtension { get; set; } = true;

        /// <summary>
        /// Enable Http Request Identifier.
        /// </summary>
        public virtual bool EnableHttpRequestIdentifier { get; set; } = true;

        /// <summary>
        /// Enable Http Request Localization.
        /// </summary>
        public virtual bool EnableHttpRequestLocalization { get; set; } = true;
    }
}