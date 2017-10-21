using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.App.Models.Mappings;

namespace Nano.Example.Models.Mappings
{
    /// <inheritdoc />
    public class ExampleEntityRelationMapping : DefaultEntityRelationMapping<ExampleEntityRelation, ExampleEntity>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<ExampleEntityRelation> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);
        }
    }
}