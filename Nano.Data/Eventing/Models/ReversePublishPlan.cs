using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nano.Data.Eventing.Models;

internal sealed class ReversePublishPlan
{
    public required Type RootType { get; init; }

    public required Type ChangedType { get; init; }

    public required IReadOnlyList<NavigationStep> Path { get; init; }

    public required HashSet<string> WatchedProperties { get; init; }
}

internal sealed class NavigationStep
{

    public required string NavigationName { get; init; }

    public required IForeignKey ForeignKey { get; init; }

    public required bool IsOnDependent { get; init; }
}

