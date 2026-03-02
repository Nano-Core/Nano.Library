using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Consts;

namespace Nano.Data.Mappings.Identity;

/// <summary>
/// Configures the EF Core mapping for <see cref="IdentityApiKey{TIdentity}"/> entities.
/// Sets table name, query filters, indexes, and relationships.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityApiKeyMapping<TIdentity> : BaseEntityIdentityMapping<IdentityApiKey<TIdentity>, TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Configures the entity using the <see cref="EntityTypeBuilder{TEntity}"/>.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public override void Configure(EntityTypeBuilder<IdentityApiKey<TIdentity>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Configure(builder);

        builder
            .ToTable(TableNames.IDENTITY_API_KEY);

        builder
            .HasQueryFilter(x => x.IdentityUser.IsActive);

        builder
            .HasOne(x => x.IdentityUser)
            .WithMany()
            .IsRequired();

        builder
            .Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(x => x.Hash)
            .IsRequired();

        builder
            .Property(x => x.CreatedAt)
            .IsRequired();

        builder
            .Property(x => x.RevokedAt);

        builder
            .HasIndex(x => x.RevokedAt);
    }
}