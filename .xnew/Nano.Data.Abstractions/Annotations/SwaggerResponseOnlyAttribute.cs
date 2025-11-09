using System;

namespace Nano.Models.Attributes;

/// <summary>
/// Swagger Response Only Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SwaggerResponseOnlyAttribute : Attribute;