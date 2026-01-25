using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Entities;
using Nano.Data.Consts;

namespace Nano.Data.Mappings;

/// <summary>
/// Default mapping for <see cref="DefaultAuditEntryProperty"/>.
/// Configures table, relationships, and audited property columns.
/// </summary>
public class DefaultAuditEntryPropertyMapping : DefaultEntityMapping<DefaultAuditEntryProperty>
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<DefaultAuditEntryProperty> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Map(builder);

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