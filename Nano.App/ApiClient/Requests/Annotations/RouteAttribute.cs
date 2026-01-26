using System;

namespace Nano.App.ApiClient.Requests.Annotations;

/// <summary>
/// Marks a property to be used as a route parameter in a request URL.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RouteAttribute : Attribute
{
    /// <summary>
    /// The order in which the route parameters are applied in the URL.
    /// </summary>
    public virtual int Order { get; set; }
}