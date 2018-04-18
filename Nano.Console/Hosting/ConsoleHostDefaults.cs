namespace Nano.Console.Hosting
{
    /// <summary>
    /// Console Host Defaults.
    /// </summary>
    public static class ConsoleHostDefaults
    {
        /// <summary>
        /// Environment Key.
        /// </summary>
        public static readonly string environmentKey = "environment";

        /// <summary>
        /// Application Key.
        /// </summary>
        public static readonly string applicationKey = "applicationName";

        /// <summary>
        /// Startup Assembly Key.
        /// </summary>
        public static readonly string startupAssemblyKey = "startupAssembly";

        /// <summary>
        /// Console Root Key.
        /// </summary>
        public static readonly string consoleRootKey = "consoleroot";

        /// <summary>
        /// Content Root Key.
        /// </summary>
        public static readonly string contentRootKey = "contentRoot";

        /// <summary>
        /// Detailed Errors Key.
        /// </summary>
        public static readonly string detailedErrorsKey = "detailedErrors";

        /// <summary>
        /// Shutdown Timeout Key.
        /// </summary>
        public static readonly string shutdownTimeoutKey = "shutdownTimeoutSeconds";

        /// <summary>
        /// Capture Startup Errors Key.
        /// </summary>
        public static readonly string captureStartupErrorsKey = "captureStartupErrors";

        /// <summary>
        /// Prevent Hosting Startup Key.
        /// </summary>
        public static readonly string preventHostingStartupKey = "preventHostingStartup";

        /// <summary>
        /// Suppress Status Messages Key.
        /// </summary>
        public static readonly string suppressStatusMessagesKey = "suppressStatusMessages";

        /// <summary>
        /// Hosting Startup Assemblies Key.
        /// </summary>
        public static readonly string hostingStartupAssembliesKey = "hostingStartupAssemblies";

        /// <summary>
        /// Hosting Startup Exclude Assemblies Key.
        /// </summary>
        public static readonly string hostingStartupExcludeAssembliesKey = "hostingStartupExcludeAssemblies";
    }
}