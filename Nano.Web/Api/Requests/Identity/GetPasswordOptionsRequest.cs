namespace Nano.Web.Api.Requests.Identity;

/// <inheritdoc />
public class GetPasswordOptionsRequest : BaseRequestGet
{
    /// <inheritdoc />
    public GetPasswordOptionsRequest()
    {
        this.Action = "password/options";
    }
}