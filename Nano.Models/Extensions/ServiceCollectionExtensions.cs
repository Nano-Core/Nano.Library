using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Nano.Models.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Gets a colletion of dependencies registered in the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="isUnique">Bool indicating if only unique service dependencies should be returned.</param>
        /// <returns>The <see cref="IEnumerator{T}"/> of <see cref="ServiceDescriptor"/> items.</returns>
        public static IEnumerable<ServiceDescriptor> GetServices(this IServiceCollection services, bool isUnique = true)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var list = new List<ServiceDescriptor>();

            services
                .OrderBy(x => x.ServiceType.FullName)
                .ToList()
                .ForEach(x =>
                {
                    if (isUnique)
                    {
                        var exists = list.Any(y =>
                            x.ServiceType == y.ServiceType &&
                            x.ImplementationType == y.ImplementationType &&
                            x.Lifetime == y.Lifetime);

                        if (!exists)
                            list.Add(x);
                    }
                    else
                        list.Add(x);
                });

            return list;
        }
    }
}