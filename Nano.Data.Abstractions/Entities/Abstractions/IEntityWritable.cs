namespace Nano.Data.Abstractions.Entities.Abstractions;

/// <summary>
/// Represents a writable entity that can be created, updated, and soft-deleted.
/// Implements <see cref="IEntityCreatableAndUpdatable"/> and <see cref="IEntityDeletableSoft"/>.
/// </summary>
public interface IEntityWritable : IEntityCreatableAndUpdatable, IEntityDeletableSoft;