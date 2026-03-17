namespace Nano.Data.Abstractions.Models.Enums;

/// <summary>
/// Values that represent audit entry states.
/// </summary>
public enum AuditState
{
    /// <summary>
    /// An enum constant representing the entity added option.
    /// </summary>
    Added = 0,
    
    /// <summary>
    /// An enum constant representing the entity deleted option.
    /// </summary>
    Deleted = 1,
    
    /// <summary>
    /// An enum constant representing the entity modified option.
    /// </summary>
    Modified = 2,
    
    /// <summary>
    /// An enum constant representing the entity soft added option.
    /// </summary>
    SoftAdded = 3,

    /// <summary>
    /// An enum constant representing the entity soft deleted option.
    /// </summary>
    SoftDeleted = 4,
    
    /// <summary>
    /// An enum constant representing the relationship added option.
    /// </summary>
    RelationshipAdded = 5,

    /// <summary>
    /// An enum constant representing the relationship deleted option.
    /// </summary>
    RelationshipDeleted = 6,
    
    /// <summary>
    /// An enum constant representing the entity current option.
    /// </summary>
    Current = 7
}