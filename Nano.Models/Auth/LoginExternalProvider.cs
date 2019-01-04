namespace Nano.Models.Auth
{
    /// <summary>
    /// External Login Provider.
    /// </summary>
    public class LoginExternalProvider
    {
        /// <summary>
        /// Name.
        /// </summary>
        public virtual string Name { get; set; }
        
        /// <summary>
        /// Display Name.
        /// </summary>
        public virtual string DisplayName { get; set; }
    }
}