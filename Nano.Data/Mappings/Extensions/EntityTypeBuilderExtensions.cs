using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nano.Data.Mappings.Extensions;

/// <summary>
/// Entity Type Builder Extensions.
/// </summary>
public static class EntityTypeBuilderExtensions
{
    private static readonly ConcurrentDictionary<Type, HashSet<Delegate>> registeredTriggers = new();

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
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        AddOnce<TEntity>(action, x => { Triggers<TEntity>.Inserted += (Action<IInsertedEntry<TEntity>>)x; });
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
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        AddOnce<TEntity>(action, x => { Triggers<TEntity>.Inserting += (Action<IInsertingEntry<TEntity>>)x; });
    }

    /// <summary>
    /// Adds an insert failed event trigger to the model.
    /// The passed <paramref name="action"/> will be invoked, before the entity of type <typeparamref name="TEntity"/> is inserted.
    /// </summary>
    /// <typeparam name="TEntity">the type of entity.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
    /// <param name="action">The <see cref="Action"/> invoked.</param>
    public static void OnInsertFailed<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IInsertFailedEntry<TEntity>> action)
        where TEntity : class, IEntityCreatable
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        AddOnce<TEntity>(action, x => { Triggers<TEntity>.InsertFailed += (Action<IInsertFailedEntry<TEntity>>)x; });
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
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        AddOnce<TEntity>(action, x => { Triggers<TEntity>.Updated += (Action<IUpdatedEntry<TEntity>>)x; });
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
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        AddOnce<TEntity>(action, x => { Triggers<TEntity>.Updating += (Action<IUpdatingEntry<TEntity>>)x; });
    }

    /// <summary>
    /// Adds an update failed event trigger to the model.
    /// The passed <paramref name="action"/> will be invoked, before the entity of type <typeparamref name="TEntity"/> is updated.
    /// </summary>
    /// <typeparam name="TEntity">the type of entity.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
    /// <param name="action">The <see cref="Action"/> invoked.</param>
    public static void OnUpdateFailed<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IUpdateFailedEntry<TEntity>> action)
        where TEntity : class, IEntityUpdatable
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        AddOnce<TEntity>(action, x => { Triggers<TEntity>.UpdateFailed += (Action<IUpdateFailedEntry<TEntity>>)x; });
    }

    /// <summary>
    /// Adds a deleted event trigger to the model.
    /// The passed <paramref name="action"/> will be invoked, after the entity of type <typeparamref name="TEntity"/> is deleted.
    /// </summary>
    /// <typeparam name="TEntity">the type of entity.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
    /// <param name="action">The <see cref="Action"/> invoked.</param>
    public static void OnDeleted<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IDeletedEntry<TEntity>> action)
        where TEntity : class, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        AddOnce<TEntity>(action, x => { Triggers<TEntity>.Deleted += (Action<IDeletedEntry<TEntity>>)x; });
    }

    /// <summary>
    /// Adds an deleted event trigger to the model.
    /// The passed <paramref name="action"/> will be invoked, before the entity of type <typeparamref name="TEntity"/> is deleted.
    /// </summary>
    /// <typeparam name="TEntity">the type of entity.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
    /// <param name="action">The <see cref="Action"/> invoked.</param>
    public static void OnDeleting<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IDeletingEntry<TEntity>> action)
        where TEntity : class, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        AddOnce<TEntity>(action, x => { Triggers<TEntity>.Deleting += (Action<IDeletingEntry<TEntity>>)x; });
    }

    /// <summary>
    /// Adds an delete failed event trigger to the model.
    /// The passed <paramref name="action"/> will be invoked, before the entity of type <typeparamref name="TEntity"/> is deleted.
    /// </summary>
    /// <typeparam name="TEntity">the type of entity.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/>.</param>
    /// <param name="action">The <see cref="Action"/> invoked.</param>
    public static void OnDeleteFailed<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IDeleteFailedEntry<TEntity>> action)
        where TEntity : class, IEntityDeletable
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(action);

        AddOnce<TEntity>(action, x => { Triggers<TEntity>.DeleteFailed += (Action<IDeleteFailedEntry<TEntity>>)x; });
    }


    private static void AddOnce<TEntity>(Delegate action, Action<Delegate> addAction)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(addAction);

        var entityType = typeof(TEntity);

        var delegates = registeredTriggers
            .GetOrAdd(entityType, _ => []);

        lock (delegates)
        {
            if (!delegates.Contains(action))
            {
                addAction(action);

                delegates
                    .Add(action);
            }
        }
    }
}