using System;

namespace Nano.Data.Abstractions.Annotations;

/// <summary>
/// Swagger Response Only Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SwaggerResponseOnlyAttribute : Attribute;