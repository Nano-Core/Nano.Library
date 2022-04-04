namespace Nano.Security.Models
{
    /// <summary>
    /// Login External Auth Code.
    /// </summary>
    public class LoginExternalAuthCode<TProvider> : BaseLoginExternal<TProvider> 
        where TProvider : LoginExternalProviderAuthCode, new()
    {

    }
}
