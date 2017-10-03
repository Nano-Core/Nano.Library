using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.App.Models.Mappings;
using Nano.Example.Models;

namespace Nano.Example.Data.Mappings
{
    /// <inheritdoc />
    public class ExampleEntityMapping : DefaultEntityMapping<ExampleEntity>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<ExampleEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .Property(x => x.PropertyOne)
                .IsRequired();

            builder
                .Property(x => x.PropertyTwo)
                .IsRequired();

            builder
                .HasIndex(x => x.PropertyOne)
                .IsUnique();

            builder
                .HasIndex(x => x.PropertyTwo)
                .IsUnique();
        }
    }
}