using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nano.Api;
using Nano.Controllers.Criterias.Binders.Providers;
using Nano.Data;
using Nano.Data.Interfaces;
using Nano.Eventing;
using Nano.Eventing.Interfaces;
using Nano.Hosting;
using Nano.Hosting.Middleware;
using Nano.Logging;
using Nano.Services;
using Nano.Services.Interfaces;
using Serilog;
using Serilog.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

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

            services
                .AddSingleton<IEventingProvider, TProvider>()
                .AddSingleton(provider => provider
                    .GetRequiredService<IEventingProvider>()
                    .Configure())
                .AddEventingHandlers();

            return services;
        }

        /// <summary>
        /// Adds data provider for <see cref="IDbContext"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <see cref="IDataProvider"/> implementation.</typeparam>
        /// <typeparam name="TContext">The <see cref="IDbContext"/> implementation.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddDataContext<TProvider, TContext>(this IServiceCollection services)
            where TProvider : class, IDataProvider
            where TContext : DbContext, IDbContext
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<IDbContext, TContext>()
                .AddSingleton<IDataProvider, TProvider>()
                .AddDbContext<TContext>((provider, builder) =>
                {
                    provider
                        .GetRequiredService<IDataProvider>()
                        .Configure(builder);
                });

            return services;
        }

        /// <summary>
        /// Adds <see cref="BaseApi"/> implementations to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddApi(this IServiceCollection services)
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

        /// <summary>
        /// Adds <see cref="IConfiguration"/> options to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddOptions()
                .AddSingleton(configuration)
                .AddConfigOptions<AppOptions>(configuration, "App", out _);

            return services;
        }

        /// <summary>
        /// Adds a options <see cref="IConfigurationSection"/> as <see cref="IOptions{TOptions}"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TOption">The option implementation type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <param name="name">The name of the <see cref="IConfigurationSection"/>.</param>
        /// <param name="options">The <typeparamref name="TOption"/> instance.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddConfigOptions<TOption>(this IServiceCollection services, IConfiguration configuration, string name, out TOption options)
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

            services
                .AddSingleton(options)
                .Configure<TOption>(section);

            return services;
        }

        /// <summary>
        /// Adds <see cref="HostingOptions"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHosting(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddVersioning()
                .AddHttpContextException()
                .AddHttpContextContentTypes()
                .AddConfigOptions<HostingOptions>(configuration, HostingOptions.SectionName, out var options)
                .AddMvc(x =>
                {
                    x.ModelBinderProviders.Insert(0, new QueryModelBinderProvider());
                })
                .AddControllersAsServices()
                .AddViewComponentsAsServices()
                .AddApplicationPart(Assembly.GetExecutingAssembly());

            if (options.EnableSession)
                services.AddSession();

            if (options.EnableDocumentation)
                services.AddDocumentation(configuration);

            if (options.EnableGzipCompression)
                services.AddGzipCompression();

            if (options.EnableHttpContextLogging)
                services.AddHttpContextLogging();

            if (options.EnableHttpContextIdentifier)
                services.AddHttpContextIdentifier();

            if (options.EnableHttpContextLocalization)
                services.AddHttpContextLocalization();

            return services;
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

            services
                .AddConfigOptions<LoggingOptions>(configuration, LoggingOptions.SectionName, out _);

            services
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

            return services;
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

            services
                .AddConfigOptions<EventingOptions>(configuration, EventingOptions.SectionName, out _);

            return services;
        }

        /// <summary>
        /// Adds eventing handlers for all implementations of <see cref="IEventingHandler{TEvent}"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddEventingHandlers(this IServiceCollection services)
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
                        .GetMethod("Consume")
                        .MakeGenericMethod(eventType)
                        .Invoke(eventing, new object[] { @delegate, false });
                });

            return services;
        }

        /// <summary>
        /// Adds <see cref="DataOptions"/> options to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddScoped<IService, DefaultService>()
                .AddScoped<IServiceSpatial, DefaultServiceSpatial>()
                .AddConfigOptions<DataOptions>(configuration, DataOptions.SectionName, out var options);

            if (options.UseMemoryCache)
                services.AddDistributedMemoryCache();

            return services;
        }

        /// <summary>
        /// Adds Swagger generated documentation to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddDocumentation(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var options = configuration.GetSection("App").Get<AppOptions>();

            return services
                .AddSwaggerGen(x =>
                {
                    // BUG: DOCUMENTATION: Issue with Swagger not able to generate documentation from generic operations (path/action needs to be unique, and it can't see that in generic base class controller it seems) 
                    x.SwaggerDoc(options.Version, new Info
                    {
                        Title = options.Name,
                        Version = options.Version,
                        Description = options.Description
                    });
                });
        }

        /// <summary>
        /// Adds api versioningto the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddVersioning(this IServiceCollection services)
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

        /// <summary>
        /// Adds <see cref="GzipCompressionProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddGzipCompression(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddResponseCompression(y => y.Providers.Add<GzipCompressionProvider>());

            return services;
        }

        /// <summary>
        /// Adds <see cref="HttpContextLoggingMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpContextLogging(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<HttpContextLoggingMiddleware>();
        }

        /// <summary>
        /// Adds <see cref="HttpContextExceptionMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpContextException(this IServiceCollection services)
        {
            services
                .AddScoped<HttpContextExceptionMiddleware>();

            return services;
        }

        /// <summary>
        /// Adds <see cref="HttpContextIdentifierMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpContextIdentifier(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<HttpContextIdentifierMiddleware>();
        }

        /// <summary>
        /// Adds Request and View localiztion to the <see cref="IMvcBuilder"/> and <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpContextLocalization(this IServiceCollection services)
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

        /// <summary>
        /// Adds <see cref="HttpContextContentTypeMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpContextContentTypes(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<HttpContextExceptionMiddleware>()
                .AddScoped<HttpContextContentTypeMiddleware>()
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
    }
}