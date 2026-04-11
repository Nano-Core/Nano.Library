using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Nano.Data.Eventing.Models;

internal sealed class ReversePublishPlans : Dictionary<Type, List<ReversePublishPlan>>
{
    internal bool TryAddWatchedProperty(Type rootType, Type changedType, IReadOnlyList<INavigation> navigationSegments, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(rootType);
        ArgumentNullException.ThrowIfNull(changedType);
        ArgumentNullException.ThrowIfNull(navigationSegments);
        ArgumentNullException.ThrowIfNull(propertyName);

        if (!this.TryGetValue(changedType, out var publishPlans))
        {
            return false;
        }

        foreach (var plan in publishPlans)
        {
            if (plan.RootType != rootType)
            {
                continue;
            }

            if (plan.Path.Count != navigationSegments.Count)
            {
                continue;
            }

            var match = !navigationSegments
                .Where((t, i) => plan.Path[i].NavigationName != t.Name)
                .Any();

            if (!match)
            {
                continue;
            }

            plan.WatchedProperties
                .Add(propertyName);
            
            return true;
        }

        return false;
    }

    internal List<ReversePublishPlan> GetOrCreatePlans(Type changedType)
    {
        ArgumentNullException.ThrowIfNull(changedType);

        if (!this.TryGetValue(changedType, out var publishPlans))
        {
            publishPlans = [];
            this[changedType] = publishPlans;
        }

        return publishPlans;
    }

    internal NavigationStep[] GetNavigationSteps(Type changedType, IReadOnlyList<INavigation> navigationSegments)
    {
        ArgumentNullException.ThrowIfNull(changedType);
        ArgumentNullException.ThrowIfNull(navigationSegments);

        var navigationSteps = new NavigationStep[navigationSegments.Count];

        for (var i = 0; i < navigationSegments.Count; i++)
        {
            var segment = navigationSegments[i];

            navigationSteps[i] = new NavigationStep
            {
                NavigationName = segment.Name,
                TargetType = segment.ClrType,
                ForeignKey = segment.ForeignKey,
                IsOnDependent = segment.IsOnDependent
            };
        }

        return navigationSteps;
    }
}