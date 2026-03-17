using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Enums;
using Nano.Data.Consts;

namespace Nano.Data.Mappings;

/// <summary>
/// Default mapping for <see cref="AuditEntry{TIdentity}"/>.
/// Configures table, properties, indexes, and relationships.
/// </summary>
public class AuditEntryMapping<TIdentity> : BaseEntityIdentityMapping<AuditEntry<TIdentity>, TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<AuditEntry<TIdentity>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Configure(builder);

        builder
            .ToTable(TableNames.AUDIT);

        builder
            .Property(x => x.CreatedBy)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .HasIndex(y => y.CreatedBy);

        builder
            .Property(x => x.EntityKey)
            .IsRequired();

        builder
            .Property(x => x.EntitySetName)
            .HasMaxLength(256);

        builder
            .Property(x => x.EntityTypeName)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .HasIndex(y => y.EntityTypeName);

        builder
            .Property(x => x.EntityState)
            .HasDefaultValue(AuditState.Added)
            .IsRequired();

        builder
            .HasIndex(y => y.EntityState);

        builder
            .Property(y => y.RequestId)
            .HasMaxLength(256);

        builder
            .HasIndex(y => y.RequestId);

        builder
            .HasMany(x => x.Properties)
            .WithOne(x => x.Parent)
            .IsRequired();
    }
}