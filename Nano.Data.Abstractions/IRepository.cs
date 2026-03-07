using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Enums;
using DynamicExpression.Interfaces;
using Nano.Data.Abstractions.Annotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Abstractions;

/// <summary>
/// (Base) Interface for a repository.
/// Defines methods for the most rudimentary operations (get, criteria, add, update, delete) on instances of <see cref="IEntity"/>,
/// <see cref="IEntityCreatable"/>, <see cref="IEntityUpdatable"/>, and <see cref="IEntityDeletable"/>.
/// </summary>
public interface IRepository : IDisposable
{
    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/> by its unique key.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The identity type.</typeparam>
    /// <param name="key">The unique key of the <see cref="IEntity"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the given key, or null if not found.</returns>
    Task<TEntity?> GetAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/> by its unique key, including related entities to the specified depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The identity type.</typeparam>
    /// <param name="key">The unique key of the <see cref="IEntity"/>.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the given key, or null if not found.</returns>
    Task<TEntity?> GetAsync<TEntity, TKey>(TKey key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/> by its integer key.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Int32}"/> type.</typeparam>
    /// <param name="key">The integer key of the entity.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the key, or null if not found.</returns>
    Task<TEntity?> GetAsync<TEntity>(int key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/> by its integer key, including related entities to the specified depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Int32}"/> type.</typeparam>
    /// <param name="key">The integer key of the entity.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the key, or null if not found.</returns>
    Task<TEntity?> GetAsync<TEntity>(int key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/> by its long key.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Int64}"/> type.</typeparam>
    /// <param name="key">The long key of the entity.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the key, or null if not found.</returns>
    Task<TEntity?> GetAsync<TEntity>(long key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/> by its long key, including related entities to the specified depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Int64}"/> type.</typeparam>
    /// <param name="key">The long key of the entity.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the key, or null if not found.</returns>
    Task<TEntity?> GetAsync<TEntity>(long key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/> by its string key.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{String}"/> type.</typeparam>
    /// <param name="key">The string key of the entity.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the key, or null if not found.</returns>
    Task<TEntity?> GetAsync<TEntity>(string key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/> by its string key, including related entities to the specified depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{String}"/> type.</typeparam>
    /// <param name="key">The string key of the entity.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the key, or null if not found.</returns>
    Task<TEntity?> GetAsync<TEntity>(string key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/> by its GUID key.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Guid}"/> type.</typeparam>
    /// <param name="key">The GUID key of the entity.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the key, or null if not found.</returns>
    Task<TEntity?> GetAsync<TEntity>(Guid key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/> by its GUID key, including related entities to the specified depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Guid}"/> type.</typeparam>
    /// <param name="key">The GUID key of the entity.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the key, or null if not found.</returns>
    Task<TEntity?> GetAsync<TEntity>(Guid key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>;

    /// <summary>
    /// Gets the first entity of type <typeparamref name="TEntity"/> matching the specified criteria.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> type.</typeparam>
    /// <param name="criteria">The query criteria.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the criteria, or null if not found.</returns>
    Task<TEntity?> GetFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets the first entity of type <typeparamref name="TEntity"/> matching the specified criteria, including related entities to the given depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> type.</typeparam>
    /// <param name="criteria">The query criteria.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the criteria, or null if not found.</returns>
    Task<TEntity?> GetFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> criteria, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets the first entity of type <typeparamref name="TEntity"/> matching the given predicate.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The predicate to filter entities.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the predicate, or null if not found.</returns>
    Task<TEntity?> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets the first or default instance of type <typeparamref name="TEntity"/> matching the given predicate, including related entities to the specified depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the predicate, or null if not found.</returns>
    Task<TEntity?> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets the first or default instance of type <typeparamref name="TEntity"/> matching the given predicate, ordered by the specified criteria.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="ordering">The ordering to apply.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the predicate, or null if not found.</returns>
    Task<TEntity?> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets the first or default instance of type <typeparamref name="TEntity"/> matching the given predicate, ordered by the specified criteria, including related entities to the given depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="ordering">The ordering to apply.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The first entity matching the predicate, or null if not found.</returns>
    Task<TEntity?> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/> matching the specified keys.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="keys">The keys uniquely identifying the entities.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The entities matching the specified keys.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/> matching the specified keys, including related entities to the given depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="keys">The keys uniquely identifying the entities.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The entities matching the specified keys.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(IEnumerable<TKey> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/> matching the specified integer keys.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The integer keys uniquely identifying the entities.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The entities matching the specified keys.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<int> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/> matching the specified integer keys, including related entities to the given depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The integer keys uniquely identifying the entities.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The entities matching the specified keys.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<int> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/> matching the specified long keys.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The long keys uniquely identifying the entities.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The entities matching the specified keys.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<long> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/> matching the specified long keys, including related entities to the given depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The long keys uniquely identifying the entities.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>The entities matching the specified keys.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<long> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified keys.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>, including related entities to the given depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified keys.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified keys.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<Guid> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>, including related entities to the given depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified keys.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<Guid> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="query">The <see cref="IQuery"/> to apply.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified query.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IQuery query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="query"/>, including related entities to the given depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="query">The <see cref="IQuery"/> to apply.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified query.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IQuery query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="criteria"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> type.</typeparam>
    /// <param name="criteria">The <see cref="IQuery{TCriteria}"/> to apply.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified criteria.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TCriteria>(IQuery<TCriteria> criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="criteria"/>, including related entities to the given depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> type.</typeparam>
    /// <param name="criteria">The <see cref="IQuery{TCriteria}"/> to apply.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified criteria.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TCriteria>(IQuery<TCriteria> criteria, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the where clause.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, including related entities to the given depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the where clause.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordered by the specified <paramref name="ordering"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="ordering">The ordering to apply.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the where clause and ordering.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordered by the specified <paramref name="ordering"/>, including related entities to the given depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="ordering">The ordering to apply.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the where clause and ordering.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, using pagination.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="pagination">The <see cref="Pagination"/> parameters.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the where clause and pagination.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, using pagination and include depth.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="pagination">The <see cref="Pagination"/> parameters.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, using pagination and ordered by <paramref name="ordering"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="pagination">The <see cref="Pagination"/> parameters.</param>
    /// <param name="ordering">The ordering to apply.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, using pagination, ordering by <paramref name="ordering"/>, and including related entities up to <paramref name="includeDepth"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="pagination">The <see cref="Pagination"/> parameters.</param>
    /// <param name="ordering">The ordering to apply.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordered by <paramref name="ordering"/> and using pagination.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="ordering">The ordering to apply.</param>
    /// <param name="pagination">The <see cref="Pagination"/> parameters.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, Pagination pagination, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordered by <paramref name="ordering"/>, using pagination, and including related entities up to <paramref name="includeDepth"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="ordering">The ordering to apply.</param>
    /// <param name="pagination">The <see cref="Pagination"/> parameters.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, Pagination pagination, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordered by the <paramref name="orderBy"/> key selector.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The key type to order by.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="orderBy">The key selector for ordering.</param>
    /// <param name="orderDirection">The <see cref="OrderingDirection"/> (ascending or descending).</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, OrderingDirection orderDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordered by the <paramref name="orderBy"/> key selector and including related entities up to <paramref name="includeDepth"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The key type to order by.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="orderBy">The key selector for ordering.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="orderingDirection">The <see cref="OrderingDirection"/> (ascending or descending).</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, int includeDepth, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordered by <paramref name="orderBy"/> and paginated.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The key type to order by.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="orderBy">The key selector for ordering.</param>
    /// <param name="pagination">The <see cref="Pagination"/> parameters.</param>
    /// <param name="orderingDirection">The <see cref="OrderingDirection"/> (ascending or descending).</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, Pagination pagination, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordered by <paramref name="orderBy"/>, paginated, and including related entities up to <paramref name="includeDepth"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The key type to order by.</typeparam>
    /// <param name="where">The where clause predicate.</param>
    /// <param name="orderBy">The key selector for ordering.</param>
    /// <param name="pagination">The <see cref="Pagination"/> parameters.</param>
    /// <param name="includeDepth">The depth of related entities to include.</param>
    /// <param name="orderingDirection">The <see cref="OrderingDirection"/> (ascending or descending).</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances matching the specified parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, Pagination pagination, int includeDepth, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Adds a single instance of <see cref="IEntityCreatable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to add.</typeparam>
    /// <param name="entity">The entity instance to add.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>The added entity.</returns>
    Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable;

    /// <summary>
    /// Adds a single instance of <see cref="IEntityCreatable"/> and reloads it to include any <see cref="IncludeAttribute"/> relations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to add.</typeparam>
    /// <typeparam name="TKey">The type of the entity's identity.</typeparam>
    /// <param name="entity">The entity instance to add.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>The added entity with related entities loaded, or null if not added.</returns>
    Task<TEntity?> AddAndGetAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Adds multiple instances of <see cref="IEntityCreatable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to add.</typeparam>
    /// <param name="entities">The collection of entities to add.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable;

    /// <summary>
    /// Bulk adds multiple instances of <see cref="IEntityCreatable"/> using Entity Framework Plus Enterprise.
    /// Read more at: https://entityframework-plus.net/download
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to add.</typeparam>
    /// <param name="entities">The collection of entities to add.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable;

    /// <summary>
    /// Updates a single instance of <see cref="IEntityUpdatable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to update.</typeparam>
    /// <param name="entity">The entity instance to update.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entity.</returns>
    Task<TEntity> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable;

    /// <summary>
    /// Updates a single instance of <see cref="IEntityUpdatable"/> and reloads it to include any <see cref="IncludeAttribute"/> relations.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to update.</typeparam>
    /// <typeparam name="TKey">The type of the entity's identity.</typeparam>
    /// <param name="entity">The entity instance to update.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>The updated entity with related entities loaded, or null if not updated.</returns>
    Task<TEntity?> UpdateAndGetAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Updates multiple instances of <see cref="IEntityUpdatable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to update.</typeparam>
    /// <param name="entities">The collection of entities to update.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable;

    /// <summary>
    /// Bulk updates multiple instances of <see cref="IEntityUpdatable"/> based on the given criteria.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to update.</typeparam>
    /// <typeparam name="TCriteria">The type of criteria used to filter entities.</typeparam>
    /// <param name="criteria">The criteria to select entities to update.</param>
    /// <param name="propertyUpdates">A dictionary of property names and their new values.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateManyAsync<TEntity, TCriteria>(TCriteria criteria, Dictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Updates multiple instances of <see cref="IEntityUpdatable"/> matching the given where clause.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to update.</typeparam>
    /// <param name="where">The predicate to select entities to update.</param>
    /// <param name="propertyUpdates">A dictionary of property names and their new values.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Dictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable;

    /// <summary>
    /// Bulk updates multiple instances of <see cref="IEntityUpdatable"/> using Entity Framework Plus Enterprise.
    /// Read more at: https://entityframework-plus.net/download
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to update.</typeparam>
    /// <param name="entities">The collection of entities to update.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable;

    /// <summary>
    /// Bulk updates multiple instances in bulk of <see cref="IEntityUpdatable"/> based on the given criteria.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to update.</typeparam>
    /// <typeparam name="TCriteria">The type of criteria used to filter entities.</typeparam>
    /// <param name="criteria">The criteria to select entities to update.</param>
    /// <param name="propertyUpdates">A dictionary of property names and their new values.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateManyBulkAsync<TEntity, TCriteria>(TCriteria criteria, Dictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Bulk updates multiple instances of <see cref="IEntityUpdatable"/> matching the given where clause.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to update.</typeparam>
    /// <param name="where">The predicate to select entities to update.</param>
    /// <param name="propertyUpdates">A dictionary of property names and their new values.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateManyBulkAsync<TEntity>(Expression<Func<TEntity, bool>> where, Dictionary<string, object> propertyUpdates, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable;

    /// <summary>
    /// Adds or updates a single instance of <see cref="IEntityCreatableAndUpdatable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to add or update.</typeparam>
    /// <param name="entity">The entity instance to add or update.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>The added or updated entity.</returns>
    Task<TEntity> AddOrUpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatableAndUpdatable;

    /// <summary>
    /// Adds or updates multiple instances of <see cref="IEntityCreatableAndUpdatable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to add or update.</typeparam>
    /// <param name="entities">The collection of entities to add or update.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddOrUpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatableAndUpdatable;

    /// <summary>
    /// Deletes a single instance of <see cref="IEntityDeletable"/> by its key.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to delete.</typeparam>
    /// <typeparam name="TKey">The type of the entity's identity.</typeparam>
    /// <param name="id">The entity's key.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync<TEntity, TKey>(TKey id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<TKey>, new()
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Deletes the instance of <typeparamref name="TEntity"/> with the specified integer ID.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{TKey}"/>.</typeparam>
    /// <param name="id">The integer ID of the entity to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous delete operation.</returns>
    Task DeleteAsync<TEntity>(int id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<int>, new();

    /// <summary>
    /// Deletes the instance of <typeparamref name="TEntity"/> with the specified long ID.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{TKey}"/>.</typeparam>
    /// <param name="id">The long ID of the entity to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous delete operation.</returns>
    Task DeleteAsync<TEntity>(long id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<long>, new();

    /// <summary>
    /// Deletes the instance of <typeparamref name="TEntity"/> with the specified string ID.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{TKey}"/>.</typeparam>
    /// <param name="id">The string ID of the entity to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous delete operation.</returns>
    Task DeleteAsync<TEntity>(string id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<string>, new();

    /// <summary>
    /// Deletes the instance of <typeparamref name="TEntity"/> with the specified Guid ID.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{TKey}"/>.</typeparam>
    /// <param name="id">The Guid ID of the entity to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous delete operation.</returns>
    Task DeleteAsync<TEntity>(Guid id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<Guid>, new();

    /// <summary>
    /// Deletes a specific instance of <see cref="IEntityDeletable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to delete.</typeparam>
    /// <param name="entity">The entity instance to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable;

    /// <summary>
    /// Deletes multiple instances of <see cref="IEntityDeletable"/> by their keys.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to delete.</typeparam>
    /// <typeparam name="TKey">The type of the entity's identity.</typeparam>
    /// <param name="ids">The collection of entity keys to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteManyAsync<TEntity, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<TKey>, new()
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Deletes all instances of <typeparamref name="TEntity"/> with the specified <see cref="Guid"/> keys.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{Guid}"/>.</typeparam>
    /// <param name="ids">The collection of <see cref="Guid"/> keys identifying the instances to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyAsync<TEntity>(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<Guid>, new();

    /// <summary>
    /// Deletes all instances of <typeparamref name="TEntity"/> with the specified integer keys.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{Integer}"/>.</typeparam>
    /// <param name="ids">The collection of integer keys identifying the instances to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyAsync<TEntity>(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<int>, new();

    /// <summary>
    /// Deletes all instances of <typeparamref name="TEntity"/> with the specified long keys.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{Long}"/>.</typeparam>
    /// <param name="ids">The collection of long keys identifying the instances to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyAsync<TEntity>(IEnumerable<long> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<long>, new();

    /// <summary>
    /// Deletes all instances of <typeparamref name="TEntity"/> with the specified string keys.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{String}"/>.</typeparam>
    /// <param name="ids">The collection of string keys identifying the instances to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyAsync<TEntity>(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<string>, new();

    /// <summary>
    /// Deletes multiple instances of <see cref="IEntityDeletable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to delete.</typeparam>
    /// <param name="entities">The collection of entities to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable;

    /// <summary>
    /// Deletes all instances of <typeparamref name="TEntity"/> that match the specified criteria.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities to delete.</typeparam>
    /// <typeparam name="TCriteria">The type of query criteria.</typeparam>
    /// <param name="criteria">The criteria used to select entities to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteManyAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Deletes all instances of <typeparamref name="TEntity"/> matching the specified filter expression.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/>.</typeparam>
    /// <param name="expression">A filter expression used to identify entities to be deleted.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous delete operation.</returns>
    Task DeleteManyAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable;

    /// <summary>
    /// Bulk deletes all instances of <typeparamref name="TEntity"/> with the specified keys.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{TKey}"/>.</typeparam>
    /// <typeparam name="TKey">The type of the entity key.</typeparam>
    /// <param name="ids">The collection of entity keys identifying the instances to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyBulkAsync<TEntity, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<TKey>, new()
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Bulk deletes all instances of <typeparamref name="TEntity"/> with the specified <see cref="Guid"/> keys.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{Guid}"/>.</typeparam>
    /// <param name="ids">The collection of <see cref="Guid"/> keys identifying the instances to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyBulkAsync<TEntity>(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<Guid>, new();

    /// <summary>
    /// Bulk deletes all instances of <typeparamref name="TEntity"/> with the specified integer keys.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{Integer}"/>.</typeparam>
    /// <param name="ids">The collection of integer keys identifying the instances to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyBulkAsync<TEntity>(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<int>, new();

    /// <summary>
    /// Bulk deletes all instances of <typeparamref name="TEntity"/> with the specified long keys.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{Long}"/>.</typeparam>
    /// <param name="ids">The collection of long keys identifying the instances to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyBulkAsync<TEntity>(IEnumerable<long> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<long>, new();

    /// <summary>
    /// Bulk deletes all instances of <typeparamref name="TEntity"/> with the specified string keys.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/> and <see cref="IEntityIdentity{String}"/>.</typeparam>
    /// <param name="ids">The collection of string keys identifying the instances to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyBulkAsync<TEntity>(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<string>, new();

    /// <summary>
    /// Bulk deletes all specified instances of <typeparamref name="TEntity"/>.
    /// This operation requires Entity Framework Plus Enterprise.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/>.</typeparam>
    /// <param name="entities">The collection of entity instances to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable;

    /// <summary>
    /// Bulk deletes all instances of <typeparamref name="TEntity"/> matching the specified query criteria.
    /// This operation requires Entity Framework Plus Enterprise.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/>.</typeparam>
    /// <typeparam name="TCriteria">The query criteria type.</typeparam>
    /// <param name="criteria">The criteria used to identify entities to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyBulkAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Bulk deletes all instances of <typeparamref name="TEntity"/> matching the specified filter expression.
    /// This operation requires Entity Framework Plus Enterprise.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to delete. Must implement <see cref="IEntityDeletable"/>.</typeparam>
    /// <param name="expression">A filter expression used to identify entities to delete.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous bulk delete operation.</returns>
    Task DeleteManyBulkAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable;

    /// <summary>
    /// Returns the total number of <typeparamref name="TEntity"/> instances matching the specified query criteria.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to count. Must implement <see cref="IEntity"/>.</typeparam>
    /// <typeparam name="TCriteria">The query criteria type.</typeparam>
    /// <param name="criteria">The criteria used to filter the entities.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> containing the number of matching entities.</returns>
    Task<long> CountAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Returns the total number of <typeparamref name="TEntity"/> instances matching the specified filter expression.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to count. Must implement <see cref="IEntity"/>.</typeparam>
    /// <param name="expression">A filter expression used to identify entities to count.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> containing the number of matching entities.</returns>
    Task<long> CountAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Calculates the sum of the specified numeric expression for entities matching the given filter.
    /// </summary>
    /// <typeparam name="TEntity">The entity type. Must implement <see cref="IEntity"/>.</typeparam>
    /// <param name="whereExpr">A filter expression used to identify entities to include in the calculation.</param>
    /// <param name="sumExpr">An expression selecting the numeric value to sum.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> containing the calculated sum.</returns>
    Task<decimal> SumAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpr, Expression<Func<TEntity, decimal>> sumExpr, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Calculates the average of the specified numeric expression for entities matching the given filter.
    /// </summary>
    /// <typeparam name="TEntity">The entity type. Must implement <see cref="IEntity"/>.</typeparam>
    /// <param name="whereExpr">A filter expression used to identify entities to include in the calculation.</param>
    /// <param name="avgExpr">An expression selecting the numeric value to average.</param>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> containing the calculated average.</returns>
    Task<decimal> AverageAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpr, Expression<Func<TEntity, decimal>> avgExpr, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Persists all pending changes to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken">Optional <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous save operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}