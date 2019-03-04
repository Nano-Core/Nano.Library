using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Nano.App;
using Nano.Config.Extensions;
using Nano.Web.Controllers;
using Nano.Web.Hosting.Conventions;
using Nano.Web.Hosting.Documentation;
using Nano.Web.Hosting.ModelBinders;
using Nano.Web.Hosting.Serialization;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.XPath;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Nano.Data;
using Nano.Models.Extensions;
using Nano.Repository;
using Nano.Repository.Interfaces;
using Nano.Security;
using Nano.Web.Api;
using Nano.Web.Hosting.Filters;
using Nano.Web.Hosting.HealthChecks;
using Nano.Web.Hosting.Middleware;
using Nano.Web.Hosting.Startup;
using Nano.Web.Hosting.Startup.Tasks;
using Newtonsoft.Json.Converters;

namespace Nano.Web.Hosting.Extensions
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
                .AddConfigOptions<WebOptions>(configuration, WebOptions.SectionName, out var options);

            var serviceProvider = services.BuildServiceProvider();
            var dataOptions = serviceProvider.GetService<DataOptions>();
            var securityOptions = serviceProvider.GetService<SecurityOptions>();

            services
                .AddCors()
                .AddSession()
                .AddSecurity()
                .AddRepository()
                .AddVersioning()
                .AddDocumentation()
                .AddLocalizations()
                .AddCompression()
                .AddContentTypeFormatters()
                .AddSingleton<ExceptionHandlingMiddleware>()
                .AddSingleton<HttpRequestOptionsMiddleware>()
                .AddSingleton<HttpRequestIdentifierMiddleware>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddMvc(x =>
                {
                    var routeAttribute = new RouteAttribute(options.Hosting.Root);
                    var routePrefixConvention = new RoutePrefixConvention(routeAttribute);
                    var queryModelBinderProvider = new QueryModelBinderProvider();

                    x.Conventions.Insert(0, routePrefixConvention);
                    x.ModelBinderProviders.Insert(0, queryModelBinderProvider);

                    if (dataOptions.ConnectionString == null)
                        x.Conventions.Insert(1, new AduitControllerDisabledConvention());

                    if (dataOptions.ConnectionString == null || !securityOptions.IsEnabled)
                        x.Conventions.Insert(0, new AuthControllerDisabledConvention());

                    if (options.Hosting.UseHttpsRequired)
                        x.Filters.Add<RequireHttpsAttribute>();

                    x.Filters.Add<ModelStateValidationFilter>();
                    x.Filters.Add<DisableLazyLoadingResultFilterAttribute>();
                })
                .AddJsonOptions(x =>
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
                .AddApplicationPart(assembly)
                .SetCompatibilityVersion(options.CompatabilityVersion);

            services
                .Configure<RazorViewEngineOptions>(x =>
                {
                    x.FileProviders
                        .Add(new EmbeddedFileProvider(assembly));
                });

            services
                .AddApis()
                .AddStartupTasks()
                .AddHealthChecking(options);

            return services;
        }

        private static IServiceCollection AddApis(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x =>
                    !x.IsAbstract &&
                    x.IsTypeDef(typeof(BaseApi)))
                .Distinct()
                .ToList()
                .ForEach(x =>
                {
                    var section = configuration.GetSection(x.Name);
                    var options = section?.Get<ApiOptions>();

                    if (options == null)
                        return;

                    services
                        .AddSingleton(x, Activator.CreateInstance(x, options))
                        .AddHealthChecks()
                            .AddTcpHealthCheck(y => y.AddHost(options.Host, options.Port), options.Host);
                });

            return services;
        }
        private static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var options = services
                .BuildServiceProvider()
                .GetService<SecurityOptions>() ?? new SecurityOptions();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            if (!options.IsEnabled)
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

                    x.Audience = options.Jwt.Audience;
                    x.ClaimsIssuer = options.Jwt.Issuer;

                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateActor = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = options.Jwt.Issuer,
                        ValidAudience = options.Jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Jwt.SecretKey)),
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                })
                .AddCookie(x =>
                {
                    x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    x.Cookie.Expiration = TimeSpan.FromDays(options.Jwt.ExpirationInHours);
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
        private static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var appOptions = services.BuildServiceProvider().GetService<AppOptions>() ?? new AppOptions();

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
                .AddSingleton<StartupTaskContext>()
                .AddHostedService<InitializeDatabaseStartupTask>()
                .AddHostedService<InitializeApplicationStartupTask>();

            return services;
        }
        private static IServiceCollection AddHealthChecking(this IServiceCollection services, WebOptions options)
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
                var appOptions = config.GetSection(AppOptions.SectionName).Get<AppOptions>();

                var healthz = new
                {
                    HealthChecks = new[]
                    {
                        new
                        {
                            Name = appOptions.Name.ToLower(),
                            Uri = $"http://localhost:{port}/healthz"
                        }
                    }
                };

                config["HealthChecks-UI"] = JsonConvert.SerializeObject(healthz); // BUG: string.Concat("{\"HealthChecks\": [{\"Name\": \"app\",\"Uri\": \"http://localhost:", port, "/healthz\"}]}");
                config[HostDefaults.ContentRootKey] = Directory.GetCurrentDirectory();
                
                services
                    .AddHealthChecksUI();

                services.AddScoped<DbContextOptions, DbContextOptions<NullDbContext>>(); // BUG: Wihtout this line, EF-Core fails when no Data Provider / Context is registerd and 'NullDbContext' is used.
            }

            return services;
        }
        private static IServiceCollection AddDocumentation(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var provider = services.BuildServiceProvider();

            var appOptions = provider.GetService<AppOptions>();
            var webOptions = provider.GetService<WebOptions>();
            var securityOptions = provider.GetService<SecurityOptions>();

            return services
                .AddSwaggerGen(x =>
                {
                    x.IgnoreObsoleteActions();
                    x.IgnoreObsoleteProperties();
                    x.DescribeAllEnumsAsStrings();
                    x.CustomSchemaIds(y => y.FullName);
                    x.OrderActionsBy(y => y.RelativePath);
                    x.DocumentFilter<LowercaseDocumentFilter>();

                    x.SwaggerDoc(appOptions.Version, new Info
                    {
                        Title = appOptions.Name,
                        Description = appOptions.Description,
                        Version = appOptions.Version,
                        TermsOfService = appOptions.TermsOfService,
                        Contact = webOptions.Documentation.Contact,
                        License = webOptions.Documentation.License
                    });

                    if (securityOptions.IsEnabled)
                    {
                        x.AddSecurityDefinition("Bearer", new ApiKeyScheme
                        {
                            In = "header",
                            Type = "apiKey",
                            Name = "Authorization",
                            Description = "JWT Authorization header using the Bearer scheme. Format: Authorization: Bearer [token]"
                        });

                        x.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                        {
                            { "Bearer", new string[] { } }
                        });
                    }

                    AppDomain.CurrentDomain
                         .GetAssemblies()
                         .SelectMany(y => y.GetTypes())
                         .Where(y => y.IsTypeDef(typeof(BaseController)))
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