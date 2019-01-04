namespace Nano.Models.Auth
{
    /// <summary>
    /// External Login.
    /// </summary>
    public class LoginExternal
    {
        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Callback Url.
        /// </summary>
        public virtual string CallbackUrl { get; set; }
    }
}