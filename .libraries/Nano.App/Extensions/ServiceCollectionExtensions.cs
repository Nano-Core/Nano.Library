using System;
using System.Linq;
using System.Reflection;
using EasyNetQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nano.Eventing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.Data;
using Nano.Data.Interfaces;
using Nano.Eventing.Attributes;
using Nano.Eventing.Interfaces;
using Nano.Eventing.Providers;
using Nano.Services;
using Nano.Services.Data;
using Nano.Services.Eventing;
using Nano.Services.Interfaces;
using Nano.Web.Api;
using Nano.Web.Controllers.Binders.Providers;
using Nano.Web.Controllers.Extensions.Const;
using Nano.Web.Middleware;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using LoggingOptions = Nano.Logging.LoggingOptions;

namespace Nano.App.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
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
                .AddSingleton(provider => provider
                    .GetRequiredService<IEventingProvider>()
                    .Configure())
                .AddEventingHandlers();
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
                .AddSingleton<IDataProvider, TProvider>()
                .AddScoped<DbContext, TContext>()
                .AddScoped<BaseDbContext, TContext>()
                .AddScoped<DefaultDbContext, TContext>()
                .AddDbContext<TContext>((provider, builder) =>
                {
                    provider
                        .GetRequiredService<IDataProvider>()
                        .Configure(builder);
                });
        }

        /// <summary>
        /// Adds a options <see cref="IConfigurationSection"/> as <see cref="IOptions{TOptions}"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TOption">The option implementation type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="name">The name of the <see cref="IConfigurationSection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddConfigOptions<TOption>(this IServiceCollection services, string name)
            where TOption : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var configuration = services
                .BuildServiceProvider()
                .GetRequiredService<IConfiguration>();

            services
                .AddConfigOptions(configuration, name, out TOption _);

            return services;
        }

        /// <summary>
        /// Adds <see cref="AppOptions"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddApp(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddConfigOptions<AppOptions>(configuration, AppOptions.SectionName, out var options);

            services
                .AddApi()
                .AddVersioning()
                .AddExceptionMiddleware()
                .AddContentTypeMiddleware()
                .AddMvc(x =>
                {
                    x.ModelBinderProviders.Insert(0, new QueryModelBinderProvider());
                })
                .AddJsonOptions(y =>
                {
                    y.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    y.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    y.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    y.SerializerSettings.ContractResolver = new DefaultContractResolver(); // TODO: Custom contract resolver from appsettings?
                })
                .AddControllersAsServices()
                .AddViewComponentsAsServices()
                .AddApplicationPart(Assembly.GetExecutingAssembly());

            if (options.Switches.EnableSession)
                services.AddSession();

            if (options.Switches.EnableDocumentation)
                services.AddDocumentation();

            if (options.Switches.EnableGzipCompression)
                services.AddGzipCompression();

            if (options.Switches.EnableHttpContextLogging)
                services.AddLoggingMiddleware();

            if (options.Switches.EnableHttpContextIdentifier)
                services.AddTraceIdentifierMiddleware();

            if (options.Switches.EnableHttpContextLocalization)
                services.AddLocalizations();

            return services;
        }

        /// <summary>
        /// Adds <see cref="DataOptions"/> options to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddScoped<BaseDbContext, DefaultDbContext>()
                .AddScoped<IService, DefaultService>()
                .AddScoped<IServiceSpatial, DefaultServiceSpatial>()
                .AddConfigOptions<DataOptions>(configuration, DataOptions.SectionName, out var options);

            if (options.UseMemoryCache)
                services.AddDistributedMemoryCache();

            return services;
        }

        /// <summary>
        /// Adds <see cref="IConfiguration"/> options to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
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
        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddConfigOptions<LoggingOptions>(configuration, LoggingOptions.SectionName, out _);

            return services
                .AddSingleton(Log.Logger)
                .AddSingleton<ILoggerProvider>(x => new SerilogLoggerProvider(Log.Logger, true))
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
        }

        /// <summary>
        /// Adds <see cref="EventingOptions"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddEventing(this IServiceCollection services, IConfiguration configuration)
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
                        .AddSingleton(x);
                });

            return services;
        }
        private static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddApiVersioning(x =>
                {
                    x.ReportApiVersions = true;
                    x.DefaultApiVersion = new ApiVersion(1, 0);
                    x.AssumeDefaultVersionWhenUnspecified = true;
                    x.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
                });
        }
        private static IServiceCollection AddDocumentation(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var options = services.BuildServiceProvider().GetRequiredService<AppOptions>();

            return services
                .AddSwaggerGen(x =>
                {
                    x.SwaggerDoc(options.Version, new Info
                    {
                        Title = options.Name,
                        Version = options.Version,
                        Description = options.Description
                    });
                });
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
                .SelectMany(x => x.GetInterfaces(), (x, y) => new { x, y })
                .Where(x =>
                    x.x.BaseType != null &&
                    x.x.BaseType.IsGenericType && typeof(IEventingHandler<>).IsAssignableFrom(x.x.BaseType.GetGenericTypeDefinition()) ||
                    x.y.IsGenericType && typeof(IEventingHandler<>).IsAssignableFrom(x.y.GetGenericTypeDefinition()))
                .ToList()
                .ForEach(x =>
                {
                    var handlerType = x.x;
                    var genericHandlerType = x.y;
                    var eventType = x.y.GetGenericArguments()[0];

                    services
                        .AddScoped(genericHandlerType, handlerType);

                    var provider = services.BuildServiceProvider();
                    var eventing = provider.GetRequiredService<IEventing>();
                    var eventHandler = provider.GetRequiredService(genericHandlerType);

                    var callback = handlerType.GetMethod("Callback");
                    var action = typeof(Action<>).MakeGenericType(eventType);
                    var @delegate = Delegate.CreateDelegate(action, eventHandler, callback);

                    eventing
                        .GetType()
                        .GetMethod("Subscribe")
                        .MakeGenericMethod(eventType)
                        .Invoke(eventing, new object[] { @delegate, string.Empty }); 
                });

            // TODO: EVENTING: Event Handler Subscribe setup
            AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.GetAttributes<SubscribeAttribute>().Any())
                .ToList()
                .ForEach(x =>
                {
                    var handlerType = typeof(EntityEventHandler);
                    var genericHandlerType = typeof(IEventingHandler<EntityEvent>);
                    var eventType = typeof(EntityEvent);

                    var provider = services.BuildServiceProvider();
                    var eventing = provider.GetRequiredService<IEventing>();
                    var eventHandler = provider.GetRequiredService(genericHandlerType);

                    var callback = handlerType.GetMethod("Callback");
                    var action = typeof(Action<>).MakeGenericType(eventType);
                    var @delegate = Delegate.CreateDelegate(action, eventHandler, callback);

                    eventing
                        .GetType()
                        .GetMethod("Subscribe")
                        .MakeGenericMethod(eventType)
                        .Invoke(eventing, new object[] { @delegate, x.Name });
                });

            return services;
        }
        private static IServiceCollection AddLoggingMiddleware(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<LoggingExtensionMiddleware>();
        }
        private static IServiceCollection AddExceptionMiddleware(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<ExceptionHandlingMiddleware>();

            return services;
        }
        private static IServiceCollection AddContentTypeMiddleware(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<ExceptionHandlingMiddleware>()
                .AddScoped<ContentTypeMiddleware>()
                .AddMvc(x =>
                    {
                        x.ReturnHttpNotAcceptable = true;
                        x.RespectBrowserAcceptHeader = true;
                        x.FormatterMappings.SetMediaTypeMappingForFormat("xml", HttpContentType.Xml);
                        x.FormatterMappings.SetMediaTypeMappingForFormat("json", HttpContentType.Json);
                        x.FormatterMappings.SetMediaTypeMappingForFormat("json", HttpContentType.JavaScript);
                        x.FormatterMappings.SetMediaTypeMappingForFormat("html", HttpContentType.Html);
                    })
                    .AddXmlSerializerFormatters()
                    .AddXmlDataContractSerializerFormatters();

            return services;
        }
        private static IServiceCollection AddTraceIdentifierMiddleware(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<TraceIdentifierMiddleware>();

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