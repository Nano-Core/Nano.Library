using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models;
using Nano.Data.Consts;

namespace Nano.Data.Mappings;

/// <summary>
/// Default mapping for <see cref="AuditEntryProperty{TIdentity}"/>.
/// Configures table, relationships, and audited property columns.
/// </summary>
public class AuditEntryPropertyMapping<TIdentity> : BaseEntityIdentityMapping<AuditEntryProperty<TIdentity>, TIdentity>
    where TIdentity : IEquatable<TIdentity>
{
    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<AuditEntryProperty<TIdentity>> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Configure(builder);

        builder
            .ToTable(TableNames.AUDIT_PROPERTIES);

        builder
            .HasOne(x => x.Parent)
            .WithMany(x => x.Properties)
            .IsRequired();

        builder
            .Property(y => y.PropertyName)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .HasIndex(y => y.PropertyName);

        builder
            .Property(y => y.RelationName)
            .HasMaxLength(256);

        builder
            .Property(y => y.NewValue);

        builder
            .Property(y => y.OldValue);
    }
}