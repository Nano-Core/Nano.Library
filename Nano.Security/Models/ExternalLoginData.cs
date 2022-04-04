namespace Nano.Security.Models
{
    /// <summary>
    /// External Login Data.
    /// </summary>
    public class ExternalLoginData
    {
        /// <summary>
        /// Provider Name.
        /// </summary>
        public virtual string ProviderName { get; set; }

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
        public virtual string ExternalToken { get; set; }

        /// <summary>
        /// External Refresh Token.
        /// </summary>
        public virtual string ExternalRefreshToken { get; set; }
    }
}