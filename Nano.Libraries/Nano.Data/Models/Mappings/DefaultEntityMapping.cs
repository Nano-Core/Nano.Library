using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models;
using Nano.Models.Interfaces;

namespace Nano.Data.Models.Mappings
{
    /// <inheritdoc />
    public class DefaultEntityMapping<TEntity> : BaseEntityIdentityMapping<TEntity, Guid>
        where TEntity : DefaultEntity, IEntityIdentity<Guid>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .HasQueryFilter(x => x.IsActive);

            builder
                .Property(y => y.IsActive)
                .IsRequired();

            builder
                .Property(x => x.CreatedAt)
                .ValueGeneratedOnAdd()
                .IsRequired();
 
            builder
                .HasIndex(x => x.IsActive);

            builder
                .HasIndex(x => x.CreatedAt);
        }
    }
}