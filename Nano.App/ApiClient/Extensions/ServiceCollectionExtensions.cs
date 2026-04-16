using Microsoft.Extensions.DependencyInjection;
using Nano.App.ApiClient.Config;
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
using Nano.Common.Consts;
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
            .Where(x => x is { IsAbstract: false, IsGenericType: false } && x.IsTypeOf(typeof(BaseApi)))
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
                        .GetService<IHttpContextAccessor>();

                    var apiClient = Activator.CreateInstance(type, apiOptions, httpClient, httpContextAccessor);

                    return apiClient ?? throw new NullReferenceException(nameof(apiClient));
                });

            if (!hosts.Contains(options.Host) && options.HealthCheck != null)
            {
                var failureStatus = options.HealthCheck.UnhealthyStatus
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