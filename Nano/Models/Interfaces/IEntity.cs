using System;

namespace Nano.Models.Interfaces
{
    /// <summary>
    /// Entity Interface (base).
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Is Active.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Created At.
        /// </summary>
        DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Updated At.
        /// </summary>
        DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Expire At.
        /// </summary>
        DateTimeOffset? ExpireAt { get; set; }
    }
}