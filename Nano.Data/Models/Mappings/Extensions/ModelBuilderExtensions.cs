using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nano.Models.Extensions;
using Nano.Models.Interfaces;

namespace Nano.Data.Models.Mappings.Extensions;

/// <summary>
/// Model Builder Extensions.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds a mapping for the type <typeparamref name="TEntity"/> using the <typeparamref name="TMapping"/> implementation.
    /// </summary>
    /// <typeparam name="TEntity">The <see cref="IEntity"/>.</typeparam>
    /// <typeparam name="TMapping">The <see cref="BaseEntityMapping{TEntity}"/>.</typeparam>
    /// <param name="builder">The <see cref="ModelBuilder"/>.</param>
    /// <returns>The <see cref="ModelBuilder"/>.</returns>
    public static ModelBuilder AddMapping<TEntity, TMapping>(this ModelBuilder builder)
        where TEntity : class, IEntity
        where TMapping : BaseEntityMapping<TEntity>, new()
    {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

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
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

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
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        var isDeletableSoft = typeof(TEntity).IsTypeOf(typeof(IEntityDeletableSoft));

        if (isDeletableSoft)
        {
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
        }

        return builder;
    }
}