using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nano.App.Models.Mappings
{
    /// <inheritdoc />
    public class DefaultEntityIdentityMapping<TEntity> : BaseEntityIdentityMapping<TEntity, Guid>
        where TEntity : BaseEntityIdentity<Guid>
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