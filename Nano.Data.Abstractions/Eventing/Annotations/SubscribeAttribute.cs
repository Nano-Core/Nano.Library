using System;
using Nano.Data.Abstractions.Eventing.Models;

namespace Nano.Data.Abstractions.Eventing.Annotations;

/// <summary>
/// Indicates that the annotated type subscribes to <see cref="EntityEvent"/> for entities with
/// a matching name that have a <see cref="PublishAttribute"/> applied.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class SubscribeAttribute : Attribute;