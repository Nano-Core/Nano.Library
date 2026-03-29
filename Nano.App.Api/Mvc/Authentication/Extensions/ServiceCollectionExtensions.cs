using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Nano.App.Api.Config;
using Nano.App.Api.Mvc.Authentication.Abstractions;
using Nano.App.Config;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Config;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using Nano.Common.Helpers;
using AuthenticationOptions = Nano.App.Api.Config.AuthenticationOptions;

namespace Nano.App.Api.Mvc.Authentication.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoAuthentication(this IServiceCollection services, AuthenticationOptions options, ApiKeyOptions? apiKeyOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap
            .Clear();

        if (options.Jwt == null && apiKeyOptions == null)
        {
            return services;
        }

        var defaultScheme = (options.Jwt != null, apiKeyOptions != null) switch
        {
            (true, true) => AuthenticationSchemes.JWT_OR_APIKEY,
            (true, false) => AuthenticationSchemes.JWT,
            (false, true) => AuthenticationSchemes.API_KEY,
            _ => throw new ArgumentOutOfRangeException(nameof(options))
        };

        var authenticationBuilder = services
            .AddAuthentication(x =>
            {
                x.DefaultScheme = defaultScheme;
                x.DefaultAuthenticateScheme = defaultScheme;
                x.DefaultChallengeScheme = defaultScheme;
                x.DefaultForbidScheme = defaultScheme;
                x.DefaultSignInScheme = null;
                x.DefaultSignOutScheme = null;
            })
            .AddJwtAuthentication(options.Jwt);

        if (defaultScheme == AuthenticationSchemes.JWT_OR_APIKEY)
        {
            authenticationBuilder
                .AddPolicyScheme(AuthenticationSchemes.JWT_OR_APIKEY, null, x =>
                {
                    x.ForwardDefaultSelector = context =>
                    {
                        if (context.Request.Headers.ContainsKey(HeaderNames.Authorization))
                        {
                            return AuthenticationSchemes.JWT;
                        }

                        if (context.Request.Headers.ContainsKey(NanoHeaderNames.X_API_KEY))
                        {
                            return AuthenticationSchemes.API_KEY;
                        }

                        return AuthenticationSchemes.JWT;
                    };
                });
        }

        if (options.Jwt != null)
        {
            services
                .AddAuthRepository()
                .AddAuthJwtRepository(options.Jwt)
                .AddAuthRootRepository(options.Jwt.RootLogin)
                .AddAuthTransientRepository()
                .AddAuthExternalFacebookRepository(options.Jwt.ExternalLogins.Facebook)
                .AddAuthExternalGoogleRepository(options.Jwt.ExternalLogins.Google)
                .AddAuthExternalMicrosoftRepository(options.Jwt.ExternalLogins.Microsoft)
                .AddCustomAuthExternalRepositories()
                .AddAuthExternalRepositoryAggregator();

            services
                .AddScoped<IRegisterTransientAuthEndpointsTask, RegisterTransientAuthEndpointsTask>();
        }

        return services;
    }


    private static IServiceCollection AddAuthRepository(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddScoped<IAuthRepository, AuthRepository>()
            .AddScoped(typeof(IAuthRepository<>), typeof(AuthRepository<>));

        return services;
    }
    private static IServiceCollection AddAuthJwtRepository(this IServiceCollection services, JwtAuthenticationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddScoped<IAuthJwtRepository>(x =>
            {
                var apiOptions = x
                    .GetRequiredService<IOptionsMonitor<ApiOptions>>();

                return new AuthJwtRepository(apiOptions.CurrentValue.Authentication.Jwt!);
            });

        return services;
    }
    private static IServiceCollection AddAuthRootRepository(this IServiceCollection services, LogInRootOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddScoped<IAuthRootRepository>(x =>
            {
                var apiOptions = x
                    .GetRequiredService<IOptionsMonitor<ApiOptions>>();

                var logInRootOptions = apiOptions.CurrentValue.Authentication.Jwt?.RootLogin;

                if (logInRootOptions == null)
                {
                    throw new NullReferenceException(nameof(logInRootOptions));
                }

                var authJwtRepository = x
                    .GetRequiredService<IAuthJwtRepository>();

                return new AuthRootRepository(logInRootOptions, authJwtRepository);
            });

        return services;
    }
    private static IServiceCollection AddAuthTransientRepository(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddScoped<IAuthTransientRepository, AuthTransientRepository>();

        return services;
    }
    private static IServiceCollection AddAuthExternalFacebookRepository(this IServiceCollection services, FacebookOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddHttpClient<AuthExternalFacebookRepository>();

        services
            .AddScoped<IAuthExternalRepository, AuthExternalFacebookRepository>(x =>
            {
                var apiOptions = x
                    .GetRequiredService<IOptionsMonitor<ApiOptions>>();

                var httpClientFactory = x
                    .GetRequiredService<IHttpClientFactory>();

                var httpClient = httpClientFactory
                    .CreateClient(nameof(AuthExternalFacebookRepository));

                return new AuthExternalFacebookRepository(apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Facebook!, httpClient);
            });

        return services;
    }
    private static IServiceCollection AddAuthExternalGoogleRepository(this IServiceCollection services, GoogleOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddScoped<IAuthExternalRepository, AuthExternalGoogleRepository>(x =>
            {
                var apiOptions = x
                    .GetRequiredService<IOptionsMonitor<ApiOptions>>();

                return new AuthExternalGoogleRepository(apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Google!);
            });

        return services;
    }
    private static IServiceCollection AddAuthExternalMicrosoftRepository(this IServiceCollection services, MicrosoftOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddHttpClient<AuthExternalMicrosoftRepository>();

        services
            .AddScoped<IAuthExternalRepository, AuthExternalMicrosoftRepository>(x =>
            {
                var apiOptions = x
                    .GetRequiredService<IOptionsMonitor<ApiOptions>>();

                var httpClientFactory = x
                    .GetRequiredService<IHttpClientFactory>();

                var httpClient = httpClientFactory
                    .CreateClient(nameof(AuthExternalMicrosoftRepository));

                return new AuthExternalMicrosoftRepository(apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Microsoft!, httpClient);
            });

        return services;
    }
    private static IServiceCollection AddCustomAuthExternalRepositories(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var customProviders = TypesHelper
            .GetAllTypes()
            .Where(x =>
                typeof(IAuthExternalRepository).IsAssignableFrom(x) &&
                !typeof(IBuiltInAuthExternalRepository).IsAssignableFrom(x) &&
                x is { IsClass: true, IsAbstract: false });

        foreach (var provider in customProviders)
        {
            services
                .AddScoped(typeof(IAuthExternalRepository), provider);
        }

        return services;
    }
    private static IServiceCollection AddAuthExternalRepositoryAggregator(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddScoped<IAuthExternalRepositoryAggregator, AuthExternalRepositoryAggregator>();

        return services;
    }
}