using System;

namespace Nano.App.ApiClient.Requests.Attributes;

/// <summary>
/// Header Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class HeaderAttribute : Attribute
{
    /// <summary>
    /// Name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Value Prefix.
    /// </summary>
    public virtual string? ValuePrefix { get; set; }
}