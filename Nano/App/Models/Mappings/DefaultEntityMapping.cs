using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nano.App.Models.Mappings
{
    /// <inheritdoc />
    public abstract class DefaultEntityMapping<TEntity> : BaseEntityIdentityMapping<TEntity, Guid>
        where TEntity : DefaultEntity
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .Property(y => y.IsActive)
                .IsRequired();

            builder
                .Property(x => x.CreatedAt)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(x => x.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .IsRequired();

            builder
                .HasIndex(y => new { y.IsActive });

            builder
                .HasIndex(y => new { y.CreatedAt });

            builder
                .HasIndex(y => new { y.UpdatedAt });

            builder
                .HasIndex(y => new { y.ExpireAt });
        }
    }
}