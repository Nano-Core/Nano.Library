using System;
using System.Collections.Generic;

namespace Nano.Data.Eventing.Models;

internal sealed class ReversePublishPlan
{
    internal required Type RootType { get; init; }

    internal required Type ChangedType { get; init; }

    internal required HashSet<string> WatchedProperties { get; init; }

    internal required IReadOnlyList<NavigationStep> Path { get; init; }
}