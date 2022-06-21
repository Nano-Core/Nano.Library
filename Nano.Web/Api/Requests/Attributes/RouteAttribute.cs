using System;

namespace Nano.Web.Api.Requests.Attributes;

/// <summary>
/// Route Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RouteAttribute : Attribute
{
    /// <summary>
    /// Order.
    /// </summary>
    public virtual int Order { get; set; }
}