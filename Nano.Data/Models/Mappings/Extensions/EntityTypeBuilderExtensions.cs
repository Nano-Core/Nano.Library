using System;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Models.Interfaces;

namespace Nano.Data.Models.Mappings.Extensions;

/// <summary>
/// Entity Type Builder Extensions.
/// </summary>
public static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Adds an inserted event trigger to the model.
    /// The passed <paramref name="action"/> will be invoked, after the entity of type <typeparamref name="TEntity"/> is inserted.
    /// </summary>
    /// <typeparam name="TEntity">the type of entity.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
    /// <param name="action">The <see cref="Action"/> invoked.</param>
    public static void OnInserted<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IInsertedEntry<TEntity>> action)
        where TEntity : class, IEntityCreatable
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        Triggers<TEntity>.Inserted += action;
    }

    /// <summary>
    /// Adds an inserting event trigger to the model.
    /// The passed <paramref name="action"/> will be invoked, before the entity of type <typeparamref name="TEntity"/> is inserted.
    /// </summary>
    /// <typeparam name="TEntity">the type of entity.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
    /// <param name="action">The <see cref="Action"/> invoked.</param>
    public static void OnInserting<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IInsertingEntry<TEntity>> action)
        where TEntity : class, IEntityCreatable
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        Triggers<TEntity>.Inserting += action;
    }

    /// <summary>
    /// Adds an updated event trigger to the model.
    /// The passed <paramref name="action"/> will be invoked, after the entity of type <typeparamref name="TEntity"/> is updated.
    /// </summary>
    /// <typeparam name="TEntity">the type of entity.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
    /// <param name="action">The <see cref="Action"/> invoked.</param>
    public static void OnUpdated<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IUpdatedEntry<TEntity>> action)
        where TEntity : class, IEntityUpdatable
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        Triggers<TEntity>.Updated += action;
    }

    /// <summary>
    /// Adds an updating event trigger to the model.
    /// The passed <paramref name="action"/> will be invoked, before the entity of type <typeparamref name="TEntity"/> is updated.
    /// </summary>
    /// <typeparam name="TEntity">the type of entity.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
    /// <param name="action">The <see cref="Action"/> invoked.</param>
    public static void OnUpdating<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IUpdatingEntry<TEntity>> action)
        where TEntity : class, IEntityUpdatable
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        Triggers<TEntity>.Updating += action;
    }

    /// <summary>
    /// Adds a deleted event trigger to the model.
    /// The passed <paramref name="action"/> will be invoked, after the entity of type <typeparamref name="TEntity"/> is deleted.
    /// </summary>
    /// <typeparam name="TEntity">the type of entity.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
    /// <param name="action">The <see cref="Action"/> invoked.</param>
    public static void OnDeleted<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IDeletedEntry<TEntity>> action)
        where TEntity : class, IEntityDeletableSoft
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        Triggers<TEntity>.Deleted += action;
    }

    /// <summary>
    /// Adds an deleted event trigger to the model.
    /// The passed <paramref name="action"/> will be invoked, before the entity of type <typeparamref name="TEntity"/> is deleted.
    /// </summary>
    /// <typeparam name="TEntity">the type of entity.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
    /// <param name="action">The <see cref="Action"/> invoked.</param>
    public static void OnDeleting<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IDeletingEntry<TEntity>> action)
        where TEntity : class, IEntityDeletableSoft
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        Triggers<TEntity>.Deleting += action;
    }
}