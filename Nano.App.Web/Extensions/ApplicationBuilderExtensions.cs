using Asp.Versioning.ApiExplorer;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Nano.App.Web.Config;
using Nano.App.Web.Extensions.Const;
using Nano.Common.Config.Helpers;
using NWebsec.AspNetCore.Mvc;
using NWebsec.Core.Common.Middleware.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nano.App.Web.Config.Enums;
using Nano.App.Web.Middleware;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestTimeZone.Providers;
using Vivet.AspNetCore.RequestVirusScan.Extensions;
using CspOptions = Nano.App.Web.Config.CspOptions;
using HealthCheckOptions = Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions;
using ReferrerPolicy = NWebsec.AspNetCore.Mvc.ReferrerPolicy;

namespace Nano.App.Web.Extensions;

internal static class ApplicationBuilderExtensions
{
    internal static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseMiddleware<ExceptionHandlingMiddleware>();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpRequestOptions(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseMiddleware<HttpRequestOptionsMiddleware>();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpRequestIdentifier(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseMiddleware<HttpRequestIdentifierMiddleware>();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpHealthChecks(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HealthCheck == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseHealthChecks(HealthzCheckUris.Path, new HealthCheckOptions
            {
                Predicate = _ => true,
                AllowCachingResponses = true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        if (webOptions.HealthCheck.UseHealthCheckUi)
        {
            applicationBuilder
                .UseHealthChecksUI(x =>
                {
                    x.PageTitle = $"{nameof(Nano)} - {webOptions.Name} Healthz v{webOptions.Version} ({ConfigManager.Environment})";
                    x.UIPath = HealthzCheckUris.UiPath;
                    x.ApiPath = HealthzCheckUris.ApiPath;
                    x.ResourcesPath = HealthzCheckUris.RexPath;
                    x.WebhookPath = HealthzCheckUris.WebHooksPath;
                    x.UseRelativeApiPath = true;
                    x.UseRelativeResourcesPath = true;
                    x.UseRelativeWebhookPath = true;
                });
        }

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpDocumentataion(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.Documentation != null)
        {
            applicationBuilder
                .UseSwagger(x =>
                {
                    x.RouteTemplate = "docs/{documentName}/swagger.json";
                })
                .UseSwaggerUI(x =>
                {
                    var apiVersionDescriptionProvider = applicationBuilder.ApplicationServices
                        .GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        var defaultVersionText = webOptions.Documentation.UseDefaultVersion && description.ApiVersion.IsDefault()
                            ? " (Default)"
                            : string.Empty;

                        x.SwaggerEndpoint($"{description.GroupName}/swagger.json", $"{nameof(Nano)} - {webOptions.Name} {description.ApiVersion}{defaultVersionText} ({ConfigManager.Environment})");
                    }

                    x.RoutePrefix = "docs";
                    x.DocumentTitle = $"{nameof(Nano)} - {webOptions.Name} Docs ({ConfigManager.Environment})";

                    x.EnableFilter();
                    x.EnableDeepLinking();
                    x.EnableValidator(null);
                    x.ShowExtensions();
                    x.DisplayOperationId();
                    x.DisplayRequestDuration();
                    x.MaxDisplayedTags(-1);
                    x.DefaultModelExpandDepth(2);
                    x.DefaultModelsExpandDepth(1);
                    x.DefaultModelRendering(ModelRendering.Example);
                    x.DocExpansion(DocExpansion.None);

                    if (webOptions.Documentation.CspNonce != null)
                    {
                        var originalIndexStreamFactory = x.IndexStream;

                        x.IndexStream = () =>
                        {
                            using var originalStream = originalIndexStreamFactory();
                            using var originalStreamReader = new StreamReader(originalStream);
                            
                            var originalIndexHtmlContents = originalStreamReader
                                .ReadToEnd();

                            const string PATTERN = "<(script|style)([^>]*)>";
                            var replacement = $"<$1$2 nonce=\"{webOptions.Documentation.CspNonce}\">";
                            var nonceEnabledIndexHtmlContents = Regex.Replace(originalIndexHtmlContents, PATTERN, replacement, RegexOptions.IgnoreCase);

                            var bytes = Encoding.UTF8
                                .GetBytes(nonceEnabledIndexHtmlContents);

                            return new MemoryStream(bytes);
                        };
                    }
                });
        }

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpRequestLocalization(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        applicationBuilder
            .UseRequestLocalization(x =>
            {
                x.DefaultRequestCulture = new RequestCulture(webOptions.Cultures.Default);
                x.SupportedCultures = webOptions.Cultures.Supported.Select(y => new CultureInfo(y)).ToArray();
                x.SupportedUICultures = webOptions.Cultures.Supported.Select(y => new CultureInfo(y)).ToArray();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpRequestTimeZone(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseRequestTimeZone();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpRequestVirusScan(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.VirusScan == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseRequestVirusScan();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpSession(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.Session == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseSession();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpResponseCaching(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.ResponseCache == null)
        {
            applicationBuilder
                .UseNoCacheHttpHeaders();

            return applicationBuilder;
        }

        applicationBuilder
            .UseResponseCaching()
            // BUG: WHy this, We are adding 2 middlwware for response caching
            .Use((context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = webOptions.ResponseCache.MaxAge
                    };

                context.Response.Headers[HeaderNames.Vary] =
                    new[]
                    {
                        HeaderNames.Authorization,
                        HeaderNames.AcceptEncoding,
                        HeaderNames.AcceptLanguage,
                        RequestTimeZoneHeaderProvider.Headerkey
                    };

                var responseCachingFeature = context.Features
                    .Get<IResponseCachingFeature>();

                if (responseCachingFeature != null)
                {
                    responseCachingFeature.VaryByQueryKeys =
                    [
                        "*"
                    ];
                }

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpResponseCompression(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.Hosting.UseResponseCompression)
        {
            applicationBuilder
                .UseResponseCompression();
        }

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpXForwardedHeaders(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.Hosting.UseForwardedHeaders)
        {
            applicationBuilder
                .UseForwardedHeaders();
        }

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpXRobotsTagHeaders(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.Robots == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseXRobotsTag(x =>
            {
                if (webOptions.HttpPolicyHeaders.Robots.UseNoIndex)
                {
                    x.NoIndex();
                }

                if (webOptions.HttpPolicyHeaders.Robots.UseNoFollow)
                {
                    x.NoFollow();
                }

                if (webOptions.HttpPolicyHeaders.Robots.UseNoSnippet)
                {
                    x.NoSnippet();
                }

                if (webOptions.HttpPolicyHeaders.Robots.UseNoArchive)
                {
                    x.NoArchive();
                }

                if (webOptions.HttpPolicyHeaders.Robots.UseNoOdp)
                {
                    x.NoOdp();
                }

                if (webOptions.HttpPolicyHeaders.Robots.UseNoTranslate)
                {
                    x.NoTranslate();
                }

                if (webOptions.HttpPolicyHeaders.Robots.UseNoImageIndex)
                {
                    x.NoImageIndex();
                }
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpReferrerPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.ReferrerPolicyHeader == ReferrerPolicy.Disabled)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseReferrerPolicy(x =>
            {
                switch (webOptions.HttpPolicyHeaders.ReferrerPolicyHeader)
                {
                    case ReferrerPolicy.NoReferrer:
                        x.NoReferrer();
                        break;

                    case ReferrerPolicy.NoReferrerWhenDowngrade:
                        x.NoReferrerWhenDowngrade();
                        break;

                    case ReferrerPolicy.SameOrigin:
                        x.SameOrigin();
                        break;

                    case ReferrerPolicy.Origin:
                        x.Origin();
                        break;

                    case ReferrerPolicy.StrictOrigin:
                        x.StrictOrigin();
                        break;

                    case ReferrerPolicy.OriginWhenCrossOrigin:
                        x.OriginWhenCrossOrigin();
                        break;

                    case ReferrerPolicy.StrictOriginWhenCrossOrigin:
                        x.StrictOriginWhenCrossOrigin();
                        break;

                    case ReferrerPolicy.UnsafeUrl:
                        x.UnsafeUrl();
                        break;

                    case ReferrerPolicy.Disabled:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(webOptions.HttpPolicyHeaders.ReferrerPolicyHeader), webOptions.HttpPolicyHeaders.ReferrerPolicyHeader, "Argument out of range.");
                }
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpXFrameOptionsPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.FrameOptionsPolicyHeader == XFrameOptionsPolicy.Disabled)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseXfo(x =>
            {
                switch (webOptions.HttpPolicyHeaders.FrameOptionsPolicyHeader)
                {
                    case XFrameOptionsPolicy.Deny:
                        x.Deny();
                        break;

                    case XFrameOptionsPolicy.SameOrigin:
                        x.SameOrigin();
                        break;

                    case XFrameOptionsPolicy.Disabled:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(webOptions.HttpPolicyHeaders.FrameOptionsPolicyHeader), webOptions.HttpPolicyHeaders.FrameOptionsPolicyHeader, "Argument out of range.");
                }
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpXXssProtectionPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.XssProtectionPolicyHeader == XXssProtectionPolicyBlockMode.Disabled)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseXXssProtection(x =>
            {
                switch (webOptions.HttpPolicyHeaders.XssProtectionPolicyHeader)
                {
                    case XXssProtectionPolicyBlockMode.FilterEnabled:
                        x.Enabled();
                        break;

                    case XXssProtectionPolicyBlockMode.FilterDisabled:
                        x.Disabled();
                        break;

                    case XXssProtectionPolicyBlockMode.FilterEnabledBlockMode:
                        x.EnabledWithBlockMode();
                        break;

                    case XXssProtectionPolicyBlockMode.Disabled:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(webOptions.HttpPolicyHeaders.XssProtectionPolicyHeader), webOptions.HttpPolicyHeaders.XssProtectionPolicyHeader, "Argument is out of range.");
                }
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseXHttpContentTypeOptionsPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.UseContentTypeOptions)
        {
            applicationBuilder
                .UseXContentTypeOptions();
        }

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpStrictTransportSecurityPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.Hsts == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseHsts(x =>
            {
                if (webOptions.HttpPolicyHeaders.Hsts.IncludeSubdomains)
                {
                    x.IncludeSubdomains();

                    if (webOptions.HttpPolicyHeaders.Hsts.MaxAge.HasValue)
                    {
                        const int MAX_WEEKS = 18;
                        var weeks = webOptions.HttpPolicyHeaders.Hsts.MaxAge.Value.TotalDays / 7;

                        if (webOptions.HttpPolicyHeaders.Hsts.UsePreload && weeks >= MAX_WEEKS)
                        {
                            x.Preload();
                        }
                    }
                }

                if (webOptions.HttpPolicyHeaders.Hsts.MaxAge.HasValue)
                {
                    x.MaxAge(webOptions.HttpPolicyHeaders.Hsts.MaxAge.Value.Days, webOptions.HttpPolicyHeaders.Hsts.MaxAge.Value.Hours, webOptions.HttpPolicyHeaders.Hsts.MaxAge.Value.Minutes, webOptions.HttpPolicyHeaders.Hsts.MaxAge.Value.Seconds);
                }
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpContentSecurityPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.Csp == null)
        {
            return applicationBuilder;
        }

        // TODO: CSP: Missing from NWebSec
        // navigate-to,
        // prefetch-src,
        // report-to,
        // require-trusted-types-for,
        // trusted-types,
        // require-sri-for,
        // script-src-attr,
        // script-src-elem,
        // style-src-attr,
        // style-src-elem,
        // Sandbox (allow-downloads-without-user-activation, allow-storage-access-by-user-activation, allow-top-navigation-by-user-activation)

        if (webOptions.HttpPolicyHeaders.Csp.UpgradeInsecureRequests)
        {
            applicationBuilder
                .UseRedirectValidation(x =>
                {
                    if (webOptions.Hosting.PortsHttps.Any())
                    {
                        x.AllowSameHostRedirectsToHttps(webOptions.Hosting.PortsHttps);
                    }
                    else
                    {
                        x.AllowSameHostRedirectsToHttps();
                    }
                });
        }

        applicationBuilder
            .UseCsp(options =>
            {
                if (webOptions.HttpPolicyHeaders.Csp.BlockAllMixedContent)
                {
                    options
                        .BlockAllMixedContent();
                }

                if (webOptions.HttpPolicyHeaders.Csp.UpgradeInsecureRequests)
                {
                    if (webOptions.Hosting.PortsHttps.Any())
                    {
                        options
                            .UpgradeInsecureRequests(webOptions.Hosting.PortsHttps.FirstOrDefault());
                    }
                    else
                    {
                        options
                            .UpgradeInsecureRequests();
                    }
                }

                options
                    .UseCspReportUris(webOptions.HttpPolicyHeaders.Csp.ReportUris)
                    .UseCspPluginTypes(webOptions.HttpPolicyHeaders.Csp.PluginTypes)
                    .UseCspDefaults(webOptions.HttpPolicyHeaders.Csp.Defaults)
                    .UseCspStyles(webOptions.HttpPolicyHeaders.Csp.Styles)
                    .UseCspScripts(webOptions.HttpPolicyHeaders.Csp.Scripts)
                    .UseCspObjects(webOptions.HttpPolicyHeaders.Csp.Objects)
                    .UseCspImages(webOptions.HttpPolicyHeaders.Csp.Images)
                    .UseCspMedia(webOptions.HttpPolicyHeaders.Csp.Media)
                    .UseCspFrames(webOptions.HttpPolicyHeaders.Csp.Frames)
                    .UseCspFrameAncestors(webOptions.HttpPolicyHeaders.Csp.FrameAncestors)
                    .UseCspFonts(webOptions.HttpPolicyHeaders.Csp.Fonts)
                    .UseCspConnections(webOptions.HttpPolicyHeaders.Csp.Connections)
                    .UseCspBaseUris(webOptions.HttpPolicyHeaders.Csp.BaseUris)
                    .UseCspChildren(webOptions.HttpPolicyHeaders.Csp.Children)
                    .UseCspForms(webOptions.HttpPolicyHeaders.Csp.Forms)
                    .UseCspManifests(webOptions.HttpPolicyHeaders.Csp.Manifests)
                    .UseCspWorkers(webOptions.HttpPolicyHeaders.Csp.Workers)
                    .UseCspSandbox(webOptions.HttpPolicyHeaders.Csp.Sandbox);
            })
            .UseCspPermissionsPolicy(webOptions.HttpPolicyHeaders.Csp.PermissionsPolicy);

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseHttpCorsPolicy(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.Cors == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseCors()
            .Use((context, next) =>
            {
                switch (webOptions.HttpPolicyHeaders.Cors.Origin.EmbedderPolicy)
                {
                    case CrossOriginEmbedderPolicy.UnsafeNone:
                        context.Response.Headers
                            .Add("Cross-Origin-Embedder-Policy", "unsafe-none");
                        break;

                    case CrossOriginEmbedderPolicy.RequireCorp:
                        context.Response.Headers
                            .Add("Cross-Origin-Embedder-Policy", "require-corp");
                        break;

                    case CrossOriginEmbedderPolicy.Credentialless:
                        context.Response.Headers
                            .Add("Cross-Origin-Embedder-Policy", "credentialless");
                        break;

                    case null:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (webOptions.HttpPolicyHeaders.Cors.Origin.OpenerPolicy)
                {
                    case CrossOriginOpenerPolicy.UnsafeNone:
                        context.Response.Headers
                            .Add("Cross-Origin-Opener-Policy", "unsafe-none");
                        break;

                    case CrossOriginOpenerPolicy.SameOrigin:
                        context.Response.Headers
                            .Add("Cross-Origin-Opener-Policy", "same-origin");
                        break;

                    case CrossOriginOpenerPolicy.SameOriginAllowPopups:
                        context.Response.Headers
                            .Add("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
                        break;

                    case null:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (webOptions.HttpPolicyHeaders.Cors.Origin.ResourcePolicy)
                {
                    case CrossOriginResourcePolicy.SameSite:
                        context.Response.Headers
                            .Add("Cross-Origin-Resource-Policy", "same-site");
                        break;

                    case CrossOriginResourcePolicy.SameOrigin:
                        context.Response.Headers
                            .Add("Cross-Origin-Resource-Policy", "same-origin");
                        break;

                    case CrossOriginResourcePolicy.CrossOrigin:
                        context.Response.Headers
                            .Add("Cross-Origin-Resource-Policy", "cross-origin");
                        break;

                    case null:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return next();
            });

        return applicationBuilder;
    }


    private static IApplicationBuilder UseCspPermissionsPolicy(this IApplicationBuilder applicationBuilder, CspOptions.CspDirectivePermissionsPolicy cspDirectivePermissionsPolicy)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (cspDirectivePermissionsPolicy == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .Use((context, next) =>
            {
                var permissionPolicyValues = string.Empty;

                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.Accelerometer, "accelerometer");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.AmbientLightSensor, "ambient-light-sensor");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.AutoPlay, "autoplay");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.Battery, "battery");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.Camera, "camera");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.DisplayCapture, "display-capture");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.DocumentDomain, "document-domain");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.EncryptedMedia, "encrypted-media");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.ExecutionWhileNotRendered, "execution-while-not-rendered");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.ExecutionWhileOutOfViewport, "execution-while-out-of-viewport");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.FullScreen, "fullscreen");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.Gamepad, "gamepad");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.Geolocation, "geolocation");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.Gyroscope, "gyroscope");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.LayoutAnimations, "layout-animations");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.LegacyImageFormats, "legacy-image-formats");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.Magnetometer, "magnetometer");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.Microphone, "microphone");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.Midi, "midi");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.NavigationOverride, "navigation-override");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.OversizedImages, "oversized-images");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.Payment, "payment");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.PictureInPicture, "picture-in-picture");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.PublicKeyCredentialsGet, "publickey-credentials-get");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.SpeakerSelection, "speaker-selection");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.SyncXhr, "sync-xhr");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.UnoptimizedImages, "unoptimized-images");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.UnsizedMedia, "unsized-media");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.Usb, "usb");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.ScreenWakeLock, "screen-wake-lock");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.WebShare, "web-share");
                permissionPolicyValues += UseCspPermissionsPolicyDirective(cspDirectivePermissionsPolicy.XrSpatialTracking, "xr-spatial-tracking");

                context.Response.Headers
                    .Add("Permissions-Policy", permissionPolicyValues);

                return next();
            });

        return applicationBuilder;
    }

    private static IFluentCspOptions UseCspReportUris(this IFluentCspOptions configurer, string[] reportUris)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (!reportUris.Any())
        {
            return configurer;
        }

        configurer
            .ReportUris(x =>
                x.Uris(reportUris));

        return configurer;
    }
    private static IFluentCspOptions UseCspPluginTypes(this IFluentCspOptions configurer, string[] pluginTypes)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (!pluginTypes.Any())
        {
            return configurer;
        }

        configurer
            .PluginTypes(x =>
                x.MediaTypes(pluginTypes));

        return configurer;
    }
    private static IFluentCspOptions UseCspDefaults(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .DefaultSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspStyles(this IFluentCspOptions configurer, CspOptions.CspDirectiveStyles cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .StyleSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.IsUnsafeInline)
                    {
                        x.UnsafeInline();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspScripts(this IFluentCspOptions configurer, CspOptions.CspDirectiveScripts cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .ScriptSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.IsUnsafeEval)
                    {
                        x.UnsafeEval();
                    }

                    if (cspDirective.IsUnsafeInline)
                    {
                        x.UnsafeInline();
                    }

                    if (cspDirective.StrictDynamic)
                    {
                        x.StrictDynamic();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspObjects(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .ObjectSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspImages(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .ImageSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspMedia(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .MediaSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspFrames(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .FrameSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspFrameAncestors(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .FrameAncestors(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspFonts(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .FontSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspConnections(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .ConnectSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspBaseUris(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .BaseUris(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspChildren(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .ChildSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspForms(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .FormActions(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspManifests(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .ManifestSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspWorkers(this IFluentCspOptions configurer, CspOptions.CspDirective cspDirective = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
        {
            return configurer;
        }

        configurer
            .WorkerSources(x =>
            {
                if (cspDirective.IsNone)
                {
                    x.None();
                }
                else
                {
                    if (cspDirective.IsSelf)
                    {
                        x.Self();
                    }

                    if (cspDirective.Sources.Any())
                    {
                        x.CustomSources(cspDirective.Sources);
                    }
                }
            });

        return configurer;
    }
    private static IFluentCspOptions UseCspSandbox(this IFluentCspOptions configurer, CspOptions.CspDirectiveSandbox cspDirectiveSandbox = null)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirectiveSandbox == null)
        {
            return configurer;
        }

        configurer
            .Sandbox(y =>
            {
                if (cspDirectiveSandbox.AllowForms)
                {
                    y.AllowForms();
                }

                if (cspDirectiveSandbox.AllowModals)
                {
                    y.AllowModals();
                }

                if (cspDirectiveSandbox.AllowOrientationLock)
                {
                    y.AllowOrientationLock();
                }

                if (cspDirectiveSandbox.AllowPointerLock)
                {
                    y.AllowPointerLock();
                }

                if (cspDirectiveSandbox.AllowPopups)
                {
                    y.AllowPopups();
                }

                if (cspDirectiveSandbox.AllowPopupsToEscapeSandbox)
                {
                    y.AllowPopupsToEscapeSandbox();
                }

                if (cspDirectiveSandbox.AllowPresentation)
                {
                    y.AllowPresentation();
                }

                if (cspDirectiveSandbox.AllowSameOrigin)
                {
                    y.AllowSameOrigin();
                }

                if (cspDirectiveSandbox.AllowScripts)
                {
                    y.AllowScripts();
                }

                if (cspDirectiveSandbox.AllowTopNavigation)
                {
                    y.AllowTopNavigation();
                }
            });

        return configurer;
    }
    
    private static string UseCspPermissionsPolicyDirective<T>(T cspDirective = null, string name = null)
        where T : CspOptions.CspDirective
    {
        if (cspDirective == null)
        {
            return string.Empty;
        }

        if (name == null)
        {
            return string.Empty;
        }

        if (cspDirective.IsNone)
        {
            return $"{name}=(none);";
        }

        var values = cspDirective.Sources
            .Aggregate(string.Empty, (current, x) => current + $"{x} ");

        return cspDirective.IsSelf
            ? $"{name}=(self {values});"
            : $"{name}=({values});";
    }
}