using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Xml.XPath;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nano.App;
using Nano.Config.Extensions;
using Nano.Data;
using Nano.Models.Const;
using Nano.Models.Extensions;
using Nano.Models.Helpers;
using Nano.Models.Serialization.Json;
using Nano.Repository.Extensions;
using Nano.Security;
using Nano.Web.Controllers;
using Nano.Web.Hosting.Conventions;
using Nano.Web.Hosting.Documentation.Filters;
using Nano.Web.Hosting.Filters;
using Nano.Web.Hosting.HealthChecks;
using Nano.Web.Hosting.Middleware;
using Nano.Web.Hosting.ModelBinders;
using Vivet.AspNetCore.RequestTimeZone.Enums;
using Vivet.AspNetCore.RequestTimeZone.Extensions;

namespace Nano.Web.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="WebOptions"/> and services to the <see cref="IServiceCollection"/>.
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
            .AddConfigOptions<WebOptions>(configuration, WebOptions.SectionName, out var webOptions);

        var serviceProvider = services
            .BuildServiceProvider();

        var appOptions = serviceProvider
            .GetService<AppOptions>() ?? new AppOptions();

        var dataOptions = serviceProvider
            .GetService<DataOptions>() ?? new DataOptions();

        var securityOptions = serviceProvider
            .GetService<SecurityOptions>() ?? new SecurityOptions();

        services
            .AddCors()
            .AddSession()
            .AddCaching(webOptions)
            .AddSecurity(securityOptions)
            .AddRepository()
            .AddVersioning(appOptions)
            .AddDocumentation(appOptions, webOptions)
            .AddLocalizations()
            .AddTimeZone(appOptions)
            .AddCompression()
            .AddContentTypeFormatters()
            .Configure<ForwardedHeadersOptions>(x =>
            {
                x.ForwardedHeaders = ForwardedHeaders.All;
            })
            .AddSingleton<ExceptionHandlingMiddleware>()
            .AddSingleton<HttpRequestOptionsMiddleware>()
            .AddSingleton<HttpRequestIdentifierMiddleware>()
            .AddRouting()
            .AddMvc(x =>
            {
                var routeAttribute = new RouteAttribute(webOptions.Hosting.Root);
                var routePrefixConvention = new RoutePrefixConvention(routeAttribute);
                var queryModelBinderProvider = new QueryModelBinderProvider();

                x.Conventions.Insert(0, routePrefixConvention);
                x.ModelBinderProviders.Insert(0, queryModelBinderProvider);

                if (dataOptions.ConnectionString == null || !securityOptions.IsAuth)
                {
                    x.Conventions.Insert(1, new AuthControllerDisabledConvention());
                }

                if (dataOptions.ConnectionString == null || !dataOptions.UseAudit)
                {
                    x.Conventions.Insert(2, new AuditControllerDisabledConvention());
                }

                if (webOptions.Hosting.UseHttpsRequired)
                {
                    x.Filters.Add<RequireHttpsAttribute>();
                }

                x.MaxValidationDepth = 128;

                x.Filters.Add<ModelStateValidationFilter>();
            })
            .AddJsonOptions(x =>
            {
                x.AllowInputFormatterExceptionMessages = true;

                x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                x.JsonSerializerOptions.PropertyNamingPolicy = null;
                x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                x.JsonSerializerOptions.MaxDepth = 128;
                x.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers =
                    {
                        LazyLoaderTypeInfoResolver.IgnoreLazyLoader,
                        EnumerableTypeInfoResolver.IgnoreEmptyCollections,
                    }
                };
                x.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            })
            .AddControllersAsServices()
            .AddViewComponentsAsServices()
            .AddApplicationPart(assembly);

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
    private static IServiceCollection AddSecurity(this IServiceCollection services, SecurityOptions securityOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (securityOptions == null)
            throw new ArgumentNullException(nameof(securityOptions));

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services
            .AddAuthorization()
            .AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                var rsaSecurityKey = services
                    .BuildServiceProvider()
                    .GetRequiredService<RsaSecurityKey>();

                x.SaveToken = true;
                x.IncludeErrorDetails = true;
                x.RequireHttpsMetadata = false;

                x.Audience = securityOptions.Jwt.Audience;
                x.ClaimsIssuer = securityOptions.Jwt.Issuer;

                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateActor = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = securityOptions.Jwt.Issuer,
                    ValidAudience = securityOptions.Jwt.Audience,
                    IssuerSigningKey = rsaSecurityKey,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            const string KEY = "Token-Expired";

                            context.Response.Headers
                                .Add(KEY, true.ToString());
                        }

                        return Task.CompletedTask;
                    }
                };
            })
            .AddExternalLogins(securityOptions)
            .AddCookie(x =>
            {
                x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                x.Cookie.Expiration = TimeSpan.FromHours(securityOptions.Jwt.ExpirationInHours);
            });

        return services;
    }
    private static IServiceCollection AddVersioning(this IServiceCollection services, AppOptions appOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (appOptions == null)
            throw new ArgumentNullException(nameof(appOptions));

        ApiVersionParser.Default.TryParse(appOptions.Version, out var apiVersion);

        services
            .AddApiVersioning(x =>
            {
                x.ReportApiVersions = true;
                x.DefaultApiVersion = apiVersion ?? new ApiVersion(1, 0);
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
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
    private static IServiceCollection AddDocumentation(this IServiceCollection services, AppOptions appOptions, WebOptions webOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (appOptions == null)
            throw new ArgumentNullException(nameof(appOptions));

        if (webOptions == null)
            throw new ArgumentNullException(nameof(webOptions));

        if (webOptions.Documentation.IsEnabled)
        {
            return services
                .AddSwaggerGen(x =>
                {
                    var info = new OpenApiInfo
                    {
                        Title = appOptions.Name,
                        Description = appOptions.Description,
                        Version = appOptions.Version,
                        Contact = webOptions.Documentation.Contact,
                        License = webOptions.Documentation.License
                    };

                    if (!string.IsNullOrEmpty(appOptions.TermsOfService))
                    {
                        info.TermsOfService = new Uri(appOptions.TermsOfService);
                    }

                    x.SwaggerDoc(appOptions.Version, info);
                    x.IgnoreObsoleteActions();
                    x.IgnoreObsoleteProperties();
                    x.EnableAnnotations(true, true);
                    x.CustomSchemaIds(y => y.FullName);
                    x.OrderActionsBy(y => y.RelativePath);

                    x.SchemaFilter<SwaggerExcludeFilter>();

                    var securityScheme = new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "JWT Authorization header using the Bearer scheme. Format: Authorization: Bearer [token]"
                    };

                    x.AddSecurityDefinition("Bearer", securityScheme);
                    x.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    { securityScheme, new string[] { } }
                    });

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
                });
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
                x.JsonSerializerType = JsonSerializerType.Microsoft;
            });

        return services;
    }
    private static IServiceCollection AddContentTypeFormatters(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddMvc(x =>
            {
                x.ReturnHttpNotAcceptable = true;
                x.RespectBrowserAcceptHeader = true;

                x.FormatterMappings.SetMediaTypeMappingForFormat("xml", HttpContentType.XML);
                x.FormatterMappings.SetMediaTypeMappingForFormat("json", HttpContentType.JSON);
                x.FormatterMappings.SetMediaTypeMappingForFormat("html", HttpContentType.HTML);
            })
            .AddXmlSerializerFormatters()
            .AddXmlDataContractSerializerFormatters();

        return services;
    }
}