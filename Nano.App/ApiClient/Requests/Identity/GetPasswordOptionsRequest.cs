namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetPasswordOptionsRequest : BaseRequestGet
{
    /// <inheritdoc />
    public GetPasswordOptionsRequest()
    {
        this.Action = "password/options";
    }
}