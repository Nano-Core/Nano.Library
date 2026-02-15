using DynamicExpression.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Extensions;
using Nano.App.Api.Mvc.HealthChecks;
using Nano.App.Api.Mvc.Middleware;
using Nano.App.Api.Mvc.Serialization.Json;
using Nano.Common.Consts;
using Nano.Common.Mvc.HealthChecks.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Globalization;
using System.Linq;
using Nano.App.Api.Mvc;
using Nano.App.Api.Mvc.Documentation;
using Vivet.AspNetCore.RequestTimeZone.Enums;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestTimeZone.Providers;
using Vivet.AspNetCore.RequestVirusScan.Extensions;
using ForwardedHeadersOptions = Nano.App.Api.Config.ForwardedHeadersOptions;
using ResponseCompressionOptions = Nano.App.Api.Config.ResponseCompressionOptions;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;
using SessionOptions = Nano.App.Api.Config.SessionOptions;

namespace Nano.App.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoExceptionHandling(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddScoped<ExceptionHandlingMiddleware>();

        return services;
    }

    internal static IServiceCollection AddNanoCors(this IServiceCollection services, CorsOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var defaultExposedHeaders = new[]
        {
            NanoHeaderNames.REQUEST_ID,
            RequestTimeZoneHeaderProvider.Headerkey,
            HeaderNames.ContentDisposition,
            NanoHeaderNames.API_SUPPORTED_VERSIONS
        };

        if (options == null)
        {
            services
                .AddCors(x =>
                {
                    x.AddPolicy(x.DefaultPolicyName, y =>
                    {
                        y.WithExposedHeaders(defaultExposedHeaders);
                    });
                });

            return services;
        }

        services
            .AddCors(x =>
            {
                x.AddPolicy(x.DefaultPolicyName, y =>
                {
                    if (options.AllowedOrigins.Length == 0)
                    {
                        y.SetIsOriginAllowed(_ => true);
                    }
                    else
                    {
                        y.WithOrigins(options.AllowedOrigins);
                        y.SetIsOriginAllowedToAllowWildcardSubdomains();
                    }

                    if (options.AllowedHeaders.Length == 0)
                    {
                        y.AllowAnyHeader();
                    }
                    else
                    {
                        y.WithHeaders(options.AllowedHeaders);
                    }

                    if (options.AllowedMethods.Length == 0)
                    {
                        y.AllowAnyMethod();
                    }
                    else
                    {
                        y.WithMethods(options.AllowedMethods);
                    }

                    if (options.AllowCredentials)
                    {
                        y.AllowCredentials();
                    }
                    else
                    {
                        y.DisallowCredentials();
                    }

                    var exposedHeaders = options.ExposedHeaders
                        .Union(defaultExposedHeaders)
                        .Distinct()
                        .ToArray();

                    y.WithExposedHeaders(exposedHeaders);
                });
            });

        return services;
    }

    internal static IServiceCollection AddNanoForwardedHeaders(this IServiceCollection services, ForwardedHeadersOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(x =>
            {
                x.AllowedHosts = ["*"];
                x.ForwardLimit = 1;
                x.ForwardedHeaders = ForwardedHeaders.All;
            });

        return services;
    }

    internal static IServiceCollection AddNanoHsts(this IServiceCollection services, HstsOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddHsts(x =>
            {
                x.MaxAge = options.MaxAge;
                x.IncludeSubDomains = options.IncludeSubdomains;

                if (options is { IncludeSubdomains: true })
                {
                    const int MAX_WEEKS = 18;
                    var weeks = options.MaxAge.TotalDays / 7;

                    if (options.UsePreload && weeks >= MAX_WEEKS)
                    {
                        x.Preload = true;
                    }
                }

                x.ExcludedHosts
                    .Clear();
            });

        return services;
    }

    internal static IServiceCollection AddNanoCookies(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddCookiePolicy(x =>
            {
                x.HttpOnly = HttpOnlyPolicy.Always;
                x.Secure = CookieSecurePolicy.SameAsRequest;
                x.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

        return services;
    }

    internal static IServiceCollection AddNanoSession(this IServiceCollection services, SessionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddSession(x =>
            {
                x.IdleTimeout = options.Timeout;
            });

        return services;
    }

    internal static IServiceCollection AddNanoResponseCaching(this IServiceCollection services, ResponseCacheOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddResponseCaching(x =>
            {
                x.UseCaseSensitivePaths = false;
                x.SizeLimit = options.MaxSize * 1024;
                x.MaximumBodySize = options.MaxBodySize * 1024;
            });

        return services;
    }

    internal static IServiceCollection AddNanoVersioning(this IServiceCollection services, string version = "1.0.0.0")
    {
        ArgumentNullException.ThrowIfNull(services);

        var parsedVersion = version
            .ParseVersion();

        var apiVersion = new ApiVersion(parsedVersion.Major, parsedVersion.Minor);

        services
            .AddApiVersioning(x =>
            {
                x.ReportApiVersions = true;
                x.DefaultApiVersion = apiVersion;
                x.AssumeDefaultVersionWhenUnspecified = true;

                x.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader(NanoHeaderNames.API_VERSION),
                    new QueryStringApiVersionReader("api-version"));
            })
            .AddVersionedApiExplorer(x =>
            {
                x.GroupNameFormat = "'v'VV";
                x.SubstituteApiVersionInUrl = true;
                x.DefaultApiVersion = apiVersion;
                x.AssumeDefaultVersionWhenUnspecified = true;
            });

        return services;
    }

    internal static IServiceCollection AddNanoRequestLocalization(this IServiceCollection services, LocalizationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddLocalization();

        return services;
    }

    internal static IServiceCollection AddNanoRequestTimeZone(this IServiceCollection services, TimeZoneOptions? options)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddRequestTimeZone(x =>
            {
                x.DefaultTimeZone = options.DefaultTimeZone;
                x.EnableRequestToUtc = true;
                x.EnableResponseToLocal = true;
                x.JsonSerializerType = JsonSerializerType.Newtonsoft;
            });

        return services;
    }

    internal static IServiceCollection AddNanoVirusScan(this IServiceCollection services, VirusScanOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        var failureStatus = options.HealthCheck?.UnhealthyStatus
            .GetHealthStatus() ?? HealthStatus.Unhealthy;

        services
            .AddRequestVirusScan(x =>
            {
                x.Host = options.Host;
                x.Port = options.Port;
                x.UseHealthCheck = options.HealthCheck != null;
                x.HealthCheckFailureStatus = failureStatus;
            });

        return services;
    }

    internal static IServiceCollection AddNanoResponseCompression(this IServiceCollection services, ResponseCompressionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddResponseCompression(x =>
            {
                x.EnableForHttps = true;

                if (options.UseGzip)
                {
                    x.Providers
                        .Add<GzipCompressionProvider>();
                }

                if (options.UseBrotli)
                {
                    x.Providers
                        .Add<BrotliCompressionProvider>();
                }
            });

        return services;
    }

    internal static IServiceCollection AddNanoRequestOptions(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddScoped<HttpRequestOptionsMiddleware>();

        return services;
    }

    internal static IServiceCollection AddNanoRequestIdentifier(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddScoped<HttpRequestIdentifierMiddleware>();

        return services;
    }

    internal static IServiceCollection AddNanoFormOptions(this IServiceCollection services, MultipartLimitsOptions? options)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = options?.MaxUploadBytes ?? int.MaxValue;
                x.MultipartBoundaryLengthLimit = 256;
                x.MultipartHeadersLengthLimit = 64 * 1024;
            });

        return services;
    }

    internal static IServiceCollection AddNanoHttpsRedirection(this IServiceCollection services, HttpOptions httpOptions, HttpsOptions? httpsOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(httpOptions);

        if (httpsOptions == null)
        {
            return services;
        }

        if (!httpOptions.UseHttpsRedirection)
        {
            return services;
        }

        if (httpsOptions.Ports.Length == 0)
        {
            return services;
        }

        services
            .AddHttpsRedirection(x =>
            {
                x.HttpsPort = httpsOptions.Ports
                    .FirstOrDefault();
            });


        services
            .AddScoped<ExceptionHandlingMiddleware>();

        return services;
    }

    internal static IServiceCollection AddNanoMvc(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddRouting(x =>
            {
                x.LowercaseUrls = true;
            })
            .AddQueryModelBinders()
            .AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>()
            .AddControllersWithViews()
            .AddNewtonsoftJson(x =>
            {
                var serializerSettings = SerializerSettings.GetMVcJsonSerializerSettings();

                x.AllowInputFormatterExceptionMessages = true;

                x.SerializerSettings.Culture = CultureInfo.CurrentCulture;
                x.SerializerSettings.NullValueHandling = serializerSettings.NullValueHandling;
                x.SerializerSettings.ReferenceLoopHandling = serializerSettings.ReferenceLoopHandling;
                x.SerializerSettings.PreserveReferencesHandling = serializerSettings.PreserveReferencesHandling;
                x.SerializerSettings.ContractResolver = serializerSettings.ContractResolver;

                foreach (var serializerSettingsConverter in serializerSettings.Converters)
                {
                    x.SerializerSettings.Converters
                        .Add(serializerSettingsConverter);
                }
            })
            .AddViewLocalization()
            .AddDataAnnotationsLocalization()
            .AddControllersAsServices();

        return services;
    }

    internal static IServiceCollection AddNanoDocumentation(this IServiceCollection services, DocumentationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddTransient<IConfigureOptions<SwaggerGenOptions>>(x =>
            {
                var authenticationSchemeProvider = x
                    .GetRequiredService<IAuthenticationSchemeProvider>();

                var apiVersionDescriptionProvider = x
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                var apiOptions = x
                    .GetRequiredService<IOptionsMonitor<ApiOptions>>();

                return new ConfigureSwaggerOptions(apiOptions, authenticationSchemeProvider, apiVersionDescriptionProvider);
            })
            .AddSwaggerGen()
            .AddSwaggerGenNewtonsoftSupport();

        return services;
    }

    internal static IServiceCollection AddNanoHealthChecking(this IServiceCollection services, int? port, HealthCheckOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddHealthChecks()
            .AddCheck<StartupHealthCheck>("startup");

        services
            .AddHealthChecksUI(x =>
            {
                x.AddHealthCheckEndpoint("app", $"http://localhost:{port ?? 80}/healthz");

                x.SetApiMaxActiveRequests(1);
                x.SetEvaluationTimeInSeconds(options.EvaluationInterval);
                x.SetMinimumSecondsBetweenFailureNotifications(options.FailureNotificationInterval);
                x.MaximumHistoryEntriesPerEndpoint(options.MaximumHistoryEntriesPerEndpoint);

                foreach (var webHook in options.WebHooks)
                {
                    x.AddWebhookNotification(webHook.Name, webHook.Uri, webHook.Payload ?? "");
                }
            })
            .AddInMemoryStorage();

        return services;
    }
}