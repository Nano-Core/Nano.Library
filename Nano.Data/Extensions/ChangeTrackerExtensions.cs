using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nano.Data.Extensions;

internal static class ChangeTrackerExtensions
{
    internal static bool IsTracked(this ChangeTracker changeTracker, object entity)
    {
        ArgumentNullException.ThrowIfNull(changeTracker);

        return changeTracker
            .Entries()
            .Any(x => x.Entity == entity);
    }
}