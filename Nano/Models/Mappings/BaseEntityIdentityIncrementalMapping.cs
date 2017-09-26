using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Interfaces;

namespace Nano.Models.Mappings
{
    /// <inheritdoc />
    public abstract class BaseEntityIdentityIncrementalMapping<TEntity> : BaseEntityIdentityMapping<TEntity, long>
        where TEntity : class, IEntityIdentity<long>
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