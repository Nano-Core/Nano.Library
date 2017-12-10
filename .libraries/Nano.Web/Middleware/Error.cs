namespace Nano.Web.Middleware
{
    /// <summary>
    /// Error Result.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Summary.
        /// </summary>
        public virtual string Summary { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Errors.
        /// </summary>
        public virtual string[] Errors { get; set; }
    }
}