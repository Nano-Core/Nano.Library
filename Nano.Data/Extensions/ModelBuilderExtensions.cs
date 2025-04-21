using System;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Const;
using Nano.Models.Data;

namespace Nano.Data.Extensions;

/// <summary>
/// Model Builder Extensions.
/// </summary>
internal static class ModelBuilderExtensions
{
    /// <summary>
    /// Map Default Identity.
    /// </summary>
    /// <typeparam name="TIdentity"></typeparam>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/>.</param>
    /// <returns>The <see cref="ModelBuilder"/>.</returns>
    internal static ModelBuilder MapDefaultIdentity<TIdentity>(this ModelBuilder modelBuilder) 
        where TIdentity : IEquatable<TIdentity>
    {
        if (modelBuilder == null)
            throw new ArgumentNullException(nameof(modelBuilder));

        modelBuilder
            .Entity<IdentityUser<TIdentity>>()
            .ToTable(TableNames.IDENTITY_USER);

        modelBuilder
            .Entity<IdentityUser<TIdentity>>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder
            .Entity<IdentityUser<TIdentity>>()
            .HasIndex(x => x.PhoneNumber)
            .IsUnique();

        modelBuilder
            .Entity<IdentityUserLogin<TIdentity>>()
            .ToTable(TableNames.IDENTITY_USER_LOGIN_TABLE_NAME);
        modelBuilder
            .Entity<IdentityUserRole<TIdentity>>()
            .ToTable(TableNames.IDENTITY_USER_ROLE);

        modelBuilder
            .Entity<IdentityUserClaim<TIdentity>>()
            .ToTable(TableNames.IDENTITY_USER_CLAIM);

        modelBuilder
            .Entity<IdentityRoleClaim<TIdentity>>()
            .ToTable(TableNames.IDENTITY_ROLE_CLAIM);

        modelBuilder
            .Entity<IdentityRole<TIdentity>>()
            .ToTable(TableNames.IDENTITY_ROLE);

        modelBuilder
            .Entity<DataProtectionKey>()
            .ToTable(TableNames.IDENTITY_DATA_PROTECTION_KEYS);

        modelBuilder
            .Entity<IdentityUserTokenExpiry<TIdentity>>()
            .ToTable(TableNames.IDENTITY_USER_TOKEN_EXPIRY)
            .Property(x => x.ExpireAt);

        return modelBuilder;
    }
}