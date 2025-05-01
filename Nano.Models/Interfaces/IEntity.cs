using System;

namespace Nano.Models.Interfaces;

/// <summary>
/// Entity interface.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Created At.
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }
}