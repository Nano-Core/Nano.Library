using System;

namespace Nano.Models.Attributes;

/// <summary>
/// Include Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IncludeAttribute : Attribute;