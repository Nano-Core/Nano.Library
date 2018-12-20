using NWebsec.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Swagger;
using ReferrerPolicy = NWebsec.Core.Common.HttpHeaders.ReferrerPolicy;

namespace Nano.Web
{
    /// <summary>
    /// Web Options.
    /// </summary>
    public class WebOptions
    {
        /// <summary>
        /// Section Name.
        /// </summary>
        public static string SectionName => "Web";

        /// <summary>
        /// Hosting.
        /// </summary>
        public virtual HostingOptions Hosting { get; set; } = new HostingOptions();

        /// <summary>
        /// Documentation.
        /// </summary>
        public virtual DocumentationOptions Documentation { get; set; } = new DocumentationOptions();

        /// <summary>
        /// Hosting Options.
        /// </summary>
        public class HostingOptions
        {
            /// <summary>
            /// Root.
            /// </summary>
            public virtual string Root { get; set; } = "api";

            /// <summary>
            /// Ports.
            /// </summary>
            public virtual int[] Ports { get; set; } = new int[0];

            /// <summary>
            /// Use Ssl.
            /// </summary>
            public virtual bool UseSsl { get; set; }

            /// <summary>
            /// Use Hsts.
            /// </summary>
            public virtual bool UseHsts { get; set; } = false;

            /// <summary>
            /// Use Cache.
            /// </summary>
            public virtual bool UseNoCache { get; set; } = false;
            
            /// <summary>
            /// Use X-Robots Tag.
            /// </summary>
            public virtual bool UseXRobotsTag { get; set; } = false;

            /// <summary>
            /// Use X-Download Options.
            /// </summary>
            public virtual bool UseXDownloadOptions { get; set; } = false;

            /// <summary>
            /// Use Redirect Validation.
            /// </summary>
            public virtual bool UseRedirectValidation { get; set; } = false;

            /// <summary>
            /// Use X-Content Type Options.
            /// </summary>
            public virtual bool UseXContentTypeOptions { get; set; } = false;

            /// <summary>
            /// Referrer Policy.
            /// </summary>
            public virtual ReferrerPolicy ReferrerPolicy { get; set; } = ReferrerPolicy.Disabled;

            /// <summary>
            /// X-Frame Options Policy.
            /// </summary>
            public virtual XFrameOptionsPolicy XFrameOptionsPolicy { get; set; } = XFrameOptionsPolicy.Disabled;

            /// <summary>
            /// XXss Protection Policy.
            /// </summary>
            public virtual XXssProtectionPolicy XXssProtectionPolicy { get; set; } = XXssProtectionPolicy.Disabled;
        }

        /// <summary>
        /// Documentation Options.
        /// </summary>
        public class DocumentationOptions
        {
            /// <summary>
            /// Contact.
            /// </summary>
            public virtual Contact Contact { get; set; } = new Contact();

            /// <summary>
            /// License.
            /// </summary>
            public virtual License License { get; set; } = new License();
        }
    }
}