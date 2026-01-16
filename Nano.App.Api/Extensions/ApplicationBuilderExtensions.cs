using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Nano.App.Api.Config;
using Nano.App.Api.Config.Enums;
using Nano.App.Api.Extensions.Const;
using Nano.App.Api.Mvc.Extensions;
using Nano.App.Api.Mvc.Middleware;
using Swashbuckle.AspNetCore.SwaggerUI;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestTimeZone.Providers;
using Vivet.AspNetCore.RequestVirusScan.Extensions;
using HealthCheckOptions = Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions;

namespace Nano.App.Api.Extensions;

internal static class ApplicationBuilderExtensions
{
    internal static IApplicationBuilder UseNanoExceptionHandling(this IApplicationBuilder applicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        applicationBuilder
            .UseMiddleware<ExceptionHandlingMiddleware>();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpCorsPolicy(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        if (apiOptions.HttpPolicyHeaders.Cors == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseCors()
            .Use((context, next) =>
            {
                context.Response
                    .AddCrossOriginEmbedderPolicyHeader(apiOptions.HttpPolicyHeaders.Cors.Origin.EmbedderPolicy)
                    .AddCrossOriginOpenerPolicyHeader(apiOptions.HttpPolicyHeaders.Cors.Origin.OpenerPolicy)
                    .AddCrossOriginResourcePolicyHeader(apiOptions.HttpPolicyHeaders.Cors.Origin.ResourcePolicy);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXForwardedHeaders(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        ArgumentNullException.ThrowIfNull(apiOptions);

        if (apiOptions.Hosting.UseForwardedHeaders)
        {
            applicationBuilder
                .UseForwardedHeaders();
        }

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXRobotsTagHeaders(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        if (apiOptions.HttpPolicyHeaders.Robots == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .Use((context, next) =>
            {
                context.Response
                    .AddXRobotsHeader(apiOptions.HttpPolicyHeaders.Robots);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXFrameOptionsPolicyHeader(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        applicationBuilder
            .Use((context, next) =>
            {
                context.Response
                    .AddFrameOptionsPolicyHeader(apiOptions.HttpPolicyHeaders.FrameOptionsPolicyHeader);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXXssProtectionPolicyHeader(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        applicationBuilder
            .UseSecurityHeaders(x =>
            {
                switch (apiOptions.HttpPolicyHeaders.XssProtectionPolicyHeader)
                {
                    case XXssProtectionPolicyBlockMode.FilterEnabled:
                        x.AddXssProtectionEnabled();
                        break;

                    case XXssProtectionPolicyBlockMode.FilterDisabled:
                        x.AddXssProtectionDisabled();
                        break;

                    case XXssProtectionPolicyBlockMode.FilterEnabledBlockMode:
                        x.AddXssProtectionBlock();
                        break;

                    case XXssProtectionPolicyBlockMode.Disabled:
                        x.AddXssProtectionDisabled();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(apiOptions.HttpPolicyHeaders.XssProtectionPolicyHeader), apiOptions.HttpPolicyHeaders.XssProtectionPolicyHeader, "Argument is out of range.");
                }
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpContentTypeOptionsPolicyHeader(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        if (apiOptions.HttpPolicyHeaders.UseContentTypeOptionsNoSniff)
        {
            applicationBuilder
                .Use((context, next) =>
                {
                    context.Response
                        .AddContentTypeOptionsNoSniffHeader();

                    return next();
                });
        }

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpReferrerPolicyHeader(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        applicationBuilder
            .UseSecurityHeaders(x =>
            {
                switch (apiOptions.HttpPolicyHeaders.ReferrerPolicyHeader)
                {
                    case ReferrerPolicy.NoReferrer:
                        x.AddReferrerPolicyNoReferrer();
                        break;

                    case ReferrerPolicy.NoReferrerWhenDowngrade:
                        x.AddReferrerPolicyNoReferrerWhenDowngrade();
                        break;

                    case ReferrerPolicy.SameOrigin:
                        x.AddReferrerPolicySameOrigin();
                        break;

                    case ReferrerPolicy.Origin:
                        x.AddReferrerPolicyOrigin();
                        break;

                    case ReferrerPolicy.StrictOrigin:
                        x.AddReferrerPolicyStrictOrigin();
                        break;

                    case ReferrerPolicy.OriginWhenCrossOrigin:
                        x.AddReferrerPolicyOriginWhenCrossOrigin();
                        break;

                    case ReferrerPolicy.StrictOriginWhenCrossOrigin:
                        x.AddReferrerPolicyStrictOriginWhenCrossOrigin();
                        break;

                    case ReferrerPolicy.UnsafeUrl:
                        x.AddReferrerPolicyUnsafeUrl();
                        break;

                    case ReferrerPolicy.Disabled:
                        x.AddReferrerPolicyNone();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(apiOptions.HttpPolicyHeaders.ReferrerPolicyHeader), apiOptions.HttpPolicyHeaders.ReferrerPolicyHeader, "Argument out of range.");
                }
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpStrictTransportSecurityPolicyHeader(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        if (apiOptions.HttpPolicyHeaders.Hsts == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseHsts();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpContentSecurityPolicyHeader(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        if (apiOptions.HttpPolicyHeaders.Csp == null)
        {
            return applicationBuilder;
        }

        if (apiOptions.HttpPolicyHeaders.Csp.ReportOnly)
        {
            applicationBuilder
                .UseSecurityHeaders(new HeaderPolicyCollection()
                    .AddContentSecurityPolicyReportOnly(x =>
                    {
                        if (apiOptions.HttpPolicyHeaders.Csp.UpgradeInsecureRequests)
                        {
                            x.AddUpgradeInsecureRequests();
                        }

                        if (apiOptions.HttpPolicyHeaders.Csp.BlockAllMixedContent)
                        {
                            x.AddBlockAllMixedContent();
                        }

                        x.UseCspReportUris(apiOptions.HttpPolicyHeaders.Csp.ReportUris);
                        x.UseCspDefaults(apiOptions.HttpPolicyHeaders.Csp.Defaults);
                        x.UseCspStyles(apiOptions.HttpPolicyHeaders.Csp.Styles);
                        x.UseCspScripts(apiOptions.HttpPolicyHeaders.Csp.Scripts);
                        x.UseCspObjects(apiOptions.HttpPolicyHeaders.Csp.Objects);
                        x.UseCspImages(apiOptions.HttpPolicyHeaders.Csp.Images);
                        x.UseCspMedia(apiOptions.HttpPolicyHeaders.Csp.Media);
                        x.UseCspFrames(apiOptions.HttpPolicyHeaders.Csp.Frames);
                        x.UseCspFrameAncestors(apiOptions.HttpPolicyHeaders.Csp.FrameAncestors);
                        x.UseCspFonts(apiOptions.HttpPolicyHeaders.Csp.Fonts);
                        x.UseCspConnections(apiOptions.HttpPolicyHeaders.Csp.Connections);
                        x.UseCspBaseUris(apiOptions.HttpPolicyHeaders.Csp.BaseUris);
                        x.UseCspChildren(apiOptions.HttpPolicyHeaders.Csp.Children);
                        x.UseCspForms(apiOptions.HttpPolicyHeaders.Csp.Forms);
                        x.UseCspManifests(apiOptions.HttpPolicyHeaders.Csp.Manifests);
                        x.UseCspWorkers(apiOptions.HttpPolicyHeaders.Csp.Workers);
                        x.UseCspSandbox(apiOptions.HttpPolicyHeaders.Csp.Sandbox);
                    }));
        }
        else
        {
            applicationBuilder
                .UseSecurityHeaders(new HeaderPolicyCollection()
                    .AddContentSecurityPolicy(x =>
                    {
                        if (apiOptions.HttpPolicyHeaders.Csp.UpgradeInsecureRequests)
                        {
                            x.AddUpgradeInsecureRequests();
                        }

                        if (apiOptions.HttpPolicyHeaders.Csp.BlockAllMixedContent)
                        {
                            x.AddBlockAllMixedContent();
                        }

                        x.UseCspReportUris(apiOptions.HttpPolicyHeaders.Csp.ReportUris);
                        x.UseCspDefaults(apiOptions.HttpPolicyHeaders.Csp.Defaults);
                        x.UseCspStyles(apiOptions.HttpPolicyHeaders.Csp.Styles);
                        x.UseCspScripts(apiOptions.HttpPolicyHeaders.Csp.Scripts);
                        x.UseCspObjects(apiOptions.HttpPolicyHeaders.Csp.Objects);
                        x.UseCspImages(apiOptions.HttpPolicyHeaders.Csp.Images);
                        x.UseCspMedia(apiOptions.HttpPolicyHeaders.Csp.Media);
                        x.UseCspFrames(apiOptions.HttpPolicyHeaders.Csp.Frames);
                        x.UseCspFrameAncestors(apiOptions.HttpPolicyHeaders.Csp.FrameAncestors);
                        x.UseCspFonts(apiOptions.HttpPolicyHeaders.Csp.Fonts);
                        x.UseCspConnections(apiOptions.HttpPolicyHeaders.Csp.Connections);
                        x.UseCspBaseUris(apiOptions.HttpPolicyHeaders.Csp.BaseUris);
                        x.UseCspChildren(apiOptions.HttpPolicyHeaders.Csp.Children);
                        x.UseCspForms(apiOptions.HttpPolicyHeaders.Csp.Forms);
                        x.UseCspManifests(apiOptions.HttpPolicyHeaders.Csp.Manifests);
                        x.UseCspWorkers(apiOptions.HttpPolicyHeaders.Csp.Workers);
                        x.UseCspSandbox(apiOptions.HttpPolicyHeaders.Csp.Sandbox);
                    }));
        }

        applicationBuilder
            .Use((context, next) =>
            {
                context.Response
                    .AddPermissionsPolicyHeader(apiOptions.HttpPolicyHeaders.Csp.PermissionsPolicy);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoSession(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        if (apiOptions.Session == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseSession();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoRequestOptions(this IApplicationBuilder applicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        applicationBuilder
            .UseMiddleware<HttpRequestOptionsMiddleware>();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoRequestIdentifier(this IApplicationBuilder applicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        applicationBuilder
            .UseMiddleware<HttpRequestIdentifierMiddleware>();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoRequestVirusScan(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        if (apiOptions.VirusScan == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseRequestVirusScan();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoRequestLocalization(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        var cultureInfos = apiOptions.Cultures.Supported
            .Select(y => new CultureInfo(y))
            .ToArray();

        applicationBuilder
            .UseRequestLocalization(x =>
            {
                x.DefaultRequestCulture = new RequestCulture(apiOptions.Cultures.Default);
                x.SupportedCultures = cultureInfos;
                x.SupportedUICultures = cultureInfos;
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoRequestTimeZone(this IApplicationBuilder applicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        applicationBuilder
            .UseRequestTimeZone();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoResponseCompression(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        if (apiOptions.Hosting.UseResponseCompression)
        {
            applicationBuilder
                .UseResponseCompression();
        }

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoResponseCaching(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);

        applicationBuilder
            .Use((context, next) =>
            {
                if (apiOptions.ResponseCache == null)
                {
                    context.Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
                    context.Response.Headers[HeaderNames.Pragma] = "no-cache";
                    context.Response.Headers[HeaderNames.Expires] = "0";
                }
                else
                {
                    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = apiOptions.ResponseCache.MaxAge
                    };

                    var existingVary = context.Response.Headers[HeaderNames.Vary].ToString();
                    var varyHeaders = new List<string>(existingVary.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        HeaderNames.Authorization,
                        HeaderNames.AcceptEncoding,
                        HeaderNames.AcceptLanguage,
                        RequestTimeZoneHeaderProvider.Headerkey
                    };
                    context.Response.Headers[HeaderNames.Vary] = string.Join(", ", varyHeaders.Distinct());

                    var responseCachingFeature = context.Features
                        .Get<IResponseCachingFeature>();

                    responseCachingFeature?.VaryByQueryKeys = ["*"];
                }

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoDocumentataion(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions, string environment)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);
        ArgumentNullException.ThrowIfNull(environment);

        if (apiOptions.Documentation != null)
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
                        var defaultVersion = apiOptions.Version
                            .ParseVersion();

                        var defaultApiVersion = new ApiVersion(defaultVersion.Major, defaultVersion.Minor);

                        var defaultVersionText = apiOptions.Documentation.UseDefaultVersion && description.ApiVersion == defaultApiVersion
                            ? " (Default)"
                            : string.Empty;

                        x.SwaggerEndpoint($"{description.GroupName}/swagger.json", $"{nameof(Nano)} - {apiOptions.Name} {description.ApiVersion}{defaultVersionText} ({environment})");
                    }

                    x.RoutePrefix = "docs";
                    x.DocumentTitle = $"{nameof(Nano)} - {apiOptions.Name} Docs ({environment})";

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

                    if (apiOptions.Documentation.CspNonce == null)
                    {
                        return;
                    }

                    var originalIndexStreamFactory = x.IndexStream;

                    x.IndexStream = () =>
                    {
                        using var originalStream = originalIndexStreamFactory();
                        using var originalStreamReader = new StreamReader(originalStream);

                        var originalIndexHtmlContents = originalStreamReader
                            .ReadToEnd();

                        const string PATTERN = "<(script|style)([^>]*)>";
                        var replacement = $"<$1$2 nonce=\"{apiOptions.Documentation.CspNonce}\">";
                        var nonceEnabledIndexHtmlContents = Regex.Replace(originalIndexHtmlContents, PATTERN, replacement, RegexOptions.IgnoreCase);

                        var bytes = Encoding.UTF8
                            .GetBytes(nonceEnabledIndexHtmlContents);

                        return new MemoryStream(bytes);
                    };
                });
        }

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHealthChecks(this IApplicationBuilder applicationBuilder, ApiOptions apiOptions, string environment)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(apiOptions);
        ArgumentNullException.ThrowIfNull(environment);

        if (apiOptions.HealthCheck == null)
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

        applicationBuilder
            .UseHealthChecksUI(x =>
            {
                x.PageTitle = $"{nameof(Nano)} - {apiOptions.Name} Healthz v{apiOptions.Version} ({environment})";
                x.UIPath = HealthzCheckUris.UiPath;
                x.ApiPath = HealthzCheckUris.ApiPath;
                x.ResourcesPath = HealthzCheckUris.RexPath;
                x.WebhookPath = HealthzCheckUris.WebHooksPath;
                x.UseRelativeApiPath = true;
                x.UseRelativeResourcesPath = true;
                x.UseRelativeWebhookPath = true;
            });

        return applicationBuilder;
    }
}