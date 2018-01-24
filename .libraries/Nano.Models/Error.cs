namespace Nano.Models
{
    /// <summary>
    /// Error.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Message.
        /// </summary>
        public virtual string Summary { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string[] Errors { get; set; } = new string[0];
    }
}