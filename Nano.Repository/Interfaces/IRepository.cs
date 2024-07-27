using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Enums;
using DynamicExpression.Interfaces;
using Microsoft.EntityFrameworkCore;
using Nano.Models.Attributes;
using Nano.Models.Interfaces;

namespace Nano.Repository.Interfaces;

/// <summary>
/// (Base) Interface for a repository.
/// Defines methods for the most rudamentory operations (get, criteria2, add, update, delete) on instances of <see cref="IEntity"/>.
/// <see cref="IEntityCreatable"/>, <see cref="IEntityUpdatable"/> and <see cref="IEntityDeletable"/>).
/// </summary>
public interface IRepository : IDisposable
{
    /// <summary>
    /// Get Context.
    /// </summary>
    /// <returns></returns>
    DbContext GetContext();

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> mathcing the type of <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type</typeparam>
    /// <returns>The <see cref="DbSet{TEntity}"/>.</returns>
    DbSet<TEntity> GetEntitySet<TEntity>()
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="key"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The identity type.</typeparam>
    /// <param name="key">The <typeparamref name="TKey"/> type, uniquely identify the <see cref="IEntity"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
    Task<TEntity> GetAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="key"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The identity type.</typeparam>
    /// <param name="key">The <typeparamref name="TKey"/> type, uniquely identify the <see cref="IEntity"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
    Task<TEntity> GetAsync<TEntity, TKey>(TKey key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="key"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Int32}"/> type.</typeparam>
    /// <param name="key">The unique key.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
    Task<TEntity> GetAsync<TEntity>(int key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="key"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Int32}"/> type.</typeparam>
    /// <param name="key">The unique key.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
    Task<TEntity> GetAsync<TEntity>(int key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="key"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Int64}"/> type.</typeparam>
    /// <param name="key">The unique key.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
    Task<TEntity> GetAsync<TEntity>(long key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="key"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Int64}"/> type.</typeparam>
    /// <param name="key">The unique key.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
    Task<TEntity> GetAsync<TEntity>(long key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="key"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{String}"/> type.</typeparam>
    /// <param name="key">The unique key.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
    Task<TEntity> GetAsync<TEntity>(string key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="key"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{String}"/> type.</typeparam>
    /// <param name="key">The unique key.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
    Task<TEntity> GetAsync<TEntity>(string key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="key"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Guid}"/> type.</typeparam>
    /// <param name="key">The unique key.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
    Task<TEntity> GetAsync<TEntity>(Guid key, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>;

    /// <summary>
    /// Gets an instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="key"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityIdentity{Guid}"/> type.</typeparam>
    /// <param name="key">The unique key.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The first instance, matching the passed <paramref name="key"/>.</returns>
    Task<TEntity> GetAsync<TEntity>(Guid key, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>;

    /// <summary>
    /// Gets the first instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="criteria"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="criteria">The <see cref="IQuery{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="criteria"/>.</returns>
    Task<TEntity> GetFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets the first instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="criteria"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="criteria">The <see cref="IQuery{TCriteria}"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="criteria"/>.</returns>
    Task<TEntity> GetFirstAsync<TEntity, TCriteria>(IQuery<TCriteria> criteria, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets the fist or default instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause.</param>
    /// <param name="ordering">The order by clause</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The fist instance matching the passed <paramref name="where"/> clause.</returns>
    Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets the fist or default instance of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> of the <see cref="IEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause.</param>
    /// <param name="ordering">The order by clause</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The fist instance matching the passed <paramref name="where"/> clause.</returns>
    Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="keys"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(IEnumerable<TKey> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="keys"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(IEnumerable<TKey> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<TKey>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="keys"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<int> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="keys"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<int> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<int>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="keys"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<long> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="keys"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<long> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<long>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="keys"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="keys"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<string>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="keys"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<Guid> keys, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="keys"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="keys">The keys uniquely identifying the <see cref="IEntity"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="keys"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IEnumerable<Guid> keys, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityIdentity<Guid>;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="query">The <see cref="IQuery"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="query"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IQuery query, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="query"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="query">The <see cref="IQuery"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="query"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(IQuery query, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="criteria"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="criteria">The <see cref="IQuery{TCriteria}"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="criteria"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TCriteria>(IQuery<TCriteria> criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="criteria"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/>.</typeparam>
    /// <param name="criteria">The <see cref="IQuery{TCriteria}"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed <paramref name="criteria"/>.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TCriteria>(IQuery<TCriteria> criteria, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordering by the passed <paramref name="ordering"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="ordering">The order by clause</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordering by the passed <paramref name="ordering"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="ordering">The order by clause</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="pagination">The <see cref="Pagination"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="pagination">The <see cref="Pagination"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordering by the passed <paramref name="ordering"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="pagination">The <see cref="Pagination"/>.</param>
    /// <param name="ordering">The order by clause</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, Ordering ordering, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordering by the passed <paramref name="ordering"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="pagination">The <see cref="Pagination"/>.</param>
    /// <param name="ordering">The order by clause</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Pagination pagination, Ordering ordering, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordering by the passed <paramref name="ordering"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="ordering">The order by clause</param>
    /// <param name="pagination">The <see cref="Pagination"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, Pagination pagination, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets all instances of type <typeparamref name="TEntity"/>, matching the passed <paramref name="where"/> clause, ordering by the passed <paramref name="ordering"/>
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="ordering">The order by clause</param>
    /// <param name="pagination">The <see cref="Pagination"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity>(Expression<Func<TEntity, bool>> where, Ordering ordering, Pagination pagination, int includeDepth, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Get Many Async.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The order by key type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="orderBy">The order by expression.</param>
    /// <param name="orderingDirection">The <see cref="OrderingDirection"/>.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Get Many Async.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The order by key type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="orderBy">The order by expression.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="orderingDirection">The <see cref="OrderingDirection"/>.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, int includeDepth, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Get Many Async.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The order by key type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="orderBy">The order by expression.</param>
    /// <param name="pagination">The <see cref="Pagination"/>.</param>
    /// <param name="orderingDirection">The <see cref="OrderingDirection"/>.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, Pagination pagination, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Get Many Async.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TKey">The order by key type.</typeparam>
    /// <param name="where">The where clause</param>
    /// <param name="orderBy">The order by expression.</param>
    /// <param name="pagination">The <see cref="Pagination"/>.</param>
    /// <param name="includeDepth">The include depth.</param>
    /// <param name="orderingDirection">The <see cref="OrderingDirection"/>.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The instances, matching the passed parameters.</returns>
    Task<IEnumerable<TEntity>> GetManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> where, Func<TEntity, TKey> orderBy, Pagination pagination, int includeDepth, OrderingDirection orderingDirection = OrderingDirection.Asc, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Adds the instance of the passed <see cref="IEntityCreatable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityCreatable"/> type.</typeparam>
    /// <param name="entity">The instance of <see cref="IEntityCreatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task{TEntity}"/>.</returns>
    Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable;

    /// <summary>
    /// Adds the instance of the passed <see cref="IEntityCreatable"/>.
    /// After adding the <see cref="IEntityCreatable"/> it will be reloaded, to get <see cref="IncludeAttribute"/> relations.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityCreatable"/> type.</typeparam>
    /// <typeparam name="TKey">The identity type.</typeparam>
    /// <param name="entity">The instance of <see cref="IEntityCreatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task{TEntity}"/>.</returns>
    Task<TEntity> AddAndGetAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Adds all instances of the passed <see cref="IEntityCreatable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityCreatable"/> type.</typeparam>
    /// <param name="entities">The instances of <see cref="IEntityCreatable"/>'s.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The added entities.</returns>
    Task AddManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable;

    /// <summary>
    /// Bulk adds all instances of the passed <see cref="IEntityCreatable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityCreatable"/> type.</typeparam>
    /// <param name="entities">The instances of <see cref="IEntityCreatable"/>'s.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The added entities.</returns>
    Task AddManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatable;

    /// <summary>
    /// Updates the instance of the passed <see cref="IEntityUpdatable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityUpdatable"/> type.</typeparam>
    /// <param name="entity">The instance of <see cref="IEntityUpdatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task{TEntity}"/>.</returns>
    Task<TEntity> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable;

    /// <summary>
    /// Updates the instance of the passed <see cref="IEntityUpdatable"/>.
    /// After updating the <see cref="IEntityUpdatable"/> it will be reloaded, to get <see cref="IncludeAttribute"/> relations.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityUpdatable"/> type.</typeparam>
    /// <typeparam name="TKey">The identity type.</typeparam>
    /// <param name="entity">The instance of <see cref="IEntityUpdatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task{TEntity}"/>.</returns>
    Task<TEntity> UpdateAndGetAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable, IEntityIdentity<TKey>
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Updates all instances of the passed <see cref="IEntityUpdatable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityUpdatable"/> type.</typeparam>
    /// <param name="entities">The instances of <see cref="IEntityUpdatable"/>'s.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The updated entities.</returns>
    Task UpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable;

    /// <summary>
    /// Bulk updates all instances of the passed <see cref="IEntityUpdatable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityUpdatable"/> type.</typeparam>
    /// <param name="entities">The instances of <see cref="IEntityUpdatable"/>'s.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The updated entities.</returns>
    Task UpdateManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityUpdatable;

    /// <summary>
    /// Adds or Updates the instance of the passed <see cref="IEntityCreatableAndUpdatable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityCreatableAndUpdatable"/> type.</typeparam>
    /// <param name="entity">The instance of <see cref="IEntityCreatableAndUpdatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task{TEntity}"/>.</returns>
    Task<TEntity> AddOrUpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatableAndUpdatable;

    /// <summary>
    /// Adds or Updates the instances of the passed <see cref="IEntityCreatableAndUpdatable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityCreatableAndUpdatable"/> type.</typeparam>
    /// <param name="entities">The instances of <see cref="IEntityCreatableAndUpdatable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    Task AddOrUpdateManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityCreatableAndUpdatable;

    /// <summary>
    /// Deletes the instance of the passed id.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="Task"/> returning 'void'.</returns>
    Task DeleteAsync<TEntity, TKey>(TKey id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<TKey>, new()
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Deletes the instance of the passed id.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="Task"/> returning 'void'.</returns>
    Task DeleteAsync<TEntity>(int id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<int>, new();

    /// <summary>
    /// Deletes the instance of the passed id.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="Task"/> returning 'void'.</returns>
    Task DeleteAsync<TEntity>(long id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<long>, new();

    /// <summary>
    /// Deletes the instance of the passed id.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="Task"/> returning 'void'.</returns>
    Task DeleteAsync<TEntity>(string id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<string>, new();

    /// <summary>
    /// Deletes the instance of the passed id.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="id">The id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="Task"/> returning 'void'.</returns>
    Task DeleteAsync<TEntity>(Guid id, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<Guid>, new();

    /// <summary>
    /// Deletes the instance of the passed <see cref="IEntityDeletable"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="entity">The instance of <see cref="IEntityDeletable"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="Task"/> returning 'void'.</returns>
    Task DeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable;

    /// <summary>
    /// Deletes all instances of the passed <see cref="IEntityDeletable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="ids">The id's.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    Task DeleteManyAsync<TEntity, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<TKey>, new()
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Deletes all instances of the passed <see cref="IEntityDeletable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="entities">The instances of <see cref="IEntityDeletable"/>'s.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    Task DeleteManyAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable;

    /// <summary>
    /// Deletes all instances of <typeparamref name="TEntity"/>, matching the criterias of the passed <paramref name="criteria"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> type.</typeparam>
    /// <param name="criteria">The <see cref="IQueryCriteria"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A <see cref="Task"/> (void).</returns>
    Task DeleteManyAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Deletes all instances matching the passed criteria <see cref="Expression"/>.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityUpdatable"/> type.</typeparam>
    /// <param name="expression">The <see cref="Expression"/> to evaulate entities to be deleted.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    Task DeleteManyAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable;

    /// <summary>
    /// Bulk deletes all instances of the  passed id's matching <see cref="IEntityDeletable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="ids">The id's.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    Task DeleteManyBulkAsync<TEntity, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<TKey>, new()
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Bulk deletes all instances of the  passed id's matching <see cref="IEntityDeletable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="ids">The id's.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    Task DeleteManyBulkAsync<TEntity>(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<Guid>, new();

    /// <summary>
    /// Bulk deletes all instances of the  passed id's matching <see cref="IEntityDeletable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="ids">The id's.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    Task DeleteManyBulkAsync<TEntity>(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<int>, new();

    /// <summary>
    /// Bulk deletes all instances of the  passed id's matching <see cref="IEntityDeletable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="ids">The id's.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    Task DeleteManyBulkAsync<TEntity>(IEnumerable<long> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<long>, new();

    /// <summary>
    /// Bulk deletes all instances of the  passed id's matching <see cref="IEntityDeletable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="ids">The id's.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    Task DeleteManyBulkAsync<TEntity>(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable, IEntityIdentity<string>, new();

    /// <summary>
    /// Bulk deletes all instances of the passed <see cref="IEntityDeletable"/>'s.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="entities">The instances of <see cref="IEntityDeletable"/>'s.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The <see cref="Task"/> (void).</returns>
    Task DeleteManyBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable;

    /// <summary>
    /// Delete Many Bulk Async.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> type.</typeparam>
    /// <param name="criteria">The <see cref="IQueryCriteria"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>Void.</returns>
    Task DeleteManyBulkAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Delete Many Bulk Async.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntityDeletable"/> type.</typeparam>
    /// <param name="expression">The <see cref="Expression"/> to evaulate entities to be included in the count.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>Void.</returns>
    Task DeleteManyBulkAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : class, IEntityDeletable;

    /// <summary>
    /// Returns the count (long) of elements satisfying the passed criteria.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <typeparam name="TCriteria">The <see cref="IQueryCriteria"/> type.</typeparam>
    /// <param name="criteria">The <see cref="IQueryCriteria"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The number of elements.</returns>
    Task<long> CountAsync<TEntity, TCriteria>(TCriteria criteria, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
        where TCriteria : class, IQueryCriteria, new();

    /// <summary>
    /// Returns the count (long) of elements satisfying the passed expression.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="expression">The <see cref="Expression"/> to evaulate entities to be included in the count.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The number of elements.</returns>
    Task<long> CountAsync<TEntity>(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Returns the sum of the passed sum expression, conditioned by passed where expression.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="whereExpr">The <see cref="Expression"/> to evaulate entities to be included in the count.</param>
    /// <param name="sumExpr">The property expression of what to summarize.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The sum.</returns>
    Task<decimal> SumAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpr, Expression<Func<TEntity, decimal>> sumExpr, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Returns the average of the passed sum expression, conditioned by passed where expression.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/> type.</typeparam>
    /// <param name="whereExpr">The <see cref="Expression"/> to evaulate entities to be included in the count.</param>
    /// <param name="avgExpr">THe property expression of what to average.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>The average.</returns>
    Task<decimal> AverageAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpr, Expression<Func<TEntity, decimal>> avgExpr, CancellationToken cancellationToken = default)
        where TEntity : class, IEntity;

    /// <summary>
    /// Save Changes.
    /// Commits the changes to the database.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>Task (Void).</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}