using System;
using System.Collections.Generic;

namespace Nano.Data.Abstractions.Eventing.Models;

/// <summary>
/// Represents an event related to an entity within the system.
/// </summary>
public sealed class EntityEvent(object id, string typeName, string state)
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    public object Id { get; init; } = id ?? throw new ArgumentNullException(nameof(id));

    /// <summary>
    /// Gets or sets the type name of the entity.
    /// </summary>
    public string TypeName { get; set; } = typeName ?? throw new ArgumentNullException(nameof(typeName));

    /// <summary>
    /// Gets or sets the current state of the entity for the event.
    /// </summary>
    public string State { get; init; } = state ?? throw new ArgumentNullException(nameof(state));

    /// <summary>
    /// Gets or sets additional data related to the event as key-value pairs.
    /// </summary>
    public Dictionary<string, object?> Data { get; set; } = new();
}