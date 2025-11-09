using System;
using System.Linq;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nano.Data.Const;
using Nano.Models.Data;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;

namespace Nano.Data.Models.Mappings.Extensions;

/// <summary>
/// Model Builder Extensions.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds a mapping for the type <typeparamref name="TEntity"/> using the <typeparamref name="TMapping"/> implementation.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/>.</typeparam>
    /// <typeparam name="TMapping">The <see cref="BaseEntityMapping{TEntity}"/>.</typeparam>
    /// <param name="builder">The <see cref="ModelBuilder"/>.</param>
    /// <returns>The <see cref="ModelBuilder"/>.</returns>
    public static ModelBuilder AddMapping<TEntity, TMapping>(this ModelBuilder builder)
        where TEntity : class, IEntity
        where TMapping : BaseEntityMapping<TEntity>, new()
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var mapping = new TMapping();

        mapping
            .Map(builder.Entity<TEntity>());

        return builder
            .UpdateSoftDeleteUniuqeIndexes<TEntity>()
            .UpdateUniuqeIndexes<TEntity>();
    }

    /// <summary>
    /// Map Default Identity.
    /// </summary>
    /// <typeparam name="TIdentity"></typeparam>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/>.</param>
    /// <param name="isUniqueEmailAddressRequired">Is unique email address required.</param>
    /// <param name="isUniquePhoneNumberRequired">Is unique phone number required</param>
    /// <returns>The <see cref="ModelBuilder"/>.</returns>
    internal static ModelBuilder MapDefaultIdentity<TIdentity>(this ModelBuilder modelBuilder, bool isUniqueEmailAddressRequired, bool isUniquePhoneNumberRequired)
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
            .IsUnique(isUniqueEmailAddressRequired);

        modelBuilder
            .Entity<IdentityUser<TIdentity>>()
            .HasIndex(x => x.PhoneNumber)
            .IsUnique(isUniquePhoneNumberRequired);

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

    private static ModelBuilder UpdateUniuqeIndexes<TEntity>(this ModelBuilder builder)
        where TEntity : class, IEntity
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var entity = builder.Entity<TEntity>();

        entity.Metadata
            .GetIndexes()
            .Where(x => x.IsUnique)
            .ToList()
            .ForEach(x =>
            {
                entity.Metadata
                    .RemoveIndex(x.Properties);

                var columns = x.Properties
                    .Select(y => y.Name)
                    .ToArray();

                var tableName = x.DeclaringEntityType
                    .GetTableName();

                var columnNames = columns
                    .Aggregate("", (y, z) => y + $"_{z}");

                var indexName = $"UX_{tableName}{columnNames}";

                entity
                    .HasIndex(columns)
                    .HasDatabaseName(indexName)
                    .IsUnique();
            });

        return builder;
    }
    private static ModelBuilder UpdateSoftDeleteUniuqeIndexes<TEntity>(this ModelBuilder builder)
        where TEntity : class, IEntity
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var isDeletableSoft = typeof(TEntity).IsTypeOf(typeof(IEntityDeletableSoft));

        if (isDeletableSoft)
        {
            var entity = builder.Entity<TEntity>();

            entity.Metadata
                .GetIndexes()
                .Where(x =>
                    x.IsUnique &&
                    x.Properties.All(y => y.Name != "IsDeleted" && !y.IsKey()))
                .ToList()
                .ForEach(x =>
                {
                    entity.Metadata
                        .RemoveIndex(x.Properties);

                    var columns = x.Properties
                        .Select(y => y.Name)
                        .Union(["IsDeleted"])
                        .ToArray();

                    entity
                        .HasIndex(columns)
                        .IsUnique();
                });
        }

        return builder;
    }
}
