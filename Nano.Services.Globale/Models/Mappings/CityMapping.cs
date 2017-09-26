using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Mappings;

namespace Nano.Services.Globale.Models.Mappings
{
    /// <inheritdoc />
    public class CityMapping : BaseEntityIdentityUniqueMapping<City>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<City> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .Property(x => x.Name)
                .IsRequired();

            builder
                .Property(x => x.ZipCode)
                .IsRequired();

            builder
                .HasOne(x => x.Country)
                .WithMany(x => x.Cities)
                .IsRequired();

            builder
                .HasIndex(x => x.Name);

            builder
                .HasIndex(x => x.ZipCode);
        }
    }
}