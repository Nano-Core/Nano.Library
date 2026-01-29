using System;

namespace Nano.App.ApiClient.Annotations;

/// <summary>
/// Indicates that the decorated property should be included as a query string parameter in the request URL.
/// </summary>
/// <remarks>
///     Multiple properties may be decorated with <see cref="QueryAttribute"/>.
///     Each annotated property will be added as a separate query string parameter.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class QueryAttribute : Attribute
{
    /// <summary>
    /// The name of the query parameter. If null, the property name is used.
    /// </summary>
    public string? Name { get; set; }
}