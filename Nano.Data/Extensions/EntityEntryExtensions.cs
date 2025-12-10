using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;
using Nano.Data.Eventing.Annotations;
using NetTopologySuite.Geometries;

namespace Nano.Data.Extensions;

internal static class EntityEntryExtensions
{
    internal static EntityEvent GetEntityEvent(this EntityEntry entityEntry)
    {
        if (entityEntry == null)
            throw new ArgumentNullException(nameof(entityEntry));

        var type = entityEntry
            .GetPublishType();

        var entityEvent = entityEntry
            .GetEntityEvent(type);

        var propertyNames = entityEntry
            .GetPublishProperties();

        switch (entityEntry.State)
        {
            case EntityState.Deleted:
                return entityEvent;

            case EntityState.Modified:
            {
                var hasChanged = entityEntry
                    .HasPublishPropertiesChanged(propertyNames);

                if (!hasChanged)
                {
                    return null;
                }

                break;
            }

            case EntityState.Detached:
            case EntityState.Unchanged:
            case EntityState.Added:
                return null;

            default:
                throw new ArgumentOutOfRangeException(nameof(entityEntry.State), entityEntry.State, $"The {entityEntry.State} is out of range.");
        }

        entityEvent.Data = entityEntry
            .GetEntityEventData(propertyNames);

        return entityEvent;
    }

    internal static EntityEvent GetEntityEvent(this EntityEntry entityEntry, Type type)
    {
        if (entityEntry == null)
            throw new ArgumentNullException(nameof(entityEntry));

        if (type == null)
            throw new ArgumentNullException(nameof(type));

        var state = entityEntry.State
            .ToString();

        var typeName = type.Name
            .Replace("Proxy", string.Empty);

        return entityEntry.Entity switch
        {
            IEntityIdentity<int> @int => new EntityEvent(@int.Id, typeName, state),
            IEntityIdentity<long> @long => new EntityEvent(@long.Id, typeName, state),
            IEntityIdentity<string> @string => new EntityEvent(@string.Id, typeName, state),
            IEntityIdentity<Guid> guid => new EntityEvent(guid.Id, typeName, state),
            IEntityIdentity<dynamic> dynamic => new EntityEvent(dynamic.Id, typeName, state),
            _ => null
        };
    }

    internal static IDictionary<string, object> GetEntityEventData(this EntityEntry entityEntry, params string[] publishProperties)
    {
        if (entityEntry == null)
            throw new ArgumentNullException(nameof(entityEntry));

        var entityEventData = new Dictionary<string, object>();
        foreach (var propertyName in publishProperties)
        {
            string name;
            int indexOfDot;

            var value = entityEntry.Entity;
            var propertyNameTemp = propertyName;

            do
            {
                indexOfDot = propertyNameTemp
                    .IndexOf('.');

                name = indexOfDot > -1
                    ? propertyNameTemp[..indexOfDot]
                    : propertyNameTemp;

                propertyNameTemp = indexOfDot > -1
                    ? propertyNameTemp[(indexOfDot + 1)..]
                    : propertyNameTemp;

                var property = value?
                    .GetType()
                    .GetProperty(name);

                value = property?
                    .GetValue(value);
            } while (indexOfDot > -1);

            entityEventData
                .Add(name, value);
        }

        return entityEventData;
    }

    internal static Type GetPublishType(this EntityEntry entityEntry)
    {
        if (entityEntry == null)
            throw new ArgumentNullException(nameof(entityEntry));

        var type = entityEntry.Entity
            .GetType();

        var isAttributeDirectlyApplied = false;

        while (!isAttributeDirectlyApplied)
        {
            if (type == null)
            {
                break;
            }

            isAttributeDirectlyApplied = Attribute.IsDefined(type, typeof(PublishAttribute), false);

            if (!isAttributeDirectlyApplied)
            {
                type = type.BaseType;
            }
        }

        return type;
    }

    internal static string[] GetPublishProperties(this EntityEntry entityEntry)
    {
        if (entityEntry == null)
            throw new ArgumentNullException(nameof(entityEntry));

        var type = entityEntry.Entity
            .GetType();

        var propertyNames = new List<string>();
        while (type is { IsAbstract: false } && type.IsTypeOf(typeof(IEntity)))
        {
            var attribute = (PublishAttribute)type
                .GetCustomAttributes(typeof(PublishAttribute))
                .FirstOrDefault();

            if (attribute == null)
            {
                type = type.BaseType;
                continue;
            }

            propertyNames
                .AddRange(attribute.PropertyNames);

            type = type.BaseType;
        }

        propertyNames
            .Add(nameof(BaseEntity<object>.CreatedAt));

        return propertyNames
            .Distinct()
            .ToArray();
    }

    internal static bool HasPublishPropertiesChanged(this EntityEntry entityEntry, string[] publishProperties)
    {
        if (entityEntry == null)
            throw new ArgumentNullException(nameof(entityEntry));

        foreach (var propertyName in publishProperties)
        {
            var propertyNameTemp = propertyName;
            var nestedEntityEntry = entityEntry;

            var indexOfDot = propertyNameTemp
                .IndexOf('.');

            while (indexOfDot > -1)
            {
                var name = propertyNameTemp[..indexOfDot];
                propertyNameTemp = propertyNameTemp[(indexOfDot + 1)..];

                nestedEntityEntry = nestedEntityEntry.References
                    .FirstOrDefault(x => x.Metadata.Name == name)?
                    .TargetEntry;

                if (nestedEntityEntry == null)
                {
                    return true;
                }

                indexOfDot = propertyNameTemp
                    .IndexOf('.');
            }

            var property = nestedEntityEntry.Entity
                .GetType()
                .GetProperty(propertyNameTemp);

            if (property == null)
            {
                throw new NullReferenceException(nameof(property));
            }

            var value = property
                .GetValue(nestedEntityEntry.Entity);

            if (property.PropertyType.IsSimple() || property.PropertyType.IsSubclassOf(typeof(Geometry)))
            {
                var orginalValue = nestedEntityEntry
                    .TryGetOriginalValue(propertyNameTemp);

                var hasChanged = !(value?.Equals(orginalValue) ?? orginalValue is null);

                if (hasChanged)
                {
                    return true;
                }
            }
            else
            {
                var properties = property.PropertyType
                    .GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                    .Select(x => x.Name)
                    .ToArray();

                nestedEntityEntry = nestedEntityEntry.References
                    .FirstOrDefault(x => x.Metadata.Name == propertyNameTemp)?
                    .TargetEntry;

                var hasChanged = nestedEntityEntry
                    .HasPublishPropertiesChanged(properties);

                if (hasChanged)
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal static object TryGetOriginalValue(this EntityEntry entry, string propertyName)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));

        if (propertyName == null)
            throw new ArgumentNullException(nameof(propertyName));

        var prop = entry.OriginalValues.Properties
            .FirstOrDefault(x => x.Name == propertyName);

        return prop != null
            ? entry.OriginalValues[propertyName]
            : null;
    }
}