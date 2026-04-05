using System;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace Nano.Data.Eventing.TypeMap;

internal static class EntityEventingModelCache
{
    private static readonly ConcurrentDictionary<Type, EntityEventingModel> cache = new();

    internal static EntityEventingModel GetOrCreate(DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var contextType = dbContext
            .GetType();

        return cache
            .GetOrAdd(contextType, _ => EntityEventingModelBuilder.Build(dbContext));
    }
}