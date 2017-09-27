using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Interfaces;

namespace Nano.Data.Mappings
{
    /// <inheritdoc />
    public abstract class DefaultEntityMapping<TEntity> : BaseEntityIdentityMapping<TEntity, Guid>
        where TEntity : class, IEntityIdentity<Guid>
    {
        /// <inheritdoc />
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            base.Map(builder);
        }
    }
}