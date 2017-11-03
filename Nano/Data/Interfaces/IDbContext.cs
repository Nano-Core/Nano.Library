using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

namespace Nano.Data.Interfaces
{
    /// <summary>
    /// A DbContext definition represents a session with the database and can be used to query and save instances of your entities. 
    /// A combination of the Unit Of Work and Repository patterns.
    /// </summary>
    public interface IDbContext : 
        IDisposable, 
        IInfrastructure<IServiceProvider>, 
        IDbContextDependencies, 
        IDbSetCache, 
        IDbContextPoolable,
        IDbContextExtension
    {
        /// <summary>
        /// Changed Entries.
        /// </summary>
        List<EntityEntry> ChangedEntries { get; set; }

        /// <summary>
        /// Provides access to database related information and operations for this context.
        /// </summary>
        DatabaseFacade Database { get; }

        /// <summary>
        /// Provides access to information and operations for entity instances this context is tracking.
        /// </summary>
        ChangeTracker ChangeTracker { get; }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        int SaveChanges();

        /// <summary>
        ///     Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indicates whether <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" /> is called after the changes have been sent successfully to the database.</param>
        /// <returns>The number of state entries written to the database.</returns>
        int SaveChanges(bool acceptAllChangesOnSuccess);

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a <see cref="DbSet{TEntity}" /> that can be used to query and save instances of <typeparamref name="TEntity" />.
        /// </summary>
        /// <typeparam name="TEntity"> The type of entity for which a set should be returned. </typeparam>
        /// <returns> A set for the given entity type. </returns>
        DbSet<TEntity> Set<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Gets an <see cref="EntityEntry{TEntity}" /> for the given entity. The entry provides
        /// access to change tracking information and operations for the entity.
        /// </summary>
        /// <typeparam name="TEntity"> The type of the entity.</typeparam>
        /// <param name="entity"> The entity to get the entry for.</param>
        /// <returns> The entry for the given entity.</returns>
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Gets an <see cref="EntityEntry" /> for the given entity. The entry provides
        /// access to change tracking information and operations for the entity.
        /// This method may be called on an entity that is not tracked. You can then
        /// set the <see cref="EntityEntry.State" /> property on the returned entry
        /// to have the context begin tracking the entity in the specified state.
        /// </summary>
        /// <param name="entity"> The entity to get the entry for.</param>
        /// <returns> The entry for the given entity.</returns>
        EntityEntry Entry(object entity);

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are
        /// not already being tracked, in the <see cref="EntityState.Added" /> state such that
        /// they will be inserted into the database when <see cref="SaveChanges()" /> is called.
        /// </summary>
        /// <typeparam name="TEntity"> The type of the entity.</typeparam>
        /// <param name="entity"> The entity to add.</param>
        /// <returns>The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
        EntityEntry<TEntity> Add<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are
        /// not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
        /// be inserted into the database when <see cref="SaveChanges()" /> is called.
        /// This method is async only to allow special value generators, such as the one used by
        /// 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
        /// to access the database asynchronously. For all other cases the non async method should be used.
        /// </summary>
        /// <typeparam name="TEntity"> The type of the entity.</typeparam>
        /// <param name="entity"> The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous Add operation. The task result contains the <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
        Task<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// Begins tracking the given entity in the <see cref="EntityState.Unchanged" /> state
        /// such that no operation will be performed when <see cref="IDbContext.SaveChanges()" />
        /// is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities
        /// that are not already being tracked by the context. These entities will also begin to be tracked
        /// by the context. If a reachable entity has its primary key value set
        /// then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
        /// value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
        /// An entity is considered to have its primary key value set if the primary key property is set
        /// to anything other than the CLR default for the property type.
        /// </summary>
        /// <typeparam name="TEntity"> The type of the entity.</typeparam>
        /// <param name="entity"> The entity to attach.</param>
        /// <returns>The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
        EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Begins tracking the given entity in the <see cref="EntityState.Modified" /> state such that it will
        /// be updated in the database when <see cref="IDbContext.SaveChanges()" /> is called.
        /// All properties of the entity will be marked as modified. To mark only some properties as modified, use
        /// <see cref="Attach{TEntity}(TEntity)" /> to begin tracking the entity in the <see cref="EntityState.Unchanged" />
        /// state and then use the returned <see cref="EntityEntry" /> to mark the desired properties as modified.
        /// A recursive search of the navigation properties will be performed to find reachable entities
        /// that are not already being tracked by the context. These entities will also begin to be tracked
        /// by the context. If a reachable entity has its primary key value set
        /// then it will be tracked in the <see cref="EntityState.Modified" /> state. If the primary key
        /// value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
        /// An entity is considered to have its primary key value set if the primary key property is set
        /// to anything other than the CLR default for the property type.
        /// </summary>
        /// <typeparam name="TEntity"> The type of the entity.</typeparam>
        /// <param name="entity"> The entity to update.</param>
        /// <returns>The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
        EntityEntry<TEntity> Update<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Begins tracking the given entity in the <see cref="EntityState.Deleted" /> state such that it will
        /// be removed from the database when <see cref="SaveChanges()" /> is called.
        /// </summary>
        /// <typeparam name="TEntity"> The type of the entity. </typeparam>
        /// <param name="entity"> The entity to remove. </param>
        /// <returns>The <see cref="EntityEntry{TEntity}" /> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
        EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are
        /// not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
        /// be inserted into the database when <see cref="SaveChanges()" /> is called.
        /// </summary>
        /// <param name="entity"> The entity to add. </param>
        /// <returns>The <see cref="EntityEntry" /> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
        EntityEntry Add(object entity);

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are
        /// not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
        /// be inserted into the database when <see cref="SaveChanges()" /> is called.
        /// This method is async only to allow special value generators, such as the one used by
        /// 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
        /// to access the database asynchronously. For all other cases the non async method should be used.
        /// </summary>
        /// <param name="entity"> The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous Add operation. The task result contains the <see cref="EntityEntry" /> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
        Task<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins tracking the given entity in the <see cref="EntityState.Unchanged" /> state
        /// such that no operation will be performed when <see cref="IDbContext.SaveChanges()" />
        /// is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities
        /// that are not already being tracked by the context. These entities will also begin to be tracked
        /// by the context. If a reachable entity has its primary key value set
        /// then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
        /// value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
        /// An entity is considered to have its primary key value set if the primary key property is set
        /// to anything other than the CLR default for the property type.
        /// </summary>
        /// <param name="entity"> The entity to attach. </param>
        /// <returns>The <see cref="EntityEntry" /> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
        EntityEntry Attach(object entity);

        /// <summary>
        /// Begins tracking the given entity in the <see cref="EntityState.Modified" /> state such that it will
        /// be updated in the database when <see cref="IDbContext.SaveChanges()" /> is called.
        /// All properties of the entity will be marked as modified. To mark only some properties as modified, use
        /// <see cref="Attach(object)" /> to begin tracking the entity in the <see cref="EntityState.Unchanged" />
        /// state and then use the returned <see cref="EntityEntry" /> to mark the desired properties as modified.
        /// A recursive search of the navigation properties will be performed to find reachable entities
        /// that are not already being tracked by the context. These entities will also begin to be tracked
        /// by the context. If a reachable entity has its primary key value set
        /// then it will be tracked in the <see cref="EntityState.Modified" /> state. If the primary key
        /// value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
        /// An entity is considered to have its primary key value set if the primary key property is set
        /// to anything other than the CLR default for the property type.
        /// </summary>
        /// <param name="entity"> The entity to update. </param>
        /// <returns>The <see cref="EntityEntry" /> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
        EntityEntry Update(object entity);

        /// <summary>
        /// Begins tracking the given entity in the <see cref="EntityState.Deleted" /> state such that it will
        /// be removed from the database when <see cref="SaveChanges()" /> is called.
        /// </summary>
        /// <param name="entity"> The entity to remove. </param>
        /// <returns>The <see cref="EntityEntry" /> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
        EntityEntry Remove(object entity);

        /// <summary>
        /// Begins tracking the given entities, and any other reachable entities that are
        /// not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
        /// be inserted into the database when <see cref="SaveChanges()" /> is called.
        /// </summary>
        /// <param name="entities"> The entities to add. </param>
        void AddRange(params object[] entities);

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are
        /// not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
        /// be inserted into the database when <see cref="SaveChanges()" /> is called.
        /// This method is async only to allow special value generators, such as the one used by
        /// 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
        /// to access the database asynchronously. For all other cases the non async method should be used.
        /// </summary>
        /// <param name="entities"> The entities to add. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task AddRangeAsync(params object[] entities);

        /// <summary>
        /// Begins tracking the given entities in the <see cref="EntityState.Unchanged" /> state
        /// such that no operation will be performed when <see cref="IDbContext.SaveChanges()" />
        /// is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities
        /// that are not already being tracked by the context. These entities will also begin to be tracked
        /// by the context. If a reachable entity has its primary key value set
        /// then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
        /// value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
        /// An entity is considered to have its primary key value set if the primary key property is set
        /// to anything other than the CLR default for the property type.
        /// </summary>
        /// <param name="entities"> The entities to attach. </param>
        void AttachRange(params object[] entities);

        /// <summary>
        /// Begins tracking the given entities in the <see cref="EntityState.Modified" /> state such that they will
        /// be updated in the database when <see cref="IDbContext.SaveChanges()" /> is called.
        /// All properties of each entity will be marked as modified. To mark only some properties as modified, use
        /// <see cref="Attach(object)" /> to begin tracking each entity in the <see cref="EntityState.Unchanged" />
        /// state and then use the returned <see cref="EntityEntry" /> to mark the desired properties as modified.
        /// A recursive search of the navigation properties will be performed to find reachable entities
        /// that are not already being tracked by the context. These entities will also begin to be tracked
        /// by the context. If a reachable entity has its primary key value set
        /// then it will be tracked in the <see cref="EntityState.Modified" /> state. If the primary key
        /// value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
        /// An entity is considered to have its primary key value set if the primary key property is set
        /// to anything other than the CLR default for the property type.
        /// </summary>
        /// <param name="entities"> The entities to update. </param>
        void UpdateRange(params object[] entities);

        /// <summary>
        /// Begins tracking the given entity in the <see cref="EntityState.Deleted" /> state such that it will
        /// be removed from the database when <see cref="SaveChanges()" /> is called.
        /// </summary>
        /// <param name="entities"> The entities to remove. </param>
        void RemoveRange(params object[] entities);

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indicates whether <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" /> is called after the changes have been sent successfully to the database.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins tracking the given entities, and any other reachable entities that are
        /// not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
        /// be inserted into the database when <see cref="SaveChanges()" /> is called.
        /// </summary>
        /// <param name="entities"> The entities to add. </param>
        void AddRange(IEnumerable<object> entities);

        /// <summary>
        /// Begins tracking the given entity, and any other reachable entities that are
        /// not already being tracked, in the <see cref="EntityState.Added" /> state such that they will
        /// be inserted into the database when <see cref="SaveChanges()" /> is called.
        /// This method is async only to allow special value generators, such as the one used by
        /// 'Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.SequenceHiLo',
        /// to access the database asynchronously. For all other cases the non async method should be used.
        /// </summary>
        /// <param name="entities"> The entities to add. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// </returns>
        Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins tracking the given entities in the <see cref="EntityState.Unchanged" /> state
        /// such that no operation will be performed when <see cref="IDbContext.SaveChanges()" /> is called.
        /// A recursive search of the navigation properties will be performed to find reachable entities
        /// that are not already being tracked by the context. These entities will also begin to be tracked
        /// by the context. If a reachable entity has its primary key value set
        /// then it will be tracked in the <see cref="EntityState.Unchanged" /> state. If the primary key
        /// value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
        /// An entity is considered to have its primary key value set if the primary key property is set
        /// to anything other than the CLR default for the property type.
        /// </summary>
        /// <param name="entities"> The entities to attach. </param>
        void AttachRange(IEnumerable<object> entities);

        /// <summary>
        /// Begins tracking the given entities in the <see cref="EntityState.Modified" /> state such that they will
        /// be updated in the database when <see cref="IDbContext.SaveChanges()" /> is called.
        /// All properties of each entity will be marked as modified. To mark only some properties as modified, use
        /// <see cref="Attach(object)" /> to begin tracking each entity in the <see cref="EntityState.Unchanged" />
        /// state and then use the returned <see cref="EntityEntry" /> to mark the desired properties as modified.
        /// A recursive search of the navigation properties will be performed to find reachable entities
        /// that are not already being tracked by the context. These entities will also begin to be tracked
        /// by the context. If a reachable entity has its primary key value set
        /// then it will be tracked in the <see cref="EntityState.Modified" /> state. If the primary key
        /// value is not set then it will be tracked in the <see cref="EntityState.Added" /> state.
        /// An entity is considered to have its primary key value set if the primary key property is set
        /// to anything other than the CLR default for the property type.
        /// </summary>
        /// <param name="entities"> The entities to update. </param>
        void UpdateRange(IEnumerable<object> entities);

        /// <summary>
        /// Begins tracking the given entity in the <see cref="EntityState.Deleted" /> state such that it will
        /// be removed from the database when <see cref="SaveChanges()" /> is called.
        /// </summary>
        /// <param name="entities"> The entities to remove. </param>
        void RemoveRange(IEnumerable<object> entities);

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values
        /// is being tracked by the context, then it is returned immediately without making a request to the
        /// database. Otherwise, a query is made to the database for an entity with the given primary key values
        /// and this entity, if found, is attached to the context and returned. If no entity is found, then
        /// null is returned.
        /// </summary>
        /// <param name="entityType"> The type of entity to find. </param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        object Find(Type entityType, params object[] keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values
        /// is being tracked by the context, then it is returned immediately without making a request to the
        /// database. Otherwise, a query is made to the database for an entity with the given primary key values
        /// and this entity, if found, is attached to the context and returned. If no entity is found, then
        /// null is returned.
        /// </summary>
        /// <param name="entityType"> The type of entity to find. </param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        Task<object> FindAsync(Type entityType, params object[] keyValues);

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values
        /// is being tracked by the context, then it is returned immediately without making a request to the
        /// database. Otherwise, a query is made to the database for an entity with the given primary key values
        /// and this entity, if found, is attached to the context and returned. If no entity is found, then
        /// null is returned.
        /// </summary>
        /// <param name="entityType"> The type of entity to find. </param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The entity found, or null.</returns>
        Task<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken);

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values
        /// is being tracked by the context, then it is returned immediately without making a request to the
        /// database. Otherwise, a query is made to the database for an entity with the given primary key values
        /// and this entity, if found, is attached to the context and returned. If no entity is found, then
        /// null is returned.
        /// </summary>
        /// <typeparam name="TEntity"> The type of entity to find. </typeparam>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        TEntity Find<TEntity>(params object[] keyValues)
            where TEntity : class;

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values
        /// is being tracked by the context, then it is returned immediately without making a request to the
        /// database. Otherwise, a query is made to the database for an entity with the given primary key values
        /// and this entity, if found, is attached to the context and returned. If no entity is found, then
        /// null is returned.
        /// </summary>
        /// <typeparam name="TEntity"> The type of entity to find. </typeparam>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        Task<TEntity> FindAsync<TEntity>(params object[] keyValues)
            where TEntity : class;

        /// <summary>
        /// Finds an entity with the given primary key values. If an entity with the given primary key values
        /// is being tracked by the context, then it is returned immediately without making a request to the
        /// database. Otherwise, a query is made to the database for an entity with the given primary key values
        /// and this entity, if found, is attached to the context and returned. If no entity is found, then
        /// null is returned.
        /// </summary>
        /// <typeparam name="TEntity"> The type of entity to find. </typeparam>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The entity found, or null.</returns>
        Task<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken)
            where TEntity : class;
    }
}