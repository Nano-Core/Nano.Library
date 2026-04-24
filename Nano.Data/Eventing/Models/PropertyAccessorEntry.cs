using System;

namespace Nano.Data.Eventing.Models;

internal sealed record PropertyAccessorEntry(Type EntityType, string Path, string Name, Func<object, object?> Accessor);