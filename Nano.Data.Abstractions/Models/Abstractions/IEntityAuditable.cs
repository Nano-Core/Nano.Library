namespace Nano.Data.Abstractions.Models.Abstractions;

/// <summary>
/// Interface for entities that support auditing.
/// Implementing entities will have audit logging applied.
/// </summary>
public interface IEntityAuditable : IEntity;