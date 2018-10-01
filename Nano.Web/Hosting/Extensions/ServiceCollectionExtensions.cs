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
using Nano.Web.Hosting.Middleware;
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
using Microsoft.IdentityModel.Tokens;
using Nano.Data;
using Nano.Models.Extensions;
using Nano.Repository;
using Nano.Repository.Interfaces;
using Nano.Security;
using Nano.Web.Api;
using Nano.Web.Hosting.Filters;

namespace Nano.Web.Hosting.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="WebOptions"/> to the <see cref="IServiceCollection"/>.
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

            // TODO: Data Protection: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-2.1&tabs=aspnetcore2x 

            services
                .AddScoped<IRepository, DefaultRepository>()
                .AddScoped<IRepositorySpatial, DefaultRepositorySpatial>();

            var serviceProvider = services.BuildServiceProvider();
            var dataOptions = serviceProvider.GetService<DataOptions>();
            var securityOptions = serviceProvider.GetService<SecurityOptions>();

            services
                .AddApis()
                .AddCors()
                .AddSession()
                .AddVersioning()
                .AddDocumentation()
                .AddLocalizations()
                .AddSecurity(options)
                .AddGzipCompression()
                .AddContentTypeFormatters()
                .AddSingleton<ExceptionHandlingMiddleware>()
                .AddSingleton<HttpRequestUserMiddleware>()
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
                        x.Conventions.Insert(0, new AduitControllerDisabledConvention());

                    if (!securityOptions.IsEnabled)
                        x.Conventions.Insert(1, new AuthControllerDisabledConvention());

                    if (options.Hosting.UseSsl)
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
                })
                .AddControllersAsServices()
                .AddViewComponentsAsServices()
                .AddApplicationPart(assembly)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services
                .Configure<RazorViewEngineOptions>(x =>
                {
                    x.FileProviders.Add(new EmbeddedFileProvider(assembly));
                });

            return services;
        }

        private static IServiceCollection AddApis(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(y => y.GetTypes())
                .Where(y =>
                    !y.IsAbstract &&
                    y.IsTypeDef(typeof(BaseApi)))
                .Distinct()
                .ToList()
                .ForEach(x =>
                {
                    services
                        .AddSingleton(x, y =>
                        {
                            var configuration = y.GetRequiredService<IConfiguration>();
                            var section = configuration.GetSection(x.Name);
                            var options = section?.Get<ApiOptions>() ?? new ApiOptions();

                            return Activator.CreateInstance(x, options);
                        });
                });

            return services;
        }
        private static IServiceCollection AddSecurity(this IServiceCollection services, WebOptions webOptions)
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

            // TODO: Policy-based Authorization

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

                    x.Audience = options.Jwt.Issuer;
                    x.ClaimsIssuer = options.Jwt.Issuer;

                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateActor = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = options.Jwt.Issuer,
                        ValidAudience = options.Jwt.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Jwt.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                })
                .AddCookie(x =>
                {
                    x.LoginPath = $"/{webOptions.Hosting.Root}/auth/login";
                    x.LogoutPath = $"/{webOptions.Hosting.Root}/auth/logout";
                    x.AccessDeniedPath = $"/{webOptions.Hosting.Root}/auth/forbidden";
                    x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    x.Cookie.Expiration = TimeSpan.FromDays(options.Jwt.ExpirationInHours);
                });

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

                    if (securityOptions != null)
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
        private static IServiceCollection AddGzipCompression(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddResponseCompression(y => y.Providers.Add<GzipCompressionProvider>());

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
                    x.FormatterMappings.SetMediaTypeMappingForFormat("html", HttpContentType.HTML);
                    x.FormatterMappings.SetMediaTypeMappingForFormat("json", HttpContentType.JSON);
                })
                .AddXmlSerializerFormatters()
                .AddXmlDataContractSerializerFormatters();

            return services;
        }
    }
}