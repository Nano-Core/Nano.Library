namespace Nano.App.Models.Types
{
    /// <summary>
    /// Authentication Credential.
    /// </summary>
    public class AuthenticationCredential
    {
        /// <summary>
        /// Username.
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Token.
        /// </summary>
        public virtual string Token { get; set; }
    }
}