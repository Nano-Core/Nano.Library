namespace Nano.App
{
    /// <summary>
    /// App Options.
    /// </summary>
    public partial class AppOptions
    {
        /// <summary>
        /// Section Name.
        /// </summary>
        internal static string SectionName => "App";

        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Name { get; set; } = "";

        /// <summary>
        /// Version.
        /// </summary>
        public virtual string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Terms Url.
        /// </summary>
        public virtual string TermsUrl { get; set; }

        /// <summary>
        /// License Url.
        /// </summary>
        public virtual string LicenseUrl { get; set; }

        /// <summary>
        /// Cultures.
        /// </summary>
        public virtual Culture Cultures { get; set; } = new Culture();

        /// <summary>
        /// Switches.
        /// </summary>
        public virtual SwitchOptions Switches { get; set; } = new SwitchOptions();

        /// <summary>
        /// Hosting.
        /// </summary>
        public virtual HostingOptions Hosting { get; set; } = new HostingOptions();

        /// <summary>
        /// Culture (nested class).
        /// </summary>
        public class Culture
        {
            /// <summary>
            /// Default.
            /// </summary>
            public virtual string Default { get; set; } = "en-US";

            /// <summary>
            /// Supported.
            /// </summary>
            public virtual string[] Supported { get; set; }
        }

        /// <summary>
        /// Hosting Options.
        /// </summary>
        public class HostingOptions
        {
            /// <summary>
            /// Port.
            /// </summary>
            public virtual int Port { get; set; } = 5000;

            /// <summary>
            /// Path.
            /// </summary>
            public virtual string Path { get; set; } = "app";
        }

        /// <summary>
        /// Options.
        /// </summary>
        public class SwitchOptions
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
}