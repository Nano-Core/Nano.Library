using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nano.Models.Interfaces;

namespace Nano.App.Data.Extensions
{
    /// <summary>
    /// Db Context Extensions.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Sets the global transaction isolation level to the passed <paramref name="isolationLevel"/>.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="isolationLevel">The global transaction isolation level.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task AddIsolationLevel(this DbContext dbContext, string isolationLevel)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            if (isolationLevel == null)
                throw new ArgumentNullException(nameof(isolationLevel));

            await dbContext.Database
                .OpenConnectionAsync()
                .ContinueWith(x =>
                {
                    // BUG: MySql permission issue for set global isolation level.

                    //var sql = $"SET GLOBAL TRANSACTION ISOLATION LEVEL {isolationLevel};";

                    // dbContext.Database.ExecuteSqlCommand(sql); 
                    dbContext.Database.CloseConnection();
                });
        }

        /// <summary>
        /// Updates a of <see cref="IEntityUpdatable"/> entity async.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="entity">The <see cref="IEntityUpdatable"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task UpdateAsync(this DbContext dbContext, IEntityUpdatable entity, CancellationToken cancellationToken = new CancellationToken())
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            dbContext
                .Update(entity);

            await dbContext
                .SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates a range of <see cref="IEntityUpdatable"/> entities async.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="entities">The <see cref="IEntityUpdatable"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task UpdateRangeAsync(this DbContext dbContext, IEnumerable<IEntityUpdatable> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await entities
                .ToAsyncEnumerable()
                .ForEachAsync(x =>
                    dbContext.Update(x), cancellationToken);

            await dbContext
                .SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Removes a of <see cref="IEntityUpdatable"/> entity async.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="entity">The <see cref="IEntityUpdatable"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task RemoveAsync(this DbContext dbContext, IEntityDeletable entity, CancellationToken cancellationToken = new CancellationToken())
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            dbContext
                .Remove(entity);

            await dbContext
                .SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Removes a range of <see cref="IEntityUpdatable"/> entities async.
        /// </summary>
        /// <param name="dbContext">The <see cref="DbContext"/>.</param>
        /// <param name="entities">The <see cref="IEntityUpdatable"/>'s.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static async Task RemoveRangeAsync(this DbContext dbContext, IEnumerable<IEntityDeletable> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await entities
                .ToAsyncEnumerable()
                .ForEachAsync(x =>
                    dbContext.Remove(x), cancellationToken);

            await dbContext
                .SaveChangesAsync(cancellationToken);
        }
    }
}