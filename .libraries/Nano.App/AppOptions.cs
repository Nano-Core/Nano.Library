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
        public static string SectionName => "api";

        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Name { get; set; }

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
        /// Hosting.
        /// </summary>
        public virtual HostingOptions Hosting { get; set; } = new HostingOptions();

        /// <summary>
        /// Cultures.
        /// </summary>
        public virtual CultureOptions Cultures { get; set; } = new CultureOptions();

        /// <summary>
        /// Culture Options (nested class).
        /// </summary>
        public class CultureOptions
        {
            /// <summary>
            /// Default.
            /// </summary>
            public virtual string Default { get; set; } = "en-US";

            /// <summary>
            /// Supported.
            /// </summary>
            public virtual string[] Supported { get; set; } = new string[0];
        }

        /// <summary>
        /// Hosting Options.
        /// </summary>
        public class HostingOptions
        {
            /// <summary>
            /// Path.
            /// </summary>
            public virtual string Path { get; set; } = "app";

            /// <summary>
            /// Ports.
            /// </summary>
            public virtual int[] Ports { get; set; } = new int[0];
        }
    }
}