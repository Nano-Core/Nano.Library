using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Entities.Abstractions;

namespace Nano.Data.Mappings;

/// <summary>
/// Base mapping class for EF Core entities.
/// Provides an abstract method to configure the entity type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to map.</typeparam>
public abstract class BaseEntityMapping<TEntity>
    where TEntity : class, IEntity
{
    /// <summary>
    /// Configures the EF Core model for <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the entity.</param>
    public abstract void Map(EntityTypeBuilder<TEntity> builder);
}