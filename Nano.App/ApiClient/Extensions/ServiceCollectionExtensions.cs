using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.ApiClient.Abstractions;
using Nano.App.ApiClient.Config;
using Nano.App.Config;
using Nano.Common;
using Nano.Common.Consts;
using Nano.Common.Extensions;
using Nano.Common.Mvc.HealthChecks.Extensions;

namespace Nano.App.ApiClient.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoApis(this IServiceCollection services, BaseAppOptions appOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(appOptions);

        foreach (var (name, options) in appOptions.Apis)
        {
            var type = TypeCache
                .GetAllTypes()
                .FirstOrDefault(x => x.Name == name && x is { IsAbstract: false, IsGenericType: false } && x.IsTypeOf(typeof(BaseApiClient<>)));

            if (type == null)
            {
                continue;
            }

            services
                .AddKeyedSingleton(type.Name, options);

            services
                .AddHttpClient(type)
                .AddApiClients(type)
                .AddHealthCheck(options);
        }

        return services;
    }


    private static IServiceCollection AddHttpClient(this IServiceCollection services, MemberInfo type)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(type);

        services
            .AddHttpClient(type.Name, (serviceProvider, client) =>
            {
                var apiOptions = serviceProvider
                    .GetRequiredKeyedService<ApiClientOptions>(type.Name);

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

        return services;
    }
    private static IServiceCollection AddApiClients(this IServiceCollection services, Type type)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(type);

        services
            .AddSingleton<IAccessTokenProvider, AccessTokenProvider>();

        services
            .AddKeyedScoped<ApiClient>(type, (serviceProvider, _) =>
            {
                var apiOptions = serviceProvider
                    .GetRequiredKeyedService<ApiClientOptions>(type.Name);

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

        return services;
    }
    private static IServiceCollection AddHealthCheck(this IServiceCollection services, ApiClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        if (options.HealthCheck == null)
        {
            return services;
        }

        var failureStatus = options.HealthCheck.UnhealthyStatus
            .GetHealthStatus();

        services.AddHealthChecks()
            .AddTcpHealthCheck(y => y
                .AddHost(options.Host, options.Port), options.Host, failureStatus);

        return services;
    }
}