using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace Nano.Data.Eventing.Models;

internal sealed class NavigationStep
{
    public required string NavigationName { get; init; }

    public required IForeignKey ForeignKey { get; init; }

    public required bool IsOnDependent { get; init; }

    public required Type TargetType { get; init; }
}