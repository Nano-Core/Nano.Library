using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nano.Repository;

/// <summary>
/// Db Context Extensions.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Adds or updates (if exists) the entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of <paramref name="entity"/>.</typeparam>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <param name="entity">The <see cref="object"/> of type <typeparamref name="TEntity"/>.</param>
    /// <returns>A <see cref="EntityEntry"/>.</returns>
    public static EntityEntry<TEntity> AddOrUpdate<TEntity>(this DbContext dbContext, TEntity entity)
        where TEntity : class
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var dbSet = dbContext
            .Set<TEntity>();

        var tracked = dbSet
            .SingleOrDefault(x => x == entity);

        if (tracked != null)
        {
            dbContext
                .Entry(tracked)
                .CurrentValues
                .SetValues(entity);

            return dbContext
                .Entry(tracked);
        }

        return dbSet
            .Add(entity);
    }

    /// <summary>
    /// Adds or updates (if exists) the entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of <paramref name="entities"/>.</typeparam>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <param name="entities">The <see cref="object"/>'s of type <typeparamref name="TEntity"/>.</param>
    /// <returns>A <see cref="EntityEntry{TEntity}"/>.</returns>
    public static void AddOrUpdateMany<TEntity>(this DbContext dbContext, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        if (entities == null)
            throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities)
        {
            dbContext
                .AddOrUpdate(entity);
        }
    }
}