namespace Nano.Models.Auth
{
    /// <summary>
    /// Set Password.
    /// </summary>
    public class SetPassword
    {
        /// <summary>
        /// User Id.
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// New Password.
        /// </summary>
        public virtual string NewPassword { get; set; }
    }
}