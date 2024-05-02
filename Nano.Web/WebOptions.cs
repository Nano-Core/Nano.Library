using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using Nano.Web.Enums;
using NWebsec.AspNetCore.Mvc;

namespace Nano.Web;

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
    public virtual HostingOptions Hosting { get; set; } = new();

    /// <summary>
    /// Documentation.
    /// </summary>
    public virtual DocumentationOptions Documentation { get; set; } = new();

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
        public virtual int[] Ports { get; set; } = Array.Empty<int>();

        /// <summary>
        /// Ports Https.
        /// </summary>
        public virtual int[] PortsHttps { get; set; } = Array.Empty<int>();

        /// <summary>
        /// Shutdown Timeout.
        /// The number of seconds the application waits after receiving a SIGTERM signal, before shutting down.
        /// </summary>
        public virtual int ShutdownTimeout { get; set; } = 10;

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
        public virtual CspOptions Csp { get; set; } = new();

        /// <summary>
        /// Cors.
        /// </summary>
        public virtual CorsOptions Cors { get; set; } = new();

        /// <summary>
        /// Hsts.
        /// Settings for Strict-Transport-Security.
        /// </summary>
        public virtual HstsOptions Hsts { get; set; } = new();

        /// <summary>
        /// Cache.
        /// Options for caching responses.
        /// </summary>
        public virtual CacheOptions Cache { get; set; } = new();

        /// <summary>
        /// Robots.
        /// Settings for robots (search engines) behavior.
        /// </summary>
        public virtual RobotOptions Robots { get; set; } = new();

        /// <summary>
        /// Session.
        /// Settings for session behavior.
        /// </summary>
        public virtual SessionOptions Session { get; set; } = new();

        /// <summary>
        /// Certificate (ssl)
        /// </summary>
        public virtual CertificateOptions Certificate { get; set; } = new();

        /// <summary>
        /// Health-Check.
        /// </summary>
        public virtual HealthCheckOptions HealthCheck { get; set; } = new();

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
        /// Cors Options.
        /// </summary>
        public class CorsOptions
        {
            /// <summary>
            /// Allowed Origins.
            /// </summary>
            public virtual string[] AllowedOrigins { get; set; } = Array.Empty<string>();

            /// <summary>
            /// Allowed Headers.
            /// </summary>
            public virtual string[] AllowedHeaders { get; set; } = Array.Empty<string>(); 

            /// <summary>
            /// Allowed methods.
            /// </summary>
            public virtual string[] AllowedMethods { get; set; } = Array.Empty<string>(); 

            /// <summary>
            /// Allow Credentials.
            /// </summary>
            public virtual bool AllowCredentials { get; set; } = true;

            /// <summary>
            /// Origin.
            /// </summary>
            public virtual OriginOptions Origin { get; set; } = new();

            /// <summary>
            /// Origin Options.
            /// </summary>
            public class OriginOptions
            {
                /// <summary>
                /// Embedder Policy.
                /// </summary>
                public virtual CrossOriginEmbedderPolicy? EmbedderPolicy { get; set; }

                /// <summary>
                /// Opener Policy.
                /// </summary>
                public virtual CrossOriginOpenerPolicy? OpenerPolicy { get; set; }

                /// <summary>
                /// Resource Policy.
                /// </summary>
                public virtual CrossOriginResourcePolicy? ResourcePolicy { get; set; }
            }
        }

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
            public virtual CspDirective Defaults { get; set; } = new();

            /// <summary>
            /// Styles.
            /// </summary>
            public virtual CspDirectiveStyles Styles { get; set; } = new();

            /// <summary>
            /// Scripts.
            /// </summary>
            public virtual CspDirectiveScripts Scripts { get; set; } = new();

            /// <summary>
            /// Objects.
            /// </summary>
            public virtual CspDirective Objects { get; set; } = new();

            /// <summary>
            /// Images.
            /// </summary>
            public virtual CspDirective Images { get; set; } = new();

            /// <summary>
            /// Media.
            /// </summary>
            public virtual CspDirective Media { get; set; } = new();

            /// <summary>
            /// Frames.
            /// </summary>
            public virtual CspDirective Frames { get; set; } = new();

            /// <summary>
            /// Frame Ancestors.
            /// </summary>
            public virtual CspDirective FrameAncestors { get; set; } = new();

            /// <summary>
            /// Fonts.
            /// </summary>
            public virtual CspDirective Fonts { get; set; } = new();

            /// <summary>
            /// Connections.
            /// </summary>
            public virtual CspDirective Connections { get; set; } = new();

            /// <summary>
            /// Base Uri's.
            /// </summary>
            public virtual CspDirective BaseUris { get; set; } = new();

            /// <summary>
            /// Children.
            /// </summary>
            public virtual CspDirective Children { get; set; } = new();

            /// <summary>
            /// Forms.
            /// </summary>
            public virtual CspDirective Forms { get; set; } = new();

            /// <summary>
            /// Manifests.
            /// </summary>
            public virtual CspDirective Manifests { get; set; } = new();

            /// <summary>
            /// Workers.
            /// </summary>
            public virtual CspDirective Workers { get; set; } = new();

            /// <summary>
            /// Sandbox.
            /// </summary>
            public virtual CspDirectiveSandbox Sandbox { get; set; } = new();

            /// <summary>
            /// Permissions Policy.
            /// </summary>
            public virtual CspDirectivePermissionsPolicy PermissionsPolicy { get; set; } = new();

            /// <summary>
            /// Report Uris.
            /// </summary>
            public virtual string[] ReportUris { get; set; } = Array.Empty<string>();

            /// <summary>
            /// Plugin Types.
            /// </summary>
            public virtual string[] PluginTypes { get; set; } = Array.Empty<string>();

            /// <summary>
            /// Is Enabled.
            /// </summary>
            internal virtual bool IsEnabled => this.BlockAllMixedContent ||
                                               this.UpgradeInsecureRequests ||
                                               this.Defaults.IsEnabled ||
                                               this.Styles.IsEnabled ||
                                               this.Scripts.IsEnabled ||
                                               this.Objects.IsEnabled ||
                                               this.Images.IsEnabled ||
                                               this.Media.IsEnabled ||
                                               this.Frames.IsEnabled ||
                                               this.FrameAncestors.IsEnabled ||
                                               this.Fonts.IsEnabled ||
                                               this.Connections.IsEnabled ||
                                               this.BaseUris.IsEnabled ||
                                               this.Children.IsEnabled ||
                                               this.Forms.IsEnabled ||
                                               this.Manifests.IsEnabled ||
                                               this.Workers.IsEnabled ||
                                               this.Sandbox.IsEnabled ||
                                               this.PermissionsPolicy.IsEnabled;

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
                public virtual string[] Sources { get; set; } = Array.Empty<string>();

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
                internal override bool IsEnabled => base.IsEnabled ||
                                                    this.IsUnsafeEval ||
                                                    this.IsUnsafeInline ||
                                                    this.StrictDynamic;
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
                internal virtual bool IsEnabled => this.AllowForms ||
                                                   this.AllowModals ||
                                                   this.AllowOrientationLock ||
                                                   this.AllowPointerLock ||
                                                   this.AllowPopups ||
                                                   this.AllowPopupsToEscapeSandbox ||
                                                   this.AllowPresentation ||
                                                   this.AllowSameOrigin ||
                                                   this.AllowScripts ||
                                                   this.AllowTopNavigation;
            }

            /// <summary>
            /// Csp Directive Permissions Policy.
            /// </summary>
            public class CspDirectivePermissionsPolicy
            {
                /// <summary>
                /// Accelerometer.
                /// Controls whether the current document is allowed to gather information about the acceleration
                /// of the device through the Accelerometer interface.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyAccelerometer Accelerometer { get; set; } = new();

                /// <summary>
                /// Ambient Light Sensor.
                /// Controls whether the current document is allowed to gather information about the amount of light
                /// in the environment around the device through the AmbientLightSensor interface.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyAmbientLightSensor AmbientLightSensor { get; set; } = new();

                /// <summary>
                /// Auto Play.
                /// Controls whether the current document is allowed to autoplay media requested through the HTMLMediaElement interface.
                /// When this policy is disabled and there were no user gestures, the Promise returned by HTMLMediaElement.play() will reject with a DOMException.
                /// The autoplay attribute on audio and video elements will be ignored.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyAutoPlay AutoPlay { get; set; } = new();

                /// <summary>
                /// Battery.
                /// Controls whether the use of the Battery Status API is allowed. When this policy is disabled,
                /// the Promise returned by Navigator.getBattery() will reject with a NotAllowedError DOMException.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyBattery Battery { get; set; } = new();

                /// <summary>
                /// Camera.
                /// Controls whether the current document is allowed to use video input devices.
                /// When this policy is disabled, the Promise returned by getUserMedia() will reject with a NotAllowedError DOMException.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyCamera Camera { get; set; } = new();

                /// <summary>
                /// Display Capture.
                /// Controls whether the current document is permitted to use the getDisplayMedia() method to capture screen contents.
                /// When this policy is disabled, the promise returned by getDisplayMedia() will reject with a NotAllowedError
                /// if permission is not obtained to capture the display's contents.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyDisplayCapture DisplayCapture { get; set; } = new();

                /// <summary>
                /// Document Domain.
                /// Controls whether the current document is allowed to set document.domain.
                /// When this policy is disabled, attempting to set document.domain will fail and cause a SecurityError DOMException to be thrown.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyDocumentDomain DocumentDomain { get; set; } = new();

                /// <summary>
                /// Encrypted Media.
                /// Controls whether the current document is allowed to use the Encrypted Media Extensions API (EME).
                /// When this policy is disabled, the Promise returned by Navigator.requestMediaKeySystemAccess() will reject with a DOMException.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyEncryptedMedia EncryptedMedia { get; set; } = new();

                /// <summary>
                /// Execution While Not Rendered.
                /// Controls whether tasks should execute in frames while they're not being rendered (e.g. if an iframe is hidden or display: none).
                /// </summary>
                public virtual CspDirectivePermissionsPolicyExecutionWhileNotRendered ExecutionWhileNotRendered { get; set; } = new();

                /// <summary>
                /// Execution While Out Of Viewport.
                /// Controls whether tasks should execute in frames while they're outside the visible viewport.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyExecutionWhileOutOfViewport ExecutionWhileOutOfViewport { get; set; } = new();

                /// <summary>
                /// FullScreen.
                /// Controls whether the current document is allowed to use Element.requestFullScreen().
                /// When this policy is disabled, the returned Promise rejects with a TypeError.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyFullscreen FullScreen { get; set; } = new();

                /// <summary>
                /// Gamepad.
                /// Controls whether the current document is allowed to use the Gamepad API.
                /// When this policy is disabled, calls to Navigator.getGamepads() will throw a SecurityError DOMException,
                /// and the gamepadconnected and gamepaddisconnected events will not fire.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyGamepad Gamepad { get; set; } = new();

                /// <summary>
                /// Geo location.
                /// Controls whether the current document is allowed to use the Geolocation Interface.
                /// When this policy is disabled, calls to getCurrentPosition() and watchPosition() will cause those functions' callbacks to be invoked
                /// with a GeolocationPositionError code of PERMISSION_DENIED.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyGeolocation Geolocation { get; set; } = new();

                /// <summary>
                /// Gyroscope.
                /// Controls whether the current document is allowed to gather information about the orientation
                /// of the device through the Gyroscope interface.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyGyroscope Gyroscope { get; set; } = new();

                /// <summary>
                /// Layout Animations.
                /// Controls whether the current document is allowed to show layout animations.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyLayoutAnimations LayoutAnimations { get; set; } = new();

                /// <summary>
                /// Legacy Image Formats.
                /// Controls whether the current document is allowed to display images in legacy formats.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyLegacyImageFormats LegacyImageFormats { get; set; } = new();

                /// <summary>
                /// Magnetometer.
                /// Controls whether the current document is allowed to gather information about the orientation of the device through the Magnetometer interface.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyMagnetometer Magnetometer { get; set; } = new();

                /// <summary>
                /// Microphone.
                /// Controls whether the current document is allowed to use audio input devices.
                /// When this policy is disabled, the Promise returned by MediaDevices.getUserMedia() will reject with a NotAllowedError.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyMicrophone Microphone { get; set; } = new();

                /// <summary>
                /// Midi.
                /// Controls whether the current document is allowed to use the Web MIDI API.
                /// When this policy is disabled, the Promise returned by Navigator.requestMIDIAccess() will reject with a DOMException.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyMidi Midi { get; set; } = new();

                /// <summary>
                /// Navigation Override.
                /// Controls the availability of mechanisms that enables the page author to take control over
                /// the behavior of spatial navigation, or to cancel it outright.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyNavigationOverride NavigationOverride { get; set; } = new();

                /// <summary>
                /// Oversized Images.
                /// Controls whether the current document is allowed to download and display large images.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyOversizedImages OversizedImages { get; set; } = new();

                /// <summary>
                /// Payment.
                /// Controls whether the current document is allowed to use the Payment Request API.
                /// When this policy is enabled, the PaymentRequest() constructor will throw a SecurityError DOMException.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyPayment Payment { get; set; } = new();

                /// <summary>
                /// Picture In Picture.
                /// Controls whether the current document is allowed to play a video in a Picture-in-Picture mode via the corresponding API.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyPictureInPicture PictureInPicture { get; set; } = new();

                /// <summary>
                /// Public Key Credentials Get.
                /// Controls whether the current document is allowed to use the Web Authentication API to retrieve already stored public-key credentials,
                /// i.e. via navigator.credentials.get({publicKey: ..., ...}).
                /// </summary>
                public virtual CspDirectivePermissionsPolicyPublicKeyCredentialsGet PublicKeyCredentialsGet { get; set; } = new();

                /// <summary>
                /// Speaker Selection.
                /// Controls whether the current document is allowed to use the Audio Output Devices API to list and select speakers.
                /// </summary>
                public virtual CspDirectivePermissionsPolicySpeakerSelection SpeakerSelection { get; set; } = new();

                /// <summary>
                /// Sync Xhr.
                /// Controls whether the current document is allowed to make synchronous XMLHttpRequest requests.
                /// </summary>
                public virtual CspDirectivePermissionsPolicySyncXhr SyncXhr { get; set; } = new();

                /// <summary>
                /// Unoptimized Images.
                /// Controls whether the current document is allowed to download and display unoptimized images.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyUnoptimizedImages UnoptimizedImages { get; set; } = new();

                /// <summary>
                /// Unsized Media.
                /// Controls whether the current document is allowed to change the size of media elements after the initial layout is complete.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyUnsizedMedia UnsizedMedia { get; set; } = new();

                /// <summary>
                /// Usb.
                /// Controls whether the current document is allowed to use the WebUSB API
                /// </summary>
                public virtual CspDirectivePermissionsPolicyUsb Usb { get; set; } = new();

                /// <summary>
                /// Screen Wake Lock.
                /// Controls whether the current document is allowed to use Screen Wake Lock API
                /// to indicate that device should not turn off or dim the screen.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyScreenWakeLock ScreenWakeLock { get; set; } = new();

                /// <summary>
                /// Web Share.
                /// Controls whether the current document is allowed to use the Navigator.share() of Web Share API to
                /// share text, links, images, and other content to arbitrary destinations of user's choice, e.g. mobile apps.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyWebShare WebShare { get; set; } = new();

                /// <summary>
                /// Xr Spatial Tracking.
                /// Controls whether the current document is allowed to use the WebXR Device API to interact with a WebXR session.
                /// </summary>
                public virtual CspDirectivePermissionsPolicyXrSpatialTracking XrSpatialTracking { get; set; } = new();

                /// <summary>
                /// Is Enabled.
                /// </summary>
                internal virtual bool IsEnabled => this.Accelerometer.IsEnabled ||
                                                   this.AmbientLightSensor.IsEnabled ||
                                                   this.AutoPlay.IsEnabled ||
                                                   this.Battery.IsEnabled ||
                                                   this.Camera.IsEnabled ||
                                                   this.DisplayCapture.IsEnabled ||
                                                   this.DocumentDomain.IsEnabled ||
                                                   this.EncryptedMedia.IsEnabled ||
                                                   this.ExecutionWhileNotRendered.IsEnabled ||
                                                   this.ExecutionWhileOutOfViewport.IsEnabled ||
                                                   this.FullScreen.IsEnabled ||
                                                   this.Gamepad.IsEnabled ||
                                                   this.Geolocation.IsEnabled ||
                                                   this.Gyroscope.IsEnabled ||
                                                   this.LayoutAnimations.IsEnabled ||
                                                   this.LegacyImageFormats.IsEnabled ||
                                                   this.Magnetometer.IsEnabled ||
                                                   this.Microphone.IsEnabled ||
                                                   this.Midi.IsEnabled ||
                                                   this.NavigationOverride.IsEnabled ||
                                                   this.OversizedImages.IsEnabled ||
                                                   this.Payment.IsEnabled ||
                                                   this.PictureInPicture.IsEnabled ||
                                                   this.PublicKeyCredentialsGet.IsEnabled ||
                                                   this.SpeakerSelection.IsEnabled ||
                                                   this.SyncXhr.IsEnabled ||
                                                   this.UnoptimizedImages.IsEnabled ||
                                                   this.UnsizedMedia.IsEnabled ||
                                                   this.Usb.IsEnabled ||
                                                   this.ScreenWakeLock.IsEnabled ||
                                                   this.WebShare.IsEnabled ||
                                                   this.XrSpatialTracking.IsEnabled;

                /// <summary>
                /// Csp Directive Permissions Policy Accelerometer.
                /// </summary>
                public class CspDirectivePermissionsPolicyAccelerometer : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Ambient Light Sensor
                /// </summary>
                public class CspDirectivePermissionsPolicyAmbientLightSensor : CspDirective;

                /// <summary>
                /// Csp Directive Permissions PolicyAuto Play.
                /// </summary>
                public class CspDirectivePermissionsPolicyAutoPlay : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Battery.
                /// </summary>
                public class CspDirectivePermissionsPolicyBattery : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Camera
                /// </summary>
                public class CspDirectivePermissionsPolicyCamera : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Display Capture.
                /// </summary>
                public class CspDirectivePermissionsPolicyDisplayCapture : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Document Domain.
                /// </summary>
                public class CspDirectivePermissionsPolicyDocumentDomain : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Encrypted Media.
                /// </summary>
                public class CspDirectivePermissionsPolicyEncryptedMedia : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Execution While Not Rendered.
                /// </summary>
                public class CspDirectivePermissionsPolicyExecutionWhileNotRendered : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Execution While Out Of Viewport.
                /// </summary>
                public class CspDirectivePermissionsPolicyExecutionWhileOutOfViewport : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Fullscreen.
                /// </summary>
                public class CspDirectivePermissionsPolicyFullscreen : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Gamepad.
                /// </summary>
                public class CspDirectivePermissionsPolicyGamepad : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Geo location.
                /// </summary>
                public class CspDirectivePermissionsPolicyGeolocation : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Gyroscope.
                /// </summary>
                public class CspDirectivePermissionsPolicyGyroscope : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Layout Animations.
                /// </summary>
                public class CspDirectivePermissionsPolicyLayoutAnimations : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Legacy Image Formats.
                /// </summary>
                public class CspDirectivePermissionsPolicyLegacyImageFormats : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Magnetometer.
                /// </summary>
                public class CspDirectivePermissionsPolicyMagnetometer : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Microphone.
                /// </summary>
                public class CspDirectivePermissionsPolicyMicrophone : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Midi.
                /// </summary>
                public class CspDirectivePermissionsPolicyMidi : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Navigation Override.
                /// </summary>
                public class CspDirectivePermissionsPolicyNavigationOverride : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Oversized Images.
                /// </summary>
                public class CspDirectivePermissionsPolicyOversizedImages : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Payment.
                /// </summary>
                public class CspDirectivePermissionsPolicyPayment : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Picture-In-Picture.
                /// </summary>
                public class CspDirectivePermissionsPolicyPictureInPicture : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Public Key Credentials Get.
                /// </summary>
                public class CspDirectivePermissionsPolicyPublicKeyCredentialsGet : CspDirective;

                /// <summary>
                /// Csp Directive Permissions PolicySpeakerSelection.
                /// </summary>
                public class CspDirectivePermissionsPolicySpeakerSelection : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Sync Xhr.
                /// </summary>
                public class CspDirectivePermissionsPolicySyncXhr : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Unoptimized Images.
                /// </summary>
                public class CspDirectivePermissionsPolicyUnoptimizedImages : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Unsized Media.
                /// </summary>
                public class CspDirectivePermissionsPolicyUnsizedMedia : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Usb.
                /// </summary>
                public class CspDirectivePermissionsPolicyUsb : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Screen Wake Lock.
                /// </summary>
                public class CspDirectivePermissionsPolicyScreenWakeLock : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Web Share.
                /// </summary>
                public class CspDirectivePermissionsPolicyWebShare : CspDirective;

                /// <summary>
                /// Csp Directive Permissions Policy Xr Spatial Tracking.
                /// </summary>
                public class CspDirectivePermissionsPolicyXrSpatialTracking : CspDirective;
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
            /// Maximum History Entries Per Endpoint.
            /// The maximum number of historical entries per endpoint in the UI database.
            /// </summary>
            public virtual int MaximumHistoryEntriesPerEndpoint { get; set; } = 50;

            /// <summary>
            /// Web-Hooks.
            /// </summary>
            public virtual HealthCheckWebHookOptions[] WebHooks { get; set; } = Array.Empty<HealthCheckWebHookOptions>();

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
        /// Is Enabled.
        /// </summary>
        public virtual bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Csp Nonce.
        /// </summary>
        public virtual string CspNonce { get; set; }

        /// <summary>
        /// Use Default Version.
        /// </summary>
        public virtual bool UseDefaultVersion { get; set; } = true;
        
        /// <summary>
        /// Contact.
        /// </summary>
        public virtual OpenApiContact Contact { get; set; } = new();

        /// <summary>
        /// License.
        /// </summary>
        public virtual OpenApiLicense License { get; set; } = new();
    }
}