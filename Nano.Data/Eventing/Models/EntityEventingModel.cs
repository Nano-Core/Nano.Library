using System;
using System.Collections.Generic;

namespace Nano.Data.Eventing.Models;

internal sealed class EntityEventingModel
{
    internal Dictionary<Type, PublishMetadata> Metadata { get; } = new();

    internal Dictionary<Type, List<ReversePublishPlan>> ReversePlans { get; } = new();

    internal Dictionary<(Type Type, string Path), Func<object, object?>> Accessors { get; } = new();
}