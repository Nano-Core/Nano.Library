namespace Nano.Security.Models
{
    /// <summary>
    /// LogIn External Provider Google.
    /// </summary>
    public class LogInExternalProviderGoogle : LogInExternalProviderImplicit
    {
        /// <inheritdoc />
        public LogInExternalProviderGoogle()
        {
            this.Name = "Google";
        }
    }
}