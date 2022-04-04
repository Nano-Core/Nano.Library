namespace Nano.Security.Models
{
    /// <summary>
    /// Login External Provider Microsoft.
    /// </summary>
    public class LoginExternalProviderMicrosoft : LoginExternalProviderAuthCode
    {
        /// <inheritdoc />
        public LoginExternalProviderMicrosoft()
        {
            this.Name = "Microsoft";
        }
    }
}