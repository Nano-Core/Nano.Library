using System;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Models.Identity;
using Z.EntityFramework.Plus;

namespace Nano.Data.Extensions;

internal static class AuditConfigurationExtensions
{
    internal static void IncludeIdentity<TIdentity>(this AuditConfiguration configuration)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.Include<IdentityUserEx<TIdentity>>();
        configuration.Include<IdentityUserRole<TIdentity>>();
        configuration.Include<IdentityUserClaim<TIdentity>>();
        configuration.Include<IdentityUserLogin<TIdentity>>();
        configuration.Include<IdentityRole<TIdentity>>();
        configuration.Include<IdentityRoleClaim<TIdentity>>();
        configuration.Include<IdentityApiKey<TIdentity>>();
    }

    internal static void ExcludeIdentity<TIdentity>(this AuditConfiguration configuration)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(configuration);

        configuration.ExcludeProperty<IdentityUserEx<TIdentity>>(x => x.ConcurrencyStamp);
        configuration.ExcludeProperty<IdentityUserEx<TIdentity>>(x => x.NormalizedEmail);
        configuration.ExcludeProperty<IdentityUserEx<TIdentity>>(x => x.PasswordHash);
        configuration.ExcludeProperty<IdentityUserEx<TIdentity>>(x => x.NormalizedUserName);
        configuration.ExcludeProperty<IdentityUserEx<TIdentity>>(x => x.SecurityStamp);
        configuration.Include<IdentityUserLogin<TIdentity>>();
        configuration.ExcludeProperty<IdentityUserLogin<TIdentity>>(x => x.ProviderKey);
        configuration.ExcludeProperty<IdentityRole<TIdentity>>(x => x.ConcurrencyStamp);
        configuration.ExcludeProperty<IdentityRole<TIdentity>>(x => x.NormalizedName);
        configuration.ExcludeProperty<IdentityApiKey<TIdentity>>(x => x.Hash);
    }
}