using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.Config.Extensions;
using Nano.Eventing.Attributes;
using Nano.Eventing.Handlers;
using Nano.Eventing.Interfaces;
using Nano.Eventing.Providers.EasyNetQ;
using Nano.Models.Extensions;

namespace Nano.Eventing.Extensions
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

            var options = services
                .BuildServiceProvider()
                .GetService<EventingOptions>();

            services
                .AddSingleton<IEventingProvider, TProvider>()
                .AddScoped(x => x
                    .GetRequiredService<IEventingProvider>()
                    .Configure())
                .AddEventingHandlers()
                .AddEventingHandlerAttributes()
                .AddEventingHealthChecks<TProvider>(options);

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

            return services
                .AddScoped<IEventing, NullEventing>()
                .AddConfigOptions<EventingOptions>(configuration, EventingOptions.SectionName, out _);
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
                    !x.Type.IsAbstract &&
                    x.Type.IsTypeOf(typeof(IEventingHandler<>)) &&
                    x.Type != typeof(EntityEventHandler))
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

                    eventing
                        .GetType()
                        .GetMethod("SubscribeAsync")?
                        .MakeGenericMethod(eventType)
                        .Invoke(eventing, new object[]
                        {
                            provider, 
                            string.Empty
                        });
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
                .Where(x =>
                    !x.IsAbstract &&
                    !x.Name.EndsWith("Proxy") &&
                    x.GetCustomAttributes<SubscribeAttribute>().Any())
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

                    eventing
                        .GetType()
                        .GetMethod("SubscribeAsync")?
                        .MakeGenericMethod(eventType)
                        .Invoke(eventing, new object[] { provider, x.Name });
                });

            return services;
        }
        private static IServiceCollection AddEventingHealthChecks<TProvider>(this IServiceCollection services, EventingOptions options)
            where TProvider : class, IEventingProvider
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (!options.UseHealthCheck)
                return services;

            if (typeof(TProvider) == typeof(EasyNetQProvider))
            {
                services
                    .AddHealthChecks()
                    .AddRabbitMQ(options.ConnectionString);
            }

            return services;
        }
    }
}