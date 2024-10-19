using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nano.Config.Extensions;

namespace Nano.Security.Extensions;

/// <summary>
/// Service Collection Extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="SecurityOptions"/> to the <see cref="IServiceCollection"/>, and configures security.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        services
            .AddConfigOptions<SecurityOptions>(configuration, SecurityOptions.SectionName, out var options);

        services
            .AddSecurityIdentity(options);

        return services;
    }

    /// <summary>
    /// Adds the <see cref="BaseIdentityManager{TIdentity}"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    internal static IServiceCollection AddIdentityManager<TIdentity>(this IServiceCollection services)
        where TIdentity : IEquatable<TIdentity>
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        services
            .AddScoped<BaseIdentityManager<TIdentity>>()
            .AddScoped<DefaultIdentityManager>();

        return services;
    }

    private static IServiceCollection AddSecurityIdentity(this IServiceCollection services, SecurityOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        services
            .AddSingleton(_ =>
            {
                var rsaSecurityKey = RSA.Create();
                var publicKey = Convert.FromBase64String(options.Jwt.PublicKey);

                rsaSecurityKey
                    .ImportRSAPublicKey(publicKey, out var _);

                return new RsaSecurityKey(rsaSecurityKey);
            });

        services
            .Configure<IdentityOptions>(x =>
            {
                x.User.RequireUniqueEmail = true;
                x.User.AllowedUserNameCharacters = options.User.AllowedUserNameCharacters;

                x.Password.RequireDigit = options.Password.RequireDigit;
                x.Password.RequiredLength = options.Password.RequiredLength;
                x.Password.RequireNonAlphanumeric = options.Password.RequireNonAlphanumeric;
                x.Password.RequireLowercase = options.Password.RequireLowercase;
                x.Password.RequireUppercase = options.Password.RequireUppercase;
                x.Password.RequiredUniqueChars = options.Password.RequiredUniqueCharacters;

                x.SignIn.RequireConfirmedEmail = options.SignIn.RequireConfirmedEmail;
                x.SignIn.RequireConfirmedPhoneNumber = options.SignIn.RequireConfirmedPhoneNumber;

                x.Lockout.AllowedForNewUsers = options.Lockout.AllowedForNewUsers;
                x.Lockout.DefaultLockoutTimeSpan = options.Lockout.DefaultLockoutTimeSpan;
                x.Lockout.MaxFailedAccessAttempts = options.Lockout.MaxFailedAccessAttempts;
            });

        services
            .Configure<DataProtectionTokenProviderOptions>(x =>
            {
                x.TokenLifespan = TimeSpan.FromHours(options.TokensExpirationInHours);
            });

        return services;
    }
}