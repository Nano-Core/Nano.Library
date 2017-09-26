using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.App.Data.Mappings;
using Nano.Services.Globale.Models;

namespace Nano.Services.Globale.Data.Mappings
{
    /// <inheritdoc />
    public class CityMapping : BaseEntityIdentityMapping<City, Guid>
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