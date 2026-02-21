using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Nano.App.Api.Config;
using Nano.App.Api.Identity.Authentication.Abstractions;
using Nano.App.Api.Identity.Authentication.Consts;
using Nano.App.Config;
using Nano.Common.Consts;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using AuthenticationOptions = Nano.App.Api.Config.AuthenticationOptions;

namespace Nano.App.Api.Identity.Authentication.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddNanoIdentityAuthentication(this IServiceCollection services, AuthenticationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap
            .Clear();

        services
            .AddAuthentication(x =>
            {
                x.DefaultScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
                x.DefaultAuthenticateScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
                x.DefaultChallengeScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
                x.DefaultForbidScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
                x.DefaultSignInScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
                x.DefaultSignOutScheme ??= AuthenticationSchemes.JWT_OR_API_KEY;
            })
            .AddJwtAuthentication(options.Jwt)
            .AddPolicyScheme(AuthenticationSchemes.JWT_OR_API_KEY, "JWT or API Key", x =>
            {
                x.ForwardDefaultSelector = context =>
                {
                    var schemeProvider = context.RequestServices
                        .GetRequiredService<IAuthenticationSchemeProvider>();

                    var schemes = schemeProvider
                        .GetAllSchemesAsync()
                        .GetAwaiter()
                        .GetResult()
                        .ToArray();

                    var hasJwt = schemes
                        .Any(s => s.Name == AuthenticationSchemes.JWT);

                    var hasApiKey = schemes
                        .Any(s => s.Name == AuthenticationSchemes.API_KEY);

                    if (hasJwt && context.Request.Headers.ContainsKey(HeaderNames.Authorization))
                    {
                        return AuthenticationSchemes.JWT;
                    }

                    if (hasApiKey && context.Request.Headers.ContainsKey(NanoHeaderNames.X_API_KEY))
                    {
                        return AuthenticationSchemes.API_KEY;
                    }

                    return hasJwt
                        ? AuthenticationSchemes.JWT
                        : hasApiKey
                            ? AuthenticationSchemes.API_KEY
                            : null;
                };
            });

        if (options.Jwt != null)
        {
            services
                .AddAuthJwtRepository(options.Jwt)
                .AddAuthRootRepository(options.Jwt.RootLogin)
                .AddAuthTransientRepository(options.Jwt.ExternalLogins)
                .AddAuthExternalRepository(options.Jwt.ExternalLogins)
                .AddAuthExternalFacebookRepository(options.Jwt.ExternalLogins.Facebook)
                .AddAuthExternalGoogleRepository(options.Jwt.ExternalLogins.Google)
                .AddAuthExternalMicrosoftRepository(options.Jwt.ExternalLogins.Microsoft);
        }

        return services;
    }

    internal static IServiceCollection AddNanoIdentityAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddAuthorization(x =>
            {
                x.FallbackPolicy = null;
                x.InvokeHandlersAfterFailure = false;

                x.AddPolicy(AuthorizationPolicies.DEFAULT, y =>
                {
                    y.RequireAssertion(z =>
                    {
                        var httpContext = z.Resource as HttpContext ?? (z.Resource as AuthorizationFilterContext)?.HttpContext;

                        if (httpContext == null)
                        {
                            return false;
                        }

                        var schemesConfigured = httpContext.RequestServices
                            .GetRequiredService<IAuthenticationSchemeProvider>()
                            .GetAllSchemesAsync()
                            .GetAwaiter()
                            .GetResult();

                        var isAuthConfigured = schemesConfigured
                            .Any(a => a.Name == AuthenticationSchemes.JWT_OR_API_KEY);

                        return !isAuthConfigured || (z.User.Identity?.IsAuthenticated ?? false);
                    });
                });
            });

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

                var jwtAuthenticationOptions = apiOptions.CurrentValue.Authentication?.Jwt;

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

                var logInRootOptions = apiOptions.CurrentValue.Authentication?.Jwt?.RootLogin;

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
    private static IServiceCollection AddAuthTransientRepository(this IServiceCollection services, ExternalLoginOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options is not { IsConfigured: true })
        {
            return services;
        }

        services
            .AddScoped<IAuthTransientRepository>(x =>
            {
                var apiOptions = x
                    .GetRequiredService<IOptionsMonitor<ApiOptions>>();

                var externalLoginsOptions = apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins;

                if (externalLoginsOptions == null)
                {
                    throw new NullReferenceException(nameof(externalLoginsOptions));
                }

                var authJwtRepository = x
                    .GetRequiredService<IAuthJwtRepository>();

                var authExternalRepository = x
                    .GetService<IAuthExternalRepository>();

                return new AuthTransientRepository(externalLoginsOptions, authJwtRepository, authExternalRepository);
            });
        services
            .AddScoped<IAuthExternalRepository, AuthExternalRepository>();

        return services;
    }
    private static IServiceCollection AddAuthExternalRepository(this IServiceCollection services, ExternalLoginOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options is not { IsConfigured: true })
        {
            return services;
        }

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

                var facebookOptions = apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Facebook;

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

                var googleOptions = apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Google;

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

                var microsoftOptions = apiOptions.CurrentValue.Authentication?.Jwt?.ExternalLogins.Microsoft;

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