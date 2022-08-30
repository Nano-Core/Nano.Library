using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Z.EntityFramework.Plus;

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
            dbContext.AddOrUpdate(entity);
        }
    }

    /// <summary>
    /// Save Changes With Audit And Triggers.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <returns>The rows affected.</returns>
    public static int SaveChangesWithAuditAndTriggers(this DbContext dbContext)
    {
        if (dbContext == null)
            throw new ArgumentNullException(nameof(dbContext));

        var audit = new Audit();

        audit
            .PreSaveChanges(dbContext);

        var rowAffecteds = dbContext
            .SaveChangesWithTriggers(dbContext.SaveChanges);

        audit
            .PostSaveChanges();

        var autoSavePreAction = audit.Configuration.AutoSavePreAction ?? AuditManager.DefaultConfiguration.AutoSavePreAction;

        if (autoSavePreAction != null)
        {
            autoSavePreAction
                .Invoke(dbContext, audit);

            dbContext
                .SaveChanges();
        }

        return rowAffecteds;
    }

    /// <summary>
    /// Save Changes With Audit And Triggers.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The rows affected.</returns>
    public static async Task<int> SaveChangesWithAuditAndTriggersAsync(this DbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (dbContext == null)
            throw new ArgumentNullException(nameof(dbContext));

        var audit = new Audit();

        audit
            .PreSaveChanges(dbContext);

        var rowAffecteds = await dbContext
            .SaveChangesWithTriggersAsync(dbContext.SaveChangesAsync, cancellationToken)
            .ConfigureAwait(false);

        audit
            .PostSaveChanges();

        var autoSavePreAction = audit.Configuration.AutoSavePreAction ?? AuditManager.DefaultConfiguration.AutoSavePreAction;

        if (autoSavePreAction != null)
        {
            autoSavePreAction
                .Invoke(dbContext, audit);

            await dbContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        return rowAffecteds;
    }
}