using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Entities.Identity;
using Nano.Data.Consts;
using System;

namespace Nano.Data.Mappings.Identity;

/// <summary>
/// Configures the EF Core mapping for <see cref="IdentityUserRefreshToken{TIdentity}"/> entities.
/// Sets table name, indexes, query filters, and relationships.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityUserRefreshTokenMapping<TIdentity> : BaseEntityIdentityMapping<IdentityUserRefreshToken<TIdentity>, TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Configures the entity using the <see cref="EntityTypeBuilder{TEntity}"/>.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public override void Map(EntityTypeBuilder<IdentityUserRefreshToken<TIdentity>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Map(builder);

        builder
            .ToTable(TableNames.IDENTITY_USER_REFRESH_TOKEN);

        builder
            .HasQueryFilter(x => x.IdentityUser.IsActive);

        builder
            .HasOne(x => x.IdentityUser)
            .WithOne();

        builder
            .Property(x => x.AppId)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .HasIndex(x => new
            {
                x.IdentityUserId,
                x.AppId
            })
            .IsUnique();

        builder
            .Property(x => x.Value)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(x => x.ExpireAt);

        builder
            .HasIndex(x => x.ExpireAt);
    }
}