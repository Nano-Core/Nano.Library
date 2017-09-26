using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.App.Data.Mappings;
using TimeZone = Nano.Services.Globale.Models.TimeZone;

namespace Nano.Services.Globale.Data.Mappings
{
    /// <inheritdoc />
    public class TimeZoneMapping : BaseEntityIdentityMapping<TimeZone, Guid>
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