using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Mappings.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ModelBuilder"/> to register entity mappings.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds a mapping for <typeparamref name="TEntity"/> using <typeparamref name="TMapping"/> implementation.
    /// Also updates unique and soft-delete-aware indexes.
    /// </summary>
    /// <typeparam name="TEntity">The entity type implementing <see cref="IEntity"/>.</typeparam>
    /// <typeparam name="TMapping">The mapping type inheriting <see cref="BaseEntityMapping{TEntity}"/>.</typeparam>
    /// <param name="builder">The EF Core <see cref="ModelBuilder"/>.</param>
    /// <returns>The same <see cref="ModelBuilder"/> instance for chaining.</returns>
    public static ModelBuilder AddMapping<TEntity, TMapping>(this ModelBuilder builder)
        where TEntity : class, IEntity
        where TMapping : BaseEntityMapping<TEntity>, new()
    {
        ArgumentNullException.ThrowIfNull(builder);

        var mapping = new TMapping();

        mapping
            .Map(builder.Entity<TEntity>());

        return builder
            .UpdateSoftDeleteUniuqeIndexes<TEntity>()
            .UpdateUniuqeIndexes<TEntity>();
    }


    private static ModelBuilder UpdateUniuqeIndexes<TEntity>(this ModelBuilder builder)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(builder);

        var entity = builder.Entity<TEntity>();

        entity.Metadata
            .GetIndexes()
            .Where(x => x.IsUnique)
            .ToList()
            .ForEach(x =>
            {
                entity.Metadata
                    .RemoveIndex(x.Properties);

                var columns = x.Properties
                    .Select(y => y.Name)
                    .ToArray();

                var tableName = x.DeclaringEntityType
                    .GetTableName();

                var columnNames = columns
                    .Aggregate("", (y, z) => y + $"_{z}");

                var indexName = $"UX_{tableName}{columnNames}";

                entity
                    .HasIndex(columns)
                    .HasDatabaseName(indexName)
                    .IsUnique();
            });

        return builder;
    }
    private static ModelBuilder UpdateSoftDeleteUniuqeIndexes<TEntity>(this ModelBuilder builder)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(builder);

        var isDeletableSoft = typeof(TEntity).IsTypeOf(typeof(IEntityDeletableSoft));

        if (!isDeletableSoft)
        {
            return builder;
        }

        var entity = builder.Entity<TEntity>();

        entity.Metadata
            .GetIndexes()
            .Where(x =>
                x.IsUnique &&
                x.Properties.All(y => y.Name != "IsDeleted" && !y.IsKey()))
            .ToList()
            .ForEach(x =>
            {
                entity.Metadata
                    .RemoveIndex(x.Properties);

                var columns = x.Properties
                    .Select(y => y.Name)
                    .Union(["IsDeleted"])
                    .ToArray();

                entity
                    .HasIndex(columns)
                    .IsUnique();
            });

        return builder;
    }
}