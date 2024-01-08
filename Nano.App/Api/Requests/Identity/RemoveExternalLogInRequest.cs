namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class RemoveExternalLogInRequest : BaseRequestPost
{
    /// <inheritdoc />
    public RemoveExternalLogInRequest()
    {
        this.Action = "external/login/remove";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return null;
    }
}