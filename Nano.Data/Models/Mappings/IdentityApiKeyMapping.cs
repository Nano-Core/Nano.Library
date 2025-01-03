using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Const;
using Nano.Security.Data.Models;

namespace Nano.Data.Models.Mappings;

/// <inheritdoc />
public class IdentityApiKeyMapping<TIdentity> : BaseEntityIdentityMapping<IdentityApiKey<TIdentity>, TIdentity> 
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<IdentityApiKey<TIdentity>> builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        base.Map(builder);

        builder
            .ToTable(TableNames.IDENTITY_API_KEY);

        builder
            .HasOne(x => x.IdentityUser)
            .WithMany()
            .IsRequired();

        builder
            .Property(x => x.Name)
            .HasMaxLength(255)
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