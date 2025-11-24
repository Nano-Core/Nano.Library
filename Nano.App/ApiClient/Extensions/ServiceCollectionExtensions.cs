using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.ApiClient.Consts;
using Nano.Common.Extensions;
using Nano.Common.Helpers;

namespace Nano.App.ApiClient.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddApis(this IServiceCollection services, IConfiguration configuration)
    {
        var hosts = new List<string>();

        var types = TypesHelper.GetAllTypes()
            .Where(x => !x.IsAbstract && x.IsTypeOf(typeof(BaseApi)))
            .Distinct();

        foreach (var type in types)
        {
            var section = configuration.GetSection(type.Name);
            var options = section.Get<ApiOptions>();

            if (options == null)
            {
                continue;
            }

            var optionsServiceId = $"{type.Name}_Options";

            services
                .AddKeyedSingleton(optionsServiceId, options);

            services
                .AddHttpClient(type.Name, (serviceProvider, client) =>
                {
                    var apiOptions = serviceProvider
                        .GetRequiredKeyedService<ApiOptions>(optionsServiceId);

                    client.Timeout = TimeSpan.FromSeconds(apiOptions.TimeoutInSeconds);
                    client.BaseAddress = new Uri(apiOptions.Host);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentType.JSON));
                    client.DefaultRequestVersion = new Version(2, 0);
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler
                    {
                        AllowAutoRedirect = true,
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                    });

            services
                .AddScoped(type, serviceProvider =>
                {
                    var httpClientFactory = serviceProvider
                        .GetRequiredService<IHttpClientFactory>();

                    var httpClient = httpClientFactory
                        .CreateClient(type.Name);

                    var apiOptions = serviceProvider
                        .GetRequiredKeyedService<ApiOptions>(optionsServiceId);

                    return Activator.CreateInstance(type, apiOptions, httpClient);
                });

            if (!hosts.Contains(options.Host) && options.UseHealthCheck)
            {
                services.AddHealthChecks()
                    .AddTcpHealthCheck(y => y
                        .AddHost(options.Host, options.Port), options.Host, options.UnhealthyStatus);

                hosts
                    .Add(options.Host);
            }
        }

        return services;
    }
}