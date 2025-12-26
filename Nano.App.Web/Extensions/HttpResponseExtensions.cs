using Microsoft.AspNetCore.Http;
using Nano.App.Web.Config;
using Nano.App.Web.Config.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.App.Web.Extensions;

internal static class HttpResponseExtensions
{
    internal static HttpResponse AddCrossOriginEmbedderPolicyHeader(this HttpResponse httpResponse, CrossOriginEmbedderPolicy? crossOriginEmbedderPolicy)
    {
        if (httpResponse == null) 
            throw new ArgumentNullException(nameof(httpResponse));
        
        switch (crossOriginEmbedderPolicy)
        {
            case CrossOriginEmbedderPolicy.UnsafeNone:
                httpResponse.Headers
                    .Add("Cross-Origin-Embedder-Policy", "unsafe-none");
                break;

            case CrossOriginEmbedderPolicy.RequireCorp:
                httpResponse.Headers
                    .Add("Cross-Origin-Embedder-Policy", "require-corp");
                break;

            case CrossOriginEmbedderPolicy.Credentialless:
                httpResponse.Headers
                    .Add("Cross-Origin-Embedder-Policy", "credentialless");
                break;

            case null:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(crossOriginEmbedderPolicy), crossOriginEmbedderPolicy, "Argument out of range.");
        }

        return httpResponse;
    }

    internal static HttpResponse AddCrossOriginOpenerPolicyHeader(this HttpResponse httpResponse, CrossOriginOpenerPolicy? crossOriginOpenerPolicy)
    {
        if (httpResponse == null)
            throw new ArgumentNullException(nameof(httpResponse));

        switch (crossOriginOpenerPolicy)
        {
            case CrossOriginOpenerPolicy.UnsafeNone:
                httpResponse.Headers
                    .Add("Cross-Origin-Opener-Policy", "unsafe-none");
                break;

            case CrossOriginOpenerPolicy.SameOrigin:
                httpResponse.Headers
                    .Add("Cross-Origin-Opener-Policy", "same-origin");
                break;

            case CrossOriginOpenerPolicy.SameOriginAllowPopups:
                httpResponse.Headers
                    .Add("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
                break;

            case null:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(crossOriginOpenerPolicy), crossOriginOpenerPolicy, "Argument out of range.");
        }

        return httpResponse;
    }

    internal static HttpResponse AddCrossOriginResourcePolicyHeader(this HttpResponse httpResponse, CrossOriginResourcePolicy? crossOriginResourcePolicy)
    {
        if (httpResponse == null)
            throw new ArgumentNullException(nameof(httpResponse));

        switch (crossOriginResourcePolicy)
        {
            case CrossOriginResourcePolicy.SameSite:
                httpResponse.Headers
                    .Add("Cross-Origin-Resource-Policy", "same-site");
                break;

            case CrossOriginResourcePolicy.SameOrigin:
                httpResponse.Headers
                    .Add("Cross-Origin-Resource-Policy", "same-origin");
                break;

            case CrossOriginResourcePolicy.CrossOrigin:
                httpResponse.Headers
                    .Add("Cross-Origin-Resource-Policy", "cross-origin");
                break;

            case null:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(crossOriginResourcePolicy), crossOriginResourcePolicy, "Argument out of range.");
        }

        return httpResponse;
    }

    internal static HttpResponse AddXRobotsHeader(this HttpResponse httpResponse, RobotsOptions options)
    {
        if (httpResponse == null)
            throw new ArgumentNullException(nameof(httpResponse));

        var directives = new List<string>();

        if (options.UseNoIndex)
        {
            directives
                .Add("noindex");
        }

        if (options.UseNoFollow)
        {
            directives
                .Add("nofollow");
        }

        if (options.UseNoSnippet)
        {
            directives
                .Add("nosnippet");
        }

        if (options.UseNoArchive)
        {
            directives
                .Add("noarchive");
        }

        if (options.UseNoOdp)
        {
            directives
                .Add("noodp");
        }

        if (options.UseNoTranslate)
        {
            directives
                .Add("notranslate");
        }

        if (options.UseNoImageIndex)
        {
            directives
                .Add("noimageindex");
        }

        if (directives.Count > 0)
        {
            httpResponse.Headers
                .Add("X-Robots-Tag", string.Join(", ", directives));
        }

        return httpResponse;
    }

    internal static HttpResponse AddFrameOptionsPolicyHeader(this HttpResponse httpResponse, XFrameOptionsPolicy? frameOptionsPolicy)
    {
        if (httpResponse == null)
            throw new ArgumentNullException(nameof(httpResponse));

        const string KEY = "X-Frame-Options";

        switch (frameOptionsPolicy)
        {
            case XFrameOptionsPolicy.Deny:
                httpResponse.Headers
                    .Add(KEY, "DENY");
                break;

            case XFrameOptionsPolicy.SameOrigin:
                httpResponse.Headers
                    .Add(KEY, "SAMEORIGIN");
                break;

            case XFrameOptionsPolicy.Disabled:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(frameOptionsPolicy), frameOptionsPolicy, "Argument out of range.");
        }

        return httpResponse;
    }

    internal static HttpResponse AddContentTypeOptionsNoSniffHeader(this HttpResponse httpResponse)
    {
        if (httpResponse == null)
            throw new ArgumentNullException(nameof(httpResponse));

        httpResponse.Headers
            .Add("X-Content-Type-Options", "nosniff");

        return httpResponse;
    }

    internal static HttpResponse AddPermissionsPolicyHeader(this HttpResponse httpResponse, CspOptions.CspDirectivePermissionsPolicy cspDirectivePermissionsPolicy)
    {
        if (httpResponse == null)
            throw new ArgumentNullException(nameof(httpResponse));

        var permissionPolicyValues = string.Empty;

        permissionPolicyValues += UseCspPermissionsPolicyDirective("accelerometer", cspDirectivePermissionsPolicy.Accelerometer);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("ambient-light-sensor", cspDirectivePermissionsPolicy.AmbientLightSensor);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("autoplay", cspDirectivePermissionsPolicy.AutoPlay);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("battery", cspDirectivePermissionsPolicy.Battery);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("camera", cspDirectivePermissionsPolicy.Camera);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("display-capture", cspDirectivePermissionsPolicy.DisplayCapture);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("document-domain", cspDirectivePermissionsPolicy.DocumentDomain);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("encrypted-media", cspDirectivePermissionsPolicy.EncryptedMedia);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("execution-while-not-rendered", cspDirectivePermissionsPolicy.ExecutionWhileNotRendered);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("execution-while-out-of-viewport", cspDirectivePermissionsPolicy.ExecutionWhileOutOfViewport);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("fullscreen", cspDirectivePermissionsPolicy.FullScreen);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("gamepad", cspDirectivePermissionsPolicy.Gamepad);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("geolocation", cspDirectivePermissionsPolicy.Geolocation);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("gyroscope", cspDirectivePermissionsPolicy.Gyroscope);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("legacy-image-formats", cspDirectivePermissionsPolicy.LegacyImageFormats);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("magnetometer", cspDirectivePermissionsPolicy.Magnetometer);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("microphone", cspDirectivePermissionsPolicy.Microphone);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("midi", cspDirectivePermissionsPolicy.Midi);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("navigation-override", cspDirectivePermissionsPolicy.NavigationOverride);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("oversized-images", cspDirectivePermissionsPolicy.OversizedImages);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("payment", cspDirectivePermissionsPolicy.Payment);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("picture-in-picture", cspDirectivePermissionsPolicy.PictureInPicture);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("publickey-credentials-get", cspDirectivePermissionsPolicy.PublicKeyCredentialsGet);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("speaker-selection", cspDirectivePermissionsPolicy.SpeakerSelection);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("sync-xhr", cspDirectivePermissionsPolicy.SyncXhr);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("unoptimized-images", cspDirectivePermissionsPolicy.UnoptimizedImages);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("unsized-media", cspDirectivePermissionsPolicy.UnsizedMedia);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("usb", cspDirectivePermissionsPolicy.Usb);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("screen-wake-lock", cspDirectivePermissionsPolicy.ScreenWakeLock);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("web-share", cspDirectivePermissionsPolicy.WebShare);
        permissionPolicyValues += UseCspPermissionsPolicyDirective("xr-spatial-tracking", cspDirectivePermissionsPolicy.XrSpatialTracking);

        permissionPolicyValues = permissionPolicyValues
            .TrimEnd(',');

        httpResponse.Headers
            .Add("Permissions-Policy", permissionPolicyValues);

        return httpResponse;
    }


    private static string UseCspPermissionsPolicyDirective<T>(string name, T cspDirective = null)
        where T : CspOptions.CspDirective
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (cspDirective == null)
        {
            return "";
        }

        if (cspDirective.IsNone)
        {
            return $"{name}=();";
        }

        var value = cspDirective.Sources
            .Aggregate(string.Empty, (current, x) => current + $"{x} ");

        return cspDirective.IsSelf
            ? $"{name}=(self {value}),"
            : $"{name}=({value}),";
    }
}