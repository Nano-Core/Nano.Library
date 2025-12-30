using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Console.Config;
using Nano.App.Console.Workers;
using Nano.Common.Extensions;
using System;
using System.Globalization;
using System.Linq;
using Nano.Common.Helpers;

namespace Nano.App.Console.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoWorkers(this IServiceCollection services)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        TypesHelper
            .GetAllTypes()
            .Where(x =>
                !x.IsAbstract &&
                x.IsTypeOf(typeof(BaseWorker)))
            .GroupBy(x => x.FullName)
            .Select(x => x.FirstOrDefault())
            .Where(x => x != null)
            .ToList()
            .ForEach(x =>
            {
                services
                    .AddSingleton(typeof(IHostedService), x);
            });

        return services;
    }

    internal static IServiceCollection AddNanoCultureInfo(this IServiceCollection services, ConsoleOptions options)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
            throw new ArgumentNullException(nameof(options));

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(options.Cultures.Default);

        return services;
    }
}