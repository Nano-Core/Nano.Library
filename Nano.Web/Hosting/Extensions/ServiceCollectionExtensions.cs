using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
using Nano.Models.Extensions;
using Nano.Services;
using Nano.Services.Interfaces;
using Nano.Web.Api;
using Nano.Web.Controllers;
using Nano.Web.Hosting.Conventions;
using Nano.Web.Hosting.Documentation;
using Nano.Web.Hosting.Middleware;
using Nano.Web.Hosting.ModelBinders;
using Nano.Web.Hosting.Serialization;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

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

            services
                .AddScoped<IService, DefaultService>()
                .AddScoped<IServiceSpatial, DefaultServiceSpatial>()
                .AddConfigOptions<WebOptions>(configuration, WebOptions.SectionName, out var options);

            var assembly = typeof(BaseController).GetTypeInfo().Assembly;

            services
                .AddApi()
                .AddCors()
                .AddSession()
                .AddVersioning()
                .AddDocumentation()
                .AddLocalizations()
                .AddGzipCompression()
                .AddContentTypeFormatters()
                .AddSingleton<ExceptionHandlingMiddleware>()
                .AddSingleton<HttpRequestUserMiddleware>()
                .AddSingleton<HttpRequestIdentifierMiddleware>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddMvc(x =>
                {
                    var routeAttribute = new RouteAttribute(options.Hosting.Root);
                    var routePrefixConvention = new RoutePrefixConvention(routeAttribute);
                    var modelBinderProvider = new QueryModelBinderProvider();

                    x.Conventions.Insert(0, routePrefixConvention);
                    x.ModelBinderProviders.Insert(0, modelBinderProvider);

                    if (options.Hosting.UseSsl)
                        x.Filters.Add(new RequireHttpsAttribute());
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
                .AddApplicationPart(assembly);

            services
                .Configure<RazorViewEngineOptions>(x =>
                {
                    x.FileProviders.Add(new EmbeddedFileProvider(assembly));
                });

            return services;
        }

        private static IServiceCollection AddApi(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsSubclassOf(typeof(BaseApi)))
                .ToList()
                .ForEach(x =>
                {
                    services
                        .AddScoped(x);
                });

            return services;
        }
        private static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var appOptions = services.BuildServiceProvider().GetService<AppOptions>();

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

            return services
                .AddSwaggerGen(x =>
                {
                    x.IgnoreObsoleteActions();
                    x.IgnoreObsoleteProperties();
                    x.DescribeAllEnumsAsStrings();

                    x.DocumentFilter<LowercaseDocumentFilter>();
                    x.DocumentFilter<ActionOrderingDocumentFilter>();

                    x.SwaggerDoc(appOptions.Version, new Info
                    {
                        Title = appOptions.Name,
                        Description = appOptions.Description,
                        Version = appOptions.Version,
                        TermsOfService = appOptions.TermsOfService,
                        Contact = webOptions.Documentation.Contact,
                        License = webOptions.Documentation.License
                    });

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

                    AppDomain.CurrentDomain
                        .GetAssemblies()
                        .SelectMany(y => y.GetTypes())
                        .Where(y => y.IsTypeDef(typeof(Controller)))
                        .Select(y => y.Module)
                        .Distinct()
                        .ToList()
                        .ForEach(y =>
                        {
                            // COSMETIC: Generates swagger error when xml included. Also Swagger doesn't add documentation from xml file in nuget packages.
                            //var fileName = y.Name.Replace(".dll", ".xml").Replace(".exe", ".xml");
                            //var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

                            //if (File.Exists(filePath))
                            //    x.IncludeXmlComments(filePath);
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
                    x.FormatterMappings.SetMediaTypeMappingForFormat("text", HttpContentType.TEXT);
                })
                .AddXmlSerializerFormatters()
                .AddXmlDataContractSerializerFormatters();

            return services;
        }
    }
}