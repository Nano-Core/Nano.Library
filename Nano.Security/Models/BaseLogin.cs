namespace Nano.Security.Models
{
    /// <summary>
    /// Base Login (abstract).
    /// </summary>
    public abstract class BaseLogin
    {
        /// <summary>
        /// Parameters.
        /// </summary>
        public virtual LoginParameters Parameters { get; } = new();
    }
}