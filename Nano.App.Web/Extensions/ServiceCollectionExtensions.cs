using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using DynamicExpression.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nano.App;
using Nano.Data;
using Nano.Data.Extensions;
using Nano.Models;
using Nano.Models.Const;
using Nano.Security;
using Nano.Web.Controllers;
using Nano.Web.Hosting.Authentication;
using Nano.Web.Hosting.Authentication.Const;
using Nano.Web.Hosting.Conventions;
using Nano.Web.Hosting.Documentation.Filters.Document;
using Nano.Web.Hosting.Documentation.Filters.Operation;
using Nano.Web.Hosting.Documentation.Filters.Schema;
using Nano.Web.Hosting.HealthChecks;
using Nano.Web.Hosting.Middleware;
using Nano.Web.Hosting.Serialization.Json.Const;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.XPath;
using Nano.Common.Config.Extensions;
using Nano.Common.Config.Helpers;
using Nano.Common.Extensions;
using Nano.Common.Helpers;
using Nano.Data.Abstractions.Config;
using Vivet.AspNetCore.RequestTimeZone.Enums;
using Vivet.AspNetCore.RequestTimeZone.Extensions;
using Vivet.AspNetCore.RequestVirusScan.Extensions;
using AuthenticationOptions = Nano.Security.AuthenticationOptions;

namespace Nano.Web.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds web dependicies and services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var assembly = typeof(BaseController).GetTypeInfo().Assembly;

        services
            .AddConfigSection<WebOptions>(WebOptions.SectionName, out var webOptions);

        if (webOptions == null)
        {
            throw new NullReferenceException(nameof(webOptions));
        }

        var serviceProvider = services
            .BuildServiceProvider();

        var appOptions = serviceProvider
            .GetService<AppOptions>() ?? new AppOptions();

        var dataOptions = serviceProvider
            .GetService<DataOptions>() ?? new DataOptions();

        services
            .AddSingleton(webOptions.Identity);

        services
            .AddCors(x =>
            {
                x.AddPolicy(x.DefaultPolicyName, y =>
                {
                    if (webOptions.Hosting.Cors.AllowedOrigins.Any())
                    {
                        y.WithOrigins(webOptions.Hosting.Cors.AllowedOrigins);
                        y.SetIsOriginAllowedToAllowWildcardSubdomains();
                    }
                    else
                    {
                        y.SetIsOriginAllowed(_ => true);
                    }

                    if (webOptions.Hosting.Cors.AllowedHeaders.Any())
                    {
                        y.WithHeaders(webOptions.Hosting.Cors.AllowedHeaders);
                    }
                    else
                    {
                        y.AllowAnyHeader();
                    }

                    if (webOptions.Hosting.Cors.AllowedMethods.Any())
                    {
                        y.WithMethods(webOptions.Hosting.Cors.AllowedMethods);
                    }
                    else
                    {
                        y.AllowAnyMethod();
                    }

                    if (webOptions.Hosting.Cors.AllowCredentials)
                    {
                        y.AllowCredentials();
                    }
                    else
                    {
                        y.DisallowCredentials();
                    }

                    y.WithExposedHeaders("RequestId", "TZ", "Content-Disposition", "api-supported-versions");
                });
            })
            .AddSession()
            .AddCaching(webOptions)
            .AddVersioning(webOptions)
            .AddIdentityAuthenticationAndAuthorization<Guid>(webOptions.Identity.Authentication) // BUG: Should not be guid
            .AddDocumentation(appOptions, webOptions, webOptions.Identity)
            .AddLocalizations()
            .AddTimeZone(appOptions)
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
            .AddSingleton<ExceptionHandlingMiddleware>()
            .AddSingleton<DisableAuthControllerMiddleware>()
            .AddSingleton<DisableAuditControllerMiddleware>()
            .AddScoped<HttpRequestOptionsMiddleware>()
            .AddSingleton<HttpRequestIdentifierMiddleware>()
            .AddRouting()
            .AddQueryModelBinders()
            .Configure<ApiBehaviorOptions>(x =>
            {
                x.InvalidModelStateResponseFactory = context =>
                {
                    return new BadRequestObjectResult(new Error
                    {
                        Summary = "ModelState Validation Error",
                        Exceptions = context.ModelState.Values
                            .SelectMany(z => z.Errors)
                            .Select(z => z.ErrorMessage)
                            .ToArray(),
                        IsTranslated = true
                    });
                };
            })
            .AddMvc(x =>
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

                if (dataOptions.ConnectionString == null || !webOptions.Hosting.ExposeAuthController)
                {
                    x.Conventions
                        .Add(new AuthActionHidingConvention());
                }

                if (dataOptions.ConnectionString == null || !webOptions.Hosting.ExposeAuditController)
                {
                    x.Conventions
                        .Add(new AuditActionHidingConvention());
                }

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
            .AddControllersAsServices()
            .AddViewComponentsAsServices()
            .AddApplicationPart(assembly);

        //services
        //    .AddScoped<IIdentityAuthRepository<Guid>, DefaultIdentityAuthRepository>(); // BUG: We need TIdentity

        services
            .AddScoped<AuditController>();

        return services
            .AddHealthChecking(appOptions, webOptions);
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        var cacheOptions = webOptions.Hosting.Cache;

        services
            .AddResponseCaching(x =>
            {
                x.SizeLimit = cacheOptions.MaxSize * 1024;
                x.MaximumBodySize = cacheOptions.MaxBodySize * 1024;
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
    private static IServiceCollection AddHealthChecking(this IServiceCollection services, AppOptions appOptions, WebOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (!options.Hosting.HealthCheck.UseHealthCheck)
        {
            return services;
        }

        services
            .AddHealthChecks()
            .AddCheck<StartupHealthCheck>("self");

        if (options.Hosting.HealthCheck.UseHealthCheckUI)
        {
            var port = options.Hosting.Ports.FirstOrDefault();

            // TODO: HealthChecks UI: Doesn't poll: JS: Configured polling interval: NaN milliseconds (https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/issues/636)
            services
                .AddHealthChecksUI(x =>
                {
                    x.AddHealthCheckEndpoint(appOptions.Name.ToLower(), $"http://localhost:{port}/healthz");

                    x.SetApiMaxActiveRequests(1);
                    x.SetEvaluationTimeInSeconds(options.Hosting.HealthCheck.EvaluationInterval);
                    x.SetMinimumSecondsBetweenFailureNotifications(options.Hosting.HealthCheck.FailureNotificationInterval);
                    x.MaximumHistoryEntriesPerEndpoint(options.Hosting.HealthCheck.MaximumHistoryEntriesPerEndpoint);

                    foreach (var webHook in options.Hosting.HealthCheck.WebHooks)
                    {
                        x.AddWebhookNotification(webHook.Name, webHook.Uri, webHook.Payload);
                    }
                })
                .AddInMemoryStorage();
        }

        return services;
    }
    private static IServiceCollection AddDocumentation(this IServiceCollection services, AppOptions appOptions, WebOptions webOptions, IdentityOptions securityOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (appOptions == null)
            throw new ArgumentNullException(nameof(appOptions));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.Documentation.IsEnabled)
        {
            var apiVersionDescriptionProvider = services
                .BuildServiceProvider()
                .GetService<IApiVersionDescriptionProvider>();

            return services
                .AddSwaggerGen(x =>
                {
                    foreach (var provider in apiVersionDescriptionProvider.ApiVersionDescriptions)
                    {
                        var info = new OpenApiInfo
                        {
                            Title = appOptions.Name,
                            Description = appOptions.Description,
                            Version = provider.ApiVersion.ToString(),
                            Contact = webOptions.Documentation.Contact,
                            License = webOptions.Documentation.License
                        };

                        if (provider.IsDeprecated)
                        {
                            info.Description += " This version has been deprecated.";
                        }

                        if (!string.IsNullOrEmpty(appOptions.TermsOfService))
                        {
                            info.TermsOfService = new Uri(appOptions.TermsOfService);
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
                    x.OperationFilter<NonResponseType200Filter>();
                    x.DocumentFilter<RemoveVersionsRoutesFilter>();
                    x.DocumentFilter<LowercaseRoutesFilter>();

                    var openApiSecurityRequirement = new OpenApiSecurityRequirement();

                    if (securityOptions?.Authentication.Jwt != null)
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

                    if (securityOptions?.Authentication.ApiKey != null)
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
                                x.IncludeXmlComments(path);
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
        }

        return services;
    }
    private static IServiceCollection AddLocalizations(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddLocalization()
            .AddMvc()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

        return services;
    }
    private static IServiceCollection AddTimeZone(this IServiceCollection services, AppOptions appOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (appOptions == null)
            throw new ArgumentNullException(nameof(appOptions));

        services
            .AddRequestTimeZone(x =>
            {
                x.Id = appOptions.DefaultTimeZone;
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

        var virusScanOptions = webOptions.Hosting.VirusScan;

        if (virusScanOptions.IsEnabled)
        {
            services
                .AddRequestVirusScan(x =>
                {
                    x.Host = virusScanOptions.Host;
                    x.Port = virusScanOptions.Port;
                    x.UseHealthCheck = virusScanOptions.UseHealthCheck;
                });
        }

        return services;
    }

    private static IServiceCollection AddIdentityAuthenticationAndAuthorization<TIdentity>(this IServiceCollection services, AuthenticationOptions options)
        where TIdentity : IEquatable<TIdentity>
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
            .AddJwtAuthentication(options.Jwt)
            .AddApiKeyAuthentication<TIdentity>(options.ApiKey);

        return services;
    }

}