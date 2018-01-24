namespace Nano.Models.Interfaces
{
    /// <summary>
    /// Entity Deletable.
    /// Implementing entities are deletable.
    /// </summary>
    public interface IEntityDeletableSoft : IEntityDeletable
    {
        /// <summary>
        /// Is Active.
        /// Indicates whether the entity is active and relevant.
        /// NOTE: Only active instances are returned from queries, when filters are enabled (default behavior).
        /// </summary>
        bool IsActive { get; set; }
    }
}