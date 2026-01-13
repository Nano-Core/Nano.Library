using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Identity;
using Nano.Data.Identity.Consts;
using Nano.Data.Mappings;

namespace Nano.Data.Identity.Mappings;

/// <inheritdoc />
public class IdentityApiKeyMapping<TIdentity> : BaseEntityIdentityMapping<IdentityApiKey<TIdentity>, TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<IdentityApiKey<TIdentity>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Map(builder);

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