using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nano.Data.Eventing.Models;

namespace Nano.Data.Eventing.Extensions;

internal static class EntityEntryExtensions
{
    internal static CompositeKey GetCompositeKey(this EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var values = entry
            .GetPrimaryKeyValues();

        return new CompositeKey(values);
    }

    internal static object? GetPrimaryKeyValue(this EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var values = entry
            .GetPrimaryKeyValues();

        return values
            .FirstOrDefault();
    }


    private static object[] GetPrimaryKeyValues(this EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        var primaryKey = entry.Metadata
            .FindPrimaryKey() ?? throw new InvalidOperationException($"Entity type '{entry.Metadata.Name}' does not have a primary key.");

        return primaryKey.Properties
            .Select(p => entry.Property(p.Name).CurrentValue)
            .ToArray()!;
    }
}