using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Console.Workers;
using Nano.Common.Extensions;
using System;
using System.Globalization;
using System.Linq;
using Nano.App.Console.Config;
using Nano.Common.Helpers;

namespace Nano.App.Console.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoWorkers(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var types = TypesHelper
            .GetAllTypes()
            .Where(x =>
                !x.IsAbstract &&
                x.IsTypeOf(typeof(BaseWorker)))
            .GroupBy(x => x.FullName)
            .Select(x => x.FirstOrDefault())
            .Where(x => x != null);

        foreach (var type in types)
        {
            services
                .AddSingleton(typeof(IHostedService), type!);
        }

        return services;
    }

    internal static IServiceCollection AddNanoCultureInfo(this IServiceCollection services, LocalizationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(options.DefaultCulture);

        return services;
    }
}