using Nano.App.Models.Interfaces;

namespace Nano.App.Models.Mappings
{
    /// <inheritdoc />
    public abstract class BaseEntityMapping<TEntity> : BaseMapping<TEntity>
        where TEntity : class, IEntity
    {

    }
}