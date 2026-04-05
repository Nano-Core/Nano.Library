using System;
using System.Collections.Generic;

namespace Nano.Data.Eventing.TypeMap;

internal sealed class EntityEventingMetadata
{
    /// <summary>
    /// The clr type.
    /// </summary>
    internal required Type ClrType { get; init; }

    /// <summary>
    /// Final publishable property paths
    /// </summary>
    internal required string[] Properties { get; init; }

    /// <summary>
    /// Pre-split paths (performance)
    /// </summary>
    internal required string[][] PropertySegments { get; init; }

    /// <summary>
    /// Navigation map: Property => Target type
    /// </summary>
    internal required Dictionary<string, Type> Navigations { get; init; }

    /// <summary>
    /// Reverse dependencies: Target entity → list of root navigation paths
    /// </summary>
    internal required Dictionary<Type, List<string>> ReverseDependencies { get; init; }
}