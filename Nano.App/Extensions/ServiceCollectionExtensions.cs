using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nano.App.Api;
using Nano.App.Interfaces;
using Nano.Data;
using Nano.Models.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Nano.Common.Config.Extensions;
using Nano.Common.Extensions;
using Nano.Common.Helpers;
using Nano.Common.Startup;
using Nano.Common.Startup.Tasks;

namespace Nano.App.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="AppOptions"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddApp<TApplication>(this IServiceCollection services, IConfiguration configuration) 
        where TApplication : class, IApplication
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        services
            .AddSingleton<IApplication, TApplication>();

        services
            .AddConfigSection<AppOptions>(AppOptions.SectionName, out _);

        TypesHelper.GetAllTypes()
            .Where(x =>
                !x.IsAbstract &&
                x.IsTypeOf(typeof(BaseStartupTask)))
            .ToList()
            .ForEach(x =>
            {
                services
                    .AddSingleton(typeof(IHostedService), x);
            });

        services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddSingleton<StartupTaskContext>();

        services
            .AddConfig(configuration);
            //.AddApis(configuration); // BUG: AddApis

        return services;
    }

    ///// <summary>
    ///// Add Apis to the <see cref="IServiceCollection"/>..
    ///// </summary>
    ///// <param name="services">The <see cref="IServiceCollection"/>.</param>
    ///// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    ///// <returns>The <see cref="IServiceCollection"/>.</returns>
    //public static IServiceCollection AddApis(this IServiceCollection services, IConfiguration configuration)
    //{
    //    var hosts = new List<string>();

    //    var types = TypesHelper.GetAllTypes()
    //        .Where(x => !x.IsAbstract && x.IsTypeOf(typeof(BaseApi)))
    //        .Distinct();

    //    foreach (var type in types)
    //    {
    //        var section = configuration.GetSection(type.Name);
    //        var options = section.Get<ApiOptions>();

    //        if (options == null)
    //        {
    //            continue;
    //        }

    //        var optionsServiceId = $"{type.Name}_Options";

    //        services
    //            .AddKeyedSingleton(optionsServiceId, options);

    //        services
    //            .AddHttpClient(type.Name, (serviceProvider, client) =>
    //            {
    //                var apiOptions = serviceProvider
    //                    .GetRequiredKeyedService<ApiOptions>(optionsServiceId);

    //                client.Timeout = TimeSpan.FromSeconds(apiOptions.TimeoutInSeconds);
    //                client.BaseAddress = new Uri(apiOptions.Host);
    //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentType.JSON));
    //                client.DefaultRequestVersion = new Version(2, 0);
    //            })
    //            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    //            .ConfigurePrimaryHttpMessageHandler(() =>
    //                new HttpClientHandler
    //                {
    //                    AllowAutoRedirect = true,
    //                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    //                });

    //        services
    //            .AddScoped(type, serviceProvider =>
    //            {
    //                var httpClientFactory = serviceProvider
    //                    .GetRequiredService<IHttpClientFactory>();
                    
    //                var httpClient = httpClientFactory
    //                    .CreateClient(type.Name);

    //                var apiOptions = serviceProvider
    //                    .GetRequiredKeyedService<ApiOptions>(optionsServiceId);

    //                return Activator.CreateInstance(type, apiOptions, httpClient);
    //            });

    //        if (!hosts.Contains(options.Host) && options.UseHealthCheck)
    //        {
    //            services.AddHealthChecks()
    //                .AddTcpHealthCheck(y => y
    //                    .AddHost(options.Host, options.Port), options.Host, options.UnhealthyStatus);

    //            hosts
    //                .Add(options.Host);
    //        }
    //    }

    //    return services;
    //}
}