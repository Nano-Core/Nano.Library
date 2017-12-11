using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Models.Mappings;
using NanoCore.Example.Models;

namespace NanoCore.Example.Data.Models.Mappings
{
    /// <inheritdoc />
    public class ExampleEntitySpatialMapping : DefaultEntitySpatialMapping<ExampleEntitySpatial>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<ExampleEntitySpatial> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .Property(x => x.PropertyOne);
        }
    }
}