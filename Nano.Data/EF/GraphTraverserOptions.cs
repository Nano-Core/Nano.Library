using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nano.Data;

internal sealed class GraphTraverserOptions
{
    internal required Func<EntityEntry, IEnumerable<string>> GetPaths { get; init; }

    internal required bool LoadNavigations { get; init; }
    
    internal required Func<EntityEntry, string, bool> ShouldTraverseNavigation { get; init; }
    
    internal required Action<EntityEntry, string?> OnEntityVisited { get; init; }
    
    internal required bool IncludeCollections { get; init; }
}