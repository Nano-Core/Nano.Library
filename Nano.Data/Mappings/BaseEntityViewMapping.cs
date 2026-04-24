using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models;

namespace Nano.Data.Mappings;

/// <summary>
/// Base mapping class for read-only entities mapped to database views.
/// Configures the entity to have no key and maps it to a view.
/// </summary>
/// <typeparam name="TEntity">The read-only entity type.</typeparam>
public abstract class BaseEntityViewMapping<TEntity> : BaseMapping<TEntity>
    where TEntity : BaseEntityView
{
    /// <inheritdoc />
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder
            .ToView(typeof(TEntity).Name)
            .HasNoKey();
    }
}