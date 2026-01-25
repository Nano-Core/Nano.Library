using System;

namespace Nano.Data.Abstractions.Annotations;

/// <summary>
/// Marks a property to be ignored when processing requests for serialization.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RequestIgnoreAttribute : Attribute;