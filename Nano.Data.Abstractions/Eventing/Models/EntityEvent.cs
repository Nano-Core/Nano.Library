using System;
using System.Collections.Generic;

namespace Nano.Data.Abstractions.Eventing.Models;

/// <summary>
/// Represents an event related to an entity within the system.
/// </summary>
public sealed class EntityEvent
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    public object Id { get; set; }

    /// <summary>
    /// Gets or sets the type name of the entity.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the current state of the entity for the event.
    /// </summary>
    public string State { get; set; }

    /// <summary>
    /// Gets or sets additional data related to the event as key-value pairs.
    /// </summary>
    public IDictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityEvent"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the entity. Cannot be <c>null</c>.</param>
    /// <param name="type">The type name of the entity. Cannot be <c>null</c>.</param>
    /// <param name="state">The state of the entity for this event. Cannot be <c>null</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/>, <paramref name="type"/>, or <paramref name="state"/> is <c>null</c>.</exception>
    public EntityEvent(object id, string type, string state)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        State = state ?? throw new ArgumentNullException(nameof(state));
    }
}