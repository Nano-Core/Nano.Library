using System;
using System.Collections.Generic;
using Nano.Data.Abstractions.Eventing.Models;

namespace Nano.Data.Abstractions.Eventing.Annotations;

/// <summary>
/// Indicates that changes to the annotated entity will trigger an event publication.
/// Entities with this attribute will publish an <see cref="EntityEvent"/> whenever they are inserted, updated, or deleted.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class PublishAttribute : Attribute
{
    /// <summary>
    /// Gets the names of the properties that should be included in the <see cref="EntityEvent"/>.
    /// If empty, only Id and CreatedAt is published.
    /// </summary>
    public IEnumerable<string> PropertyNames { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishAttribute"/> class.
    /// </summary>
    /// <param name="propertyNames">The property names that should be included in the <see cref="EntityEvent"/>. If none are specified, only Id and CreatedAt is published.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="propertyNames"/> is <c>null</c>.</exception>
    public PublishAttribute(params string[] propertyNames)
    {
        this.PropertyNames = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));
    }
}