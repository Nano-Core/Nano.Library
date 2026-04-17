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
using Nano.App.ApiClient.Abstractions;
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
            .Where(x => x is { IsAbstract: false, IsGenericType: false } && x.IsTypeOf(typeof(BaseApi<>)))
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
                .AddKeyedSingleton(optionsServiceId, options)
                .AddSingleton<IAccessTokenProvider, AccessTokenProvider>();

            services
                .AddHttpClient(type.Name, (serviceProvider, client) =>
                {
                    var apiOptions = serviceProvider
                        .GetRequiredKeyedService<ApiClientOptions>(optionsServiceId);

                    client.Timeout = apiOptions.Timeout;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpContentType.JSON));
                    client.DefaultRequestVersion = HttpVersion.Version30;
                    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new SocketsHttpHandler
                    {
                        AllowAutoRedirect = true,
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                    });

            services
                .AddKeyedScoped<ApiClient>(type, (serviceProvider, _) =>
                {
                    var apiOptions = serviceProvider
                        .GetRequiredKeyedService<ApiClientOptions>(optionsServiceId);

                    var httpClientFactory = serviceProvider
                        .GetRequiredService<IHttpClientFactory>();

                    var httpClient = httpClientFactory
                        .CreateClient(type.Name);

                    var accessTokenProvider = serviceProvider
                        .GetRequiredService<IAccessTokenProvider>();

                    var httpContextAccessor = serviceProvider
                        .GetService<IHttpContextAccessor>();

                    return new ApiClient(apiOptions, httpClient, accessTokenProvider, httpContextAccessor);
                });

            services
                .AddScoped(type, serviceProvider =>
                {
                    var apiClient = serviceProvider
                        .GetRequiredKeyedService<ApiClient>(type);

                    var api = Activator.CreateInstance(type, apiClient);

                    return api ?? throw new NullReferenceException(nameof(api));
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