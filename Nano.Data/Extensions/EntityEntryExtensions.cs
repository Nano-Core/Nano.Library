using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nano.Data.Extensions;

internal static class EntityEntryExtensions
{
    internal static string GetKeyName(this EntityEntry entityEntry)
    {
        ArgumentNullException.ThrowIfNull(entityEntry);

        var primaryKey = entityEntry.Metadata
            .FindPrimaryKey()!;

        var keyPropertyName = primaryKey.Properties
            .Select(y => entityEntry.Property(y.Name).Metadata.Name)
            .First();

        return keyPropertyName;
    }

    internal static TIdentity? GetKeyValue<TIdentity>(this EntityEntry entityEntry)
        where TIdentity : IEquatable<TIdentity>
    {
        ArgumentNullException.ThrowIfNull(entityEntry);

        var primaryKey = entityEntry.Metadata
            .FindPrimaryKey()!;

        return (TIdentity?)primaryKey.Properties
            .Select(y => entityEntry.Property(y.Name).CurrentValue)
            .First();
    }
}