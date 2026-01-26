using System;

namespace Nano.App.ApiClient.Requests.Annotations;

/// <summary>
/// Marks a property to be included as a header in an HTTP request.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class HeaderAttribute : Attribute
{
    /// <summary>
    /// The name of the header. If null, the property name is used.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// A prefix to prepend to the header value, e.g., "Bearer ".
    /// </summary>
    public virtual string? ValuePrefix { get; set; }
}