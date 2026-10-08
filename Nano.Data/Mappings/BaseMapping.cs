using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Mappings;

/// <inheritdoc />
public abstract class BaseMapping<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IEntity
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        this.ConfigureBase(builder);
        this.ConfigureEntity(builder);
    }

    internal virtual void ConfigureBase(EntityTypeBuilder<TEntity> builder)
    {
    }

    /// <summary>
    /// Configures the mapping specific to this entity.
    /// Nano's inherited configuration has already been applied when this runs, so there is no base method to call.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}