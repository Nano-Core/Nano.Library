namespace Nano.Models
{
    /// <summary>
    /// Login.
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Username.
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public virtual string Password { get; set; }
    }
}