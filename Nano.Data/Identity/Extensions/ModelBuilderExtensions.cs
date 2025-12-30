using System;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Identity.Consts;

namespace Nano.Data.Identity.Extensions;

internal static class ModelBuilderExtensions
{
    internal static ModelBuilder MapIdentity<TIdentity>(this ModelBuilder modelBuilder, bool isUniqueEmailAddressRequired, bool isUniquePhoneNumberRequired)
        where TIdentity : IEquatable<TIdentity>
    {
        if (modelBuilder == null)
            throw new ArgumentNullException(nameof(modelBuilder));

        modelBuilder
            .MapIdentityUser<TIdentity>(isUniqueEmailAddressRequired, isUniquePhoneNumberRequired)
            .MapIdentityUserRole<TIdentity>()
            .MapIdentityUserClaim<TIdentity>()
            .MapIdentityUserLogin<TIdentity>()
            .MapIdentityUserToken<TIdentity>()
            .MapIdentityRole<TIdentity>()
            .MapIdentityRoleClaim<TIdentity>()
            .MapIdentityUserLogin<TIdentity>()
            .MapDataProtectionKey<TIdentity>();

        return modelBuilder;
    }


    private static ModelBuilder MapIdentityUser<TIdentity>(this ModelBuilder modelBuilder, bool isUniqueEmailAddressRequired, bool isUniquePhoneNumberRequired)
        where TIdentity : IEquatable<TIdentity>
    {
        var entityTypeBuilder = modelBuilder
            .Entity<IdentityUserExt<TIdentity>>();

        entityTypeBuilder
            .ToTable(TableNames.IDENTITY_USER);

        entityTypeBuilder
            .HasQueryFilter(x => x.IsActive);

        entityTypeBuilder
            .Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        entityTypeBuilder
            .HasIndex(x => x.IsActive);

        entityTypeBuilder.HasIndex(x => x.Email)
            .IsUnique(isUniqueEmailAddressRequired);

        entityTypeBuilder.HasIndex(x => x.PhoneNumber)
            .IsUnique(isUniquePhoneNumberRequired);

        return modelBuilder;
    }
    private static ModelBuilder MapIdentityUserRole<TIdentity>(this ModelBuilder modelBuilder)
        where TIdentity : IEquatable<TIdentity>
    {
        var entityTypeBuilder = modelBuilder
            .Entity<IdentityUserRole<TIdentity>>();

        entityTypeBuilder
            .ToTable(TableNames.IDENTITY_USER_ROLE);

        return modelBuilder;
    }
    private static ModelBuilder MapIdentityUserClaim<TIdentity>(this ModelBuilder modelBuilder)
        where TIdentity : IEquatable<TIdentity>
    {
        var entityTypeBuilder = modelBuilder
            .Entity<IdentityUserClaim<TIdentity>>();

        entityTypeBuilder
            .ToTable(TableNames.IDENTITY_USER_CLAIM);


        return modelBuilder;
    }
    private static ModelBuilder MapIdentityUserLogin<TIdentity>(this ModelBuilder modelBuilder)
        where TIdentity : IEquatable<TIdentity>
    {
        var entityTypeBuilder = modelBuilder
            .Entity<IdentityUserLogin<TIdentity>>();

        entityTypeBuilder
            .ToTable(TableNames.IDENTITY_USER_LOGIN_TABLE_NAME);

        return modelBuilder;
    }
    private static ModelBuilder MapIdentityUserToken<TIdentity>(this ModelBuilder modelBuilder)
        where TIdentity : IEquatable<TIdentity>
    {
        var entityTypeBuilder = modelBuilder
            .Entity<IdentityUserTokenExpiry<TIdentity>>();

        entityTypeBuilder
            .ToTable(TableNames.IDENTITY_USER_TOKEN_EXPIRY);

        entityTypeBuilder
            .Property(x => x.ExpireAt);

        return modelBuilder;
    }
    private static ModelBuilder MapIdentityRole<TIdentity>(this ModelBuilder modelBuilder)
        where TIdentity : IEquatable<TIdentity>
    {
        var entityTypeBuilder = modelBuilder
            .Entity<IdentityRole<TIdentity>>();

        entityTypeBuilder
            .ToTable(TableNames.IDENTITY_ROLE);

        return modelBuilder;
    }
    private static ModelBuilder MapIdentityRoleClaim<TIdentity>(this ModelBuilder modelBuilder)
        where TIdentity : IEquatable<TIdentity>
    {
        var entityTypeBuilder = modelBuilder
            .Entity<IdentityRoleClaim<TIdentity>>();

        entityTypeBuilder
            .ToTable(TableNames.IDENTITY_ROLE_CLAIM);

        return modelBuilder;
    }
    private static ModelBuilder MapDataProtectionKey<TIdentity>(this ModelBuilder modelBuilder)
        where TIdentity : IEquatable<TIdentity>
    {
        var entityTypeBuilder = modelBuilder
            .Entity<DataProtectionKey>();

        entityTypeBuilder
            .ToTable(TableNames.IDENTITY_DATA_PROTECTION_KEYS);

        return modelBuilder;
    }
}
