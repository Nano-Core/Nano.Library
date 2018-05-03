using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <inheritdoc />
    public abstract class BaseEntity : IEntityWritable
    {
        /// <inheritdoc />
        public virtual long IsDeleted { get; set; } = 0L;
    }
}