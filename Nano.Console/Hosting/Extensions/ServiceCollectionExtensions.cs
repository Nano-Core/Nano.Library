using System;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Console.Hosting.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Clone the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The cloned <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection Clone(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            IServiceCollection clonedServices = new ServiceCollection();

            foreach (var service in services)
            {
                clonedServices.Add(service);
            }

            return clonedServices;
        }
    }
}