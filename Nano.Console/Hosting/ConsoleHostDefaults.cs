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
        public static string EnvironmentKey => "environment";

        /// <summary>
        /// Application Key.
        /// </summary>
        public static string ApplicationKey => "applicationName";

        /// <summary>
        /// Startup Assembly Key.
        /// </summary>
        public static string StartupAssemblyKey => "startupAssembly";

        /// <summary>
        /// Console Root Key.
        /// </summary>
        public static string ConsoleRootKey => "consoleroot";

        /// <summary>
        /// Content Root Key.
        /// </summary>
        public static string ContentRootKey => "contentRoot";

        /// <summary>
        /// Shutdown Timeout Key.
        /// </summary>
        public static string ShutdownTimeoutKey => "shutdownTimeoutSeconds";

        /// <summary>
        /// Capture Startup Errors Key.
        /// </summary>
        public static string CaptureStartupErrorsKey => "captureStartupErrors";

        /// <summary>
        /// Hosting Startup Assemblies Key.
        /// </summary>
        public static string HostingStartupAssembliesKey => "hostingStartupAssemblies";

        /// <summary>
        /// Hosting Startup Exclude Assemblies Key.
        /// </summary>
        public static string HostingStartupExcludeAssembliesKey => "hostingStartupExcludeAssemblies";
    }
}