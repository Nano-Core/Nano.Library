using System;
using System.Collections.Generic;

namespace Nano.Eventing.Attributes;

/// <summary>
/// Publish Attribute.
/// Types with this annotation, defines that an event will be published for the entity when it changes.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class PublishAttribute : Attribute
{
    /// <summary>
    /// Property Names.
    /// </summary>
    public IEnumerable<string> PropertyNames { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="propertyNames">The property names.</param>
    public PublishAttribute(params string[] propertyNames)
    {
        this.PropertyNames = propertyNames ?? Array.Empty<string>();
    }
}