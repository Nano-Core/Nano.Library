using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Interceptors.Helpers;

internal static class UpdatedAtStamper
{
    private const long TICKS_PER_MICROSECOND = 10L;

    internal static void Stamp(EntityEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        if (entry.State is not (EntityState.Modified or EntityState.Unchanged))
        {
            return;
        }

        if (entry.Metadata.FindProperty(nameof(IEntityReadOnly<Guid>.UpdatedAt)) == null)
        {
            return;
        }

        var property = entry
            .Property(nameof(IEntityReadOnly<Guid>.UpdatedAt));

        var original = (DateTimeOffset)property.OriginalValue!;
        var current = (DateTimeOffset)property.CurrentValue!;

        if (current != original)
        {
            return;
        }

        var now = Truncate(DateTimeOffset.UtcNow);

        property.CurrentValue = now > original
            ? now
            : original.AddTicks(TICKS_PER_MICROSECOND);
    }

    internal static DateTimeOffset Truncate(DateTimeOffset value)
    {
        return value.AddTicks(-(value.Ticks % TICKS_PER_MICROSECOND));
    }
}
