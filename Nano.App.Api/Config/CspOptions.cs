using System.ComponentModel.DataAnnotations;

namespace Nano.App.Api.Config;

/// <summary>
/// Options for configuring Content Security Policy (CSP) headers.
/// </summary>
public class CspOptions
{
    /// <summary>
    /// If true, the CSP will be enforced in report-only mode.
    /// Violations will be reported but not blocked.
    /// </summary>
    [Required]
    public virtual bool ReportOnly { get; set; } = false;

    /// <summary>
    /// If true, instructs the browser to upgrade all HTTP requests to HTTPS automatically.
    /// </summary>
    [Required]
    public virtual bool UpgradeInsecureRequests { get; set; } = false;

    /// <summary>
    /// CSP Report-To header configuration.
    /// Specifies where CSP violation reports are sent.
    /// </summary>
    public virtual CspReportToOptions? ReportTo { get; set; }

    /// <summary>
    /// Default directive (default-src) for all unspecified content types.
    /// </summary>
    public virtual CspDirectiveOptions? Defaults { get; set; }

    /// <summary>
    /// Controls allowed sources for scripts (script-src).
    /// </summary>
    public virtual CspDirectiveScriptsOptions? Scripts { get; set; }

    /// <summary>
    /// Controls allowed sources for script elements (script-src-elem).
    /// </summary>
    public virtual CspDirectiveScriptsElemOptions? ScriptsElem { get; set; }

    /// <summary>
    /// Controls allowed sources for inline script attributes (script-src-attr).
    /// </summary>
    public virtual CspDirectiveScriptsAttrOptions? ScriptsAttr { get; set; }

    /// <summary>
    /// Controls allowed sources for stylesheets (style-src).
    /// </summary>
    public virtual CspDirectiveStylesOptions? Styles { get; set; }

    /// <summary>
    /// Controls allowed sources for style elements (style-src-elem).
    /// </summary>
    public virtual CspDirectiveStylesElemOptions? StylesElem { get; set; }

    /// <summary>
    /// Controls allowed sources for inline style attributes (style-src-attr).
    /// </summary>
    public virtual CspDirectiveStylesAttrOptions? StylesAttr { get; set; }

    /// <summary>
    /// Controls allowed sources for object elements (object-src).
    /// </summary>
    public virtual CspDirectiveOptions? Objects { get; set; }

    /// <summary>
    /// Controls allowed sources for images (img-src).
    /// </summary>
    public virtual CspDirectiveOptions? Images { get; set; }

    /// <summary>
    /// Controls allowed sources for audio and video elements (media-src).
    /// </summary>
    public virtual CspDirectiveOptions? Media { get; set; }

    /// <summary>
    /// Controls allowed sources for frames and iframes (frame-src).
    /// </summary>
    public virtual CspDirectiveOptions? Frames { get; set; }

    /// <summary>
    /// Controls allowed sources for fenced frames (fenced-frame-src).
    /// </summary>
    public virtual CspDirectiveOptions? FencedFrames { get; set; }

    /// <summary>
    /// Controls which sources can embed this document (frame-ancestors).
    /// </summary>
    public virtual CspDirectiveOptions? FrameAncestors { get; set; }

    /// <summary>
    /// Controls allowed sources for fonts (font-src).
    /// </summary>
    public virtual CspDirectiveOptions? Fonts { get; set; }

    /// <summary>
    /// Controls allowed URLs for fetch, XHR, WebSocket, EventSource (connect-src).
    /// </summary>
    public virtual CspDirectiveOptions? Connections { get; set; }

    /// <summary>
    /// Controls allowed base URLs for the document (base-uri).
    /// </summary>
    public virtual CspDirectiveOptions? BaseUris { get; set; }

    /// <summary>
    /// Controls allowed sources for nested browsing contexts (child-src).
    /// </summary>
    public virtual CspDirectiveOptions? Children { get; set; }

    /// <summary>
    /// Controls allowed URLs for form submissions (form-action).
    /// </summary>
    public virtual CspDirectiveOptions? Forms { get; set; }

    /// <summary>
    /// Controls allowed sources for web app manifests (manifest-src).
    /// </summary>
    public virtual CspDirectiveOptions? Manifests { get; set; }

    /// <summary>
    /// Controls allowed sources for web workers, service workers, and shared workers (worker-src).
    /// </summary>
    public virtual CspDirectiveOptions? Workers { get; set; }

    /// <summary>
    /// Restricts which Trusted Types policies are allowed to create DOM objects.
    /// </summary>
    public virtual CspDirectiveTrustedTypesOptions? TrustedTypes { get; set; }

    /// <summary>
    /// Restricts features of a page when embedded in an iframe (sandbox).
    /// </summary>
    public virtual CspDirectiveSandboxOptions? Sandbox { get; set; }

    /// <summary>
    /// Controls access to powerful browser features (permissions-policy).
    /// </summary>
    public virtual CspDirectivePermissionsPolicyOptions? PermissionsPolicy { get; set; }

    #region Nested Classes

    /// <summary>
    /// Represents a generic CSP directive (e.g., default-src, img-src, connect-src).
    /// </summary>
    public class CspDirectiveOptions
    {
        /// <summary>
        /// If true, only 'none' is allowed. All other sources are ignored.
        /// </summary>
        [Required]
        public virtual bool IsNone { get; set; } = false;

        /// <summary>
        /// If true, 'self' is allowed as a source.
        /// </summary>
        [Required]
        public virtual bool IsSelf { get; set; } = false;

        /// <summary>
        /// Custom sources for the directive.
        /// </summary>
        [Required]
        public virtual string[] Sources { get; set; } = [];
    }

    /// <summary>
    /// Represents script-specific CSP directive (script-src).
    /// </summary>
    public class CspDirectiveScriptsOptions : CspDirectiveOptions
    {
        /// <summary>
        /// Allows inline scripts ('unsafe-inline').
        /// </summary>
        [Required]
        public virtual bool IsUnsafeInline { get; set; } = false;

        /// <summary>
        /// Allows eval() and similar constructs ('unsafe-eval').
        /// </summary>
        [Required]
        public virtual bool IsUnsafeEval { get; set; } = false;

        /// <summary>
        /// Allows WebAssembly unsafe evaluation ('wasm-unsafe-eval').
        /// </summary>
        [Required]
        public virtual bool IsUnsafeWasmEval { get; set; } = false;

        /// <summary>
        /// Enables 'strict-dynamic' behavior for script execution.
        /// </summary>
        [Required]
        public virtual bool StrictDynamic { get; set; } = false;

        /// <summary>
        /// Allows unsafe hashes for inline scripts.
        /// </summary>
        [Required]
        public virtual bool IsUnsafeHashes { get; set; } = false;

        /// <summary>
        /// Allows unsafe hashed attributes ('unsafe-hashed-attributes').
        /// </summary>
        [Required]
        public virtual bool UnsafeHashedAttributes { get; set; } = false;

        /// <summary>
        /// Allows redirects from unsafe sources ('unsafe-allow-redirects').
        /// </summary>
        [Required]
        public virtual bool UnsafeAllowRedirects { get; set; } = false;

        /// <summary>
        /// Specific nonces to allow inline scripts.
        /// </summary>
        [Required]
        public virtual string[] Nonces { get; set; } = [];

        /// <summary>
        /// SHA hashes to allow inline script content.
        /// Must be prefixed with 'sha256-', 'sha384-', or 'sha512-'.
        /// </summary>
        [Required]
        public virtual string[] Hashes { get; set; } = [];

        /// <summary>
        /// Requires Subresource Integrity (SRI) for scripts.
        /// </summary>
        [Required]
        public virtual bool RequireSri { get; set; } = false;

        /// <summary>
        /// Requires Trusted Types for script execution.
        /// </summary>
        [Required]
        public virtual bool RequireTrustedTypes { get; set; } = false;

        /// <summary>
        /// If true, enables 'report-sample' in CSP violation reports.
        /// </summary>
        [Required]
        public virtual bool ReportSample { get; set; } = false;
    }

    /// <summary>
    /// CSP directive for script element-specific sources (script-src-elem).
    /// </summary>
    public class CspDirectiveScriptsElemOptions : CspDirectiveOptions
    {
        /// <summary>
        /// Allows inline content (attributes or inline styles/scripts).
        /// </summary>
        [Required]
        public virtual bool IsUnsafeInline { get; set; } = false;

        /// <summary>
        /// If true, enables 'report-sample' in CSP violation reports.
        /// </summary>
        [Required]
        public virtual bool ReportSample { get; set; } = false;
    }

    /// <summary>
    /// Represents script-specific CSP directive (script-src-attr).
    /// </summary>
    public class CspDirectiveScriptsAttrOptions
    {
        /// <summary>
        /// If true, only 'none' is allowed. All other sources are ignored.
        /// </summary>
        [Required]
        public virtual bool IsNone { get; set; } = false;

        /// <summary>
        /// Allows inline styles ('unsafe-inline').
        /// </summary>
        [Required]
        public virtual bool IsUnsafeInline { get; set; } = false;

        /// <summary>
        /// Allows unsafe hashes for inline scripts.
        /// </summary>
        [Required]
        public virtual bool IsUnsafeHashes { get; set; } = false;

        /// <summary>
        /// If true, enables 'report-sample' in CSP violation reports.
        /// </summary>
        [Required]
        public virtual bool ReportSample { get; set; } = false;
    }

    /// <summary>
    /// Represents style-specific CSP directive (style-src).
    /// </summary>
    public class CspDirectiveStylesOptions : CspDirectiveOptions
    {
        /// <summary>
        /// Allows inline styles ('unsafe-inline').
        /// </summary>
        [Required]
        public virtual bool IsUnsafeInline { get; set; } = false;

        /// <summary>
        /// Allows unsafe hashes for inline styles.
        /// </summary>
        [Required]
        public virtual bool IsUnsafeHashes { get; set; } = false;

        /// <summary>
        /// Nonces to allow specific inline styles.
        /// </summary>
        [Required]
        public virtual string[] Nonces { get; set; } = [];

        /// <summary>
        /// SHA hashes to allow specific inline styles.
        /// Must be prefixed with 'sha256-', 'sha384-', or 'sha512-'.
        /// </summary>
        [Required]
        public virtual string[] Hashes { get; set; } = [];

        /// <summary>
        /// Requires Subresource Integrity (SRI) for styles.
        /// </summary>
        [Required]
        public virtual bool RequireSri { get; set; } = false;

        /// <summary>
        /// If true, enables 'report-sample' in CSP violation reports.
        /// </summary>
        [Required]
        public virtual bool ReportSample { get; set; } = false;
    }

    /// <summary>
    /// Represents style-specific CSP directive (src-style-attr).
    /// </summary>
    public class CspDirectiveStylesAttrOptions
    {
        /// <summary>
        /// If true, only 'none' is allowed. All other sources are ignored.
        /// </summary>
        [Required]
        public virtual bool IsNone { get; set; } = false;

        /// <summary>
        /// Allows inline styles ('unsafe-inline').
        /// </summary>
        [Required]
        public virtual bool IsUnsafeInline { get; set; } = false;

        /// <summary>
        /// Allows unsafe hashes for inline styles.
        /// </summary>
        [Required]
        public virtual bool IsUnsafeHashes { get; set; } = false;

        /// <summary>
        /// If true, enables 'report-sample' in CSP violation reports.
        /// </summary>
        [Required]
        public virtual bool ReportSample { get; set; } = false;
    }

    /// <summary>
    /// CSP directive for style element-specific sources (style-src-elem).
    /// </summary>
    public class CspDirectiveStylesElemOptions : CspDirectiveOptions
    {
        /// <summary>
        /// Allows inline content (attributes or inline styles/scripts).
        /// </summary>
        [Required]
        public virtual bool IsUnsafeInline { get; set; } = false;

        /// <summary>
        /// Allows unsafe hashes for inline styles.
        /// </summary>
        [Required]
        public virtual bool IsUnsafeHashes { get; set; } = false;

        /// <summary>
        /// Nonces to allow inline element content.
        /// </summary>
        [Required]
        public virtual string[] Nonces { get; set; } = [];

        /// <summary>
        /// Precomputed hashes (sha256/384/512) for inline content.
        /// </summary>
        [Required]
        public virtual string[] Hashes { get; set; } = [];

        /// <summary>
        /// If true, enables 'report-sample' in CSP violation reports.
        /// </summary>
        [Required]
        public virtual bool ReportSample { get; set; } = false;
    }

    /// <summary>
    /// Controls which Trusted Types policies are allowed.
    /// </summary>
    public class CspDirectiveTrustedTypesOptions
    {
        /// <summary>
        /// If true, only 'none' is allowed. All other policies are ignored.
        /// </summary>
        [Required]
        public virtual bool IsNone { get; set; } = false;

        /// <summary>
        /// If true, duplicate policy names are allowed.
        /// </summary>
        [Required]
        public virtual bool AllowDuplicates { get; set; } = false;

        /// <summary>
        /// List of allowed Trusted Types policy names.
        /// </summary>
        [Required]
        public virtual string[] Policies { get; set; } = [];
    }

    /// <summary>
    /// CSP Report-To configuration for sending violation reports.
    /// Configuring this will also set the 'Report-To' and the 'Reporting-Endpoints' http header.
    /// </summary>
    public class CspReportToOptions
    {
        /// <summary>
        /// Reporting group name referenced by CSP.
        /// </summary>
        [Required]
        public virtual string Group { get; set; } = "csp-reports";

        /// <summary>
        /// Max age (seconds) for the report group.
        /// </summary>
        [Required]
        public virtual int MaxAge { get; set; } = 10886400;

        /// <summary>
        /// URLs to receive CSP reports.
        /// If no endpoints is specified, report-to will default to the built in endpoint: <c>/csp/report-to</c> in Nano.
        /// </summary>
        [Required]
        public virtual string[] Endpoints { get; set; } = [];
    }

    /// <summary>
    /// Sandbox directive configuration.
    /// Restricts iframe capabilities.
    /// </summary>
    public class CspDirectiveSandboxOptions
    {
        /// <summary>
        /// Allows downloads in the sandbox.
        /// </summary>
        public virtual bool AllowDownloads { get; set; } = false;

        /// <summary>
        /// Allows form submissions from the sandboxed page.
        /// </summary>
        [Required]
        public virtual bool AllowForms { get; set; } = false;

        /// <summary>
        /// Allows opening modal windows.
        /// </summary>
        [Required]
        public virtual bool AllowModals { get; set; } = false;

        /// <summary>
        /// Allows disabling screen orientation lock.
        /// </summary>
        [Required]
        public virtual bool AllowOrientationLock { get; set; } = false;

        /// <summary>
        /// Allows usage of Pointer Lock API.
        /// </summary>
        [Required]
        public virtual bool AllowPointerLock { get; set; } = false;

        /// <summary>
        /// Allows popups (window.open, target=_blank).
        /// </summary>
        [Required]
        public virtual bool AllowPopups { get; set; } = false;

        /// <summary>
        /// Allows popups to escape sandbox restrictions.
        /// </summary>
        [Required]
        public virtual bool AllowPopupsToEscapeSandbox { get; set; } = false;

        /// <summary>
        /// Allows initiating presentations from the sandboxed page.
        /// </summary>
        [Required]
        public virtual bool AllowPresentation { get; set; } = false;

        /// <summary>
        /// Allows same-origin access from sandboxed content.
        /// </summary>
        [Required]
        public virtual bool AllowSameOrigin { get; set; } = false;

        /// <summary>
        /// Allows execution of scripts.
        /// </summary>
        [Required]
        public virtual bool AllowScripts { get; set; } = false;

        /// <summary>
        /// Allows storage access via user activation.
        /// </summary>
        [Required]
        public virtual bool AllowStorageAccessByUserActivation { get; set; } = false;

        /// <summary>
        /// Allows top-level navigation.
        /// </summary>
        [Required]
        public virtual bool AllowTopNavigation { get; set; } = false;

        /// <summary>
        /// Allows top-level navigation via user activation.
        /// </summary>
        [Required]
        public virtual bool AllowTopNavigationByUserActivation { get; set; } = false;

        /// <summary>
        /// Allows navigation to custom protocols.
        /// </summary>
        [Required]
        public virtual bool AllowTopNavigationToCustomProtocols { get; set; } = false;
    }

    /// <summary>
    /// Csp Directive Permissions Policy.
    /// </summary>
    public class CspDirectivePermissionsPolicyOptions
    {
        /// <summary>
        /// Accelerometer.
        /// Controls whether the current document is allowed to gather information about the acceleration
        /// of the device through the Accelerometer interface.
        /// </summary>
        public virtual CspDirectiveOptions? Accelerometer { get; set; }

        /// <summary>
        /// Ambient Light Sensor.
        /// Controls whether the current document is allowed to gather information about the amount of light
        /// in the environment around the device through the AmbientLightSensor interface.
        /// </summary>
        public virtual CspDirectiveOptions? AmbientLightSensor { get; set; }

        /// <summary>
        /// Auto Play.
        /// Controls whether the current document is allowed to autoplay media requested through the HTMLMediaElement interface.
        /// When this policy is disabled and there were no user gestures, the Promise returned by HTMLMediaElement.play() will reject with a DOMException.
        /// The autoplay attribute on audio and video elements will be ignored.
        /// </summary>
        public virtual CspDirectiveOptions? AutoPlay { get; set; }

        /// <summary>
        /// Battery.
        /// Controls whether the use of the Battery Status API is allowed. When this policy is disabled,
        /// the Promise returned by Navigator.getBattery() will reject with a NotAllowedError DOMException.
        /// </summary>
        public virtual CspDirectiveOptions? Battery { get; set; }

        /// <summary>
        /// Camera.
        /// Controls whether the current document is allowed to use video input devices.
        /// When this policy is disabled, the Promise returned by getUserMedia() will reject with a NotAllowedError DOMException.
        /// </summary>
        public virtual CspDirectiveOptions? Camera { get; set; }

        /// <summary>
        /// Display Capture.
        /// Controls whether the current document is permitted to use the getDisplayMedia() method to capture screen contents.
        /// When this policy is disabled, the promise returned by getDisplayMedia() will reject with a NotAllowedError
        /// if permission is not obtained to capture the display's contents.
        /// </summary>
        public virtual CspDirectiveOptions? DisplayCapture { get; set; }

        /// <summary>
        /// Document Domain.
        /// Controls whether the current document is allowed to set document.domain.
        /// When this policy is disabled, attempting to set document.domain will fail and cause a SecurityError DOMException to be thrown.
        /// </summary>
        public virtual CspDirectiveOptions? DocumentDomain { get; set; }

        /// <summary>
        /// Encrypted Media.
        /// Controls whether the current document is allowed to use the Encrypted Media Extensions API (EME).
        /// When this policy is disabled, the Promise returned by Navigator.requestMediaKeySystemAccess() will reject with a DOMException.
        /// </summary>
        public virtual CspDirectiveOptions? EncryptedMedia { get; set; }

        /// <summary>
        /// Execution While Not Rendered.
        /// Controls whether tasks should execute in frames while they're not being rendered (e.g. if an iframe is hidden or display: none).
        /// </summary>
        public virtual CspDirectiveOptions? ExecutionWhileNotRendered { get; set; }

        /// <summary>
        /// Execution While Out Of Viewport.
        /// Controls whether tasks should execute in frames while they're outside the visible viewport.
        /// </summary>
        public virtual CspDirectiveOptions? ExecutionWhileOutOfViewport { get; set; }

        /// <summary>
        /// FullScreen.
        /// Controls whether the current document is allowed to use Element.requestFullScreen().
        /// When this policy is disabled, the returned Promise rejects with a TypeError.
        /// </summary>
        public virtual CspDirectiveOptions? FullScreen { get; set; }

        /// <summary>
        /// Gamepad.
        /// Controls whether the current document is allowed to use the Gamepad API.
        /// When this policy is disabled, calls to Navigator.getGamepads() will throw a SecurityError DOMException,
        /// and the gamepadconnected and gamepaddisconnected events will not fire.
        /// </summary>
        public virtual CspDirectiveOptions? Gamepad { get; set; }

        /// <summary>
        /// Geo location.
        /// Controls whether the current document is allowed to use the Geolocation Interface.
        /// When this policy is disabled, calls to getCurrentPosition() and watchPosition() will cause those functions' callbacks to be invoked
        /// with a GeolocationPositionError code of PERMISSION_DENIED.
        /// </summary>
        public virtual CspDirectiveOptions? Geolocation { get; set; }

        /// <summary>
        /// Gyroscope.
        /// Controls whether the current document is allowed to gather information about the orientation
        /// of the device through the Gyroscope interface.
        /// </summary>
        public virtual CspDirectiveOptions? Gyroscope { get; set; }

        /// <summary>
        /// Layout Animations.
        /// Controls whether the current document is allowed to show layout animations.
        /// </summary>
        public virtual CspDirectiveOptions? LayoutAnimations { get; set; }

        /// <summary>
        /// Legacy Image Formats.
        /// Controls whether the current document is allowed to display images in legacy formats.
        /// </summary>
        public virtual CspDirectiveOptions? LegacyImageFormats { get; set; }

        /// <summary>
        /// Magnetometer.
        /// Controls whether the current document is allowed to gather information about the orientation of the device through the Magnetometer interface.
        /// </summary>
        public virtual CspDirectiveOptions? Magnetometer { get; set; }

        /// <summary>
        /// Microphone.
        /// Controls whether the current document is allowed to use audio input devices.
        /// When this policy is disabled, the Promise returned by MediaDevices.getUserMedia() will reject with a NotAllowedError.
        /// </summary>
        public virtual CspDirectiveOptions? Microphone { get; set; }

        /// <summary>
        /// Midi.
        /// Controls whether the current document is allowed to use the Web MIDI API.
        /// When this policy is disabled, the Promise returned by Navigator.requestMIDIAccess() will reject with a DOMException.
        /// </summary>
        public virtual CspDirectiveOptions? Midi { get; set; }

        /// <summary>
        /// Navigation Override.
        /// Controls the availability of mechanisms that enables the page author to take control over
        /// the behavior of spatial navigation, or to cancel it outright.
        /// </summary>
        public virtual CspDirectiveOptions? NavigationOverride { get; set; }

        /// <summary>
        /// Oversized Images.
        /// Controls whether the current document is allowed to download and display large images.
        /// </summary>
        public virtual CspDirectiveOptions? OversizedImages { get; set; }

        /// <summary>
        /// Payment.
        /// Controls whether the current document is allowed to use the Payment Request API.
        /// When this policy is enabled, the PaymentRequest() constructor will throw a SecurityError DOMException.
        /// </summary>
        public virtual CspDirectiveOptions? Payment { get; set; }

        /// <summary>
        /// Picture In Picture.
        /// Controls whether the current document is allowed to play a video in a Picture-in-Picture mode via the corresponding API.
        /// </summary>
        public virtual CspDirectiveOptions? PictureInPicture { get; set; }

        /// <summary>
        /// Public Key Credentials Get.
        /// Controls whether the current document is allowed to use the Web Authentication API to retrieve already stored public-key credentials,
        /// i.e. via navigator.credentials.get({publicKey: ..., ...}).
        /// </summary>
        public virtual CspDirectiveOptions? PublicKeyCredentialsGet { get; set; }

        /// <summary>
        /// Speaker Selection.
        /// Controls whether the current document is allowed to use the Audio Output Devices API to list and select speakers.
        /// </summary>
        public virtual CspDirectiveOptions? SpeakerSelection { get; set; }

        /// <summary>
        /// Sync Xhr.
        /// Controls whether the current document is allowed to make synchronous XMLHttpRequest requests.
        /// </summary>
        public virtual CspDirectiveOptions? SyncXhr { get; set; }

        /// <summary>
        /// Unoptimized Images.
        /// Controls whether the current document is allowed to download and display unoptimized images.
        /// </summary>
        public virtual CspDirectiveOptions? UnoptimizedImages { get; set; }

        /// <summary>
        /// Unsized Media.
        /// Controls whether the current document is allowed to change the size of media elements after the initial layout is complete.
        /// </summary>
        public virtual CspDirectiveOptions? UnsizedMedia { get; set; }

        /// <summary>
        /// Usb.
        /// Controls whether the current document is allowed to use the WebUSB API
        /// </summary>
        public virtual CspDirectiveOptions? Usb { get; set; }

        /// <summary>
        /// Screen Wake Lock.
        /// Controls whether the current document is allowed to use Screen Wake Lock API
        /// to indicate that device should not turn off or dim the screen.
        /// </summary>
        public virtual CspDirectiveOptions? ScreenWakeLock { get; set; }

        /// <summary>
        /// Web Share.
        /// Controls whether the current document is allowed to use the Navigator.share() of Web Share API to
        /// share text, links, images, and other content to arbitrary destinations of user's choice, e.g. mobile apps.
        /// </summary>
        public virtual CspDirectiveOptions? WebShare { get; set; }

        /// <summary>
        /// Xr Spatial Tracking.
        /// Controls whether the current document is allowed to use the WebXR Device API to interact with a WebXR session.
        /// </summary>
        public virtual CspDirectiveOptions? XrSpatialTracking { get; set; }
    }

    #endregion
}
