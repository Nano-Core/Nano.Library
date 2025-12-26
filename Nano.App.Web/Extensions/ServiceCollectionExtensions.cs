using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using DynamicExpression.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.App.ApiClient.Consts;
using Nano.App.Web.Config;
using Nano.App.Web.Identity.Authentication.Consts;
using Nano.App.Web.Identity.Authentication.Extensions;
using Nano.Common.Config;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Nano.App.Web.Controllers;
using Nano.App.Web.Mvc.Conventions;
using Nano.App.Web.Mvc.Documentation.Config;
using Nano.App.Web.Mvc.Features;
using Nano.App.Web.Mvc.HealthChecks;
using Nano.App.Web.Mvc.Middleware;
using Nano.App.Web.Mvc.Serialization.Json.Const;
using Nano.Data.Abstractions.Config;
using Vivet.AspNetCore.RequestTimeZone.Enums;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestVirusScan.Extensions;
using AuthenticationOptions = Nano.App.Web.Config.AuthenticationOptions;

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
                // BUG: More options
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
                // BUG: More options, check what else we do with cookies. I think I removed something. check old code
                x.Secure = CookieSecurePolicy.SameAsRequest;
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

        var apiVersion = new ApiVersion(ConfigManager.Version.Major, ConfigManager.Version.Minor);

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
            .AddApiExplorer(x =>
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
    
    internal static IServiceCollection AddNanoIdentityAuthenticationAndAuthorization(this IServiceCollection services, AuthenticationOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.Jwt == null && options.ApiKey == null)
        {
            return services;
        }

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap
            .Clear();

        var authenticationSchemes = new List<string>();

        if (options.Jwt != null)
        {
            authenticationSchemes
                .Add(JwtBearerDefaults.AuthenticationScheme);
        }

        if (options.ApiKey != null)
        {
            authenticationSchemes
                .Add(ApiKeyDefaults.AuthenticationScheme);
        }

        services
            .AddAuthorization(x =>
            {
                x.FallbackPolicy = null;
                x.InvokeHandlersAfterFailure = false;

                x.AddPolicy(AuthenticationPolicies.POLICY, y => y
                    .AddAuthenticationSchemes(authenticationSchemes.ToArray())
                    .RequireAuthenticatedUser());
            });

        var defaultAuthenticationScheme = authenticationSchemes
            .FirstOrDefault();

        var authenticationBuilder = services
            .AddAuthentication(x =>
            {
                x.DefaultScheme = defaultAuthenticationScheme;
                x.DefaultChallengeScheme = defaultAuthenticationScheme;
                x.DefaultAuthenticateScheme = defaultAuthenticationScheme;
                x.DefaultForbidScheme = defaultAuthenticationScheme;
                x.DefaultSignInScheme = defaultAuthenticationScheme;
                x.DefaultSignOutScheme = defaultAuthenticationScheme;
            });

        authenticationBuilder
            .AddJwtAuthentication(options.Jwt);
        // BUG: Move to Data where we know TIdentity. hmm we have the authentication options here, maybe we need split it
        //.AddApiKeyAuthentication<TIdentity>(options.ApiKey);

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
            .AddControllersWithViews(x =>
            {
                x.ReturnHttpNotAcceptable = true;
                x.RespectBrowserAcceptHeader = true;
                x.MaxValidationDepth = 128;

                x.FormatterMappings
                    .SetMediaTypeMappingForFormat("json", HttpContentType.JSON);

                var routeAttribute = new RouteAttribute(webOptions.Hosting.Root);
                var routePrefixConvention = new RoutePrefixConvention(routeAttribute);
                var producesJsonConvention = new ProducesJsonConvention();

                x.Conventions
                    .Insert(0, routePrefixConvention);

                x.Conventions
                    .Add(producesJsonConvention);

                if (webOptions.Hosting.UseHttpsRequired)
                {
                    x.Filters
                        .Add<RequireHttpsAttribute>();
                }

                // BUG: 000: Remove actions (Auth, Transient Auth, Identity)
                x.Conventions.Add(new ConditionalActionConvention((controller, action) =>
                {
                    // Example: hide AuditController.DoSomething action
                    if (controller.ControllerType == typeof(AuditController) && action.ActionName == "DoSomething")
                    {
                        return false; // exclude this action
                    }

                    return true; // include others
                }));
            })
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
        
        services
            .AddSingleton<IStartupFilter, ConditionalControllerFeatureStartupFilter>();

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
                var apiVersionDescriptionProvider = x
                    .GetRequiredService<IApiVersionDescriptionProvider>();

                var options = x
                    .GetRequiredService<IOptionsMonitor<WebOptions>>();

                return new ConfigureSwaggerOptions(apiVersionDescriptionProvider, options);
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

        if (webOptions.HealthCheck == null)
        {
            return services;
        }

        services
            .AddHealthChecks()
            .AddCheck<StartupHealthCheck>("self");

        if (webOptions.HealthCheck.UseHealthCheckUi)
        {
            var port = webOptions.Hosting.Ports.FirstOrDefault();

            // TODO: HealthChecks UI: Doesn't poll: JS: Configured polling interval: NaN milliseconds (https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/issues/636)
            services
                .AddHealthChecksUI(x =>
                {
                    // BUG: ASK CHAT-GPT: AddHealthCheckEndpoint
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
        }

        return services;
    }
}