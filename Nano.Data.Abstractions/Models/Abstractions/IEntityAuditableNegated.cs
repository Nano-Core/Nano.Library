namespace Nano.Data.Abstractions.Models.Abstractions;

/// <summary>
/// Interface for entities that are explicitly excluded from audit tracking.
/// Implementing entities will not be included in audit logs.
/// </summary>
public interface IEntityAuditableNegated : IEntity;