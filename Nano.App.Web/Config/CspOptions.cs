using System.ComponentModel.DataAnnotations;

namespace Nano.App.Web.Config;

/// <summary>
/// Csp Options.
/// </summary>
public class CspOptions
{
    /// <summary>
    /// Report Only.
    /// </summary>
    [Required]
    public virtual bool ReportOnly { get; set; } = false;

    /// <summary>
    /// Block All Mixed Content.
    /// </summary>
    [Required]
    public virtual bool BlockAllMixedContent { get; set; } = false;

    /// <summary>
    /// Upgrade Insecure Requests.
    /// </summary>
    [Required]
    public virtual bool UpgradeInsecureRequests { get; set; } = false;

    /// <summary>
    /// Trusted Types.
    /// </summary>
    public virtual CspDirectiveTrustedTypes TrustedTypes { get; set; }

    /// <summary>
    /// Defaults.
    /// </summary>
    public virtual CspDirective Defaults { get; set; }

    /// <summary>
    /// Styles.
    /// </summary>
    public virtual CspDirectiveStyles Styles { get; set; }

    /// <summary>
    /// Scripts.
    /// </summary>
    public virtual CspDirectiveScripts Scripts { get; set; }

    /// <summary>
    /// Objects.
    /// </summary>
    public virtual CspDirective Objects { get; set; }

    /// <summary>
    /// Images.
    /// </summary>
    public virtual CspDirective Images { get; set; }

    /// <summary>
    /// Media.
    /// </summary>
    public virtual CspDirective Media { get; set; }

    /// <summary>
    /// Frames.
    /// </summary>
    public virtual CspDirective Frames { get; set; }

    /// <summary>
    /// Frame Ancestors.
    /// </summary>
    public virtual CspDirective FrameAncestors { get; set; }

    /// <summary>
    /// Fonts.
    /// </summary>
    public virtual CspDirective Fonts { get; set; }

    /// <summary>
    /// Connections.
    /// </summary>
    public virtual CspDirective Connections { get; set; }

    /// <summary>
    /// Base Uri's.
    /// </summary>
    public virtual CspDirective BaseUris { get; set; }

    /// <summary>
    /// Children.
    /// </summary>
    public virtual CspDirective Children { get; set; }

    /// <summary>
    /// Forms.
    /// </summary>
    public virtual CspDirective Forms { get; set; }

    /// <summary>
    /// Manifests.
    /// </summary>
    public virtual CspDirective Manifests { get; set; }

    /// <summary>
    /// Workers.
    /// </summary>
    public virtual CspDirective Workers { get; set; }

    /// <summary>
    /// Sandbox.
    /// </summary>
    public virtual CspDirectiveSandbox Sandbox { get; set; }

    /// <summary>
    /// Permissions Policy.
    /// </summary>
    public virtual CspDirectivePermissionsPolicy PermissionsPolicy { get; set; }

    /// <summary>
    /// Report Uris.
    /// </summary>
    [Required]
    public virtual string[] ReportUris { get; set; } = [];


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
        [Required]
        public virtual bool IsNone { get; set; } = false;

        /// <summary>
        /// Is Self.
        /// Adds the 'self' source.
        /// </summary>
        [Required]
        public virtual bool IsSelf { get; set; } = false;

        /// <summary>
        /// Sources.
        /// Adds the array of custom sources.
        /// </summary>
        [Required]
        public virtual string[] Sources { get; set; } = [];
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
        [Required]
        public virtual bool IsUnsafeEval { get; set; } = false;

        /// <summary>
        /// Is None.
        /// Adds the 'unsafe-inline' source.
        /// </summary>
        [Required]
        public virtual bool IsUnsafeInline { get; set; } = false;

        /// <summary>
        /// Is None.
        /// Adds the 'strict-dynamic' source.
        /// </summary>
        [Required]
        public virtual bool StrictDynamic { get; set; } = false;
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
        [Required]
        public virtual bool IsUnsafeInline { get; set; } = false;
    }

    /// <summary>
    /// Csp Directive Trusted Types.
    /// </summary>
    public class CspDirectiveTrustedTypes;

    /// <summary>
    /// Csp Directive Sandbox.
    /// </summary>
    public class CspDirectiveSandbox
    {
        /// <summary>
        /// Allow Forms.
        /// Allows the page to submit forms. If this keyword is not used, this operation is not allowed.
        /// </summary>
        [Required]
        public virtual bool AllowForms { get; set; } = false;

        /// <summary>
        /// Allow Modals.
        /// Allows the page to open modal windows.
        /// </summary>
        [Required]
        public virtual bool AllowModals { get; set; } = false;

        /// <summary>
        /// Allow Orientation Lock.
        /// Allows the page to disable the ability to lock the screen orientation.
        /// </summary>
        [Required]
        public virtual bool AllowOrientationLock { get; set; } = false;

        /// <summary>
        /// Allow Pointer Lock.
        /// Allows the page to use the Pointer Lock API.
        /// </summary>
        [Required]
        public virtual bool AllowPointerLock { get; set; } = false;

        /// <summary>
        /// Allow Popups.
        /// Allows popups (like from window.open, target= "_blank", showModalDialog). If this keyword is not used, that functionality will silently fail.
        /// </summary>
        [Required]
        public virtual bool AllowPopups { get; set; } = false;

        /// <summary>
        /// Allow Popups To Escape Sandbox.
        /// Allows a sandboxed document to open new windows without forcing the sandboxing flags upon them.
        /// This will allow, for example, a third-party advertisement to be safely sandboxed without forcing the same restrictions upon a landing page.
        /// </summary>
        [Required]
        public virtual bool AllowPopupsToEscapeSandbox { get; set; } = false;

        /// <summary>
        /// Allow Presentation.
        /// Allows embedders to have control over whether an iframe can start a presentation session.
        /// </summary>
        [Required]
        public virtual bool AllowPresentation { get; set; } = false;

        /// <summary>
        /// Allow Same Origin.
        /// Allows the content to be treated as being from its normal origin.
        /// If this keyword is not used, the embedded content is treated as being from a unique origin.
        /// </summary>
        [Required]
        public virtual bool AllowSameOrigin { get; set; } = false;

        /// <summary>
        /// Allow Scripts.
        /// Allows the page to run scripts (but not create pop-up windows).
        /// If this keyword is not used, this operation is not allowed.
        /// </summary>
        [Required]
        public virtual bool AllowScripts { get; set; } = false;

        /// <summary>
        /// Allow Top Navigation.
        /// Allows the page to navigate (load) content to the top-level browsing context.
        /// If this keyword is not used, this operation is not allowed.
        /// </summary>
        [Required]
        public virtual bool AllowTopNavigation { get; set; } = false;
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
        public virtual CspDirectivePermissionsPolicyAccelerometer Accelerometer { get; set; }

        /// <summary>
        /// Ambient Light Sensor.
        /// Controls whether the current document is allowed to gather information about the amount of light
        /// in the environment around the device through the AmbientLightSensor interface.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyAmbientLightSensor AmbientLightSensor { get; set; }

        /// <summary>
        /// Auto Play.
        /// Controls whether the current document is allowed to autoplay media requested through the HTMLMediaElement interface.
        /// When this policy is disabled and there were no user gestures, the Promise returned by HTMLMediaElement.play() will reject with a DOMException.
        /// The autoplay attribute on audio and video elements will be ignored.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyAutoPlay AutoPlay { get; set; }

        /// <summary>
        /// Battery.
        /// Controls whether the use of the Battery Status API is allowed. When this policy is disabled,
        /// the Promise returned by Navigator.getBattery() will reject with a NotAllowedError DOMException.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyBattery Battery { get; set; }

        /// <summary>
        /// Camera.
        /// Controls whether the current document is allowed to use video input devices.
        /// When this policy is disabled, the Promise returned by getUserMedia() will reject with a NotAllowedError DOMException.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyCamera Camera { get; set; }

        /// <summary>
        /// Display Capture.
        /// Controls whether the current document is permitted to use the getDisplayMedia() method to capture screen contents.
        /// When this policy is disabled, the promise returned by getDisplayMedia() will reject with a NotAllowedError
        /// if permission is not obtained to capture the display's contents.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyDisplayCapture DisplayCapture { get; set; }

        /// <summary>
        /// Document Domain.
        /// Controls whether the current document is allowed to set document.domain.
        /// When this policy is disabled, attempting to set document.domain will fail and cause a SecurityError DOMException to be thrown.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyDocumentDomain DocumentDomain { get; set; }

        /// <summary>
        /// Encrypted Media.
        /// Controls whether the current document is allowed to use the Encrypted Media Extensions API (EME).
        /// When this policy is disabled, the Promise returned by Navigator.requestMediaKeySystemAccess() will reject with a DOMException.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyEncryptedMedia EncryptedMedia { get; set; }

        /// <summary>
        /// Execution While Not Rendered.
        /// Controls whether tasks should execute in frames while they're not being rendered (e.g. if an iframe is hidden or display: none).
        /// </summary>
        public virtual CspDirectivePermissionsPolicyExecutionWhileNotRendered ExecutionWhileNotRendered { get; set; }

        /// <summary>
        /// Execution While Out Of Viewport.
        /// Controls whether tasks should execute in frames while they're outside the visible viewport.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyExecutionWhileOutOfViewport ExecutionWhileOutOfViewport { get; set; }

        /// <summary>
        /// FullScreen.
        /// Controls whether the current document is allowed to use Element.requestFullScreen().
        /// When this policy is disabled, the returned Promise rejects with a TypeError.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyFullscreen FullScreen { get; set; }

        /// <summary>
        /// Gamepad.
        /// Controls whether the current document is allowed to use the Gamepad API.
        /// When this policy is disabled, calls to Navigator.getGamepads() will throw a SecurityError DOMException,
        /// and the gamepadconnected and gamepaddisconnected events will not fire.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyGamepad Gamepad { get; set; }

        /// <summary>
        /// Geo location.
        /// Controls whether the current document is allowed to use the Geolocation Interface.
        /// When this policy is disabled, calls to getCurrentPosition() and watchPosition() will cause those functions' callbacks to be invoked
        /// with a GeolocationPositionError code of PERMISSION_DENIED.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyGeolocation Geolocation { get; set; }

        /// <summary>
        /// Gyroscope.
        /// Controls whether the current document is allowed to gather information about the orientation
        /// of the device through the Gyroscope interface.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyGyroscope Gyroscope { get; set; }

        /// <summary>
        /// Layout Animations.
        /// Controls whether the current document is allowed to show layout animations.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyLayoutAnimations LayoutAnimations { get; set; }

        /// <summary>
        /// Legacy Image Formats.
        /// Controls whether the current document is allowed to display images in legacy formats.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyLegacyImageFormats LegacyImageFormats { get; set; }

        /// <summary>
        /// Magnetometer.
        /// Controls whether the current document is allowed to gather information about the orientation of the device through the Magnetometer interface.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyMagnetometer Magnetometer { get; set; }

        /// <summary>
        /// Microphone.
        /// Controls whether the current document is allowed to use audio input devices.
        /// When this policy is disabled, the Promise returned by MediaDevices.getUserMedia() will reject with a NotAllowedError.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyMicrophone Microphone { get; set; }

        /// <summary>
        /// Midi.
        /// Controls whether the current document is allowed to use the Web MIDI API.
        /// When this policy is disabled, the Promise returned by Navigator.requestMIDIAccess() will reject with a DOMException.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyMidi Midi { get; set; }

        /// <summary>
        /// Navigation Override.
        /// Controls the availability of mechanisms that enables the page author to take control over
        /// the behavior of spatial navigation, or to cancel it outright.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyNavigationOverride NavigationOverride { get; set; }

        /// <summary>
        /// Oversized Images.
        /// Controls whether the current document is allowed to download and display large images.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyOversizedImages OversizedImages { get; set; }

        /// <summary>
        /// Payment.
        /// Controls whether the current document is allowed to use the Payment Request API.
        /// When this policy is enabled, the PaymentRequest() constructor will throw a SecurityError DOMException.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyPayment Payment { get; set; }

        /// <summary>
        /// Picture In Picture.
        /// Controls whether the current document is allowed to play a video in a Picture-in-Picture mode via the corresponding API.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyPictureInPicture PictureInPicture { get; set; }

        /// <summary>
        /// Public Key Credentials Get.
        /// Controls whether the current document is allowed to use the Web Authentication API to retrieve already stored public-key credentials,
        /// i.e. via navigator.credentials.get({publicKey: ..., ...}).
        /// </summary>
        public virtual CspDirectivePermissionsPolicyPublicKeyCredentialsGet PublicKeyCredentialsGet { get; set; }

        /// <summary>
        /// Speaker Selection.
        /// Controls whether the current document is allowed to use the Audio Output Devices API to list and select speakers.
        /// </summary>
        public virtual CspDirectivePermissionsPolicySpeakerSelection SpeakerSelection { get; set; }

        /// <summary>
        /// Sync Xhr.
        /// Controls whether the current document is allowed to make synchronous XMLHttpRequest requests.
        /// </summary>
        public virtual CspDirectivePermissionsPolicySyncXhr SyncXhr { get; set; }

        /// <summary>
        /// Unoptimized Images.
        /// Controls whether the current document is allowed to download and display unoptimized images.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyUnoptimizedImages UnoptimizedImages { get; set; }

        /// <summary>
        /// Unsized Media.
        /// Controls whether the current document is allowed to change the size of media elements after the initial layout is complete.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyUnsizedMedia UnsizedMedia { get; set; }

        /// <summary>
        /// Usb.
        /// Controls whether the current document is allowed to use the WebUSB API
        /// </summary>
        public virtual CspDirectivePermissionsPolicyUsb Usb { get; set; }

        /// <summary>
        /// Screen Wake Lock.
        /// Controls whether the current document is allowed to use Screen Wake Lock API
        /// to indicate that device should not turn off or dim the screen.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyScreenWakeLock ScreenWakeLock { get; set; }

        /// <summary>
        /// Web Share.
        /// Controls whether the current document is allowed to use the Navigator.share() of Web Share API to
        /// share text, links, images, and other content to arbitrary destinations of user's choice, e.g. mobile apps.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyWebShare WebShare { get; set; }

        /// <summary>
        /// Xr Spatial Tracking.
        /// Controls whether the current document is allowed to use the WebXR Device API to interact with a WebXR session.
        /// </summary>
        public virtual CspDirectivePermissionsPolicyXrSpatialTracking XrSpatialTracking { get; set; }


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