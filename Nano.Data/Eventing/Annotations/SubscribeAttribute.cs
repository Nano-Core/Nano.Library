using System;

namespace Nano.Models.Attributes;

/// <summary>
/// Subscribe Attribute.
/// Types with this annotation, subscribes to events of the declaring type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class SubscribeAttribute : Attribute;