using System;

namespace Nano.Data.Abstractions.Annotations;

/// <summary>
/// Marks a property to be ignored when processing requests for serialization in swagger.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SwaggerRequestIgnoreAttribute : Attribute;