using System;
using System.Collections.Generic;

namespace Nano.Data.Eventing.Models;

internal sealed class EntityEventingModel
{
    internal Dictionary<Type, PublishMetadata> Metadata { get; } = new(); // BUG: Post Save

    internal Dictionary<(Type Type, string Path), Func<object, object?>> Accessors { get; } = new(); // BUG: Post Save

    internal Dictionary<Type, List<ReversePublishPlan>> ReversePlans { get; } = new(); 

    internal Dictionary<Type, TraversalGraph> TraversalGraphs { get; set; } = new();
}