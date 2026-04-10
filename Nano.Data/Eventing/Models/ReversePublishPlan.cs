using System;
using System.Collections.Generic;

namespace Nano.Data.Eventing.Models;

internal sealed class ReversePublishPlan
{
    public required Type RootType { get; init; }

    public required Type ChangedType { get; init; }

    public required HashSet<string> WatchedProperties { get; init; }

    public required IReadOnlyList<NavigationStep> Path { get; init; }
}