using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nano.Config.Extensions;

namespace Nano.Security.Extensions
{
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
                .AddSecurityIdentity(options)
                .AddSecurityAuthentication(options)
                .AddSecurityAuthorization(options);

            return services;
        }

        private static IServiceCollection AddSecurityIdentity(this IServiceCollection services, SecurityOptions options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            services
                .Configure<IdentityOptions>(x =>
                {
                    x.User.RequireUniqueEmail = options.User.RequireUniqueEmail;
                    x.User.AllowedUserNameCharacters = options.User.AllowedUserNameCharacters;

                    x.Password.RequireDigit = options.Password.RequireDigit;
                    x.Password.RequiredLength = options.Password.RequiredLength;
                    x.Password.RequireNonAlphanumeric = options.Password.RequireNonAlphanumeric;
                    x.Password.RequireLowercase = options.Password.RequireLowercase;
                    x.Password.RequireUppercase = options.Password.RequireUppercase;
                    x.Password.RequiredUniqueChars = options.Password.RequiredUniqueCharecters;

                    x.SignIn.RequireConfirmedEmail = options.SignIn.RequireConfirmedEmail;
                    x.SignIn.RequireConfirmedPhoneNumber = options.SignIn.RequireConfirmedPhoneNumber;

                    x.Lockout.AllowedForNewUsers = options.Lockout.AllowedForNewUsers;
                    x.Lockout.DefaultLockoutTimeSpan = options.Lockout.DefaultLockoutTimeSpan;
                    x.Lockout.MaxFailedAccessAttempts = options.Lockout.MaxFailedAccessAttempts;
                });

            return services;
        }
        private static IServiceCollection AddSecurityAuthorization(this IServiceCollection services, SecurityOptions options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            services
                .AddAuthorization();

            return services;
        }
        private static IServiceCollection AddSecurityAuthentication(this IServiceCollection services, SecurityOptions options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .AddAuthentication(x =>
                {
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.IncludeErrorDetails = true;
                    x.RequireHttpsMetadata = false;

                    x.Audience = options.Jwt.Issuer;
                    x.ClaimsIssuer = options.Jwt.Issuer;

                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateActor = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = options.Jwt.Issuer,
                        ValidAudience = options.Jwt.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Jwt.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                })
                .AddCookie(x =>
                {
                    x.LoginPath = "/api/auth/login";
                    x.LogoutPath = "/api/auth/logout";
                    x.AccessDeniedPath = "/api/auth/forbidden";
                    x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    x.Cookie.Expiration = TimeSpan.FromDays(options.Jwt.ExpirationInHours);
                });

            return services;
        }
    }
}