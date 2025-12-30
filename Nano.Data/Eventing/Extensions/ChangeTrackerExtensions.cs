using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nano.Common.Extensions;
using Nano.Data.Abstractions.Eventing.Annotations;
using Nano.Data.Abstractions.Eventing.Models;
using Nano.Data.Abstractions.Models;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.Data.Eventing.Extensions;

internal static class ChangeTrackerExtensions
{
    internal static List<EntityEvent> GetPendingEntityEvents(this ChangeTracker changeTracker)
    {
        if (changeTracker == null) 
            throw new ArgumentNullException(nameof(changeTracker));

        return changeTracker
            .Entries<IEntity>()
            .Where(x =>
                x.Entity.GetType().IsTypeOf(typeof(BaseEntity<>)) &&
                x.Entity.GetType().GetCustomAttributes<PublishAttribute>().Any() &&
                x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(x => x
                .GetEntityEvent())
            .Where(x => x != null)
            .ToList();
    }
}