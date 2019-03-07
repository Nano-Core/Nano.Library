using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Interfaces;

namespace Nano.Data.Models.Mappings
{
    /// <summary>
    /// Base Mapping (abstract).
    /// </summary>
    /// <typeparam name="TEntity">Type implementing <see cref="IEntity"/>.</typeparam>
    public abstract class BaseEntityQueryMapping<TEntity> 
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Abstract method for mapping a type of <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="builder">The <see cref="QueryTypeBuilder{T}"/>.</param>
        public abstract void Map(QueryTypeBuilder<TEntity> builder);
    }
}