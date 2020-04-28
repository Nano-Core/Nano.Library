using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models;
using Nano.Models.Interfaces;

namespace Nano.Data.Models.Mappings
{
    /// <inheritdoc />
    public class DefaultEntityMapping<TEntity> : BaseEntityIdentityMapping<TEntity, Guid>
        where TEntity : DefaultEntity, IEntityIdentity<Guid>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .HasQueryFilter(x => x.IsDeleted == 0L);

            builder
                .Property(x => x.CreatedAt)
                .ValueGeneratedOnAdd()
                .IsRequired()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder
                .HasIndex(x => x.CreatedAt);

            builder
                .Property(y => y.IsDeleted)
                .HasDefaultValue(0L)
                .IsRequired();

            builder
                .HasIndex(x => x.IsDeleted);
        }
    }
}