using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace Nano.Data.Eventing.Models;

internal sealed record NavigationStep(Type TargetType, string NavigationName, IForeignKey ForeignKey, bool IsOnDependent);