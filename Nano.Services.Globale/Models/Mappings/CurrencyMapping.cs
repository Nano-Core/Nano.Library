using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Mappings;

namespace Nano.Services.Globale.Models.Mappings
{
    /// <inheritdoc />
    public class CurrencyMapping : BaseEntityIdentityUniqueMapping<Currency>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<Currency> builder)
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
                .Property(x => x.Iso4217)
                .IsRequired();

            builder
                .Property(x => x.Rate)
                .IsRequired();

            builder
                .HasMany(x => x.Countries)
                .WithOne(x => x.Currency)
                .IsRequired();

            builder
                .HasIndex(x => x.NativeName);

            builder
                .HasIndex(x => x.UniversalName);

            builder
                .HasIndex(x => x.Iso4217);
        }
    }
}