using System;

namespace Nano.App.ApiClient.Requests.Annotations;

/// <summary>
/// Marks a property to be included as a query string parameter in a request URL.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class QueryAttribute : Attribute
{
    /// <summary>
    /// The name of the query parameter. If null, the property name is used.
    /// </summary>
    public virtual string? Name { get; set; }
}