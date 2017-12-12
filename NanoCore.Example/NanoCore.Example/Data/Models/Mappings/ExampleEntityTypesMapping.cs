using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Models.Mappings;
using Nano.Data.Models.Mappings.Extensions;
using NanoCore.Example.Models;

namespace NanoCore.Example.Data.Models.Mappings
{
    /// <inheritdoc />
    public class ExampleEntityTypesMapping : DefaultEntityMapping<ExampleEntityTypes>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<ExampleEntityTypes> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .MapType(x => x.Angle);

            builder
                .MapType(x => x.AuthenticationCredential);

            builder
                .MapType(x => x.Distance);

            builder
                .MapType(x => x.Duration);

            builder
                .MapType(x => x.EmailAddress);

            builder
                .MapType(x => x.Location);

            builder
                .MapType(x => x.Percentage);

            builder
                .MapType(x => x.Period);

            builder
                .MapType(x => x.PhoneNumber);

            builder
                .OwnsOne(x => x.Nested)
                .MapType(x => x.Distance);

            builder
                .OwnsOne(x => x.Nested)
                .MapType(x => x.Duration);
        }
    }
}