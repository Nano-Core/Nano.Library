using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models;

namespace Nano.Data.Models.Mappings
{
    /// <inheritdoc />
    public class DefaultEntitySpatialMapping<TEntity> : DefaultEntityMapping<TEntity>
        where TEntity : DefaultEntitySpatial
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .Property(x => x.Geometry);
        }
    }
}