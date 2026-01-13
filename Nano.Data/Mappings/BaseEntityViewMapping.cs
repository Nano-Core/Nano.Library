using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Mappings;

/// <inheritdoc />
public abstract class BaseEntityViewMapping<TEntity> : BaseEntityMapping<TEntity>
    where TEntity : class, IEntity
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToView(typeof(TEntity).Name)
            .HasNoKey();
    }
}