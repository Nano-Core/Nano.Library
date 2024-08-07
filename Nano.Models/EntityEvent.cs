using System;
using System.Collections.Generic;

namespace Nano.Models;

/// <summary>
/// Entity Event.
/// </summary>
public class EntityEvent
{
    /// <summary>
    /// Id.
    /// </summary>
    public object Id { get; set; }

    /// <summary>
    /// Type.
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// State.
    /// </summary>
    public string State { get; set; }

    /// <summary>
    /// Data.
    /// </summary>
    public IDictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="state"></param>
    public EntityEvent(object id, string type, string state)
    {
        this.Id = id ?? throw new ArgumentNullException(nameof(id));
        this.Type = type ?? throw new ArgumentNullException(nameof(type));
        this.State = state ?? throw new ArgumentNullException(nameof(state));
    }
}