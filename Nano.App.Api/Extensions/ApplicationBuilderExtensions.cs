using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Consts;
using Nano.App.Api.Mvc.Documentation.Extensions;
using Nano.App.Api.Mvc.Extensions;
using Nano.App.Api.Mvc.HealthChecks.Const;
using Nano.App.Api.Mvc.Middleware;
using Nano.Common.Consts;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

    internal static IApplicationBuilder UseNanoHttpsRedirection(this IApplicationBuilder applicationBuilder, HttpOptions httpOptions, HttpsOptions? httpsOptions = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(httpOptions);

        if (httpsOptions == null || httpsOptions.Ports.Length == 0 || !httpOptions.UseHttpsRedirection)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseHttpsRedirection();

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
                    .OnStarting(() =>
                    {
                        context.Response
                            .AddCrossOriginEmbedderPolicyHeader(options.Origin.EmbedderPolicy)
                            .AddCrossOriginOpenerPolicyHeader(options.Origin.OpenerPolicy)
                            .AddCrossOriginResourcePolicyHeader(options.Origin.ResourcePolicy);

                        return Task.CompletedTask;
                    });

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

        var forwardedHeadersOptions = new Microsoft.AspNetCore.Builder.ForwardedHeadersOptions
        {
            ForwardedHeaders = options.Headers,
            RequireHeaderSymmetry = options.RequireHeaderSymmetry,
            ForwardLimit = null
        };

        forwardedHeadersOptions.KnownProxies
            .Clear();

        forwardedHeadersOptions.KnownIPNetworks
            .Clear();

        applicationBuilder
            .UseForwardedHeaders(forwardedHeadersOptions);

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
                    .OnStarting(() =>
                    {
                        context.Response
                            .AddXRobotsHeader(options);

                        return Task.CompletedTask;
                    });

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXFrameOptionsPolicyHeader(this IApplicationBuilder applicationBuilder, FrameOptionsOptions? options = null)
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
                    .OnStarting(() =>
                    {
                        context.Response
                            .AddXFrameOptionsPolicyHeader(options.FrameOptionsPolicyHeader);

                        return Task.CompletedTask;
                    });

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoHttpXXssProtectionPolicyHeader(this IApplicationBuilder applicationBuilder, XssProtectionOptions? options = null)
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
                    .OnStarting(() =>
                    {
                        context.Response
                            .AddXXssProtectionPolicyHeader(options);

                        return Task.CompletedTask;
                    });

                return next();
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

        applicationBuilder
            .Use((context, next) =>
            {
                context.Response
                    .OnStarting(() =>
                    {
                        context.Response
                            .AddContentTypeOptionsHeader(options);

                        return Task.CompletedTask;
                    });

                return next();
            });

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
            .Use((context, next) =>
            {
                context.Response
                    .OnStarting(() =>
                    {
                        context.Response
                            .AddReferrerPolicyHeader(options);

                        return Task.CompletedTask;
                    });

                return next();
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

        applicationBuilder
            .Use((context, next) =>
            {
                context.Response
                    .OnStarting(() =>
                    {
                        context.Response
                            .AddContentSecurityPolicyHeader(options);

                        return Task.CompletedTask;
                    });

                return next();
            });

        if (options.PermissionsPolicy != null)
        {
            applicationBuilder
                .Use((context, next) =>
                {
                    context.Response
                        .OnStarting(() =>
                        {
                            context.Response
                                .AddContentSecurityPolicyPermissionsHeader(options.PermissionsPolicy);

                            return Task.CompletedTask;
                        });

                    return next();
                });
        }

        if (options.ReportTo != null)
        {
            applicationBuilder
                .Use((context, next) =>
                {
                    context.Response
                        .OnStarting(() =>
                        {
                            context.Response
                                .AddContentSecurityPolicyReportToHeader(options.ReportTo);

                            return Task.CompletedTask;
                        });

                    return next();
                });

            if (options.ReportTo.Endpoints.Length == 0)
            {
                applicationBuilder
                    .Map(ActionRoutes.CSP_REPORT_TO, builder =>
                    {
                        builder
                            .Run(async context =>
                            {
                                if (!HttpMethods.IsPost(context.Request.Method))
                                {
                                    context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;

                                    return;
                                }

                                var isContentTypeJson = context.Request.ContentType?
                                    .StartsWith(HttpContentType.JSON) ?? false;

                                var isContentTypeReportsJson = context.Request.ContentType?
                                    .StartsWith(HttpContentType.REPORTS_JSON) ?? false;

                                if (!isContentTypeJson && !isContentTypeReportsJson)
                                {
                                    context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;

                                    return;
                                }

                                context.Request
                                    .EnableBuffering();

                                using var reader = new StreamReader(context.Request.Body);

                                var body = await reader
                                    .ReadToEndAsync();

                                context.Request.Body.Position = 0;

                                var message = JArray.Parse(body).ToString();

                                var logger = context.RequestServices
                                    .GetRequiredService<ILogger>();

                                logger
                                    .LogWarning(message);

                                context.Response.StatusCode = StatusCodes.Status200OK;
                            });
                    });
            }
        }

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

    internal static IApplicationBuilder UseNanoRequestIdentifier(this IApplicationBuilder applicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        applicationBuilder
            .Use((context, next) =>
            {
                var requestId = context.Request.Headers[NanoHeaderNames.REQUEST_ID].FirstOrDefault() ?? context.TraceIdentifier;

                context.Request.Headers[NanoHeaderNames.REQUEST_ID] = requestId;

                context.Response
                    .OnStarting(() =>
                    {
                        context.Response.Headers[NanoHeaderNames.REQUEST_ID] = requestId;

                        return Task.CompletedTask;
                    });

                return next();
            });

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoApiVersion(this IApplicationBuilder applicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        applicationBuilder
            .Use((context, next) =>
            {
                var apiVersion = context
                    .GetRequestedApiVersion();

                context.Response
                    .OnStarting(() =>
                    {
                        if (apiVersion != null)
                        {
                            var versionString = $"{apiVersion.MajorVersion ?? 0}.{apiVersion.MinorVersion ?? 0}";

                            context.Response.Headers[NanoHeaderNames.API_VERSION] = versionString;
                        }

                        return Task.CompletedTask;
                    });

                return next();
            });

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

    internal static IApplicationBuilder UseNanoRequestTimeZone(this IApplicationBuilder applicationBuilder, TimeZoneOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        applicationBuilder
            .UseRequestTimeZone();

        return applicationBuilder;
    }

    internal static IApplicationBuilder UseNanoRequestLocalization(this IApplicationBuilder applicationBuilder, LocalizationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        if (options == null)
        {
            return applicationBuilder;
        }

        var cultureInfos = options.SupportedCultures
            .Select(y => new CultureInfo(y))
            .ToArray();

        applicationBuilder
            .UseRequestLocalization(x =>
            {
                x.DefaultRequestCulture = new RequestCulture(options.DefaultCulture);
                x.SupportedCultures = cultureInfos;
                x.SupportedUICultures = cultureInfos;
            });

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

    internal static IApplicationBuilder UseNanoDocumentataion(this IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment, string version = "1.0.0.0", DocumentationOptions? options = null)
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

                    var defaultVersionText = options.HideDefaultVersion && description.ApiVersion == defaultApiVersion
                        ? " (Default)"
                        : string.Empty;

                    x.SwaggerEndpoint($"{description.GroupName}/swagger.json", $"{options.Name} {description.ApiVersion}{defaultVersionText} ({webHostEnvironment.EnvironmentName})");
                }

                x.RoutePrefix = "docs";
                x.DocumentTitle = $"Docs ({webHostEnvironment.EnvironmentName})";

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

    internal static IApplicationBuilder UseNanoHealthChecks(this IApplicationBuilder applicationBuilder, IWebHostEnvironment webHostEnvironment, HealthCheckOptions? options = null)
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
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponseNoExceptionDetails
            });

        applicationBuilder
            .UseHealthChecksUI(x =>
            {
                x.PageTitle = $"Healthz ({webHostEnvironment.EnvironmentName})";
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