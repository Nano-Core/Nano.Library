namespace Nano.Security.Models
{
    /// <summary>
    /// External Login Data.
    /// </summary>
    public class ExternalLoginData
    {
        /// <summary>
        /// Id.
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// External Token.
        /// </summary>
        public virtual ExternalLoginTokenData ExternalToken { get; set; } = new();
    }
}