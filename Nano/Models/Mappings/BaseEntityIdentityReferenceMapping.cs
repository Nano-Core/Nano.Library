using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Interfaces;

namespace Nano.Models.Mappings
{
    /// <inheritdoc />
    public abstract class BaseEntityIdentityReferenceMapping<TEntity> : BaseEntityIdentityMapping<TEntity, string>
        where TEntity : class, IEntityIdentity<string>
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