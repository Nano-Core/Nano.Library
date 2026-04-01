using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Config.Enums;
using Nano.Data.Abstractions.Models.Identity;
using System;
using Z.EntityFramework.Plus;
using IdentityOptions = Nano.Data.Abstractions.Config.IdentityOptions;

namespace Nano.Data.Extensions;

internal static class AuditConfigurationExtensions
{
    internal static void IncludeIdentity<TIdentity>(this AuditConfiguration configuration, IdentityOptions? options)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (options == null)
        {
            return;
        }

        configuration
            .ConfigureIdentityUser<TIdentity>(options.UseAudit)
            .ConfigureIdentityUserRole<TIdentity>(options.UseAudit)
            .ConfigureIdentityUserClaim<TIdentity>(options.UseAudit)
            .ConfigureIdentityUserLogin<TIdentity>(options.UseAudit)
            .ConfigureIdentityRole<TIdentity>(options.UseAudit)
            .ConfigureIdentityRoleClaim<TIdentity>(options.UseAudit)
            .ConfigureIdentityApiKey<TIdentity>(options.UseAudit)
            .ConfigureIdentityApiKeyClaim<TIdentity>(options.UseAudit)
            .ConfigureIdentityApiKeyRole<TIdentity>(options.UseAudit);

        configuration
            .Exclude<IdentityUserToken<TIdentity>>()
            .Exclude<IdentityUserRefreshToken<TIdentity>>()
            .Exclude<IdentityUserChangeData<TIdentity>>();
    }


    private static AuditConfiguration ConfigureIdentityUser<TIdentity>(this AuditConfiguration configuration, AuditIdentityFlags auditIdentityFlags)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (auditIdentityFlags.HasFlag(AuditIdentityFlags.User))
        {
            configuration.Include<IdentityUserEx<TIdentity>>();

            configuration.ExcludeProperty<IdentityUserEx<TIdentity>>(x => x.ConcurrencyStamp);
            configuration.ExcludeProperty<IdentityUserEx<TIdentity>>(x => x.NormalizedEmail);
            configuration.ExcludeProperty<IdentityUserEx<TIdentity>>(x => x.PasswordHash);
            configuration.ExcludeProperty<IdentityUserEx<TIdentity>>(x => x.NormalizedUserName);
            configuration.ExcludeProperty<IdentityUserEx<TIdentity>>(x => x.SecurityStamp);
        }
        else
        {
            configuration.Exclude<IdentityUserEx<TIdentity>>();
        }

        return configuration;
    }
    private static AuditConfiguration ConfigureIdentityUserRole<TIdentity>(this AuditConfiguration configuration, AuditIdentityFlags auditIdentityFlags)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (auditIdentityFlags.HasFlag(AuditIdentityFlags.UserRole))
        {
            configuration.Include<IdentityUserRole<TIdentity>>();
        }
        else
        {
            configuration.Exclude<IdentityUserRole<TIdentity>>();
        }

        return configuration;
    }
    private static AuditConfiguration ConfigureIdentityUserClaim<TIdentity>(this AuditConfiguration configuration, AuditIdentityFlags auditIdentityFlags)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (auditIdentityFlags.HasFlag(AuditIdentityFlags.UserClaim))
        {
            configuration.Include<IdentityUserClaim<TIdentity>>();
        }
        else
        {
            configuration.Exclude<IdentityUserClaim<TIdentity>>();
        }

        return configuration;
    }
    private static AuditConfiguration ConfigureIdentityUserLogin<TIdentity>(this AuditConfiguration configuration, AuditIdentityFlags auditIdentityFlags)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (auditIdentityFlags.HasFlag(AuditIdentityFlags.UserLogin))
        {
            configuration.Include<IdentityUserLogin<TIdentity>>();
        }
        else
        {
            configuration.Exclude<IdentityUserLogin<TIdentity>>();

            configuration.ExcludeProperty<IdentityUserLogin<TIdentity>>(x => x.ProviderKey);
        }

        return configuration;
    }
    private static AuditConfiguration ConfigureIdentityRole<TIdentity>(this AuditConfiguration configuration, AuditIdentityFlags auditIdentityFlags)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (auditIdentityFlags.HasFlag(AuditIdentityFlags.Role))
        {
            configuration.Include<IdentityRole<TIdentity>>();
        }
        else
        {
            configuration.Exclude<IdentityRole<TIdentity>>();

            configuration.ExcludeProperty<IdentityRole<TIdentity>>(x => x.ConcurrencyStamp);
            configuration.ExcludeProperty<IdentityRole<TIdentity>>(x => x.NormalizedName);
        }

        return configuration;
    }
    private static AuditConfiguration ConfigureIdentityRoleClaim<TIdentity>(this AuditConfiguration configuration, AuditIdentityFlags auditIdentityFlags)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (auditIdentityFlags.HasFlag(AuditIdentityFlags.RoleClaim))
        {
            configuration.Include<IdentityRoleClaim<TIdentity>>();
        }
        else
        {
            configuration.Exclude<IdentityRoleClaim<TIdentity>>();
        }

        return configuration;
    }
    private static AuditConfiguration ConfigureIdentityApiKey<TIdentity>(this AuditConfiguration configuration, AuditIdentityFlags auditIdentityFlags)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (auditIdentityFlags.HasFlag(AuditIdentityFlags.ApiKey))
        {
            configuration.Include<IdentityApiKey<TIdentity>>();

            configuration.ExcludeProperty<IdentityApiKey<TIdentity>>(x => x.Hash);
        }
        else
        {
            configuration.Exclude<IdentityApiKey<TIdentity>>();
        }

        return configuration;
    }
    private static AuditConfiguration ConfigureIdentityApiKeyClaim<TIdentity>(this AuditConfiguration configuration, AuditIdentityFlags auditIdentityFlags)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (auditIdentityFlags.HasFlag(AuditIdentityFlags.ApiKeyClaim))
        {
            configuration.Include<IdentityApiKeyClaim<TIdentity>>();
        }
        else
        {
            configuration.Exclude<IdentityApiKeyClaim<TIdentity>>();
        }

        return configuration;
    }
    private static AuditConfiguration ConfigureIdentityApiKeyRole<TIdentity>(this AuditConfiguration configuration, AuditIdentityFlags auditIdentityFlags)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (auditIdentityFlags.HasFlag(AuditIdentityFlags.ApiKeyRole))
        {
            configuration.Include<IdentityApiKeyRole<TIdentity>>();
        }
        else
        {
            configuration.Exclude<IdentityApiKeyRole<TIdentity>>();
        }

        return configuration;
    }
}