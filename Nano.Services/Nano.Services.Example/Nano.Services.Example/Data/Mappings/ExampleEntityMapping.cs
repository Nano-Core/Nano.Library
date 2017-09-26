using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.App.Data.Mappings;
using Nano.Services.Example.Models;

namespace Nano.Services.Example.Data.Mappings
{
    /// <inheritdoc />
    public class ExampleEntityMapping : BaseEntityIdentityMapping<ExampleEntity, Guid>
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