using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Nano.Models.Interfaces;

namespace Nano.Models.Mappings
{
    /// <inheritdoc />
    public abstract class BaseEntityIdentityUniqueMapping<TEntity> : BaseEntityIdentityMapping<TEntity, Guid>
        where TEntity : class, IEntityIdentity<Guid>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);

            builder
                .HasKey(y => y.Id);

            builder
                .Property(x => x.Id)
                .HasValueGenerator<GuidValueGenerator>()
                .ValueGeneratedOnAdd();
        }
    }
}