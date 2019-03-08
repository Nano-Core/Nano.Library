using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Models.Mappings.Extensions;
using Nano.Security.Models;

namespace Nano.Data.Models.Mappings
{
    /// <inheritdoc />
    public class DefaultIdentityUserMapping<TEntity> : DefaultEntityMapping<TEntity> 
        where TEntity : DefaultIdentityUser
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .HasOne(x => x.IdentityUser)
                .WithOne()
                .IsRequired();

            builder
                .Ignore(x => x.Password);

            builder
                .MapType(x => x.EmailAddress);
        }
    }
}