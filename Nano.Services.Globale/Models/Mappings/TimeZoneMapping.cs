using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Mappings;

namespace Nano.Services.Globale.Models.Mappings
{
    /// <inheritdoc />
    public class TimeZoneMapping : BaseEntityIdentityUniqueMapping<TimeZone>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TimeZone> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .Property(x => x.OlsonName)
                .IsRequired();

            builder
                .Property(x => x.MicrosoftName)
                .IsRequired();

            builder
                .HasIndex(x => x.OlsonName)
                .IsUnique();

            builder
                .HasIndex(x => x.MicrosoftName)
                .IsUnique();
        }
    }
}