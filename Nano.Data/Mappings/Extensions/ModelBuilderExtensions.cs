using Microsoft.EntityFrameworkCore;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Models.Abstractions;
using System;
using System.Linq;
using System.Reflection;
using Nano.Data.Abstractions.Models;

namespace Nano.Data.Mappings.Extensions;

internal static class ModelBuilderExtensions
{
    internal static ModelBuilder AddMapping<TEntity, TMapping>(this ModelBuilder builder)
        where TEntity : BaseEntity
        where TMapping : BaseEntityMapping<TEntity>, new()
    {
        ArgumentNullException.ThrowIfNull(builder);

        var mapping = new TMapping();

        mapping
            .Configure(builder.Entity<TEntity>());

        return builder
            .UpdateSoftDeleteUniuqeIndexes<TEntity>()
            .UpdateUniuqeIndexes<TEntity>();
    }

    internal static ModelBuilder MapEntities(this ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // BUG: We find Nano types "Default", we should make those abstract, all of them
        // Check all Default naming, e.g. DefaultRepository should we renamed to just Repository, because it should not be abstract, like AuthController.
        // We will accidently map Audit and Identity, when that should be conditional outside this method

        var entityTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x is { IsAbstract: false, IsInterface: false })
            .Where(x => x.GetInterfaces()
                .Any(y => y == typeof(IEntity)))
            .ToList();

        // AuditEntry`1
        // AuditEntryProperty`1
        // DefaultEntity
        // DefaultEntity`1
        // DefaultEntityReadOnly
        // DefaultEntityUser
        // DefaultEntityUser`1
        // IdentityApiKey`1
        // IdentityApiKeyCreated`1
        // IdentityUserChangeData`1
        // IdentityUserEx`1
        // IdentityUserRefreshToken`1



        foreach (var entityType in entityTypes)
        {
            Console.WriteLine(entityType.Name);
        }

        var assembliesWithMappings = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetTypes().Any(t =>
                t is { IsAbstract: false, IsInterface: false } &&
                t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))))
            .ToArray();

        foreach (var assembly in assembliesWithMappings)
        {
            builder
                .ApplyConfigurationsFromAssembly(assembly);
        }


        //var entityTypes = AppDomain.CurrentDomain
        //    .GetAssemblies()
        //    .SelectMany(x => x.GetTypes())
        //    .Where(x => x is { IsAbstract: false, IsInterface: false })
        //    .Where(x => x.GetInterfaces()
        //        .Any(y => y == typeof(IEntity)));

        //foreach (var entityType in entityTypes)
        //{
        //    var mappingType = AppDomain.CurrentDomain
        //        .GetAssemblies()
        //        .SelectMany(x => x.GetTypes())
        //        .FirstOrDefault(x =>
        //            x is { IsAbstract: false, IsInterface: false } &&
        //            typeof(IEntityTypeConfiguration<>).MakeGenericType(entityType).IsAssignableFrom(x)); 

        //    if (mappingType == null)
        //    {
        //        throw new NullReferenceException(nameof(mappingType));
        //    }

        //    const string METHOD_NAME = nameof(ModelBuilderExtensions.AddMapping);

        //    var addMappingMethod = typeof(ModelBuilderExtensions)
        //        .GetMethods(BindingFlags.Static | BindingFlags.Public)
        //        .FirstOrDefault(x => x is { Name: METHOD_NAME, IsGenericMethodDefinition: true } && x.GetGenericArguments().Length == 2);

        //    var genericMethod = addMappingMethod?
        //        .MakeGenericMethod(entityType, mappingType);

        //    genericMethod?
        //        .Invoke(null, [builder]);
        //}

        return builder;
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