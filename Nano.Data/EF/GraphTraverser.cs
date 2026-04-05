using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nano.Data;

internal static class GraphTraverser
{
    internal static void TraverseGraph(DbContext dbContext, EntityEntry rootEntry, GraphTraverserOptions options)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(rootEntry);
        ArgumentNullException.ThrowIfNull(options);

        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        TraverseNode(dbContext, rootEntry, options, visited, null);
    }


    private static void TraverseNode(DbContext dbContext, EntityEntry entry, GraphTraverserOptions options, HashSet<object> visited, string? currentPath)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(visited);

        if (!visited.Add(entry.Entity))
        {
            return;
        }

        options
            .OnEntityVisited(entry, currentPath);

        var paths = options
            .GetPaths(entry);

        foreach (var path in paths)
        {
            var segments = path
                .Split('.');
            
            TraversePath(dbContext, entry, segments, 0, options, visited, null);
        }
    }
    private static void TraversePath(DbContext dbContext, EntityEntry entry, string[] segments, int index, GraphTraverserOptions options, HashSet<object> visited, string? currentPath)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(visited);

        while (index < segments.Length)
        {
            var segment = segments[index];

            var navigation = entry.Metadata.FindNavigation(segment);
            if (navigation == null)
            {
                return;
            }

            if (!options.IncludeCollections && navigation.IsCollection)
            {
                return;
            }

            var navigationEntry = entry
                .Navigation(segment);

            if (options.LoadNavigations && !navigationEntry.IsLoaded)
            {
                navigationEntry
                    .Load();
            }

            if (navigationEntry.CurrentValue == null)
            {
                return;
            }

            if (!options.ShouldTraverseNavigation(entry, segment))
            {
                return;
            }

            var nextPath = currentPath == null
                ? segment
                : $"{currentPath}.{segment}";

            var childEntry = dbContext
                .Entry(navigationEntry.CurrentValue);

            TraverseNode(dbContext, childEntry, options, visited, nextPath);

            entry = childEntry;
            currentPath = nextPath;
            
            index++;
        }
    }
}