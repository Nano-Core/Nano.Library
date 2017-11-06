using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Nano.Config
{
    /// <summary>
    /// App Options.
    /// Populatd from "App" <see cref="IConfiguration"/>.
    /// </summary>
    public partial class AppOptions
    {
        /// <summary>
        /// Section Name.
        /// </summary>
        public static string SectionName => "App";

        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Name { get; set; } = "";

        /// <summary>
        /// Version.
        /// </summary>
        public virtual Version Version { get; set; } = new Version(1, 0); // TODO: test if AppOptions.Version object gets populated correctly. We need a string construcotr passing the value

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
            public virtual IEnumerable<string> Supported { get; set; } = new List<string>();
        }
    }
}