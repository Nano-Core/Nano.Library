using System;
using Nano.App.Models.Interfaces;

namespace Nano.App.Models.Mappings
{
    /// <inheritdoc />
    public abstract class DefaultEntityMapping<TEntity> : BaseEntityIdentityMapping<TEntity, Guid>
        where TEntity : class, IEntityIdentity<Guid>
    {
        
    }
}