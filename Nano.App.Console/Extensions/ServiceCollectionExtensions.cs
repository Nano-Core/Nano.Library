using Microsoft.Extensions.DependencyInjection;
using Nano.App.Console.Config;
using Nano.App.Console.Workers;
using Nano.App.Console.Workers.Abstractions;
using Nano.Common.Extensions;
using Nano.Common.Helpers;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

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
                x.IsTypeOf(typeof(IWorker)))
            .GroupBy(x => x.FullName)
            .Select(x => x.FirstOrDefault())
            .Where(x => x != null);

        foreach (var type in types)
        {
            services
                .AddScoped(typeof(IWorker), type!);
        }

        services
            .AddHostedService<WorkerHostedService>();

        return services;
    }

    internal static IServiceCollection AddNanoCultureInfo(this IServiceCollection services, LocalizationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        var culture = new CultureInfo(options.DefaultCulture);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        return services;
    }
}