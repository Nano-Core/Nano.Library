using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Consts;

namespace Nano.Data.Mappings.Identity;

/// <summary>
/// Configures the EF Core mapping for <see cref="IdentityApiKeyClaim{TIdentity}"/> entities.
/// Sets table name, query filters, indexes, and relationships.
/// </summary>
/// <typeparam name="TIdentity">The type of the identity key.</typeparam>
public class IdentityApiKeyClaimMapping<TIdentity> : BaseEntityIdentityMapping<IdentityApiKeyClaim<TIdentity>, TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Configures the entity using the <see cref="EntityTypeBuilder{TEntity}"/>.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public override void Configure(EntityTypeBuilder<IdentityApiKeyClaim<TIdentity>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Configure(builder);

        builder
            .ToTable(TableNames.IDENTITY_API_KEY_CLAIM);

        builder
            .HasQueryFilter(x => x.ApiKey.IdentityUser.IsActive);

        builder
            .HasOne(x => x.ApiKey)
            .WithMany()
            .IsRequired();

        builder
            .Property(x => x.ClaimType);

        builder
            .HasIndex(x => new
            {
                x.ApiKeyId,
                x.ClaimType
            })
            .IsUnique();

        builder
            .Property(x => x.ClaimValue);
    }
}