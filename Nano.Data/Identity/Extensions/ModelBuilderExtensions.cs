using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Mappings.Extensions;
using System;
using Microsoft.AspNetCore.Identity;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Consts;
using Nano.Data.Mappings.Identity;
using IdentityOptions = Nano.Data.Abstractions.Config.IdentityOptions;

namespace Nano.Data.Identity.Extensions;

internal static class ModelBuilderExtensions
{
    internal static ModelBuilder MapIdentity<TIdentity>(this ModelBuilder modelBuilder, IdentityOptions? options = null)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        var isUniqueEmailAddressRequired = options?.User.IsUniqueEmailAddressRequired ?? true;
        var isUniquePhoneNumberRequired = options?.User.IsUniquePhoneNumberRequired ?? true;

        modelBuilder
            .MapIdentityUserEx<TIdentity>(isUniqueEmailAddressRequired, isUniquePhoneNumberRequired)
            .MapIdentityUserRole<TIdentity>()
            .MapIdentityUserClaim<TIdentity>()
            .MapIdentityUserLogin<TIdentity>()
            .MapIdentityUserToken<TIdentity>()
            .MapIdentityRole<TIdentity>()
            .MapIdentityRoleClaim<TIdentity>()
            .MapDataProtectionKey<TIdentity>();

        modelBuilder
            .AddMapping<IdentityApiKey<TIdentity>, IdentityApiKeyMapping<TIdentity>>()
            .AddMapping<IdentityUserChangeData<TIdentity>, IdentityUserChangeDataMapping<TIdentity>>()
            .AddMapping<IdentityUserRefreshToken<TIdentity>, IdentityUserRefreshTokenMapping<TIdentity>>();

        return modelBuilder;
    }


    private static ModelBuilder MapIdentityUserEx<TIdentity>(this ModelBuilder modelBuilder, bool isUniqueEmailAddressRequired, bool isUniquePhoneNumberRequired)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        var entityTypeBuilder = modelBuilder
            .Entity<IdentityUserEx<TIdentity>>();

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

        entityTypeBuilder
            .HasIndex(x => x.Email)
            .IsUnique(isUniqueEmailAddressRequired);

        entityTypeBuilder
            .HasIndex(x => x.PhoneNumber)
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
            .Entity<IdentityUserToken<TIdentity>>();

        entityTypeBuilder
            .ToTable(TableNames.IDENTITY_USER_TOKEN);

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
