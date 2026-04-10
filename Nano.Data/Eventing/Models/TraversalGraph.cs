using System;
using System.Collections.Generic;

namespace Nano.Data.Eventing.Models;

internal sealed class TraversalGraph
{
    // Type → navigations to traverse FROM that type
    private readonly Dictionary<Type, HashSet<string>> map = new();

    public IReadOnlyDictionary<Type, HashSet<string>> Map => map;

    public void AddEdge(Type fromType, string navigationName)
    {
        if (!map.TryGetValue(fromType, out var set))
        {
            set = new HashSet<string>();
            map[fromType] = set;
        }

        set.Add(navigationName);
    }

    public bool TryGetNavigations(Type type, out HashSet<string> navigations)
        => map.TryGetValue(type, out navigations!);
}