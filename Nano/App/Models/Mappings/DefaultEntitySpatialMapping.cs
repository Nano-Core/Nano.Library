using System;
using Nano.App.Models.Interfaces;

namespace Nano.App.Models.Mappings
{
    /// <inheritdoc />
    public abstract class DefaultEntitySpatialMapping<TEntity> : BaseEntitySpatialMapping<TEntity, Guid>
        where TEntity : class, IEntityIdentity<Guid>, IEntitySpatial
    {
        
    }
}