using System;
using System.Globalization;
using System.Linq;
using DynamicExpression.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Documentation.Filters.Document;
using Nano.App.Api.Mvc.HealthChecks;
using Nano.App.Api.Mvc.Middleware;
using Nano.App.Api.Mvc.Options;
using Nano.App.Api.Mvc.Serialization.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using Vivet.AspNetCore.RequestTimeZone.Enums;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestVirusScan.Extensions;

namespace Nano.App.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoExceptionHandling(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        services
            .AddScoped<ExceptionHandlingMiddleware>();

        return services;
    }
    
    internal static IServiceCollection AddNanoCors(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        if (apiOptions.HttpPolicyHeaders.Cors == null)
        {
            return services;
        }

        services
            .AddCors(x =>
            {
                x.AddPolicy(x.DefaultPolicyName, y =>
                {
                    if (apiOptions.HttpPolicyHeaders.Cors.AllowedOrigins.Any())
                    {
                        y.WithOrigins(apiOptions.HttpPolicyHeaders.Cors.AllowedOrigins);
                        y.SetIsOriginAllowedToAllowWildcardSubdomains();
                    }
                    else
                    {
                        y.SetIsOriginAllowed(_ => true);
                    }

                    if (apiOptions.HttpPolicyHeaders.Cors.AllowedHeaders.Any())
                    {
                        y.WithHeaders(apiOptions.HttpPolicyHeaders.Cors.AllowedHeaders);
                    }
                    else
                    {
                        y.AllowAnyHeader();
                    }

                    if (apiOptions.HttpPolicyHeaders.Cors.AllowedMethods.Any())
                    {
                        y.WithMethods(apiOptions.HttpPolicyHeaders.Cors.AllowedMethods);
                    }
                    else
                    {
                        y.AllowAnyMethod();
                    }

                    if (apiOptions.HttpPolicyHeaders.Cors.AllowCredentials)
                    {
                        y.AllowCredentials();
                    }
                    else
                    {
                        y.DisallowCredentials();
                    }

                    y.WithExposedHeaders("RequestId", "TZ", "Content-Disposition", "api-supported-versions");
                });
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoForwardedHeaders(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        services
            .Configure<ForwardedHeadersOptions>(x =>
            {
                x.ForwardedHeaders = ForwardedHeaders.All;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoHsts(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        if (apiOptions.HttpPolicyHeaders.Cors == null)
        {
            return services;
        }

        services
            .AddHsts(x =>
            {
                x.IncludeSubDomains = apiOptions.HttpPolicyHeaders.Hsts.IncludeSubdomains;

                if (apiOptions.HttpPolicyHeaders.Hsts.IncludeSubdomains)
                {
                    if (apiOptions.HttpPolicyHeaders.Hsts.MaxAge.HasValue)
                    {
                        const int MAX_WEEKS = 18;
                        var weeks = apiOptions.HttpPolicyHeaders.Hsts.MaxAge.Value.TotalDays / 7;

                        if (apiOptions.HttpPolicyHeaders.Hsts.UsePreload && weeks >= MAX_WEEKS)
                        {
                            x.Preload = true;
                        }
                    }
                }

                if (apiOptions.HttpPolicyHeaders.Hsts.MaxAge.HasValue)
                {
                    x.MaxAge = apiOptions.HttpPolicyHeaders.Hsts.MaxAge.Value;
                }
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoCookies(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        services
            .AddCookiePolicy(x =>
            {
                x.Secure = CookieSecurePolicy.Always;
                x.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoSession(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        if (apiOptions.Session == null)
        {
            return services;
        }

        services
            .AddSession(x =>
            {
                x.IdleTimeout = apiOptions.Session.Timeout;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoResponseCaching(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        if (apiOptions.ResponseCache == null)
        {
            return services;
        }

        services
            .AddResponseCaching(x =>
            {
                x.UseCaseSensitivePaths = false;
                x.SizeLimit = apiOptions.ResponseCache.MaxSize * 1024;
                x.MaximumBodySize = apiOptions.ResponseCache.MaxBodySize * 1024;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoVersioning(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        var version = apiOptions.Version
            .ParseVersion();

        var apiVersion = new ApiVersion(version.Major, version.Minor);

        services
            .AddApiVersioning(x =>
            {
                x.ReportApiVersions = true;

                if (apiOptions.Documentation.UseDefaultVersion)
                {
                    x.DefaultApiVersion = apiVersion;
                    x.AssumeDefaultVersionWhenUnspecified = true;
                }

                x.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("Api-Version"));
            })
            .AddVersionedApiExplorer(x =>
            {
                x.GroupNameFormat = "'v'VV";
                x.SubstituteApiVersionInUrl = true;

                if (apiOptions.Documentation.UseDefaultVersion)
                {
                    x.DefaultApiVersion = apiVersion;
                    x.AssumeDefaultVersionWhenUnspecified = true;
                }
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoRequestLocalization(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        services
            .AddLocalization();

        return services;
    }
    
    internal static IServiceCollection AddNanoRequestTimeZone(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        services
            .AddRequestTimeZone(x =>
            {
                x.Id = apiOptions.DefaultTimeZone;
                x.EnableRequestToUtc = true;
                x.EnableResponseToLocal = true;
                x.JsonSerializerType = JsonSerializerType.Newtonsoft;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoVirusScan(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        if (apiOptions.VirusScan == null)
        {
            return services;
        }

        services
            .AddRequestVirusScan(x =>
            {
                x.Host = apiOptions.VirusScan.Host;
                x.Port = apiOptions.VirusScan.Port;
                x.UseHealthCheck = apiOptions.VirusScan.UseHealthCheck;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoResponseCompression(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null) 
            throw new ArgumentNullException(nameof(apiOptions));

        services
            .AddResponseCompression(x =>
            {
                x.EnableForHttps = true;

                x.Providers
                    .Add<GzipCompressionProvider>();

                x.Providers
                    .Add<BrotliCompressionProvider>();
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoRequestOptions(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        services
            .AddScoped<HttpRequestOptionsMiddleware>();

        return services;
    }
    
    internal static IServiceCollection AddNanoRequestIdentifier(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        services
            .AddScoped<HttpRequestIdentifierMiddleware>();

        return services;
    }
    
    internal static IServiceCollection AddNanoFormOptions(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        services
            .Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoMvc(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

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
                x.AllowInputFormatterExceptionMessages = true;

                var serializerSettings = SerializerSettings.GetMVcJsonSerializerSettings();

                x.SerializerSettings.Culture = CultureInfo.CurrentCulture;
                x.SerializerSettings.NullValueHandling = serializerSettings.NullValueHandling;
                x.SerializerSettings.ReferenceLoopHandling = serializerSettings.ReferenceLoopHandling;
                x.SerializerSettings.PreserveReferencesHandling = serializerSettings.PreserveReferencesHandling;
                x.SerializerSettings.ContractResolver = serializerSettings.ContractResolver;
                x.SerializerSettings.Converters = serializerSettings.Converters;
            })
            .AddViewLocalization()
            .AddDataAnnotationsLocalization()
            .AddControllersAsServices();

        return services;
    }

    internal static IServiceCollection AddNanoDocumentation(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        if (apiOptions.Documentation == null)
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
                
                var options = x
                    .GetRequiredService<IOptionsMonitor<ApiOptions>>();

                return new ConfigureSwaggerOptions(options, authenticationSchemeProvider, apiVersionDescriptionProvider);
            })
            .AddSwaggerGen()
            .AddSwaggerGenNewtonsoftSupport();

        return services;
    }

    internal static IServiceCollection AddNanoHealthChecking(this IServiceCollection services, ApiOptions apiOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (apiOptions == null)
            throw new ArgumentNullException(nameof(apiOptions));

        services
            .AddHealthChecks()
            .AddCheck<StartupHealthCheck>("startup");

        if (apiOptions.HealthCheck == null)
        {
            return services;
        }
        
        services
            .AddHealthChecksUI(x =>
            {
                var port = apiOptions.Hosting.Ports
                    .FirstOrDefault();

                x.AddHealthCheckEndpoint(apiOptions.Name.ToLower(), $"http://localhost:{port}/healthz"); 

                x.SetApiMaxActiveRequests(1);
                x.SetEvaluationTimeInSeconds(apiOptions.HealthCheck.EvaluationInterval);
                x.SetMinimumSecondsBetweenFailureNotifications(apiOptions.HealthCheck.FailureNotificationInterval);
                x.MaximumHistoryEntriesPerEndpoint(apiOptions.HealthCheck.MaximumHistoryEntriesPerEndpoint);

                foreach (var webHook in apiOptions.HealthCheck.WebHooks)
                {
                    x.AddWebhookNotification(webHook.Name, webHook.Uri, webHook.Payload);
                }
            })
            .AddInMemoryStorage();

        return services;
    }
}