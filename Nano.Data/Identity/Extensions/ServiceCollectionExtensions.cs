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
            .AddIdentity<IdentityUserExt<TIdentity>, IdentityRole<TIdentity>>(x =>
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
            .AddTokenProvider<DataProtectorTokenProvider<IdentityUserExt<TIdentity>>>(AuthenticationSchemes.JWT)
            .AddDefaultTokenProviders()
            .AddCustomTokenProvider();

        services
            .Configure<DataProtectionTokenProviderOptions>(x =>
            {
                x.TokenLifespan = TimeSpan.FromHours(options.TokensExpirationInHours);
            })
            .AddDataProtection()
            .PersistKeysToDbContext<TContext>();

        if (options.Authentication.Jwt != null)
        {
            services
                .AddScoped<IIdentityAuthRepository, DefaultIdentityAuthRepository>()
                .AddScoped<IIdentityAuthRepository<TIdentity>, DefaultIdentityAuthRepository<TIdentity>>();
        }

        services
            .AddScoped<IIdentityRepository, DefaultIdentityRepository>()
            .AddScoped<IIdentityRepository<TIdentity>, DefaultIdentityRepository<TIdentity>>();

        return services;
    }
}