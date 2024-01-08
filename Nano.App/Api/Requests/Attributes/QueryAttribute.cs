using System;

namespace Nano.App.Api.Requests.Attributes;

/// <summary>
/// Query Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class QueryAttribute : Attribute
{
    /// <summary>
    /// Name.
    /// </summary>
    public virtual string Name { get; set; }
}