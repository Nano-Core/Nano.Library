namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GetPasswordOptionsRequest : BaseRequestGet
{
    /// <inheritdoc />
    public GetPasswordOptionsRequest()
    {
        this.Action = "password/options";
    }
}