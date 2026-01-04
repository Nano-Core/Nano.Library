using DynamicExpression.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.App.Web.Config;
using Nano.App.Web.Mvc.HealthChecks;
using Nano.App.Web.Mvc.Middleware;
using Nano.App.Web.Mvc.Options;
using Nano.App.Web.Mvc.Serialization.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Nano.App.Web.Mvc.Documentation.Filters.Document;
using Vivet.AspNetCore.RequestTimeZone.Enums;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestVirusScan.Extensions;

namespace Nano.App.Web.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoExceptionHandling(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        services
            .AddScoped<ExceptionHandlingMiddleware>();

        return services;
    }
    
    internal static IServiceCollection AddNanoCors(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.Cors == null)
        {
            return services;
        }

        services
            .AddCors(x =>
            {
                x.AddPolicy(x.DefaultPolicyName, y =>
                {
                    if (webOptions.HttpPolicyHeaders.Cors.AllowedOrigins.Any())
                    {
                        y.WithOrigins(webOptions.HttpPolicyHeaders.Cors.AllowedOrigins);
                        y.SetIsOriginAllowedToAllowWildcardSubdomains();
                    }
                    else
                    {
                        y.SetIsOriginAllowed(_ => true);
                    }

                    if (webOptions.HttpPolicyHeaders.Cors.AllowedHeaders.Any())
                    {
                        y.WithHeaders(webOptions.HttpPolicyHeaders.Cors.AllowedHeaders);
                    }
                    else
                    {
                        y.AllowAnyHeader();
                    }

                    if (webOptions.HttpPolicyHeaders.Cors.AllowedMethods.Any())
                    {
                        y.WithMethods(webOptions.HttpPolicyHeaders.Cors.AllowedMethods);
                    }
                    else
                    {
                        y.AllowAnyMethod();
                    }

                    if (webOptions.HttpPolicyHeaders.Cors.AllowCredentials)
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
    
    internal static IServiceCollection AddNanoForwardedHeaders(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        services
            .Configure<ForwardedHeadersOptions>(x =>
            {
                x.ForwardedHeaders = ForwardedHeaders.All;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoHsts(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.HttpPolicyHeaders.Cors == null)
        {
            return services;
        }

        services
            .AddHsts(x =>
            {
                x.IncludeSubDomains = webOptions.HttpPolicyHeaders.Hsts.IncludeSubdomains;

                if (webOptions.HttpPolicyHeaders.Hsts.IncludeSubdomains)
                {
                    if (webOptions.HttpPolicyHeaders.Hsts.MaxAge.HasValue)
                    {
                        const int MAX_WEEKS = 18;
                        var weeks = webOptions.HttpPolicyHeaders.Hsts.MaxAge.Value.TotalDays / 7;

                        if (webOptions.HttpPolicyHeaders.Hsts.UsePreload && weeks >= MAX_WEEKS)
                        {
                            x.Preload = true;
                        }
                    }
                }

                if (webOptions.HttpPolicyHeaders.Hsts.MaxAge.HasValue)
                {
                    x.MaxAge = webOptions.HttpPolicyHeaders.Hsts.MaxAge.Value;
                }
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoCookies(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        services
            .AddCookiePolicy(x =>
            {
                x.Secure = CookieSecurePolicy.Always;
                x.MinimumSameSitePolicy = SameSiteMode.Strict;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoSession(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.Session == null)
        {
            return services;
        }

        services
            .AddSession(x =>
            {
                x.IdleTimeout = webOptions.Session.Timeout;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoResponseCaching(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.ResponseCache == null)
        {
            return services;
        }

        services
            .AddResponseCaching(x =>
            {
                x.UseCaseSensitivePaths = false;
                x.SizeLimit = webOptions.ResponseCache.MaxSize * 1024;
                x.MaximumBodySize = webOptions.ResponseCache.MaxBodySize * 1024;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoVersioning(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        var version = webOptions.Version
            .ParseVersion();

        var apiVersion = new ApiVersion(version.Major, version.Minor);

        services
            .AddApiVersioning(x =>
            {
                x.ReportApiVersions = true;

                if (webOptions.Documentation.UseDefaultVersion)
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

                if (webOptions.Documentation.UseDefaultVersion)
                {
                    x.DefaultApiVersion = apiVersion;
                    x.AssumeDefaultVersionWhenUnspecified = true;
                }
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoRequestLocalization(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        services
            .AddLocalization();

        return services;
    }
    
    internal static IServiceCollection AddNanoRequestTimeZone(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        services
            .AddRequestTimeZone(x =>
            {
                x.Id = webOptions.DefaultTimeZone;
                x.EnableRequestToUtc = true;
                x.EnableResponseToLocal = true;
                x.JsonSerializerType = JsonSerializerType.Newtonsoft;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoVirusScan(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.VirusScan == null)
        {
            return services;
        }

        services
            .AddRequestVirusScan(x =>
            {
                x.Host = webOptions.VirusScan.Host;
                x.Port = webOptions.VirusScan.Port;
                x.UseHealthCheck = webOptions.VirusScan.UseHealthCheck;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoResponseCompression(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null) 
            throw new ArgumentNullException(nameof(webOptions));

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
    
    internal static IServiceCollection AddNanoRequestOptions(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        services
            .AddScoped<HttpRequestOptionsMiddleware>();

        return services;
    }
    
    internal static IServiceCollection AddNanoRequestIdentifier(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        services
            .AddScoped<HttpRequestIdentifierMiddleware>();

        return services;
    }
    
    internal static IServiceCollection AddNanoFormOptions(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        services
            .Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

        return services;
    }
    
    internal static IServiceCollection AddNanoMvc(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

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

    internal static IServiceCollection AddNanoDocumentation(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.Documentation == null)
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
                    .GetRequiredService<IOptionsMonitor<WebOptions>>();

                return new ConfigureSwaggerOptions(options, authenticationSchemeProvider, apiVersionDescriptionProvider);
            })
            .AddSwaggerGen()
            .AddSwaggerGenNewtonsoftSupport();

        return services;
    }

    internal static IServiceCollection AddNanoHealthChecking(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        services
            .AddHealthChecks()
            .AddCheck<StartupHealthCheck>("startup");

        if (webOptions.HealthCheck == null)
        {
            return services;
        }
        
        services
            .AddHealthChecksUI(x =>
            {
                var port = webOptions.Hosting.Ports
                    .FirstOrDefault();

                x.AddHealthCheckEndpoint(webOptions.Name.ToLower(), $"http://localhost:{port}/healthz"); 

                x.SetApiMaxActiveRequests(1);
                x.SetEvaluationTimeInSeconds(webOptions.HealthCheck.EvaluationInterval);
                x.SetMinimumSecondsBetweenFailureNotifications(webOptions.HealthCheck.FailureNotificationInterval);
                x.MaximumHistoryEntriesPerEndpoint(webOptions.HealthCheck.MaximumHistoryEntriesPerEndpoint);

                foreach (var webHook in webOptions.HealthCheck.WebHooks)
                {
                    x.AddWebhookNotification(webHook.Name, webHook.Uri, webHook.Payload);
                }
            })
            .AddInMemoryStorage();

        return services;
    }
}