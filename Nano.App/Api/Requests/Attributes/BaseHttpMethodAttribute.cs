using System;

namespace Nano.App.Api.Requests.Attributes;

/// <summary>
/// Base Http Method Attribute (abstract).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public abstract class BaseHttpMethodAttribute : Attribute;