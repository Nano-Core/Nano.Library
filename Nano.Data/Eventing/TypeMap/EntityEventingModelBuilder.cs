using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Nano.Data.Eventing.TypeMap;

internal static class EntityEventingModelBuilder
{
    internal static EntityEventingModel Build(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var entityMap = EntityEventingTypeMapCache.GetOrCreate(dbContext);

        var model = new EntityEventingModel();

        foreach (var (rootType, metadata) in entityMap)
        {
            model.EntityMap[rootType] = metadata;

            foreach (var path in metadata.Properties)
            {
                model.Accessors[(rootType, path)] = CompileAccessor(rootType, path);
            }

            var rootEntityType = dbContext.Model
                .FindEntityType(rootType)!;

            foreach (var (targetType, paths) in metadata.ReverseDependencies)
            {
                foreach (var path in paths)
                {
                    var rootNavigation = path
                        .Split('.')[0];

                    var navigation = rootEntityType
                        .FindNavigation(rootNavigation);

                    if (navigation == null)
                    {
                        continue;
                    }

                    var foreignKey = navigation.ForeignKey;

                    if (!model.ReverseBindings.TryGetValue(rootType, out var modelReverseBinding))
                    {
                        modelReverseBinding = [];
                        model.ReverseBindings[rootType] = modelReverseBinding;
                    }

                    if (!modelReverseBinding.TryGetValue(targetType, out var navigationBindings))
                    {
                        navigationBindings = [];
                        modelReverseBinding[targetType] = navigationBindings;
                    }

                    navigationBindings
                        .Add(new ReverseNavigationBinding
                        {
                            RootType = rootType,
                            ChangedType = targetType,
                            NavigationName = rootNavigation,
                            ForeignKey = foreignKey,
                            IsOnDependent = foreignKey.DeclaringEntityType.ClrType == rootType
                        });
                }
            }
        }

        return model;
    }


    private static Func<object, object?> CompileAccessor(Type rootType, string path)
    {
        ArgumentNullException.ThrowIfNull(rootType);
        ArgumentNullException.ThrowIfNull(path);

        var parameter = Expression.Parameter(typeof(object), "obj");

        Expression current = Expression.Convert(parameter, rootType);

        foreach (var part in path.Split('.'))
        {
            var property = current.Type
                .GetProperty(part) ?? throw new InvalidOperationException($"Property '{part}' not found on '{current.Type.Name}'");

            var propertyExpression = Expression.Property(current, property);
            var equalExpression = Expression.Equal(current, Expression.Constant(null));
            var defaultExpression = Expression.Default(property.PropertyType);

            current = Expression.Condition(equalExpression, defaultExpression, propertyExpression);
        }

        var body = Expression.Convert(current, typeof(object));

        return Expression.Lambda<Func<object, object?>>(body, parameter).Compile();
    }
}