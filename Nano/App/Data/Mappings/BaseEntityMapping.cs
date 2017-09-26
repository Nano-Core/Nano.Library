using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Interfaces;

namespace Nano.App.Data.Mappings
{
    /// <inheritdoc />
    public abstract class BaseEntityMapping<TEntity> : BaseMapping<TEntity>
        where TEntity : class, IEntity
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

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