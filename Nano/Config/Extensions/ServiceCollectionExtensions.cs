using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nano.Config.Providers.Data.Interfaces;
using Nano.Config.Providers.Eventing.Interfaces;
using Nano.Config.Providers.Hosting;
using Nano.Config.Providers.Logging.Interfaces;
using Nano.Controllers.Criterias.Binders.Providers;
using Nano.Data.Interfaces;
using Nano.Services;
using Nano.Services.Interfaces;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;

namespace Nano.Config.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds logging provider of type <typeparamref name="TProvider"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TProvider">The <typeparamref name="TProvider"/> type.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddLogging<TProvider>(this IServiceCollection services)
            where TProvider : class, ILoggingProvider, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddSingleton<ILoggingProvider, TProvider>()
                .AddSingleton(x =>
                {
                    var options = x.GetRequiredService<LoggingOptions>();

                    var loggerConfiguration = new LoggerConfiguration()
                        .WriteTo.Sink<TProvider>()
                        .MinimumLevel.Is(options.LogLevel);

                    foreach (var @override in options.LogLevelOverrides)
                    {
                        loggerConfiguration
                            .MinimumLevel.Override(@override.Namespace, @override.LogLevel);
                    }

                    return Log.Logger = loggerConfiguration.CreateLogger();
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

            services
                .AddSingleton<IEventingProvider, TProvider>()
                .AddSingleton(provider =>
                {
                    var options = provider.GetRequiredService<EventingOptions>();

                    var bus = provider
                        .GetRequiredService<IEventingProvider>()
                        .Configure(options);

                    return bus;
                });

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
                    var options = provider.GetRequiredService<DataOptions>();

                    provider
                        .GetRequiredService<IDataProvider>()
                        .Configure(builder, options);
                });

            return services;
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
                .AddConfigOptions<AppOptions>(configuration, "App", out _);

            return services;
        }

        /// <summary>
        /// Adds <see cref="IConfiguration"/> options to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddConfig(this IServiceCollection services, out IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            const string NAME = "appsettings";

            var path = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddEnvironmentVariables()
                .AddJsonFile($"{NAME}.json", false, true)
                .AddJsonFile($"{NAME}.{environment}.json", true)
                .Build();

            services
                .AddOptions()
                .AddSingleton(configuration);

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
                .AddHttpContextContentTypes()
                .AddHttpContextException()
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

            if (options.EnableHttpContextExtension)
                services.AddHttpContextExtensions();

            if (options.EnableHttpRequestIdentifier)
                services.AddHttpRequestIdentifier();

            if (options.EnableHttpRequestLocalization)
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
                    x.SwaggerDoc(options.Version.ToString(), new Info
                    {
                        Title = options.Name,
                        Version = options.Version.ToString(),
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
        /// Adds <see cref="HttpRequestIdentifierMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpRequestIdentifier(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<HttpRequestIdentifierMiddleware>();
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
        /// Adds <see cref="HttpContextLoggingMiddleware"/> to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        internal static IServiceCollection AddHttpContextExtensions(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<HttpContextLoggingMiddleware>();
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
                .AddScoped<HttpContextContentTypeMiddleware>()
                .AddScoped<HttpContextExceptionMiddleware>()
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

        // TODO: Test / Fix eventing consume.
        /// <summary>
        /// Adds an eventing consumer to the <see cref="IServiceCollection"/>. 
        /// </summary>
        /// <typeparam name="TEvent">The <see cref="IEvent"/>.</typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddEventingConsumer<TEvent, THandler>(this IServiceCollection services)
            where TEvent : class, IEvent
            where THandler : class, IEventHandler<TEvent>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                .AddScoped<IEventHandler<TEvent>, THandler>(provider =>
                {
                    provider
                        .GetRequiredService<IEventingProvider>()
                        .Consume<TEvent>(x =>
                        {
                            provider
                                .GetRequiredService<IEventHandler<TEvent>>()
                                .Callback(x);
                        });

                    return null;
                });

            return services;
        }
    }
}