using System;

namespace Nano.App.Controllers.Contracts
{
    /// <summary>
    /// Criteria.
    /// </summary>
    public class Criteria
    {
        /// <summary>
        /// Is Active.
        /// </summary>
        public virtual bool IsActive { get; } = true;

        /// <summary>
        /// After At.
        /// </summary>
        public virtual DateTimeOffset? AfterAt { get; set; }

        /// <summary>
        /// Before At.
        /// </summary>
        public virtual DateTimeOffset? BeforeAt { get; set; }
    }
}