namespace Nano.App
{
    /// <summary>
    /// App Options.
    /// </summary>
    public class AppOptions
    {
        /// <summary>
        /// Section Name.
        /// </summary>
        public static string SectionName => "App";

        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Name { get; set; } = "Application";

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Terms Of Service.
        /// </summary>
        public virtual string TermsOfService { get; set; }

        /// <summary>
        /// Default Time Zone.
        /// </summary>
        public virtual string DefaultTimeZone { get; set; } = "UTC";

        /// <summary>
        /// Version.
        /// </summary>
        public virtual string Version { get; set; } = "1.0.0";

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
    }
}