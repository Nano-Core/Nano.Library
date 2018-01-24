using Nano.Models.Interfaces;

namespace Nano.Models
{
    /// <inheritdoc />
    public abstract class BaseEntity : IEntityWritable
    {
        /// <inheritdoc />
        public virtual bool IsActive { get; set; } = true;
    }
}