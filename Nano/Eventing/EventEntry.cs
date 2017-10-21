using Microsoft.EntityFrameworkCore;

namespace Nano.Eventing
{
    /// <summary>
    /// Event Entry.
    /// </summary>
    public class EventEntry
    {
        /// <summary>
        /// Id.
        /// </summary>
        public virtual object Id { get; set; }

        /// <summary>
        /// Type.
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// State.
        /// </summary>
        public virtual EntityState State { get; set; }
    }
}