using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.App.Data.Mappings;
using Nano.Services.Globale.Models;

namespace Nano.Services.Globale.Data.Mappings
{
    /// <inheritdoc />
    public class LanguageMapping : BaseEntityIdentityMapping<Language, Guid>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<Language> builder)
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
                .Property(x => x.Iso639_1)
                .IsRequired();

            builder
                .HasMany(x => x.Countries)
                .WithOne(x => x.Language)
                .IsRequired();

            builder
                .HasIndex(x => x.NativeName);

            builder
                .HasIndex(x => x.UniversalName);

            builder
                .HasIndex(x => x.Iso639_1);
        }
    }
}