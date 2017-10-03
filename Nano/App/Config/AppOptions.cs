using Microsoft.Extensions.Configuration;

namespace Nano.App.Config
{
    /// <summary>
    /// App Options.
    /// Populatd from "App" <see cref="IConfiguration"/>.
    /// </summary>
    public partial class AppOptions
    {
        /// <summary>
        /// Namemo.
        /// </summary>
        public virtual string Name { get; set; } = "Application";

        /// <summary>
        /// Version.
        /// </summary>
        public virtual string Version { get; set; } = "1.0";

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
    }
}