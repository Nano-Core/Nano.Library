using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models;
using Nano.Data.Consts;

namespace Nano.Data.Mappings;

/// <inheritdoc />
public class DefaultAuditEntryMapping : DefaultEntityMapping<DefaultAuditEntry>
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<DefaultAuditEntry> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.Map(builder);

        builder
            .ToTable(TableNames.AUDIT);

        builder
            .Property(x => x.CreatedBy)
            .HasMaxLength(256);

        builder
            .HasIndex(y => y.CreatedBy);

        builder
            .Property(x => x.EntitySetName)
            .HasMaxLength(256);

        builder
            .Property(x => x.EntityTypeName)
            .HasMaxLength(256);

        builder
            .HasIndex(y => y.EntityTypeName);

        builder
            .Property(x => x.StateName)
            .HasMaxLength(256);

        builder
            .Property(x => x.State);

        builder
            .HasIndex(y => y.State);

        builder
            .HasMany(x => x.Properties)
            .WithOne(x => x.Parent)
            .IsRequired();

        builder
            .Property(y => y.RequestId)
            .HasMaxLength(256);

        builder
            .HasIndex(y => y.RequestId);
    }
}