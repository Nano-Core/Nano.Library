using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using DynamicExpression.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Nano.App.ApiClient.Consts;
using Nano.App.Config;
using Nano.App.Web.Config;
using Nano.App.Web.Controllers;
using Nano.App.Web.Identity;
using Nano.App.Web.Identity.Abstractions;
using Nano.App.Web.Identity.Authentication.Consts;
using Nano.App.Web.Identity.Authentication.Extensions;
using Nano.Common.Config.Extensions;
using Nano.Common.Config.Helpers;
using Nano.Common.Extensions;
using Nano.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.XPath;
using Nano.App.Web.Conventions;
using Nano.App.Web.Documentation.Filters.Document;
using Nano.App.Web.Documentation.Filters.Operation;
using Nano.App.Web.Documentation.Filters.Schema;
using Nano.App.Web.HealthChecks;
using Nano.App.Web.Middleware;
using Nano.App.Web.Serialization.Json.Const;
using Vivet.AspNetCore.RequestTimeZone.Enums;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestVirusScan.Extensions;
using AuthenticationOptions = Nano.App.Web.Config.AuthenticationOptions;

namespace Nano.App.Web.Extensions;

internal sealed class ConditionalControllerFeatureProvider : ControllerFeatureProvider
{
    private readonly Func<TypeInfo, bool> predicate;

    internal ConditionalControllerFeatureProvider(Func<TypeInfo, bool> predicate)
    {
        this.predicate = predicate;
    }

    protected override bool IsController(TypeInfo typeInfo)
    {
        if (!base.IsController(typeInfo))
            return false;

        return predicate(typeInfo);
    }
}
internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        services
            .AddConfigSection<WebOptions>(BaseAppOptions.SectionName, out var webOptions);

        services
            .AddCors(webOptions)
            .AddSession(webOptions)
            .AddCaching(webOptions)
            .AddVersioning(webOptions)
            .AddIdentityAuthenticationAndAuthorization(webOptions.Identity.Authentication)
            .AddDocumentation(webOptions)
            .AddLocalization()
            .AddTimeZone(webOptions)
            .AddVirusScan(webOptions)
            .AddCompression()
            .Configure<ForwardedHeadersOptions>(x =>
            {
                x.ForwardedHeaders = ForwardedHeaders.All;
            })
            .Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            })
            .AddScoped<ExceptionHandlingMiddleware>()
            .AddScoped<HttpRequestOptionsMiddleware>()
            .AddScoped<HttpRequestIdentifierMiddleware>()
            .AddRouting(x => x.LowercaseUrls = true)
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

                x.Conventions
                    .Insert(0, routePrefixConvention);

                x.Conventions
                    .Add(new ProducesJsonConvention());

                if (webOptions.Hosting.UseHttpsRequired)
                {
                    x.Filters
                        .Add<RequireHttpsAttribute>();
                }
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
            .AddControllersAsServices()

            // BUG: 000: Remove disabled controllers like Identity if not configured or data is not configured.
            // We should hide/remove controller actions that are not configured (jwt, api-key). possibly return 404 in middleware

            // 000: Remove controllers and actions not configured,
            // e.g. Identity repository methods for data if data isn't configured. Also, Transient from Auth controller if not configured,
            // and data from auth controller if not configured.
            .AddApplicationPart(typeof(WebOptions).GetTypeInfo().Assembly)
            .ConfigureApplicationPartManager(manager =>
            {
                manager.FeatureProviders.Add(
                    new ConditionalControllerFeatureProvider(type =>
                    {
                        //if (!webOptions.Hosting.ExposeAuthController &&
                        //    type.AsType() == typeof(AuthController))
                        //    return false;

                        //if (!webOptions.Hosting.ExposeAuditController &&
                        //    type.AsType() == typeof(AuditController))
                        //    return false;

                        return true;
                    })
                );
            });

        services
            .AddScoped<IIdentityJwtRepository, IdentityJwtRepository>()
            .AddScoped<IAuthRepository, DefaultAuthRepository>()
            .AddScoped<IAuthTransientRepository, DefaultAuthTransientRepository>();

        services
            .AddHealthChecking(webOptions);

        return services;
    }


    private static IServiceCollection AddCors(this IServiceCollection services, WebOptions webOptions)
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
    private static IServiceCollection AddSession(this IServiceCollection services, WebOptions webOptions)
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
    private static IServiceCollection AddCaching(this IServiceCollection services, WebOptions webOptions)
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
                x.SizeLimit = webOptions.ResponseCache.MaxSize * 1024;
                x.MaximumBodySize = webOptions.ResponseCache.MaxBodySize * 1024;
            });

        return services;
    }
    private static IServiceCollection AddVersioning(this IServiceCollection services, WebOptions webOptions)
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
    private static IServiceCollection AddCompression(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddResponseCompression(x =>
            {
                x.EnableForHttps = true;
                x.Providers.Add<GzipCompressionProvider>();
                x.Providers.Add<BrotliCompressionProvider>();
            });

        return services;
    }
    private static IServiceCollection AddHealthChecking(this IServiceCollection services, WebOptions webOptions)
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
                    // BUG: ASK CHAT-GPT:  x.AddHealthCheckEndpoint(appOptions.Name.ToLower(), $"http://localhost:{port}/healthz");

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
    private static IServiceCollection AddDocumentation(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.Documentation == null)
        {
            return services;
        }

        // BUG: 000: BuildServiceProvider(). ONLY place
        var apiVersionDescriptionProvider = services
            .BuildServiceProvider()
            .GetService<IApiVersionDescriptionProvider>();

        services
            .AddSwaggerGen(x =>
            {
                foreach (var provider in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    var info = new OpenApiInfo
                    {
                        Title = webOptions.Name,
                        Description = webOptions.Documentation.Description,
                        Version = provider.ApiVersion.ToString(),
                    };

                    if (webOptions.Documentation.Contact != null)
                    {
                        info.Contact = webOptions.Documentation.Contact;
                    }

                    if (webOptions.Documentation.License != null)
                    {
                        info.License = webOptions.Documentation.License;
                    }

                    if (!string.IsNullOrEmpty(webOptions.Documentation.TermsOfService))
                    {
                        info.TermsOfService = new Uri(webOptions.Documentation.TermsOfService);
                    }

                    if (provider.IsDeprecated)
                    {
                        info.Description += " This version has been deprecated.";
                    }

                    x.SwaggerDoc(provider.GroupName, info);
                }

                x.IgnoreObsoleteActions();
                x.IgnoreObsoleteProperties();
                x.EnableAnnotations(true, true);
                x.CustomSchemaIds(y => y.FullName);
                x.OrderActionsBy(y => y.RelativePath);

                x.SchemaFilter<EnumsFilter>();
                x.SchemaFilter<ResponseOnlyFilter>();
                x.OperationFilter<SwaggerResponseOnlyFilter>();
                x.DocumentFilter<RemoveVersionsRoutesFilter>();

                var openApiSecurityRequirement = new OpenApiSecurityRequirement();

                if (webOptions.Identity?.Authentication.Jwt != null)
                {
                    var jwtSecurityScheme = new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "JWT Authorization header using the Bearer scheme. Format: Authorization: Bearer [token]"
                    };

                    x.AddSecurityDefinition("Bearer", jwtSecurityScheme);

                    openApiSecurityRequirement
                        .Add(jwtSecurityScheme, new List<string>());
                }

                if (webOptions.Identity?.Authentication.ApiKey != null)
                {
                    var apiKeySecurityScheme = new OpenApiSecurityScheme
                    {
                        Name = "x-api-key",
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Description = "API Key needed to access endpoints."
                    };

                    x.AddSecurityDefinition("Api-Key", apiKeySecurityScheme);

                    openApiSecurityRequirement
                        .Add(apiKeySecurityScheme, new List<string>());
                }

                x.AddSecurityRequirement(openApiSecurityRequirement);

                TypesHelper.GetAllTypes()
                    .Where(y => y.IsTypeOf(typeof(BaseController)))
                    .Select(y => y.Module)
                    .Distinct()
                    .ToList()
                    .ForEach(y =>
                    {
                        var name = y.Name.Replace(".dll", ".xml").Replace(".exe", ".xml");
                        var path = Path.Combine(AppContext.BaseDirectory, name);

                        if (File.Exists(path))
                        {
                            x.IncludeXmlComments(path, true);
                        }

                        var modelsName = y.Name.Replace(".dll", "").Replace(".exe", "") + ".Models.xml";
                        var modelsPath = Path.Combine(AppContext.BaseDirectory, modelsName);

                        if (File.Exists(modelsPath))
                        {
                            x.IncludeXmlComments(modelsPath);
                        }

                        y.Assembly
                            .GetManifestResourceNames()
                            .Where(z => z.ToLower().EndsWith(".xml"))
                            .ToList()
                            .ForEach(z =>
                            {
                                var resource = y.Assembly.GetManifestResourceStream(z);

                                if (resource != null)
                                {
                                    x.IncludeXmlComments(() => new XPathDocument(resource));
                                }
                            });
                    });
            })
            .AddSwaggerGenNewtonsoftSupport();

        return services;
    }
    private static IServiceCollection AddTimeZone(this IServiceCollection services, WebOptions webOptions)
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
    private static IServiceCollection AddVirusScan(this IServiceCollection services, WebOptions webOptions)
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
    private static IServiceCollection AddIdentityAuthenticationAndAuthorization(this IServiceCollection services, AuthenticationOptions options)
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
            // BUG: 000: Move to Data where we know TIdentity. hmm we have the authentication options here, maybe we need split it
            //.AddApiKeyAuthentication<TIdentity>(options.ApiKey);

        return services;
    }
}