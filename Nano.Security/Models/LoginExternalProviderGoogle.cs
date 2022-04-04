namespace Nano.Security.Models
{
    /// <summary>
    /// Login External Provider Google.
    /// </summary>
    public class LoginExternalProviderGoogle : LoginExternalProviderImplicit
    {
        /// <inheritdoc />
        public LoginExternalProviderGoogle()
        {
            this.Name = "Google";
        }
    }
}