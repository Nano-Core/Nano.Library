using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nano.Data.Models.Mappings
{
    /// <inheritdoc />
    public class DefaultAuditEntryMapping : BaseEntityMapping<DefaultAuditEntry>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<DefaultAuditEntry> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder
                .HasBaseType<AuditEntry>();

            builder
                .Property(y => y.RequestId)
                .HasMaxLength(255);

            builder
                .HasIndex(y => y.RequestId);

            builder
                .HasIndex(y => y.CreatedBy);

            builder
                .HasIndex(y => y.CreatedDate);

            builder
                .HasIndex(y => y.EntityTypeName);
        }
    }
}