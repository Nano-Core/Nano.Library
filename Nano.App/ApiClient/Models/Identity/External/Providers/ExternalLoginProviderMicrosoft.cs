namespace Nano.App.ApiClient.Models.Identity.External.Providers;

/// <summary>
/// External Login Provider Microsoft.
/// </summary>
public class ExternalLoginProviderMicrosoft : ExternalLoginProviderAuthCode
{
    /// <inheritdoc />
    public ExternalLoginProviderMicrosoft()
    {
        this.Name = "Microsoft";
    }
}