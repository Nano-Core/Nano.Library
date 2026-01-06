using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Nano.App.Web.Config;
using Nano.App.Web.Config.Enums;
using Nano.App.Web.Extensions.Const;
using Nano.App.Web.Mvc.Middleware;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Nano.App.Web.Mvc.Documentation.Filters.Document;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestTimeZone.Providers;
using Vivet.AspNetCore.RequestVirusScan.Extensions;
using HealthCheckOptions = Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions;

namespace Nano.App.Web.Extensions;

internal static class ApplicationBuilderExtensions
{
    internal static IApplicationBuilder UseNanoExceptionHandling(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseMiddleware<ExceptionHandlingMiddleware>();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpCorsPolicy(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
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
                context.Response
                    .AddCrossOriginEmbedderPolicyHeader(webOptions.HttpPolicyHeaders.Cors.Origin.EmbedderPolicy)
                    .AddCrossOriginOpenerPolicyHeader(webOptions.HttpPolicyHeaders.Cors.Origin.OpenerPolicy)
                    .AddCrossOriginResourcePolicyHeader(webOptions.HttpPolicyHeaders.Cors.Origin.ResourcePolicy);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXForwardedHeaders(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
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

    internal static IApplicationBuilder UseNanoHttpXRobotsTagHeaders(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
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
            .Use((context, next) =>
            {
                context.Response
                    .AddXRobotsHeader(webOptions.HttpPolicyHeaders.Robots);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXFrameOptionsPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        applicationBuilder
            .Use((context, next) =>
            {
                context.Response
                    .AddFrameOptionsPolicyHeader(webOptions.HttpPolicyHeaders.FrameOptionsPolicyHeader);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXXssProtectionPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        applicationBuilder
            .UseSecurityHeaders(x =>
            {
                switch (webOptions.HttpPolicyHeaders.XssProtectionPolicyHeader)
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
                        throw new ArgumentOutOfRangeException(nameof(webOptions.HttpPolicyHeaders.XssProtectionPolicyHeader), webOptions.HttpPolicyHeaders.XssProtectionPolicyHeader, "Argument is out of range.");
                }
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpContentTypeOptionsPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.UseContentTypeOptionsNoSniff)
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

    internal static IApplicationBuilder UseNanoHttpReferrerPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        applicationBuilder
            .UseSecurityHeaders(x =>
            {
                switch (webOptions.HttpPolicyHeaders.ReferrerPolicyHeader)
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
                        throw new ArgumentOutOfRangeException(nameof(webOptions.HttpPolicyHeaders.ReferrerPolicyHeader), webOptions.HttpPolicyHeaders.ReferrerPolicyHeader, "Argument out of range.");
                }
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpStrictTransportSecurityPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
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
            .UseHsts();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpContentSecurityPolicyHeader(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.Csp == null)
        {
            return applicationBuilder;
        }

        if (webOptions.HttpPolicyHeaders.Csp.ReportOnly)
        {
            applicationBuilder
                .UseSecurityHeaders(new HeaderPolicyCollection()
                    .AddContentSecurityPolicyReportOnly(x =>
                    {
                        if (webOptions.HttpPolicyHeaders.Csp.UpgradeInsecureRequests)
                        {
                            x.AddUpgradeInsecureRequests();
                        }

                        if (webOptions.HttpPolicyHeaders.Csp.BlockAllMixedContent)
                        {
                            x.AddBlockAllMixedContent();
                        }

                        x
                            .UseCspReportUris(webOptions.HttpPolicyHeaders.Csp.ReportUris)
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
                    }));
        }
        else
        {
            applicationBuilder
                .UseSecurityHeaders(new HeaderPolicyCollection()
                    .AddContentSecurityPolicy(x =>
                    {
                        if (webOptions.HttpPolicyHeaders.Csp.UpgradeInsecureRequests)
                        {
                            x.AddUpgradeInsecureRequests();
                        }

                        if (webOptions.HttpPolicyHeaders.Csp.BlockAllMixedContent)
                        {
                            x.AddBlockAllMixedContent();
                        }

                        x
                            .UseCspReportUris(webOptions.HttpPolicyHeaders.Csp.ReportUris)
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
                    }));
        }

        applicationBuilder
            .Use((context, next) =>
            {
                context.Response
                    .AddPermissionsPolicyHeader(webOptions.HttpPolicyHeaders.Csp.PermissionsPolicy);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoSession(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
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

    internal static IApplicationBuilder UseNanoRequestOptions(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseMiddleware<HttpRequestOptionsMiddleware>();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoRequestIdentifier(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseMiddleware<HttpRequestIdentifierMiddleware>();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoRequestVirusScan(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
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

    internal static IApplicationBuilder UseNanoRequestLocalization(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        var cultureInfos = webOptions.Cultures.Supported
            .Select(y => new CultureInfo(y))
            .ToArray();

        applicationBuilder
            .UseRequestLocalization(x =>
            {
                x.DefaultRequestCulture = new RequestCulture(webOptions.Cultures.Default);
                x.SupportedCultures = cultureInfos;
                x.SupportedUICultures = cultureInfos;
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoRequestTimeZone(this IApplicationBuilder applicationBuilder)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        applicationBuilder
            .UseRequestTimeZone();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoResponseCompression(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
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

    internal static IApplicationBuilder UseNanoResponseCaching(this IApplicationBuilder applicationBuilder, WebOptions webOptions)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        applicationBuilder
            .Use((context, next) =>
            {
                if (webOptions.ResponseCache == null)
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
                        MaxAge = webOptions.ResponseCache.MaxAge
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

    internal static IApplicationBuilder UseNanoDocumentataion(this IApplicationBuilder applicationBuilder, WebOptions webOptions, string environment)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (environment == null) 
            throw new ArgumentNullException(nameof(environment));

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
                        var defaultVersion = webOptions.Version
                            .ParseVersion();

                        var defaultApiVersion = new ApiVersion(defaultVersion.Major, defaultVersion.Minor);

                        var defaultVersionText = webOptions.Documentation.UseDefaultVersion && description.ApiVersion == defaultApiVersion
                            ? " (Default)"
                            : string.Empty;

                        x.SwaggerEndpoint($"{description.GroupName}/swagger.json", $"{nameof(Nano)} - {webOptions.Name} {description.ApiVersion}{defaultVersionText} ({environment})");
                    }

                    x.RoutePrefix = "docs";
                    x.DocumentTitle = $"{nameof(Nano)} - {webOptions.Name} Docs ({environment})";

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

    internal static IApplicationBuilder UseNanoHealthChecks(this IApplicationBuilder applicationBuilder, WebOptions webOptions, string environment)
    {
        if (applicationBuilder == null)
            throw new ArgumentNullException(nameof(applicationBuilder));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

        if (environment == null) 
            throw new ArgumentNullException(nameof(environment));

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

        applicationBuilder
            .UseHealthChecksUI(x =>
            {
                x.PageTitle = $"{nameof(Nano)} - {webOptions.Name} Healthz v{webOptions.Version} ({environment})";
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