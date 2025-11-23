using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Nano.Data.Abstractions.Identity;
using Nano.Data.Identity.Consts;
using Nano.Security;
using IdentityOptions = Nano.Data.Abstractions.Config.IdentityOptions;

namespace Nano.Data.Identity.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddIdentity<TContext, TIdentity>(this IServiceCollection services, IdentityOptions options = null)
        where TContext : BaseDbContext<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        services
            .AddIdentityStore<TContext, TIdentity>()
            .AddIdentityOptions(options);

        services
            .Configure<DataProtectionTokenProviderOptions>(x =>
            {
                x.TokenLifespan = TimeSpan.FromHours(options.TokensExpirationInHours);
            });

        return services;
    }


    private static IServiceCollection AddIdentityStore<TContext, TIdentity>(this IServiceCollection services)
        where TContext : BaseDbContext<TIdentity>
        where TIdentity : IEquatable<TIdentity>
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var identityBuilder = services
            .AddIdentity<IdentityUser<TIdentity>, IdentityRole<TIdentity>>();

        identityBuilder
            .AddEntityFrameworkStores<TContext>()
            .AddTokenProvider<DataProtectorTokenProvider<IdentityUser<TIdentity>>>(TokenProviderNames.JWT_AUTHENTICATION_SCHEME)
            .AddDefaultTokenProviders()
            .AddCustomTokenProvider();

        // BUG: ServiceCollection IIdentityRepository
        //services
        //    .AddScoped<IIdentityRepository, DefaultIdentityRepository>()
        //    .AddScoped<IIdentityRepository<TIdentity>, DefaultIdentityRepository<TIdentity>>();

        return services;
    }
    private static IServiceCollection AddIdentityOptions(this IServiceCollection services, IdentityOptions options = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (options == null)
        {
            return services;
        }

        services
            .Configure<Microsoft.AspNetCore.Identity.IdentityOptions>(x =>
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

        return services;
    }
}