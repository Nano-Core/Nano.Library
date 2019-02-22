using System;
using Microsoft.AspNetCore.Mvc;
using Nano.Web.Hosting.Enums;
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
        /// Compatibility Version.
        /// </summary>
        public virtual CompatibilityVersion CompatabilityVersion { get; set; } = CompatibilityVersion.Version_2_2;

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
            /// Ports Https.
            /// </summary>
            public virtual int[] PortsHttps { get; set; } = new int[0];

            /// <summary>
            /// Use Https Redirect.
            /// Forces https for all requests.
            /// </summary>
            public virtual bool UseHttpsRequired { get; set; } = false;

            /// <summary>
            /// Use Https Redirect.
            /// Enables middleware for automatically redirect http requests to https.
            /// </summary>
            public virtual bool UseHttpsRedirect { get; set; } = false;

            /// <summary>
            /// Use Http Response Compression.
            /// Enables middleware for dynamic compression of http responses.
            /// </summary>
            public virtual bool UseHttpResponseCompression { get; set; } = true;

            /// <summary>
            /// Use Http No Cache Header.
            /// Setting these headers will make the browser reload every page in the browsing history when the user navigates with the “Back” and “Forward” buttons.
            /// This will affect the load on your server(s) — and also the user experience.
            /// </summary>
            public virtual bool UseHttpNoCacheHeader { get; set; } = false;

            /// <summary>
            /// Use Http Forwarded Headers.
            /// Enables forwarded headers, when application is behind a proxy.
            /// </summary>
            public virtual bool UseHttpXForwardedHeaders { get; set; } = true;

            /// <summary>
            /// Use Http X-Download Options.
            /// </summary>
            public virtual bool UseHttpXDownloadOptionsHeader { get; set; } = false;

            /// <summary>
            /// Use Http X-Content Type Options Header.
            /// </summary>
            public virtual bool UseHttpXContentTypeOptionsHeader { get; set; } = false;

            /// <summary>
            /// Use Http Referrer Policy Header.
            /// </summary>
            public virtual ReferrerPolicy UseHttpReferrerPolicyHeader { get; set; } = ReferrerPolicy.Disabled;

            /// <summary>
            /// Use Http X-Frame Options Policy Header.
            /// </summary>
            public virtual XFrameOptionsPolicy UseHttpXFrameOptionsPolicyHeader { get; set; } = XFrameOptionsPolicy.Disabled;

            /// <summary>
            /// Use Http XXss Protection Policy Header.
            /// </summary>
            public virtual XXssProtectionPolicyBlockMode UseHttpXXssProtectionPolicyHeader { get; set; } = XXssProtectionPolicyBlockMode.Disabled;

            /// <summary>
            /// Use Http Session Timeout.
            /// Enables middleware for session context middleware, and sets the session timeout to the value.
            /// </summary>
            public virtual TimeSpan UseHttpSessionTimeout { get; set; } = TimeSpan.FromMinutes(20);

            /// <summary>
            /// Use Health Check.
            /// </summary>
            public virtual bool UseHealthCheck { get; set; } = true;

            /// <summary>
            /// Use Health Check UI.
            /// </summary>
            public virtual bool UseHealthCheckUI { get; set; } = true;

            /// <summary>
            /// Use Hsts.
            /// Settings for Strict-Transport-Security.
            /// </summary>
            public virtual HstsOptions Hsts { get; set; } = new HstsOptions();

            /// <summary>
            /// Robots.
            /// Settings for robots (search engines) behavior.
            /// </summary>
            public virtual RobotOptions UseHttpXRobotsTagHeaders { get; set; } = new RobotOptions();

            /// <summary>
            /// Certificate (ssl)
            /// </summary>
            public virtual CertificateOptions Certificate { get; set; } = new CertificateOptions();
            
            /// <summary>
            /// Hsts Options
            /// </summary>
            public class HstsOptions
            {
                /// <summary>
                /// Is Enabled.
                /// Enables Hsts (Strict transport security) policies.
                /// </summary>
                public virtual bool IsEnabled { get; set; } = false;

                /// <summary>
                /// Max Age.
                /// The maximum age.
                /// </summary>
                public virtual TimeSpan? MaxAge { get; set; } = null;

                /// <summary>
                /// Use Preload.
                /// Adds the preload directive, defaults to false.
                /// Max-age must be at least 18 weeks, and includeSubdomains must be enabled to use the preload directive.
                /// </summary>
                public virtual bool UsePreload { get; set; } = false;

                /// <summary>
                /// Include Subdomains.
                /// Adds includeSubDomains in the header, defaults to false
                /// </summary>
                public virtual bool IncludeSubdomains { get; set; } = false;
            }

            /// <summary>
            /// Robot Options.
            /// </summary>
            public class RobotOptions
            {
                /// <summary>
                /// Is Enabled.
                /// Enables Robots header.
                /// </summary>
                public virtual bool IsEnabled { get; set; } = false;

                /// <summary>
                /// Use No Index.
                /// Instructs search engines to not index the page
                /// </summary>
                public virtual bool UseNoIndex { get; set; } = false;
                
                /// <summary>
                /// Use No Follow
                /// Instructs search engines to not follow links on the page
                /// </summary>
                public virtual bool UseNoFollow { get; set; } = false;
                
                /// <summary>
                /// Use No Snippet.
                /// Instructs search engines to not display a snippet for the page in search results
                /// </summary>
                public virtual bool UseNoSnippet { get; set; } = false;
                
                /// <summary>
                /// Use No Archive.
                /// Instructs search engines to not offer a cached version of the page in search results
                /// </summary>
                public virtual bool UseNoArchive { get; set; } = false;
                
                /// <summary>
                /// Use No ODP.
                /// Instructs search engines to not use information from the Open Directory Project for the page’s title or snippet
                /// </summary>
                public virtual bool UseNoOdp { get; set; } = false;
                
                /// <summary>
                /// Use No Translate - Instructs search engines to not offer translation of the page in search results (Google only)
                /// </summary>
                public virtual bool UseNoTranslate { get; set; } = false;

                /// <summary>
                /// Use No Image Index.
                /// Instructs search engines to not index images on the page (Google only)
                /// </summary>
                public virtual bool UseNoImageIndex { get; set; } = false;
            }
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

        /// <summary>
        /// Certificate Options.
        /// </summary>
        public class CertificateOptions
        {
            /// <summary>
            /// Path.
            /// </summary>
            public virtual string Path { get; set; } = string.Empty;
            
            /// <summary>
            /// Password
            /// </summary>
            public virtual string Password { get; set; } = null;
        }
    }
}