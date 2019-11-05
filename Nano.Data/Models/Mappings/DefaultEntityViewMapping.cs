using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models;

namespace Nano.Data.Models.Mappings
{
    /// <inheritdoc />
    public class DefaultEntityViewMapping<TEntity> : BaseEntityMapping<TEntity>
        where TEntity : BaseEntity
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder
                .HasNoKey();
        }
    }
}