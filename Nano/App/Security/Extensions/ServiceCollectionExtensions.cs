using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nano.App.Security.Options;

namespace Nano.App.Security.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds authentication identity to the <see cref="IServiceCollection"/>.
        /// Configures <see cref="IdentityUser"/> and <see cref="IdentityRole"/> for <see cref="DbContext"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var section = configuration.GetSection("Security");
            var options = section?.Get<SecurityOptions>() ?? new SecurityOptions();

            services
                .AddSingleton(options)
                .Configure<SecurityOptions>(section)
                .AddIdentity<IdentityRole, IdentityUser>(y =>
                {
                    y.User.RequireUniqueEmail = options.User.RequireUniqueEmail;
                    y.User.AllowedUserNameCharacters = options.User.AllowedUserNameCharacters;

                    y.Password.RequireDigit = options.Password.RequireDigit;
                    y.Password.RequiredLength = options.Password.RequiredLength;
                    y.Password.RequireNonAlphanumeric = options.Password.RequireNonAlphanumeric;
                    y.Password.RequireLowercase = options.Password.RequireLowercase;
                    y.Password.RequireUppercase = options.Password.RequireUppercase;
                    y.Password.RequiredUniqueChars = options.Password.RequiredUniqueCharecters;

                    y.SignIn.RequireConfirmedEmail = options.SignIn.RequireConfirmedEmail;
                    y.SignIn.RequireConfirmedPhoneNumber = options.SignIn.RequireConfirmedPhoneNumber;

                    y.Lockout.AllowedForNewUsers = options.Lockout.AllowedForNewUsers;
                    y.Lockout.DefaultLockoutTimeSpan = options.Lockout.DefaultLockoutTimeSpan;
                    y.Lockout.MaxFailedAccessAttempts = options.Lockout.MaxFailedAccessAttempts;
                })
                .AddDefaultTokenProviders()
                ;// TODO .AddEntityFrameworkStores<SecurityDbContext>();

            return services;
        }
    }
}