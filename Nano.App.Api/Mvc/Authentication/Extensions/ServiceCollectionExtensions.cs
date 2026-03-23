using Microsoft.AspNetCore.Authentication;
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
using System.Net.Http;
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

        services
            .AddSingleton<AuthenticationSchemeCache>();

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
                x.DefaultSignInScheme = defaultScheme;
                x.DefaultSignOutScheme = defaultScheme;
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
                .AddAuthExternalRepository()
                .AddAuthExternalFacebookRepository(options.Jwt.ExternalLogins.Facebook)
                .AddAuthExternalGoogleRepository(options.Jwt.ExternalLogins.Google)
                .AddAuthExternalMicrosoftRepository(options.Jwt.ExternalLogins.Microsoft);
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

                var jwtAuthenticationOptions = apiOptions.CurrentValue.Authentication.Jwt;

                return jwtAuthenticationOptions == null
                    ? throw new NullReferenceException(nameof(jwtAuthenticationOptions))
                    : new AuthJwtRepository(jwtAuthenticationOptions);
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
            .AddScoped<IAuthTransientRepository>(x =>
            {
                var authenticationSchemeProvider = x
                    .GetRequiredService<IAuthenticationSchemeProvider>();

                var authJwtRepository = x
                    .GetRequiredService<IAuthJwtRepository>();

                var authExternalRepository = x
                    .GetService<IAuthExternalRepository>();

                return new AuthTransientRepository(authenticationSchemeProvider, authJwtRepository, authExternalRepository);
            });
        services
            .AddScoped<IAuthExternalRepository, AuthExternalRepository>();

        return services;
    }
    private static IServiceCollection AddAuthExternalRepository(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddScoped<IAuthExternalRepository>(x =>
            {
                var authExternalFacebookRepository = x
                    .GetService<IAuthExternalFacebookRepository>();

                var authExternalGoogleRepository = x
                    .GetService<IAuthExternalGoogleRepository>();

                var authExternalMicrosoftRepository = x
                    .GetService<IAuthExternalMicrosoftRepository>();

                return new AuthExternalRepository(authExternalFacebookRepository, authExternalGoogleRepository, authExternalMicrosoftRepository);
            });
        services
            .AddScoped<IAuthExternalRepository, AuthExternalRepository>();

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
            .AddHttpClient<IAuthExternalFacebookRepository>();

        services
            .AddScoped<IAuthExternalFacebookRepository>(x =>
            {
                var apiOptions = x
                    .GetRequiredService<IOptionsMonitor<ApiOptions>>();

                var facebookOptions = apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Facebook;

                if (facebookOptions == null)
                {
                    throw new NullReferenceException(nameof(facebookOptions));
                }

                var httpClientFactory = x
                    .GetRequiredService<IHttpClientFactory>();

                var httpClient = httpClientFactory
                    .CreateClient(nameof(IAuthExternalFacebookRepository));

                return new AuthExternalFacebookRepository(facebookOptions, httpClient);
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
            .AddScoped<IAuthExternalGoogleRepository>(x =>
            {
                var apiOptions = x
                    .GetRequiredService<IOptionsMonitor<ApiOptions>>();

                var googleOptions = apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Google;

                return googleOptions == null ? throw new NullReferenceException(nameof(googleOptions)) : new AuthExternalGoogleRepository(googleOptions);
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
            .AddHttpClient<IAuthExternalMicrosoftRepository>();

        services
            .AddScoped<IAuthExternalMicrosoftRepository>(x =>
            {
                var apiOptions = x
                    .GetRequiredService<IOptionsMonitor<ApiOptions>>();

                var microsoftOptions = apiOptions.CurrentValue.Authentication.Jwt?.ExternalLogins.Microsoft;

                if (microsoftOptions == null)
                {
                    throw new NullReferenceException(nameof(microsoftOptions));
                }

                var httpClientFactory = x
                    .GetRequiredService<IHttpClientFactory>();

                var httpClient = httpClientFactory
                    .CreateClient(nameof(IAuthExternalMicrosoftRepository));

                return new AuthExternalMicrosoftRepository(microsoftOptions, httpClient);
            });

        return services;
    }
}