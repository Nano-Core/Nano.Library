using System;

namespace Nano.App.Api.Requests.Attributes;

/// <summary>
/// Header Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class HeaderAttribute : Attribute
{
    /// <summary>
    /// Name.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    /// Value Prefix.
    /// </summary>
    public virtual string ValuePrefix { get; set; }
}