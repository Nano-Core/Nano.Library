using System;
using System.Collections.Generic;

namespace Nano.Data.Eventing.TypeMap;

internal sealed class EntityEventingModel
{
    internal Dictionary<Type, EntityEventingMetadata> EntityMap { get; } = new();

    internal Dictionary<(Type Type, string Path), Func<object, object?>> Accessors { get; } = new();

    internal Dictionary<Type, Dictionary<Type, List<ReverseNavigationBinding>>> ReverseBindings { get; } = new();
}