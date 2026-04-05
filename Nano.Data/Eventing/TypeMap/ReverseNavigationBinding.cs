using System;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Nano.Data.Eventing.TypeMap;

internal sealed class ReverseNavigationBinding
{
    internal required Type RootType { get; init; }

    internal required Type ChangedType { get; init; }

    internal required string NavigationName { get; init; }

    internal required IForeignKey ForeignKey { get; init; }

    internal required bool IsOnDependent { get; init; }
}