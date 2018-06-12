using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nano.Data.Models.Mappings
{
    /// <inheritdoc />
    public class DefaultAuditEntryPropertyMapping : BaseEntityMapping<DefaultAuditEntryProperty>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<DefaultAuditEntryProperty> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder
                .ToTable("__EFAuditProperties");

            builder
                .HasKey(y => y.AuditEntryPropertyId);

            builder
                .HasOne(x => x.Parent)
                .WithMany(x => x.Properties)
                .HasForeignKey(x => x.AuditEntryId)
                .IsRequired();

            builder
                .Property(y => y.PropertyName)
                .HasMaxLength(255);

            builder
                .HasIndex(y => y.PropertyName);

            builder
                .Property(y => y.RelationName)
                .HasMaxLength(255);

            builder
                .Property(y => y.NewValue);

            builder
                .Property(y => y.OldValue);
        }
    }
}