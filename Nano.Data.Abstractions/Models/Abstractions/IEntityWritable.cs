namespace Nano.Data.Abstractions.Models.Abstractions;

/// <summary>
/// Represents a writable entity that can be created, updated, and soft-deleted.
/// Implements <see cref="IEntityCreatableAndUpdatable"/> and <see cref="IEntitySoftDeletable"/>.
/// </summary>
public interface IEntityWritable : IEntityCreatableAndUpdatable, IEntityDeletable;