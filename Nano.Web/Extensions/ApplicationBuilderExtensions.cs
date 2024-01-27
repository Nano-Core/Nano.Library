using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Asp.Versioning.ApiExplorer;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Nano.App;
using Nano.Config;
using Nano.Web.Enums;
using Nano.Web.Extensions.Const;
using Nano.Web.Hosting.Middleware;
using NWebsec.AspNetCore.Mvc;
using NWebsec.Core.Common.Middleware.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestTimeZone.Providers;
using ReferrerPolicy = NWebsec.AspNetCore.Mvc.ReferrerPolicy;

namespace Nano.Web.Extensions;

/// <summary>
/// Application Builder Extensions.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds health checks middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHealthChecks(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

        if (webOptions.Hosting.HealthCheck.UseHealthCheck)
        {
            var appOptions = services.GetService<AppOptions>() ?? new AppOptions();

            applicationBuilder
                .UseHealthChecks(HealthzCheckUris.Path, new HealthCheckOptions
                {
                    Predicate = _ => true,
                    AllowCachingResponses = true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            if (webOptions.Hosting.HealthCheck.UseHealthCheckUI)
            {
                applicationBuilder
                    .UseHealthChecksUI(x =>
                    {
                        x.PageTitle = $"{nameof(Nano)} - {appOptions.Name} Healthz v{appOptions.Version} ({ConfigManager.Environment})";
                        x.UIPath = HealthzCheckUris.UiPath;
                        x.ApiPath = HealthzCheckUris.ApiPath;
                        x.ResourcesPath = HealthzCheckUris.RexPath;
                        x.WebhookPath = HealthzCheckUris.WebHooksPath;
                        x.UseRelativeApiPath = true;
                        x.UseRelativeResourcesPath = true;
                        x.UseRelativeWebhookPath = true;
                    });
            }
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds exception handling middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseMiddleware<ExceptionHandlingMiddleware>();

        return applicationBuilder;
    }

    /// <summary>
    /// Adds options action middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpRequestOptions(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseMiddleware<HttpRequestOptionsMiddleware>();

        return applicationBuilder;
    }

    /// <summary>
    /// Adds request identifier middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpRequestIdentifier(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseMiddleware<HttpRequestIdentifierMiddleware>();

        return applicationBuilder;
    }

    /// <summary>
    /// Adds documentation middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpDocumentataion(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

        if (webOptions.Documentation.IsEnabled)
        {
            var appOptions = services.GetService<AppOptions>() ?? new AppOptions();

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
                        x.SwaggerEndpoint($"{description.GroupName}/swagger.json", $"{nameof(Nano)} - {appOptions.Name} {description.GroupName} ({ConfigManager.Environment})");
                    }

                    x.RoutePrefix = "docs";
                    x.DocumentTitle = $"{nameof(Nano)} - {appOptions.Name} Docs ({ConfigManager.Environment})";

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

                            var nonceEnabledIndexHtmlContents = originalIndexHtmlContents
                                .Replace("<script>", $"<script nonce=\"{webOptions.Documentation.CspNonce}\">", StringComparison.OrdinalIgnoreCase)
                                .Replace("<style>", $"<style nonce=\"{webOptions.Documentation.CspNonce}\">", StringComparison.OrdinalIgnoreCase);

                            var bytes = Encoding.UTF8
                                .GetBytes(nonceEnabledIndexHtmlContents);

                            return new MemoryStream(bytes);
                        };
                    }
                });
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds localization middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpLocalization(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var appOptions = services.GetService<AppOptions>() ?? new AppOptions();

        applicationBuilder
            .UseRequestLocalization(x =>
            {
                x.DefaultRequestCulture = new RequestCulture(appOptions.Cultures.Default);
                x.SupportedCultures = appOptions.Cultures.Supported.Select(y => new CultureInfo(y)).ToArray();
                x.SupportedUICultures = appOptions.Cultures.Supported.Select(y => new CultureInfo(y)).ToArray();
            });

        return applicationBuilder;
    }

    /// <summary>
    /// Adds timezone middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpRequestTimeZone(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseRequestTimeZone();

        return applicationBuilder;
    }

    /// <summary>
    /// Adds http session middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpSession(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

        if (webOptions.Hosting.Session.IsEnabled)
        {
            applicationBuilder
                .UseSession(new SessionOptions
                {
                    IdleTimeout = webOptions.Hosting.Session.Timeout
                });
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds response caching middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpResponseCaching(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

        if (webOptions.Hosting.Cache.IsEnabled)
        {
            applicationBuilder
                .UseResponseCaching()
                .Use((context, next) =>
                {
                    context.Response.GetTypedHeaders().CacheControl =
                        new CacheControlHeaderValue
                        {
                            Public = true,
                            MaxAge = webOptions.Hosting.Cache.MaxAge
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
                        responseCachingFeature.VaryByQueryKeys = new[]
                        {
                            "*"
                        };
                    }

                    return next();
                });
        }
        else
        {
            applicationBuilder
                .UseNoCacheHttpHeaders();
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds response compresssion middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpResponseCompression(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

        if (webOptions.Hosting.UseResponseCompression)
        {
            applicationBuilder
                .UseResponseCompression();
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds no cache middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpXForwardedHeaders(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

        if (webOptions.Hosting.UseForwardedHeaders)
        {
            applicationBuilder
                .UseForwardedHeaders();
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds x-Robots tag middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpXRobotsTagHeaders(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();
        var xRobotTags = webOptions.Hosting.Robots;

        if (xRobotTags.IsEnabled)
        {
            applicationBuilder
                .UseXRobotsTag(x =>
                {
                    if (xRobotTags.UseNoIndex)
                    {
                        x.NoIndex();
                    }

                    if (xRobotTags.UseNoFollow)
                    {
                        x.NoFollow();
                    }

                    if (xRobotTags.UseNoSnippet)
                    {
                        x.NoSnippet();
                    }

                    if (xRobotTags.UseNoArchive)
                    {
                        x.NoArchive();
                    }

                    if (xRobotTags.UseNoOdp)
                    {
                        x.NoOdp();
                    }

                    if (xRobotTags.UseNoTranslate)
                    {
                        x.NoTranslate();
                    }

                    if (xRobotTags.UseNoImageIndex)
                    {
                        x.NoImageIndex();
                    }
                });
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds referrer policy middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpReferrerPolicyHeader(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

        if (webOptions.Hosting.ReferrerPolicyHeader != ReferrerPolicy.Disabled)
        {
            applicationBuilder
                .UseReferrerPolicy(x =>
                {
                    switch (webOptions.Hosting.ReferrerPolicyHeader)
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
                            throw new ArgumentOutOfRangeException();
                    }
                });
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds X-frame options policy middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpXFrameOptionsPolicyHeader(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

        if (webOptions.Hosting.FrameOptionsPolicyHeader != XFrameOptionsPolicy.Disabled)
        {
            applicationBuilder
                .UseXfo(x =>
                {
                    switch (webOptions.Hosting.FrameOptionsPolicyHeader)
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
                            throw new ArgumentOutOfRangeException();
                    }
                });
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds XXss protection policy middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpXXssProtectionPolicyHeader(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

        if (webOptions.Hosting.XssProtectionPolicyHeader != XXssProtectionPolicyBlockMode.Disabled)
        {
            applicationBuilder
                .UseXXssProtection(x =>
                {
                    switch (webOptions.Hosting.XssProtectionPolicyHeader)
                    {
                        case XXssProtectionPolicyBlockMode.FilterDisabled:
                            x.Disabled();
                            break;

                        case XXssProtectionPolicyBlockMode.FilterEnabled:
                            x.Enabled();
                            break;

                        case XXssProtectionPolicyBlockMode.FilterEnabledBlockMode:
                            x.EnabledWithBlockMode();
                            break;

                        case XXssProtectionPolicyBlockMode.Disabled:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds https content type options policy header middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseXHttpContentTypeOptionsPolicyHeader(this IApplicationBuilder applicationBuilder)
    {
        // https://docs.nwebsec.com/en/latest/nwebsec/Configuring-cto.html

        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

        if (webOptions.Hosting.UseContentTypeOptions)
        {
            applicationBuilder
                .UseXContentTypeOptions();
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds Hsts middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpStrictTransportSecurityPolicyHeader(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();
        var hsts = webOptions.Hosting.Hsts;

        if (hsts.IsEnabled)
        {
            var maxAge = hsts.MaxAge;

            applicationBuilder
                .UseHsts(x =>
                {
                    if (hsts.IncludeSubdomains)
                    {
                        x.IncludeSubdomains();

                        if (maxAge.HasValue)
                        {
                            const int MAX_WEEKS = 18;
                            var weeks = maxAge.Value.TotalDays / 7;

                            if (hsts.UsePreload && weeks >= MAX_WEEKS)
                                x.Preload();
                        }
                    }

                    if (maxAge.HasValue)
                    {
                        x.MaxAge(maxAge.Value.Days, maxAge.Value.Hours, maxAge.Value.Minutes, maxAge.Value.Seconds);
                    }
                });
        }

        return applicationBuilder;
    }

    /// <summary>
    /// Adds Content Security Policy (CSP) middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseContentSecurityPolicyHeader(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

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

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();
        var csp = webOptions.Hosting.Csp;

        if (!csp.IsEnabled)
        {
            return applicationBuilder;
        }

        if (csp.UpgradeInsecureRequests)
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
                if (csp.BlockAllMixedContent)
                {
                    options
                        .BlockAllMixedContent();
                }

                if (csp.UpgradeInsecureRequests)
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
                    .UseCspReportUris(csp.ReportUris)
                    .UseCspPluginTypes(csp.PluginTypes)
                    .UseCspDefaults(csp.Defaults)
                    .UseCspStyles(csp.Styles)
                    .UseCspScripts(csp.Scripts)
                    .UseCspObjects(csp.Objects)
                    .UseCspImages(csp.Images)
                    .UseCspMedia(csp.Media)
                    .UseCspFrames(csp.Frames)
                    .UseCspFrameAncestors(csp.FrameAncestors)
                    .UseCspFonts(csp.Fonts)
                    .UseCspConnections(csp.Connections)
                    .UseCspBaseUris(csp.BaseUris)
                    .UseCspChildren(csp.Children)
                    .UseCspForms(csp.Forms)
                    .UseCspManifests(csp.Manifests)
                    .UseCspWorkers(csp.Workers)
                    .UseCspSandbox(csp.Sandbox);
            })
            .UseCspPermissionsPolicy(csp.PermissionsPolicy);

        return applicationBuilder;
    }

    /// <summary>
    /// Adds CORS middleware to the <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/>.</returns>
    internal static IApplicationBuilder UseHttpCorsPolicy(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        var services = applicationBuilder.ApplicationServices;
        var webOptions = services.GetService<WebOptions>() ?? new WebOptions();

        applicationBuilder
            .UseCors()
            .Use((context, next) =>
            {
                switch (webOptions.Hosting.Cors.Origin.EmbedderPolicy)
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
                }

                switch (webOptions.Hosting.Cors.Origin.OpenerPolicy)
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
                }

                switch (webOptions.Hosting.Cors.Origin.ResourcePolicy)
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
                }

                return next();
            });

        return applicationBuilder;
    }

    private static IApplicationBuilder UseCspPermissionsPolicy(this IApplicationBuilder applicationBuilder, WebOptions.HostingOptions.CspOptions.CspDirectivePermissionsPolicy cspDirectivePermissionsPolicy)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (!cspDirectivePermissionsPolicy.IsEnabled)
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
    private static IFluentCspOptions UseCspDefaults(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspStyles(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirectiveStyles cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspScripts(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirectiveScripts cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspObjects(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspImages(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspMedia(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspFrames(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspFrameAncestors(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspFonts(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspConnections(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspBaseUris(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspChildren(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspForms(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspManifests(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspWorkers(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirective cspDirective)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirective == null)
            throw new ArgumentNullException(nameof(cspDirective));

        if (!cspDirective.IsEnabled)
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
    private static IFluentCspOptions UseCspSandbox(this IFluentCspOptions configurer, WebOptions.HostingOptions.CspOptions.CspDirectiveSandbox cspDirectiveSandbox)
    {
        if (configurer == null)
            throw new ArgumentNullException(nameof(configurer));

        if (cspDirectiveSandbox == null)
            throw new ArgumentNullException(nameof(cspDirectiveSandbox));

        if (!cspDirectiveSandbox.IsEnabled)
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
    private static string UseCspPermissionsPolicyDirective<T>(T cspDirective, string directiveName)
        where T : WebOptions.HostingOptions.CspOptions.CspDirective
    {
        if (cspDirective.IsEnabled)
        {
            if (cspDirective.IsNone)
            {
                return $"{directiveName}=(none);";
            }

            var values = cspDirective.Sources
                .Aggregate(string.Empty, (current, x) => current + $"{x} ");

            return cspDirective.IsSelf
                ? $"{directiveName}=(self {values});"
                : $"{directiveName}=({values});";
        }

        return string.Empty;
    }
}