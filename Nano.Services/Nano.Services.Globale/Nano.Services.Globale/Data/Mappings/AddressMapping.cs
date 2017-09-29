using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Mappings;
using Nano.Services.Globale.Models;

namespace Nano.Services.Globale.Data.Mappings
{
    /// <inheritdoc />
    public class AddressMapping : DefaultEntitySpatialMapping<Address>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<Address> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .Property(x => x.AddressString)
                .IsRequired();

            builder
                .HasOne(x => x.City)
                .WithMany();

            builder
                .HasOne(x => x.TimeZone)
                .WithMany();

            builder
                .OwnsOne(x => x.Details);

            builder
                .HasIndex(x => x.AddressString);
        }
    }
}