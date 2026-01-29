using System;
using Nano.App.ApiClient.Annotations.Actions;

namespace Nano.App.ApiClient.Annotations;

/// <summary>
/// Indicates that the decorated property should be used as a route parameter in the request URL.
/// </summary>
/// <remarks>
///     Multiple properties may be decorated with <see cref="RouteAttribute"/>.
///     The number of route parameters must match the placeholders defined in <see cref="ActionAttribute.ActionTemplate"/> of the <see cref="ActionAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RouteAttribute : Attribute
{
    /// <summary>
    /// The order in which the route parameters are applied in the URL.
    /// </summary>
    public int Order { get; set; }
}