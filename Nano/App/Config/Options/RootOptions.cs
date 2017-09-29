using Microsoft.Extensions.Configuration;
using Nano.App.Data.Options;
using Nano.App.Eventing.Options;
using Nano.App.Hosting.Options;
using Nano.App.Logging.Options;
using Nano.App.Security.Options;

namespace Nano.App.Config.Options
{
    /// <summary>
    /// Root Options.
    /// Populatd from "Root" <see cref="IConfiguration"/>.
    /// </summary>
    public class RootOptions
    {
        /// <summary>
        /// App Name.
        /// </summary>
        public virtual string AppName { get; set; } = "App";

        /// <summary>
        /// App Version.
        /// </summary>
        public virtual string AppVersion { get; set; } = "1.0";

        /// <summary>
        /// Hosting options.
        /// </summary>
        public virtual HostingOptions Hosting { get; set; } = new HostingOptions();

        /// <summary>
        /// Logging section.
        /// </summary>
        public virtual LoggingOptions Logging { get; set; } = new LoggingOptions();

        /// <summary>
        /// Security options.
        /// </summary>
        public virtual SecurityOptions Security { get; set; } = new SecurityOptions();

        /// <summary>
        /// Eventing options.
        /// </summary>
        public virtual EventingOptions Eventing { get; set; } = new EventingOptions();

        /// <summary>
        /// Data Context options.
        /// </summary>
        public virtual DataContextOptions Data { get; set; } = new DataContextOptions();
    }
}