using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Mappings.Extensions;

/// <summary>
/// Provides extension methods for <see cref="EntityTypeBuilder{TEntity}"/> to register entity lifecycle event triggers.
/// These triggers allow executing custom logic before or after insert, update, and delete operations on entities.
/// </summary>
public static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Registers a callback to be executed **after** a <typeparamref name="TEntity"/> is inserted into the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity implementing <see cref="IEntityCreatable"/>.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the entity.</param>
    /// <param name="action">The callback action invoked after the insert operation.</param>
    public static void OnInserted<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IInsertedEntry<TEntity>> action)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(builder);

        Triggers<TEntity>.Inserted += action;
    }

    /// <summary>
    /// Registers a callback to be executed **before** a <typeparamref name="TEntity"/> is inserted into the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity implementing <see cref="IEntityCreatable"/>.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the entity.</param>
    /// <param name="action">The callback action invoked before the insert operation.</param>
    public static void OnInserting<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IInsertingEntry<TEntity>> action)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(builder);

        Triggers<TEntity>.Inserting += action;
    }

    /// <summary>
    /// Registers a callback to be executed **after** a <typeparamref name="TEntity"/> is updated in the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity implementing <see cref="IEntityUpdatable"/>.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the entity.</param>
    /// <param name="action">The callback action invoked after the update operation.</param>
    public static void OnUpdated<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IUpdatedEntry<TEntity>> action)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(builder);

        Triggers<TEntity>.Updated += action;
    }

    /// <summary>
    /// Registers a callback to be executed **before** a <typeparamref name="TEntity"/> is updated in the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity implementing <see cref="IEntityUpdatable"/>.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the entity.</param>
    /// <param name="action">The callback action invoked before the update operation.</param>
    public static void OnUpdating<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IUpdatingEntry<TEntity>> action)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(builder);

        Triggers<TEntity>.Updating += action;
    }

    /// <summary>
    /// Registers a callback to be executed **after** a <typeparamref name="TEntity"/> is soft-deleted from the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity implementing <see cref="IEntitySoftDeletable"/>.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the entity.</param>
    /// <param name="action">The callback action invoked after the delete operation.</param>
    public static void OnDeleted<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IDeletedEntry<TEntity>> action)
        where TEntity : class, IEntitySoftDeletable
    {
        ArgumentNullException.ThrowIfNull(builder);

        Triggers<TEntity>.Deleted += action;
    }

    /// <summary>
    /// Registers a callback to be executed **before** a <typeparamref name="TEntity"/> is soft-deleted from the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity implementing <see cref="IEntitySoftDeletable"/>.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the entity.</param>
    /// <param name="action">The callback action invoked before the delete operation.</param>
    public static void OnDeleting<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IDeletingEntry<TEntity>> action)
        where TEntity : class, IEntitySoftDeletable
    {
        ArgumentNullException.ThrowIfNull(builder);

        Triggers<TEntity>.Deleting += action;
    }
}
