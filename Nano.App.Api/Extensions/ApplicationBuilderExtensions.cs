using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Nano.App.Api.Mvc.Documentation.Extensions;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestTimeZone.Providers;
using Vivet.AspNetCore.RequestVirusScan.Extensions;
using ForwardedHeadersOptions = Nano.App.Api.Config.ForwardedHeadersOptions;
using SessionOptions = Nano.App.Api.Config.SessionOptions;

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

    internal static IApplicationBuilder UseNanoHttpCorsPolicy(this IApplicationBuilder applicationBuilder, CorsOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseCors()
            .Use((context, next) =>
            {
                context.Response
                    .AddCrossOriginEmbedderPolicyHeader(options.Origin.EmbedderPolicy)
                    .AddCrossOriginOpenerPolicyHeader(options.Origin.OpenerPolicy)
                    .AddCrossOriginResourcePolicyHeader(options.Origin.ResourcePolicy);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXForwardedHeaders(this IApplicationBuilder applicationBuilder, ForwardedHeadersOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseForwardedHeaders();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXRobotsTagHeaders(this IApplicationBuilder applicationBuilder, RobotsOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .Use((context, next) =>
            {
                context.Response
                    .AddXRobotsHeader(options);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXFrameOptionsPolicyHeader(this IApplicationBuilder applicationBuilder, XFrameOptionsOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .Use((context, next) =>
            {
                context.Response
                    .AddFrameOptionsPolicyHeader(options.XFrameOptionsPolicyHeader);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXXssProtectionPolicyHeader(this IApplicationBuilder applicationBuilder, XXssProtectionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseSecurityHeaders(x =>
            {
                switch (options.XssProtectionPolicyHeader)
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
                        throw new ArgumentOutOfRangeException(nameof(options.XssProtectionPolicyHeader), options.XssProtectionPolicyHeader, "Argument is out of range.");
                }
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpContentTypeOptionsPolicyHeader(this IApplicationBuilder applicationBuilder, ContentTypeOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        if (options.NoSniff)
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

    internal static IApplicationBuilder UseNanoHttpReferrerPolicyHeader(this IApplicationBuilder applicationBuilder, ReferrerPolicyOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseSecurityHeaders(x =>
            {
                switch (options.ReferrerPolicyHeader)
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
                        throw new ArgumentOutOfRangeException(nameof(options.ReferrerPolicyHeader), options.ReferrerPolicyHeader, "Argument out of range.");
                }
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpStrictTransportSecurityPolicyHeader(this IApplicationBuilder applicationBuilder, HstsOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseHsts();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpContentSecurityPolicyHeader(this IApplicationBuilder applicationBuilder, CspOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        if (options.ReportOnly)
        {
            applicationBuilder
                .UseSecurityHeaders(new HeaderPolicyCollection()
                    .AddContentSecurityPolicyReportOnly(x =>
                    {
                        if (options.UpgradeInsecureRequests)
                        {
                            x.AddUpgradeInsecureRequests();
                        }

                        if (options.BlockAllMixedContent)
                        {
                            x.AddBlockAllMixedContent();
                        }

                        x.UseCspReportUris(options.ReportUris);
                        x.UseCspDefaults(options.Defaults);
                        x.UseCspStyles(options.Styles);
                        x.UseCspScripts(options.Scripts);
                        x.UseCspObjects(options.Objects);
                        x.UseCspImages(options.Images);
                        x.UseCspMedia(options.Media);
                        x.UseCspFrames(options.Frames);
                        x.UseCspFrameAncestors(options.FrameAncestors);
                        x.UseCspFonts(options.Fonts);
                        x.UseCspConnections(options.Connections);
                        x.UseCspBaseUris(options.BaseUris);
                        x.UseCspChildren(options.Children);
                        x.UseCspForms(options.Forms);
                        x.UseCspManifests(options.Manifests);
                        x.UseCspWorkers(options.Workers);
                        x.UseCspSandbox(options.Sandbox);
                    }));
        }
        else
        {
            applicationBuilder
                .UseSecurityHeaders(new HeaderPolicyCollection()
                    .AddContentSecurityPolicy(x =>
                    {
                        if (options.UpgradeInsecureRequests)
                        {
                            x.AddUpgradeInsecureRequests();
                        }

                        if (options.BlockAllMixedContent)
                        {
                            x.AddBlockAllMixedContent();
                        }

                        x.UseCspReportUris(options.ReportUris);
                        x.UseCspDefaults(options.Defaults);
                        x.UseCspStyles(options.Styles);
                        x.UseCspScripts(options.Scripts);
                        x.UseCspObjects(options.Objects);
                        x.UseCspImages(options.Images);
                        x.UseCspMedia(options.Media);
                        x.UseCspFrames(options.Frames);
                        x.UseCspFrameAncestors(options.FrameAncestors);
                        x.UseCspFonts(options.Fonts);
                        x.UseCspConnections(options.Connections);
                        x.UseCspBaseUris(options.BaseUris);
                        x.UseCspChildren(options.Children);
                        x.UseCspForms(options.Forms);
                        x.UseCspManifests(options.Manifests);
                        x.UseCspWorkers(options.Workers);
                        x.UseCspSandbox(options.Sandbox);
                    }));
        }

        applicationBuilder
            .Use((context, next) =>
            {
                context.Response
                    .AddPermissionsPolicyHeader(options.PermissionsPolicy);

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoSession(this IApplicationBuilder applicationBuilder, SessionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
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

    internal static IApplicationBuilder UseNanoRequestVirusScan(this IApplicationBuilder applicationBuilder, VirusScanOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
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

    internal static IApplicationBuilder UseNanoResponseCompression(this IApplicationBuilder applicationBuilder, ResponseCompressionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseResponseCompression();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoResponseCaching(this IApplicationBuilder applicationBuilder, ResponseCacheOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .Use((context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = options.MaxAge
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

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoDocumentataion(this IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment, string version = "1.0.0", DocumentationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(webHostEnvironment);

        if (options == null)
        {
            return applicationBuilder;
        }

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
                    var defaultVersion = version
                        .ParseVersion();

                    var defaultApiVersion = new ApiVersion(defaultVersion.Major, defaultVersion.Minor);

                    var defaultVersionText = options.UseDefaultVersion && description.ApiVersion == defaultApiVersion
                        ? " (Default)"
                        : string.Empty;

                    x.SwaggerEndpoint($"{description.GroupName}/swagger.json", $"{nameof(Nano)} - {webHostEnvironment.ApplicationName} {description.ApiVersion}{defaultVersionText} ({webHostEnvironment.EnvironmentName})");
                }

                x.RoutePrefix = "docs";
                x.DocumentTitle = $"{nameof(Nano)} - {webHostEnvironment.ApplicationName} Docs ({webHostEnvironment.EnvironmentName})";

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
                x.UseCspNonce(options.CspNonce);
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHealthChecks(this IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment, string version = "1.0.0", HealthCheckOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(webHostEnvironment);

        if (options == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseHealthChecks(HealthzCheckUris.Path, new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = _ => true,
                AllowCachingResponses = true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        applicationBuilder
            .UseHealthChecksUI(x =>
            {
                x.PageTitle = $"{nameof(Nano)} - {webHostEnvironment.ApplicationName} Healthz v{version} ({webHostEnvironment.EnvironmentName})";
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