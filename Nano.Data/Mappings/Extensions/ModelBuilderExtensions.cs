using Microsoft.EntityFrameworkCore;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Linq;
using System.Reflection;
using Nano.Common.Helpers;
using Nano.Data.Abstractions.Models;

namespace Nano.Data.Mappings.Extensions;

internal static class ModelBuilderExtensions
{
    internal static ModelBuilder MapEntities<TIdentity>(this ModelBuilder builder)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(builder);

        var addMappingMethod = typeof(ModelBuilderExtensions)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(x =>
                x is { Name: nameof(ModelBuilderExtensions.AddMapping), IsGenericMethodDefinition: true } &&
                x.GetGenericArguments().Length == 3 &&
                x.GetParameters().Length == 1);

        var mappingTypes = TypesHelper
            .GetAllTypes()
            .Where(x =>
                x is { IsAbstract: false, IsInterface: false, IsGenericType: false } &&
                x.IsTypeOf(typeof(BaseMapping<>)))
            .ToArray();

        foreach (var mappingType in mappingTypes)
        {
            var baseType = mappingType.BaseType!;
            var genericArgs = baseType.GetGenericArguments();
            var entityType = genericArgs[0];
            var identityType = typeof(TIdentity);

            addMappingMethod
                .MakeGenericMethod(entityType, identityType, mappingType)
                .Invoke(null, [builder]);
        }

        return builder;
    }

    internal static ModelBuilder AddMapping<TEntity, TIdentity, TMapping>(this ModelBuilder builder)
        where TEntity : BaseEntityIdentity<TIdentity>
        where TIdentity : IEquatable<TIdentity>
        where TMapping : BaseEntityIdentityMapping<TEntity, TIdentity>, new()
    {
        ArgumentNullException.ThrowIfNull(builder);

        var mapping = new TMapping();

        mapping
            .Configure(builder.Entity<TEntity>());

        return builder
            .UpdateSoftDeleteUniuqeIndexes<TEntity>()
            .UpdateUniuqeIndexes<TEntity>();
    }

    private static ModelBuilder UpdateUniuqeIndexes<TEntity>(this ModelBuilder builder)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(builder);

        var entity = builder
            .Entity<TEntity>();

        var indices = entity.Metadata
            .GetIndexes()
            .Where(x => x.IsUnique)
            .ToArray();

        foreach (var index in indices)
        {
            entity.Metadata
                .RemoveIndex(index.Properties);

            var columns = index.Properties
                .Select(y => y.Name)
                .ToArray();

            var tableName = index.DeclaringEntityType
                .GetTableName();

            var columnNames = columns
                .Aggregate("", (y, z) => y + $"_{z}");

            var indexName = $"UX_{tableName}{columnNames}";

            entity
                .HasIndex(columns)
                .HasDatabaseName(indexName)
                .IsUnique();
        }

        return builder;
    }
    private static ModelBuilder UpdateSoftDeleteUniuqeIndexes<TEntity>(this ModelBuilder builder)
        where TEntity : class, IEntity
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (!typeof(TEntity).IsTypeOf(typeof(IEntityDeletableSoft)))
        {
            return builder;
        }

        var entity = builder
            .Entity<TEntity>();

        var indices = entity.Metadata
            .GetIndexes()
            .Where(x =>
                x.IsUnique &&
                x.Properties.All(y => y.Name != nameof(BaseEntity.IsDeleted) && !y.IsKey()))
            .ToArray();

        foreach (var index in indices)
        {
            entity.Metadata
                .RemoveIndex(index.Properties);

            var columns = index.Properties
                .Select(y => y.Name)
                .Union([nameof(BaseEntity.IsDeleted)])
                .ToArray();

            entity
                .HasIndex(columns)
                .IsUnique();
        }

        return builder;
    }
}