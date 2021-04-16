using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Nano.Web.Enums;
using NWebsec.AspNetCore.Mvc;

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
            /// Ports Https.
            /// </summary>
            public virtual int[] PortsHttps { get; set; } = new int[0];

            /// <summary>
            /// Allowed Origins.
            /// </summary>
            public virtual string[] AllowedOrigins { get; set; } = new string[0];

            /// <summary>
            /// Use Https Redirect.
            /// Forces https for all requests.
            /// </summary>
            public virtual bool UseHttpsRequired { get; set; } = false;

            /// <summary>
            /// Use Forwarded Headers.
            /// Enables forwarded headers, when application is behind a proxy.
            /// </summary>
            public virtual bool UseForwardedHeaders { get; set; } = true;

            /// <summary>
            /// Use Response Compression.
            /// Enables middleware for dynamic compression of http responses.
            /// </summary>
            public virtual bool UseResponseCompression { get; set; } = true;

            /// <summary>
            /// Use Content Type Options.
            /// Added X-Content-Type Options header.
            /// </summary>
            public virtual bool UseContentTypeOptions { get; set; } = true;

            /// <summary>
            /// Csp.
            /// Settings for Content-Security-Policy.
            /// </summary>
            public virtual CspOptions Csp { get; set; } = new CspOptions();

            /// <summary>
            /// Hsts.
            /// Settings for Strict-Transport-Security.
            /// </summary>
            public virtual HstsOptions Hsts { get; set; } = new HstsOptions();

            /// <summary>
            /// Cache.
            /// Options for caching responses.
            /// </summary>
            public virtual CacheOptions Cache { get; set; } = new CacheOptions();

            /// <summary>
            /// Robots.
            /// Settings for robots (search engines) behavior.
            /// </summary>
            public virtual RobotOptions Robots { get; set; } = new RobotOptions();

            /// <summary>
            /// Session.
            /// Settings for session behavior.
            /// </summary>
            public virtual SessionOptions Session { get; set; } = new SessionOptions();

            /// <summary>
            /// Certificate (ssl)
            /// </summary>
            public virtual CertificateOptions Certificate { get; set; } = new CertificateOptions();

            /// <summary>
            /// Health-Check.
            /// </summary>
            public virtual HealthCheckOptions HealthCheck { get; set; } = new HealthCheckOptions();

            /// <summary>
            /// Use Referrer Policy Header.
            /// </summary>
            public virtual ReferrerPolicy ReferrerPolicyHeader { get; set; } = ReferrerPolicy.Disabled;

            /// <summary>
            /// Use Frame Options Policy Header.
            /// </summary>
            public virtual XFrameOptionsPolicy FrameOptionsPolicyHeader { get; set; } = XFrameOptionsPolicy.Disabled;

            /// <summary>
            /// Use Xss Protection Policy Header.
            /// </summary>
            public virtual XXssProtectionPolicyBlockMode XssProtectionPolicyHeader { get; set; } = XXssProtectionPolicyBlockMode.Disabled;

            /// <summary>
            /// Csp Options.
            /// </summary>
            public class CspOptions
            {
                /// <summary>
                /// Block All Mixed Content.
                /// </summary>
                public virtual bool BlockAllMixedContent { get; set; } = false;

                /// <summary>
                /// Upgrade Insecure Requests.
                /// </summary>
                public virtual bool UpgradeInsecureRequests { get; set; } = false;

                /// <summary>
                /// Defaults.
                /// </summary>
                public virtual CspDirective Defaults { get; set; } = new CspDirective();

                /// <summary>
                /// Styles.
                /// </summary>
                public virtual CspDirectiveStyles Styles { get; set; } = new CspDirectiveStyles();

                /// <summary>
                /// Scripts.
                /// </summary>
                public virtual CspDirectiveScripts Scripts { get; set; } = new CspDirectiveScripts();

                /// <summary>
                /// Objects.
                /// </summary>
                public virtual CspDirective Objects { get; set; } = new CspDirective();

                /// <summary>
                /// Images.
                /// </summary>
                public virtual CspDirective Images { get; set; } = new CspDirective();

                /// <summary>
                /// Media.
                /// </summary>
                public virtual CspDirective Media { get; set; } = new CspDirective();

                /// <summary>
                /// Frames.
                /// </summary>
                public virtual CspDirective Frames { get; set; } = new CspDirective();

                /// <summary>
                /// Frame Ancestors.
                /// </summary>
                public virtual CspDirective FrameAncestors { get; set; } = new CspDirective();

                /// <summary>
                /// Fonts.
                /// </summary>
                public virtual CspDirective Fonts { get; set; } = new CspDirective();

                /// <summary>
                /// Connections.
                /// </summary>
                public virtual CspDirective Connections { get; set; } = new CspDirective();

                /// <summary>
                /// Base Uri's.
                /// </summary>
                public virtual CspDirective BaseUris { get; set; } = new CspDirective();

                /// <summary>
                /// Children.
                /// </summary>
                public virtual CspDirective Children { get; set; } = new CspDirective();

                /// <summary>
                /// Forms.
                /// </summary>
                public virtual CspDirective Forms { get; set; } = new CspDirective();

                /// <summary>
                /// Manifests.
                /// </summary>
                public virtual CspDirective Manifests { get; set; } = new CspDirective();

                /// <summary>
                /// Workers.
                /// </summary>
                public virtual CspDirective Workers { get; set; } = new CspDirective();

                /// <summary>
                /// Sandbox.
                /// </summary>
                public virtual CspDirectiveSandbox Sandbox { get; set; } = new CspDirectiveSandbox();

                /// <summary>
                /// Report Uris.
                /// </summary>
                public virtual string[] ReportUris { get; set; } = new string[0];

                /// <summary>
                /// Plugin Types.
                /// </summary>
                public virtual string[] PluginTypes { get; set; } = new string[0];

                /// <summary>
                /// Is Enabled.
                /// </summary>
                internal virtual bool IsEnabled => this.BlockAllMixedContent || this.UpgradeInsecureRequests || this.Defaults.IsEnabled || this.Styles.IsEnabled || this.Scripts.IsEnabled || this.Objects.IsEnabled || this.Images.IsEnabled || this.Media.IsEnabled || this.Frames.IsEnabled || this.FrameAncestors.IsEnabled || this.Fonts.IsEnabled || this.Connections.IsEnabled || this.BaseUris.IsEnabled || this.Children.IsEnabled || this.Forms.IsEnabled || this.Manifests.IsEnabled || this.Workers.IsEnabled || this.Sandbox.IsEnabled;

                /// <summary>
                /// Csp Directive.
                /// </summary>
                public class CspDirective
                {
                    /// <summary>
                    /// Is None.
                    /// Adds the 'none' source.
                    /// All other sources are ignored.
                    /// </summary>
                    public virtual bool IsNone { get; set; } = false;

                    /// <summary>
                    /// Is Self.
                    /// Adds the 'self' source.
                    /// </summary>
                    public virtual bool IsSelf { get; set; } = false;

                    /// <summary>
                    /// Sources.
                    /// Adds the array of custom sources.
                    /// </summary>
                    public virtual string[] Sources { get; set; } = new string[0];

                    /// <summary>
                    /// Is Enabled.
                    /// </summary>
                    internal virtual bool IsEnabled => this.IsNone || this.IsSelf || this.Sources.Any();
                }

                /// <summary>
                /// Csp Directive Scripts.
                /// </summary>
                public class CspDirectiveScripts : CspDirective
                {
                    /// <summary>
                    /// Is None.
                    /// Adds the 'unsafe-eval' source.
                    /// </summary>
                    public virtual bool IsUnsafeEval { get; set; } = false;

                    /// <summary>
                    /// Is None.
                    /// Adds the 'unsafe-inline' source.
                    /// </summary>
                    public virtual bool IsUnsafeInline { get; set; } = false;

                    /// <summary>
                    /// Is None.
                    /// Adds the 'strict-dynamic' source.
                    /// </summary>
                    public virtual bool StrictDynamic { get; set; } = false;

                    /// <inheritdoc />
                    internal override bool IsEnabled => base.IsEnabled || this.IsUnsafeEval || this.IsUnsafeInline || this.StrictDynamic;
                }

                /// <summary>
                /// Csp Directive Styles.
                /// </summary>
                public class CspDirectiveStyles : CspDirective
                {
                    /// <summary>
                    /// Is None.
                    /// Adds the 'unsafe-inline' source.
                    /// </summary>
                    public virtual bool IsUnsafeInline { get; set; } = false;

                    /// <inheritdoc />
                    internal override bool IsEnabled => base.IsEnabled || this.IsUnsafeInline;
                }

                /// <summary>
                /// Sandbox.
                /// </summary>
                public class CspDirectiveSandbox
                {
                    /// <summary>
                    /// Allow Forms.
                    /// Allows the page to submit forms. If this keyword is not used, this operation is not allowed.
                    /// </summary>
                    public virtual bool AllowForms { get; set; } = false;

                    /// <summary>
                    /// Allow Modals.
                    /// Allows the page to open modal windows.
                    /// </summary>
                    public virtual bool AllowModals { get; set; } = false;

                    /// <summary>
                    /// Allow Orientation Lock.
                    /// Allows the page to disable the ability to lock the screen orientation.
                    /// </summary>
                    public virtual bool AllowOrientationLock { get; set; } = false;

                    /// <summary>
                    /// Allow Pointer Lock.
                    /// Allows the page to use the Pointer Lock API.
                    /// </summary>
                    public virtual bool AllowPointerLock { get; set; } = false;

                    /// <summary>
                    /// Allow Popups.
                    /// Allows popups (like from window.open, target= "_blank", showModalDialog). If this keyword is not used, that functionality will silently fail.
                    /// </summary>
                    public virtual bool AllowPopups { get; set; } = false;

                    /// <summary>
                    /// Allow Popups To Escape Sandbox.
                    /// Allows a sandboxed document to open new windows without forcing the sandboxing flags upon them.
                    /// This will allow, for example, a third-party advertisement to be safely sandboxed without forcing the same restrictions upon a landing page.
                    /// </summary>
                    public virtual bool AllowPopupsToEscapeSandbox { get; set; } = false;

                    /// <summary>
                    /// Allow Presentation.
                    /// Allows embedders to have control over whether an iframe can start a presentation session.
                    /// </summary>
                    public virtual bool AllowPresentation { get; set; } = false;

                    /// <summary>
                    /// Allow Same Origin.
                    /// Allows the content to be treated as being from its normal origin.
                    /// If this keyword is not used, the embedded content is treated as being from a unique origin.
                    /// </summary>
                    public virtual bool AllowSameOrigin { get; set; } = false;

                    /// <summary>
                    /// Allow Scripts.
                    /// Allows the page to run scripts (but not create pop-up windows).
                    /// If this keyword is not used, this operation is not allowed.
                    /// </summary>
                    public virtual bool AllowScripts { get; set; } = false;

                    /// <summary>
                    /// Allow Top Navigation.
                    /// Allows the page to navigate (load) content to the top-level browsing context.
                    /// If this keyword is not used, this operation is not allowed.
                    /// </summary>
                    public virtual bool AllowTopNavigation { get; set; } = false;

                    /// <summary>
                    /// Is Enabled.
                    /// </summary>
                    internal virtual bool IsEnabled => this.AllowForms || this.AllowModals || this.AllowOrientationLock || this.AllowPointerLock || this.AllowPopups || this.AllowPopupsToEscapeSandbox || this.AllowPresentation || this.AllowSameOrigin || this.AllowScripts || this.AllowTopNavigation;
                }
            }

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
            /// Cache Options.
            /// </summary>
            public class CacheOptions
            {
                /// <summary>
                /// Is Enabled.
                /// Enables Hsts (Strict transport security) policies.
                /// </summary>
                public virtual bool IsEnabled { get; set; } = true;

                /// <summary>
                /// Max Size.
                /// Default 1 MB.
                /// </summary>
                public virtual int MaxSize { get; set; } = 1024;

                /// <summary>
                /// Max Body Size.
                /// Default 100 MB.
                /// </summary>
                public virtual int MaxBodySize { get; set; } = 100 * 1024;

                /// <summary>
                /// Max Age.
                /// Default 20 minutes.
                /// </summary>
                public virtual TimeSpan MaxAge { get; set; } = TimeSpan.FromMinutes(20);
            }

            /// <summary>
            /// Robot Options.
            /// </summary>
            public class RobotOptions
            {
                /// <summary>
                /// Is Enabled.
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

            /// <summary>
            /// Session Options.
            /// </summary>
            public class SessionOptions
            {
                /// <summary>
                /// Is Enabled.
                /// Enables session.
                /// </summary>
                public virtual bool IsEnabled { get; set; } = true;

                /// <summary>
                /// Timeout.
                /// The session timeout.
                /// </summary>
                public virtual TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(20);
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

            /// <summary>
            /// Health-Check Options
            /// </summary>
            public class HealthCheckOptions
            {
                /// <summary>
                /// Use Health Check.
                /// </summary>
                public virtual bool UseHealthCheck { get; set; } = true;

                /// <summary>
                /// Use Health Check UI.
                /// </summary>
                // ReSharper disable InconsistentNaming
                public virtual bool UseHealthCheckUI { get; set; } = true;
                // ReSharper restore InconsistentNaming

                /// <summary>
                /// Evaluation Interval.
                /// The interval between health-checks.
                /// </summary>
                public virtual int EvaluationInterval { get; set; } = 10;

                /// <summary>
                /// Failure Notification Timout.
                /// The minimum number of secoends betweeen failure notificaitons.
                /// </summary>
                public virtual int FailureNotificationInterval { get; set; } = 60;

                /// <summary>
                /// Web-Hooks.
                /// </summary>
                public virtual HealthCheckWebHookOptions[] WebHooks { get; set; } = new HealthCheckWebHookOptions[0];

                /// <summary>
                /// Health-Check Web-Hook Options.
                /// </summary>
                public class HealthCheckWebHookOptions
                {
                    /// <summary>
                    /// Name.
                    /// </summary>
                    public virtual string Name { get; set; }

                    /// <summary>
                    /// Uri.
                    /// </summary>
                    public virtual string Uri { get; set; }

                    /// <summary>
                    /// Payload.
                    /// </summary>
                    public virtual string Payload { get; set; }
                }
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
            public virtual OpenApiContact Contact { get; set; } = new OpenApiContact();

            /// <summary>
            /// License.
            /// </summary>
            public virtual OpenApiLicense License { get; set; } = new OpenApiLicense();
        }
    }
}