namespace Nano.Security.Models
{
    /// <summary>
    /// Login External Provider Facebook.
    /// </summary>
    public class LoginExternalProviderFacebook : LoginExternalProviderImplicit
    {
        /// <inheritdoc />
        public LoginExternalProviderFacebook()
        {
            this.Name = "Facebook";
        }
    }
}