namespace Nano.Models.Auth
{
    /// <summary>
    /// Set Username.
    /// </summary>
    public class SetUsername
    {
        /// <summary>
        /// User Id.
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        public virtual string Username { get; set; }
    }
}