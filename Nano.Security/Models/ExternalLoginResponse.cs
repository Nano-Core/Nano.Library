namespace Nano.Security.Models
{
    /// <summary>
    /// External Login Response.
    /// </summary>
    public class ExternalLoginResponse
    {
        /// <summary>
        /// Access Token.
        /// </summary>
        public virtual AccessToken AccessToken { get; set; }

        /// <summary>
        /// Data.
        /// </summary>
        public virtual ExternalLoginData Data { get; set; }
    }
}