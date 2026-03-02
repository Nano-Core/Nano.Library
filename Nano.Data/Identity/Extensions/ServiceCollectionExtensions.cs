using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Abstractions.Identity.Authentication;
using Nano.Data.Abstractions.Identity.Authentication.Consts;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Identity.Authentication;
using Nano.Data.Identity.DataProtection.Extensions;
using IdentityOptions = Nano.Data.Abstractions.Config.IdentityOptions;

namespace Nano.Data.Identity.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddIdentity<TContext, TIdentity>(this IServiceCollection services, IdentityOptions? options = null)
        where TContext : BaseDbContext<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options == null)
        {
            return services;
        }

        services
            .AddIdentity<IdentityUserEx<TIdentity>, IdentityRole<TIdentity>>(x =>
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
            })
            .AddEntityFrameworkStores<TContext>()
            .AddTokenProvider<DataProtectorTokenProvider<IdentityUserEx<TIdentity>>>(AuthenticationSchemes.JWT)
            .AddDefaultTokenProviders()
            .AddCustomTokenProvider();

        services
            .Configure<DataProtectionTokenProviderOptions>(x =>
            {
                x.TokenLifespan = TimeSpan.FromHours(options.TokensExpirationInHours);
            })
            .AddDataProtection()
            .PersistKeysToDbContext<TContext>();

        services
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IIdentityRepository<TIdentity>, IdentityRepository<TIdentity>>();

        services
            .AddIdentityAuthRepository<IIdentityAuthRepository, DefaultIdentityAuthRepository>()
            .AddIdentityAuthRepository<IIdentityAuthRepository<TIdentity>, DefaultIdentityAuthRepository<TIdentity>>();

        return services;
    }


    private static IServiceCollection AddIdentityAuthRepository<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : TService
    {
        services
            .AddScoped<TService>(x =>
            {
                var authJwtRepository = x
                    .GetService<IAuthJwtRepository>();

                if (authJwtRepository is null)
                {
                    var serviceTypeName = typeof(TImplementation).ToString();
                    var implementationTypeName = typeof(IAuthJwtRepository).ToString();

                    throw new InvalidOperationException($"Unable to resolve service for type '{serviceTypeName}' while attempting to activate  '{implementationTypeName}'. Ensure App:Identity:Authentication:Jwt is configured.");
                }

                var identityRepository = x
                    .GetRequiredService<IIdentityRepository>();

                return ActivatorUtilities.CreateInstance<TImplementation>(x, identityRepository, authJwtRepository);
            });

        return services;
    }
}