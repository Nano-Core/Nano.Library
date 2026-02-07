using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Nano.App.Api.Config;
using Nano.App.Api.Config.Enums;
using Nano.App.Api.Mvc.Csp;
using Nano.App.Api.Mvc.Csp.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.App.Api.Mvc.Extensions;

internal static class HttpResponseExtensions
{
    internal static HttpResponse AddCrossOriginEmbedderPolicyHeader(this HttpResponse httpResponse, CrossOriginEmbedderPolicy? crossOriginEmbedderPolicy = null)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (crossOriginEmbedderPolicy == null)
        {
            return httpResponse;
        }

        switch (crossOriginEmbedderPolicy)
        {
            case CrossOriginEmbedderPolicy.UnsafeNone:
                httpResponse.Headers
                    .TryAdd("Cross-Origin-Embedder-Policy", "unsafe-none");
                break;

            case CrossOriginEmbedderPolicy.RequireCorp:
                httpResponse.Headers
                    .TryAdd("Cross-Origin-Embedder-Policy", "require-corp");
                break;

            case CrossOriginEmbedderPolicy.Credentialless:
                httpResponse.Headers
                    .TryAdd("Cross-Origin-Embedder-Policy", "credentialless");
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(crossOriginEmbedderPolicy), crossOriginEmbedderPolicy, "Argument out of range.");
        }

        return httpResponse;
    }

    internal static HttpResponse AddCrossOriginOpenerPolicyHeader(this HttpResponse httpResponse, CrossOriginOpenerPolicy? crossOriginOpenerPolicy = null)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (crossOriginOpenerPolicy == null)
        {
            return httpResponse;
        }

        switch (crossOriginOpenerPolicy)
        {
            case CrossOriginOpenerPolicy.UnsafeNone:
                httpResponse.Headers
                    .TryAdd("Cross-Origin-Opener-Policy", "unsafe-none");
                break;

            case CrossOriginOpenerPolicy.SameOrigin:
                httpResponse.Headers
                    .TryAdd("Cross-Origin-Opener-Policy", "same-origin");
                break;

            case CrossOriginOpenerPolicy.SameOriginAllowPopups:
                httpResponse.Headers
                    .TryAdd("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(crossOriginOpenerPolicy), crossOriginOpenerPolicy, "Argument out of range.");
        }

        return httpResponse;
    }

    internal static HttpResponse AddCrossOriginResourcePolicyHeader(this HttpResponse httpResponse, CrossOriginResourcePolicy? crossOriginResourcePolicy = null)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (crossOriginResourcePolicy == null)
        {
            return httpResponse;
        }

        switch (crossOriginResourcePolicy)
        {
            case CrossOriginResourcePolicy.SameSite:
                httpResponse.Headers
                    .TryAdd("Cross-Origin-Resource-Policy", "same-site");
                break;

            case CrossOriginResourcePolicy.SameOrigin:
                httpResponse.Headers
                    .TryAdd("Cross-Origin-Resource-Policy", "same-origin");
                break;

            case CrossOriginResourcePolicy.CrossOrigin:
                httpResponse.Headers
                    .TryAdd("Cross-Origin-Resource-Policy", "cross-origin");
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(crossOriginResourcePolicy), crossOriginResourcePolicy, "Argument out of range.");
        }

        return httpResponse;
    }

    internal static HttpResponse AddXRobotsHeader(this HttpResponse httpResponse, RobotsOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (options == null)
        {
            return httpResponse;
        }

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
                .TryAdd("X-Robots-Tag", string.Join(", ", directives));
        }

        return httpResponse;
    }

    internal static HttpResponse AddXFrameOptionsPolicyHeader(this HttpResponse httpResponse, XFrameOptionsPolicy? xframeOptionsPolicy = null)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (xframeOptionsPolicy == null)
        {
            return httpResponse;
        }

        const string KEY = "X-Frame-Options";

        switch (xframeOptionsPolicy)
        {
            case XFrameOptionsPolicy.Deny:
                httpResponse.Headers
                    .TryAdd(KEY, "DENY");
                break;

            case XFrameOptionsPolicy.SameOrigin:
                httpResponse.Headers
                    .TryAdd(KEY, "SAMEORIGIN");
                break;

            case XFrameOptionsPolicy.Disabled:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(xframeOptionsPolicy), xframeOptionsPolicy, "Argument out of range.");
        }

        return httpResponse;
    }

    internal static HttpResponse AddContentTypeOptionsHeader(this HttpResponse httpResponse, ContentTypeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (options == null)
        {
            return httpResponse;
        }

        if (options.NoSniff)
        {
            httpResponse.Headers
                .TryAdd(HeaderNames.XContentTypeOptions, "nosniff");
        }

        return httpResponse;
    }

    internal static HttpResponse AddReferrerPolicyHeader(this HttpResponse httpResponse, ReferrerPolicyOptions? options)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (options == null)
        {
            return httpResponse;
        }

        var headerValue = options.ReferrerPolicyHeader switch
        {
            ReferrerPolicy.NoReferrer => "no-referrer",
            ReferrerPolicy.NoReferrerWhenDowngrade => "no-referrer-when-downgrade",
            ReferrerPolicy.SameOrigin => "same-origin",
            ReferrerPolicy.Origin => "origin",
            ReferrerPolicy.StrictOrigin => "strict-origin",
            ReferrerPolicy.OriginWhenCrossOrigin => "origin-when-cross-origin",
            ReferrerPolicy.StrictOriginWhenCrossOrigin => "strict-origin-when-cross-origin",
            ReferrerPolicy.UnsafeUrl => "unsafe-url",
            ReferrerPolicy.Disabled => "",
            _ => throw new ArgumentOutOfRangeException(nameof(options.ReferrerPolicyHeader), options.ReferrerPolicyHeader, "Argument out of range.")
        };

        httpResponse.Headers
            .TryAdd("Referrer-Policy", headerValue);

        return httpResponse;
    }

    internal static HttpResponse AddXXssProtectionPolicyHeader(this HttpResponse httpResponse, XssProtectionOptions? options)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (options == null)
        {
            return httpResponse;
        }

        var headerValue = options.XssProtectionPolicyHeader switch
        {
            XssProtectionPolicyBlockMode.FilterEnabled => "1",
            XssProtectionPolicyBlockMode.FilterDisabled => "0",
            XssProtectionPolicyBlockMode.FilterEnabledBlockMode => "1; mode=block",
            XssProtectionPolicyBlockMode.ProtectionReport => $"1; report={options.ReportingUrl}",
            _ => throw new ArgumentOutOfRangeException(nameof(options.XssProtectionPolicyHeader), options.XssProtectionPolicyHeader, "Argument is out of range.")
        };

        httpResponse.Headers
            .TryAdd(HeaderNames.XXSSProtection, headerValue);

        return httpResponse;
    }

    internal static HttpResponse AddContentSecurityPolicyHeader(this HttpResponse httpResponse, CspOptions? options)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (options == null)
        {
            return httpResponse;
        }

        var builder = new CspBuilder();

        builder
            .UseUpgradeInsecureRequests(options)
            .UseCspReportTo(options.ReportTo)
            .UseCspDefault(options.Defaults)
            .UseCspScript(options.Scripts)
            .UseCspScriptAttr(options.ScriptsAttr)
            .UseCspScriptElem(options.ScriptsElem)
            .UseCspStyle(options.Styles)
            .UseCspStyleAttr(options.StylesAttr)
            .UseCspStyleElem(options.StylesElem)
            .UseCspObject(options.Objects)
            .UseCspImage(options.Images)
            .UseCspMedia(options.Media)
            .UseCspFrame(options.Frames)
            .UseCspFencedFrame(options.FencedFrames)
            .UseCspFrameAncestor(options.FrameAncestors)
            .UseCspFont(options.Fonts)
            .UseCspConnection(options.Connections)
            .UseCspBaseUri(options.BaseUris)
            .UseCspChildren(options.Children)
            .UseCspForm(options.Forms)
            .UseCspManifest(options.Manifests)
            .UseCspWorker(options.Workers)
            .UseTrustedTypes(options.TrustedTypes)
            .UseCspSandbox(options.Sandbox)
            .UseCspRequireTrustedTypesFor(options.Scripts)
            .UseCspRequireSriFor(options.Scripts, options.Styles);

        var headerValue = builder
            .Build();

        var headerName = options.ReportOnly
            ? HeaderNames.ContentSecurityPolicyReportOnly
            : HeaderNames.ContentSecurityPolicy;

        httpResponse.Headers
            .TryAdd(headerName, headerValue);

        return httpResponse;
    }

    internal static HttpResponse AddContentSecurityPolicyReportToHeader(this HttpResponse httpResponse, CspOptions.CspReportToOptions? options)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (options == null)
        {
            return httpResponse;
        }

        var endpoints = options.Endpoints;

        if (options.Endpoints.Length == 0)
        {
            var request = httpResponse.HttpContext.Request;

            endpoints = [$"{request.Scheme}://{request.Host}/csp/report-to"];
        }

        var reportTo = new
        {
            group = options.Group,
            max_age = options.MaxAge,
            endpoints = endpoints
                .Select(x => new
                {
                    url = x
                })
        };

        httpResponse.Headers["Report-To"] = JsonConvert.SerializeObject(reportTo);
        httpResponse.Headers["Reporting-Endpoints"] = $"{options.Group}={string.Join(',', options.Endpoints.Select(x => $"\"{x}\""))}";

        return httpResponse;
    }

    internal static HttpResponse AddContentSecurityPolicyPermissionsHeader(this HttpResponse httpResponse, CspOptions.CspDirectivePermissionsPolicyOptions? cspDirectivePermissionsPolicy = null)
    {
        ArgumentNullException.ThrowIfNull(httpResponse);

        if (cspDirectivePermissionsPolicy == null)
        {
            return httpResponse;
        }

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
            .TryAdd("Permissions-Policy", permissionPolicyValues);

        return httpResponse;
    }


    private static string UseCspPermissionsPolicyDirective<T>(string name, T? cspDirective = null)
        where T : CspOptions.CspDirectiveOptions
    {
        ArgumentNullException.ThrowIfNull(name);

        if (cspDirective == null)
        {
            return "";
        }

        if (cspDirective.IsNone)
        {
            return $"{name}=(),";
        }

        var value = string.Join(' ', cspDirective.Sources);

        return cspDirective.IsSelf
            ? $"{name}=(self),"
            : $"{name}=(\"{value}\"),";
    }
}