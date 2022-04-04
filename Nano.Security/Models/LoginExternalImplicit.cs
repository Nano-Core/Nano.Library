namespace Nano.Security.Models
{
    /// <summary>
    /// Login External Implicit.
    /// </summary>
    public class LoginExternalImplicit<TProvider> : BaseLoginExternal<TProvider>
        where TProvider : LoginExternalProviderImplicit, new()
    {

    }
}