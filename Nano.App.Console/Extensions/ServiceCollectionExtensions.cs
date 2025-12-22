using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Console.Config;
using Nano.App.Console.Workers;
using Nano.Common.Config.Extensions;
using Nano.Common.Extensions;
using Nano.Common.Helpers;
using System;
using System.Globalization;
using System.Linq;
using Nano.App.Config;

namespace Nano.App.Console.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="ConsoleOptions"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddConsole(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        services
            .AddConfigSection<ConsoleOptions>(BaseAppOptions.SectionName, out var options);

        TypesHelper.GetAllTypes()
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

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(options.Cultures.Default);
        return services;
    }
}