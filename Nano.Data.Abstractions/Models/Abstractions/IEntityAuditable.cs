namespace Nano.Data.Abstractions.Models.Abstractions;

/// <summary>
/// Interface for entities that support auditing.
/// Implementing entities will have audit tracking applied (e.g., CreatedAt, ModifiedAt).
/// </summary>
public interface IEntityAuditable : IEntity;