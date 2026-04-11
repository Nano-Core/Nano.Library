using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace Nano.Data.Eventing.Models;

internal sealed class NavigationStep
{
    internal required string NavigationName { get; init; }

    internal required IForeignKey ForeignKey { get; init; }

    internal required bool IsOnDependent { get; init; }

    internal required Type TargetType { get; init; }
}