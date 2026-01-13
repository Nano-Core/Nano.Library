using Microsoft.Extensions.DependencyInjection;
using Nano.App.ApiClient.Config;
using Nano.App.ApiClient.Consts;
using Nano.App.Config;
using Nano.Common.Extensions;
using Nano.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Nano.Common.Mvc.HealthChecks.Extensions;

namespace Nano.App.ApiClient.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoApis(this IServiceCollection services, BaseAppOptions appOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(appOptions);

        var hosts = new List<string>();

        var types = TypesHelper
            .GetAllTypes()
            .Where(x => !x.IsAbstract && x.IsTypeOf(typeof(BaseApi)))
            .Distinct();

        foreach (var type in types)
        {
            appOptions.Apis
                .TryGetValue(type.Name, out var options);

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
                        .GetRequiredKeyedService<ApiClientOptions>(optionsServiceId);

                    client.Timeout = apiOptions.Timeout;
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
                        .GetRequiredKeyedService<ApiClientOptions>(optionsServiceId);

                    var httpContextAccessor = serviceProvider
                        .GetRequiredService<IHttpContextAccessor>();

                    return Activator.CreateInstance(type, apiOptions, httpClient, httpContextAccessor);
                });

            if (!hosts.Contains(options.Host) && options.UseHealthCheck)
            {
                var failureStatus = options.UnhealthyStatus
                    .GetHealthStatus();

                services.AddHealthChecks()
                    .AddTcpHealthCheck(y => y
                        .AddHost(options.Host, options.Port), options.Host, failureStatus);

                hosts
                    .Add(options.Host);
            }
        }

        return services;
    }
}