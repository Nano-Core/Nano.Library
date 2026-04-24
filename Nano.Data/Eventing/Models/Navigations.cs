using System;
using System.Collections.Generic;

namespace Nano.Data.Eventing.Models;

internal sealed class Navigations : Dictionary<(Type RootType, Type CurrentType), HashSet<string>>
{
    internal void AddNavigation(Type rootType, Type currentType, string navigationName)
    {
        ArgumentNullException.ThrowIfNull(rootType);
        ArgumentNullException.ThrowIfNull(currentType);
        ArgumentNullException.ThrowIfNull(navigationName);

        var key = (rootType, currentType);

        if (!this.TryGetValue(key, out var hashSet))
        {
            hashSet = [];
            this[key] = hashSet;
        }

        hashSet
            .Add(navigationName);
    }

    internal IReadOnlyCollection<string> GetNavigations(Type rootType, Type currentType)
    {
        ArgumentNullException.ThrowIfNull(rootType);
        ArgumentNullException.ThrowIfNull(currentType);

        return this.TryGetValue((rootType, currentType), out var navigations)
            ? navigations
            : [];
    }
}