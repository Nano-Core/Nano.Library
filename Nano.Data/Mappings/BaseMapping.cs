using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Mappings;

/// <inheritdoc />
public abstract class BaseMapping<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IEntity
{
    /// <inheritdoc />
    public abstract void Configure(EntityTypeBuilder<TEntity> builder);
}