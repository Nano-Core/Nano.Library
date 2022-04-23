namespace Nano.Security.Models
{
    /// <summary>
    /// LogIn External Auth Code (abstract).
    /// </summary>
    public abstract class LogInExternalAuthCode<TProvider> : BaseLogInExternal<TProvider> 
        where TProvider : LogInExternalProviderAuthCode, new()
    {

    }
}
