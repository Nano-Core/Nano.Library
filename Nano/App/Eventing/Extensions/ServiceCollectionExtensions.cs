using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Eventing.Options;

namespace Nano.App.Eventing.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds eventing to the <see cref="IServiceCollection"/>.
        /// Configures <see cref="EventingOptions"/> for the passed <paramref name="configuration"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddEventing(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var section = configuration.GetSection("Logging");
            var options = section?.Get<EventingOptions>() ?? new EventingOptions();

            services
                .AddSingleton(options)
                .Configure<EventingOptions>(section);

            return services;
        }
    }
}