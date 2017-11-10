namespace Nano.Models.Events
{
    /// <summary>
    /// Entity Event.
    /// </summary>
    public class EntityEvent
    {
        /// <summary>
        /// Id.
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// State.
        /// </summary>
        public virtual string State { get; set; }
    }
}