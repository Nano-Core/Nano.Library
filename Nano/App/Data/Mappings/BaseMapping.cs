using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Interfaces;

namespace Nano.App.Data.Mappings
{
    /// <summary>
    /// Base Mapping (abstract).
    /// </summary>
    /// <typeparam name="TEntity">Type implementing <see cref="IEntity"/>.</typeparam>
    public abstract class BaseMapping<TEntity>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Abstract method for mapping type of <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="builder">The <see cref="EntityTypeBuilder{T}"/>.</param>
        public abstract void Map(EntityTypeBuilder<TEntity> builder);
    }
}