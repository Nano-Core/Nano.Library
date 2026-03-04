using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nano.Data.Mappings;

/// <inheritdoc />
public abstract class BaseMapping<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    /// <inheritdoc />
    public abstract void Configure(EntityTypeBuilder<TEntity> builder);
}