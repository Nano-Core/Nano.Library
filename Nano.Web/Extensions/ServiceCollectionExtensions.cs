using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nano.App;
using Nano.Config.Extensions;
using Nano.Data;
using Nano.Models.Extensions;
using Nano.Repository;
using Nano.Repository.Interfaces;
using Nano.Security;
using Nano.Web.Api;
using Nano.Web.Const;
using Nano.Web.Controllers;
using Nano.Web.Hosting.Conventions;
using Nano.Web.Hosting.Documentation.Filters;
using Nano.Web.Hosting.Filters;
using Nano.Web.Hosting.HealthChecks;
using Nano.Web.Hosting.Middleware;
using Nano.Web.Hosting.ModelBinders;
using Nano.Web.Hosting.Serialization;
using Nano.Web.Hosting.Startup.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Vivet.AspNetCore.RequestTimeZone.Extensions;

namespace Nano.Web.Extensions
{
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

            var serviceProvider = services.BuildServiceProvider();
            var appOptions = serviceProvider.GetService<AppOptions>() ?? new AppOptions();
            var dataOptions = serviceProvider.GetService<DataOptions>() ?? new DataOptions();
            var securityOptions = serviceProvider.GetService<SecurityOptions>() ?? new SecurityOptions();

            services
                .AddCors()
                .AddSession()
                .AddCaching(webOptions)
                .AddSecurity(securityOptions)
                .AddRepository()
                .AddVersioning(appOptions)
                .AddDocumentation(appOptions, webOptions, securityOptions)
                .AddLocalizations()
                .AddTimeZone(appOptions)
                .AddCompression()
                .AddContentTypeFormatters()
                .Configure<ForwardedHeadersOptions>(options => { options.ForwardedHeaders = ForwardedHeaders.All; })
                .AddSingleton<ExceptionHandlingMiddleware>()
                .AddSingleton<HttpRequestOptionsMiddleware>()
                .AddSingleton<HttpRequestIdentifierMiddleware>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddRouting()
                .AddMvc(x =>
                {
                    var routeAttribute = new RouteAttribute(webOptions.Hosting.Root);
                    var routePrefixConvention = new RoutePrefixConvention(routeAttribute);
                    var queryModelBinderProvider = new QueryModelBinderProvider();

                    x.Conventions.Insert(0, routePrefixConvention);
                    x.ModelBinderProviders.Insert(0, queryModelBinderProvider);

                    if (!securityOptions.IsAuthControllerEnabled)
                        x.Conventions.Insert(1, new AuthControllerDisabledConvention());

                    if (dataOptions.ConnectionString == null)
                        x.Conventions.Insert(2, new AuditControllerDisabledConvention());

                    if (webOptions.Hosting.UseHttpsRequired)
                        x.Filters.Add<RequireHttpsAttribute>();

                    x.Filters.Add<IsAnonymousFilter>();
                    x.Filters.Add<ModelStateValidationFilter>();
                    x.Filters.Add<DisableLazyLoadingResultFilterAttribute>();
                })
                .AddNewtonsoftJson(x =>
                {
                    x.AllowInputFormatterExceptionMessages = true;

                    x.SerializerSettings.Culture = CultureInfo.CurrentCulture;
                    x.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    x.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    x.SerializerSettings.ContractResolver = new EntityContractResolver();

                    x.SerializerSettings.Converters
                        .Add(new StringEnumConverter());
                })
                .AddControllersAsServices()
                .AddViewComponentsAsServices()
                .AddApplicationPart(assembly);

            services
                .AddApis()
                .AddStartupTasks()
                .AddHealthChecking(appOptions, webOptions);

            return services;
        }

        private static IServiceCollection AddApis(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var hosts = new List<string>();

            var configuration = services
                .BuildServiceProvider()
                .GetRequiredService<IConfiguration>();

            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    !x.IsAbstract &&
                    x.IsTypeOf(typeof(BaseApi)))
                .Distinct()
                .ToList()
                .ForEach(x =>
                {
                    var section = configuration.GetSection(x.Name);
                    var options = section?.Get<ApiOptions>();

                    if (options == null)
                        return;

                    services
                        .AddSingleton(x, Activator.CreateInstance(x, options));

                    if (hosts.Contains(options.Host))
                        return;

                    services
                        .AddHealthChecks()
                            .AddTcpHealthCheck(y => y.AddHost(options.Host, options.Port), options.Host, HealthStatus.Degraded);

                    hosts
                        .Add(options.Host);
                });

            return services;
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

            if (!securityOptions.IsEnabled)
            {
                services.AddMvc(x =>
                {
                    x.Filters.Add<AllowAnonymousFilter>();
                });
            }

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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityOptions.Jwt.SecretKey)),
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
        private static IServiceCollection AddRepository(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<IRepository, DefaultRepository>()
                .AddScoped<IRepositorySpatial, DefaultRepositorySpatial>();

            return services;
        }
        private static IServiceCollection AddVersioning(this IServiceCollection services, AppOptions appOptions)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (appOptions == null) 
                throw new ArgumentNullException(nameof(appOptions));

            var success = ApiVersion.TryParse(appOptions.Version, out var apiVersion);

            if (!success)
            {
                apiVersion = new ApiVersion(1, 0);
            }

            return services
                .AddApiVersioning(x =>
                {
                    x.ReportApiVersions = true;
                    x.DefaultApiVersion = apiVersion;
                    x.AssumeDefaultVersionWhenUnspecified = true;
                    x.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
                });
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
        private static IServiceCollection AddStartupTasks(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddHostedService<InitializeDatabaseStartupTask>();

            return services;
        }
        private static IServiceCollection AddHealthChecking(this IServiceCollection services, AppOptions appOptions, WebOptions options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
                                
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (!options.Hosting.UseHealthCheck)
                return services;

            services
                .AddHealthChecks()
                    .AddCheck<StartupHealthCheck>("self");

            if (options.Hosting.UseHealthCheckUI)
            {
                var port = options.Hosting.Ports.FirstOrDefault();
                var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

                config[HostDefaults.ContentRootKey] = Directory.GetCurrentDirectory();
                
                services
                    .AddHealthChecksUI("healthchecksdb", x =>
                        {
                            x.AddHealthCheckEndpoint(appOptions.Name.ToLower(), $"http://localhost:{port}/healthz");
                            x.SetEvaluationTimeInSeconds(10);
                            x.SetMinimumSecondsBetweenFailureNotifications(60);
                        });

                services
                    .AddScoped<DbContextOptions, DbContextOptions<NullDbContext>>(); 
            }

            return services;
        }
        private static IServiceCollection AddDocumentation(this IServiceCollection services, AppOptions appOptions, WebOptions webOptions, SecurityOptions securityOptions)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (appOptions == null) 
                throw new ArgumentNullException(nameof(appOptions));
            
            if (webOptions == null) 
                throw new ArgumentNullException(nameof(webOptions));
            
            if (securityOptions == null) 
                throw new ArgumentNullException(nameof(securityOptions));

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
                        info.TermsOfService = new Uri(appOptions.TermsOfService); 

                    x.SwaggerDoc(appOptions.Version, info);
                    x.IgnoreObsoleteActions();
                    x.IgnoreObsoleteProperties();
                    x.CustomSchemaIds(y => y.FullName);
                    x.OrderActionsBy(y => y.RelativePath);
                    x.DocumentFilter<LowercaseDocumentFilter>();

                    if (securityOptions.IsEnabled)
                    {
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
                    }

                    AppDomain.CurrentDomain
                         .GetAssemblies()
                         .SelectMany(y => y.GetTypes())
                         .Where(y => y.IsTypeOf(typeof(BaseController)))
                         .Select(y => y.Module)
                         .Distinct()
                         .ToList()
                         .ForEach(y =>
                         {
                             var name = y.Name.Replace(".dll", ".xml").Replace(".exe", ".xml");
                             var path = Path.Combine(AppContext.BaseDirectory, name);

                             if (File.Exists(path))
                                 x.IncludeXmlComments(path);

                             var modelsName = y.Name.Replace(".dll", "").Replace(".exe", "") + ".Models.xml";
                             var modelsPath = Path.Combine(AppContext.BaseDirectory, modelsName);

                             if (File.Exists(modelsPath))
                                 x.IncludeXmlComments(modelsPath);

                             y.Assembly
                                 .GetManifestResourceNames()
                                 .Where(z => z.ToLower().EndsWith(".xml"))
                                 .ToList()
                                 .ForEach(z =>
                                 {
                                     var resource = y.Assembly.GetManifestResourceStream(z);

                                     if (resource != null)
                                         x.IncludeXmlComments(() => new XPathDocument(resource));
                                 });
                         });
                });
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
}