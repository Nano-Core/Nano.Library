using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nano.Eventing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.App.Extensions.Conventions;
using Nano.App.Extensions.Documentation;
using Nano.App.Extensions.Middleware;
using Nano.App.Extensions.ModelBinders;
using Nano.App.Extensions.Serialization;
using Nano.Data;
using Nano.Data.Interfaces;
using Nano.Data.Models;
using Nano.Eventing.Attributes;
using Nano.Eventing.Interfaces;
using Nano.Logging;
using Nano.Logging.Interfaces;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;
using Nano.Services;
using Nano.Services.Data;
using Nano.Services.Eventing;
using Nano.Services.Interfaces;
using Nano.Web.Api;
using Nano.Web.Controllers;
using Nano.Web.Controllers.Extensions;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using Z.EntityFramework.Plus;

namespace Nano.App.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a appOptions <see cref="IConfigurationSection"/> as <see cref="IOptions{TOptions}"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TOption">The option implementation type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="section">The name of the <see cref="IConfigurationSection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddOptions<TOption>(this IServiceCollection services, string section)
            where TOption : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (section == null)
                throw new ArgumentNullException(nameof(section));

            var configuration = services
                .BuildServiceProvider()
                .GetRequiredService<IConfiguration>();

            services
                .AddConfigOptions(configuration, section, out TOption _);

            return services;
        }

        /// <summary>
        /// Adds logging provider of type <typeparamref name="TProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <typeparamref name="TProvider"/> type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddLogging<TProvider>(this IServiceCollection services)
            where TProvider : class, ILoggingProvider
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddSingleton<ILoggingProvider, TProvider>()
                .AddSingleton(x => x
                    .GetRequiredService<ILoggingProvider>()
                    .Configure())
                .AddSingleton(x =>
                {
                    var loggerProvider = x.GetRequiredService<ILoggerProvider>();

                    return loggerProvider.CreateLogger(null);
                })
                .AddSingleton<ILoggerFactory>(x =>
                {
                    var loggerProvider = x.GetRequiredService<ILoggerProvider>();

                    return new LoggerFactory(new[] { loggerProvider });
                });

            return services;
        }

        /// <summary>
        /// Adds eventing provider of type <typeparamref name="TProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <typeparamref name="TProvider"/> type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddEventing<TProvider>(this IServiceCollection services)
            where TProvider : class, IEventingProvider
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddSingleton<IEventingProvider, TProvider>()
                .AddSingleton(x => x
                    .GetRequiredService<IEventingProvider>()
                    .Configure())
                .AddEventingHandlers()
                .AddEventingHandlerAttributes();
        }

        /// <summary>
        /// Adds data provider for <see cref="DbContext"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <see cref="IDataProvider"/> implementation.</typeparam>
        /// <typeparam name="TContext">The <see cref="DbContext"/> implementation.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddDataContext<TProvider, TContext>(this IServiceCollection services)
            where TProvider : class, IDataProvider
            where TContext : DefaultDbContext
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<DbContext, TContext>()
                .AddScoped<BaseDbContext, TContext>()
                .AddScoped<DefaultDbContext, TContext>()
                .AddSingleton<IDataProvider, TProvider>()
                .AddDbContext<TContext>((provider, builder) =>
                {
                    provider
                        .GetRequiredService<IDataProvider>()
                        .Configure(builder);
                });
        }

        /// <summary>
        /// Adds <see cref="AppOptions"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddApp(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddConfigOptions<AppOptions>(configuration, AppOptions.SectionName, out var options);

            var assembly = typeof(HomeController).GetTypeInfo().Assembly;

            services
                .AddApi()
                .AddCors()
                .AddSession()
                // .AddAuthorization() // FEATURE: Secuirty, AddAuthorization
                .AddLocalizations()
                .AddGzipCompression()
                .AddApiVersioning(options)
                .AddApiDocumentation(options)
                .AddHttpContentTypeMiddleware()
                .AddHttpExceptionHandlingMiddleware()
                .AddHttpRequestIdentifierMiddleware()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddMvc(x =>
                {
                    x.Conventions.Insert(0, new RoutePrefixConvention(new RouteAttribute(options.Hosting.Root)));
                    x.ModelBinderProviders.Insert(0, new QueryModelBinderProvider());
                })
                .AddJsonOptions(x =>
                {
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

        /// <summary>
        /// Adds <see cref="DataOptions"/> appOptions to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddScoped<DbContext, DefaultDbContext>()
                .AddScoped<BaseDbContext, DefaultDbContext>()
                .AddScoped<IService, DefaultService>()
                .AddScoped<IServiceSpatial, DefaultServiceSpatial>()
                .AddConfigOptions<DataOptions>(configuration, DataOptions.SectionName, out var options);

            if (options.UseMemoryCache)
            {
                services
                    .AddDistributedMemoryCache();
            }

            if (options.UseAudit)
            {
                var httpContextAccessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();

                AuditManager.DefaultConfiguration.Include<IEntity>();
                AuditManager.DefaultConfiguration.IncludeProperty<IEntity>();
                AuditManager.DefaultConfiguration.IncludeDataAnnotation();
                AuditManager.DefaultConfiguration.ExcludeDataAnnotation();
                AuditManager.DefaultConfiguration.AutoSavePreAction = (dbContext, audit) =>
                {
                    var defaultDbContext = dbContext as DefaultDbContext;
                    var defaultAuditEntries = audit.Entries.Where(x => x.AuditEntryID == 0).Cast<DefaultAuditEntry>();

                    defaultDbContext?.__EFAudit.AddRange(defaultAuditEntries);
                };
                AuditManager.DefaultConfiguration.AuditEntryFactory = args =>
                {
                    var httpRequestIdentifierFeature = httpContextAccessor.HttpContext.Features.Get<IHttpRequestIdentifierFeature>();

                    return new DefaultAuditEntry
                    {
                        RequestId = httpRequestIdentifierFeature.TraceIdentifier
                    };
                };
                AuditManager.DefaultConfiguration.SoftDeleted<IEntityDeletableSoft>(x => x.IsActive);
            }
            else
            {
                AuditManager.DefaultConfiguration.Exclude(x => true);
                AuditManager.DefaultConfiguration.AutoSavePreAction = null;
            }

            return services;
        }

        /// <summary>
        /// Adds <see cref="IConfiguration"/> appOptions to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddOptions()
                .AddSingleton(configuration);
        }

        /// <summary>
        /// Adds <see cref="LoggingOptions"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return services
                .AddConfigOptions<LoggingOptions>(configuration, LoggingOptions.SectionName, out var _);
        }

        /// <summary>
        /// Adds <see cref="EventingOptions"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddEventing(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            return services
                .AddSingleton<IEventing, NullEventing>()
                .AddConfigOptions<EventingOptions>(configuration, EventingOptions.SectionName, out _);
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
        private static IServiceCollection AddEventingHandlers(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .SelectMany(x => x.GetInterfaces(), (x, y) => new { Type = x, GenericType = y })
                .Where(x =>
                    x.Type != typeof(EntityEventHandler) &&
                    x.Type.IsTypeDef(typeof(IEventingHandler<>))) 
                .ToList()
                .ForEach(x =>
                {
                    var handlerType = x.Type;
                    var genericHandlerType = x.GenericType;
                    var eventType = x.GenericType.GetGenericArguments()[0];

                    services
                        .AddScoped(genericHandlerType, handlerType);

                    var provider = services.BuildServiceProvider();
                    var eventing = provider.GetRequiredService<IEventing>();
                    var eventHandler = provider.GetRequiredService(genericHandlerType);

                    var callback = handlerType.GetMethod("CallbackAsync");
                    var action = typeof(Action<>).MakeGenericType(eventType);
                    var @delegate = Delegate.CreateDelegate(action, eventHandler, callback);

                    eventing
                        .GetType()
                        .GetMethod("SubscribeAsync")
                        .MakeGenericMethod(eventType)
                        .Invoke(eventing, new object[] { @delegate, string.Empty });
                });

            return services;
        }
        private static IServiceCollection AddEventingHandlerAttributes(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.GetCustomAttributes<SubscribeAttribute>().Any())
                .ToList()
                .ForEach(x =>
                {
                    var eventType = typeof(EntityEvent);
                    var handlerType = typeof(EntityEventHandler);
                    var genericHandlerType = typeof(IEventingHandler<EntityEvent>);

                    services
                        .AddScoped(genericHandlerType, handlerType);

                    var provider = services.BuildServiceProvider();
                    var eventing = provider.GetRequiredService<IEventing>();
                    var eventHandler = provider.GetRequiredService(genericHandlerType);

                    var callback = handlerType.GetMethod("CallbackAsync");
                    var action = typeof(Action<>).MakeGenericType(eventType);
                    var @delegate = Delegate.CreateDelegate(action, eventHandler, callback);

                    eventing
                        .GetType()
                        .GetMethod("SubscribeAsync")
                        .MakeGenericMethod(eventType)
                        .Invoke(eventing, new object[] { @delegate, x.Name });
                });

            return services;
        }
        private static IServiceCollection AddApiVersioning(this IServiceCollection services, AppOptions appOptions)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (appOptions == null)
                throw new ArgumentNullException(nameof(appOptions));

            var success = ApiVersion.TryParse(appOptions.Version, out var apiVersion);
            if (!success)
                apiVersion = new ApiVersion(1, 0);

            return services
                .AddApiVersioning(x =>
                {
                    x.ReportApiVersions = true;
                    x.DefaultApiVersion = apiVersion;
                    x.AssumeDefaultVersionWhenUnspecified = true;
                    x.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
                });
        }
        private static IServiceCollection AddApiDocumentation(this IServiceCollection services, AppOptions appOptions)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (appOptions == null)
                throw new ArgumentNullException(nameof(appOptions));

            return services
                .AddSwaggerGen(x =>
                {
                    x.IgnoreObsoleteActions();
                    x.IgnoreObsoleteProperties();
                    x.DescribeAllEnumsAsStrings();
                    x.AddSecurityDefinition("Basic", new BasicAuthScheme()); // FEATURE: Security, Swagger doc

                    x.DocumentFilter<LowercaseDocumentFilter>();

                    x.SwaggerDoc(appOptions.Version, new Info
                    {
                        Title = appOptions.Name,
                        Version = appOptions.Version,
                        Contact = appOptions.Contact,
                        Description = appOptions.Description,
                        License = appOptions.License,
                        TermsOfService = appOptions.TermsOfService
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
                            var fileName = y.Name.Replace(".dll", ".xml").Replace(".exe", ".xml");
                            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

                            if (File.Exists(filePath))
                                x.IncludeXmlComments(filePath);
                        });
                });
        }
        private static IServiceCollection AddHttpContentTypeMiddleware(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddSingleton<HttpContentTypeMiddleware>()
                .AddMvc(x =>
                {
                    x.ReturnHttpNotAcceptable = true;
                    x.RespectBrowserAcceptHeader = true;

                    x.FormatterMappings.SetMediaTypeMappingForFormat("json", HttpContentType.Json);
                    x.FormatterMappings.SetMediaTypeMappingForFormat("json", HttpContentType.JavaScript);
                    x.FormatterMappings.SetMediaTypeMappingForFormat("xml", HttpContentType.Xml);
                    x.FormatterMappings.SetMediaTypeMappingForFormat("html", HttpContentType.Html);
                    x.FormatterMappings.SetMediaTypeMappingForFormat("text", HttpContentType.Text);
                })
                .AddXmlSerializerFormatters()
                .AddXmlDataContractSerializerFormatters();

            return services;
        }
        private static IServiceCollection AddHttpExceptionHandlingMiddleware(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<HttpExceptionHandlingMiddleware>();

            return services;
        }
        private static IServiceCollection AddHttpRequestIdentifierMiddleware(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<HttpRequestIdentifierMiddleware>();

            return services;
        }
        private static IServiceCollection AddConfigOptions<TOption>(this IServiceCollection services, IConfiguration configuration, string name, out TOption options)
            where TOption : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var section = configuration.GetSection(name);
            options = section?.Get<TOption>() ?? new TOption();

            return services
                .AddSingleton(options)
                .Configure<TOption>(section);
        }
    }
}