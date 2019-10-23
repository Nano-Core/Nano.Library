using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nano.Data.Models.Mappings
{
    /// <inheritdoc />
    public class DefaultAuditEntryPropertyMapping : DefaultEntityMapping<DefaultAuditEntryProperty>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<DefaultAuditEntryProperty> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .ToTable("__EFAuditProperties");

            builder
                .HasOne(x => x.Parent)
                .WithMany(x => x.Properties)
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