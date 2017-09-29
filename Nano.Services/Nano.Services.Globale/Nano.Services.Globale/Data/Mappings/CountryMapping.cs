using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Mappings;
using Nano.Services.Globale.Models;

namespace Nano.Services.Globale.Data.Mappings
{
    /// <inheritdoc />
    public class CountryMapping : DefaultEntityMapping<Country>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<Country> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .Property(x => x.NativeName)
                .IsRequired();

            builder
                .Property(x => x.UniversalName)
                .IsRequired();

            builder
                .Property(x => x.IsoAlpha2)
                .IsRequired();

            builder
                .HasOne(x => x.Currency)
                .WithMany(x => x.Countries)
                .IsRequired();

            builder
                .HasOne(x => x.Language)
                .WithMany(x => x.Countries)
                .IsRequired();

            builder
                .HasIndex(x => x.NativeName);

            builder
                .HasIndex(x => x.UniversalName);

            builder
                .HasIndex(x => x.IsoAlpha2);
        }
    }
}