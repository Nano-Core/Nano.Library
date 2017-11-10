using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models;

namespace Nano.Data.Mappings
{
    /// <inheritdoc />
    public class DefaultEntityIdentityMapping<TEntity> : BaseEntityIdentityMapping<TEntity, Guid>
           where TEntity : DefaultEntity
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