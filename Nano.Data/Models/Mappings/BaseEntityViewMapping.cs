using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Interfaces;

namespace Nano.Data.Models.Mappings;

/// <inheritdoc />
public abstract class BaseEntityViewMapping<TEntity> : BaseEntityMapping<TEntity>
    where TEntity : class, IEntity
{
    /// <inheritdoc />
    public override void Map(EntityTypeBuilder<TEntity> builder)
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        builder
            .ToView(typeof(TEntity).Name)
            .HasNoKey();
    }
}