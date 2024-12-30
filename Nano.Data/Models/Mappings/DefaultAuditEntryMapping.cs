using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nano.Data.Models.Mappings;

/// <inheritdoc />
public class DefaultAuditEntryMapping : DefaultEntityMapping<DefaultAuditEntry>
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<DefaultAuditEntry> builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        base.Map(builder);

        builder
            .ToTable("__EFAudit");

        builder
            .Property(x => x.CreatedBy)
            .HasMaxLength(255);

        builder
            .HasIndex(y => y.CreatedBy);

        builder
            .Property(x => x.EntitySetName)
            .HasMaxLength(255);

        builder
            .Property(x => x.EntityTypeName)
            .HasMaxLength(255);

        builder
            .HasIndex(y => y.EntityTypeName);

        builder
            .Property(x => x.StateName)
            .HasMaxLength(255);

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
            .HasMaxLength(255);

        builder
            .HasIndex(y => y.RequestId);
    }
}