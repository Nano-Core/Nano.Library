using System;

namespace Nano.Data.Abstractions.Annotations;

/// <summary>
/// Request Ignore Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RequestIgnoreAttribute : Attribute;