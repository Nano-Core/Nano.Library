using System;

namespace Nano.App.ApiClient.Annotations;

/// <summary>
/// Indicates that the decorated property should be included as an HTTP request header.
/// </summary>
/// <remarks>
///     Multiple properties may be decorated with <see cref="HeaderAttribute"/>.
///     Each annotated property will be added as a separate request header.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class HeaderAttribute : Attribute
{
    /// <summary>
    /// The name of the header. If null, the property name is used.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// A prefix to prepend to the header value, e.g., "Bearer ".
    /// </summary>
    public string? ValuePrefix { get; set; }
}