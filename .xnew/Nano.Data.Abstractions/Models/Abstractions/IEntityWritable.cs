namespace Nano.Data.Abstractions.Models.Abstractions;

/// <summary>
/// Entity Writable.
/// Implementing entities are creatable, updatable and deletable.
/// Implements <see cref="IEntityCreatable"/>, <see cref="IEntityUpdatable"/> and <see cref="IEntityDeletable"/>.
/// </summary>
public interface IEntityWritable : IEntityCreatableAndUpdatable, IEntityDeletableSoft;