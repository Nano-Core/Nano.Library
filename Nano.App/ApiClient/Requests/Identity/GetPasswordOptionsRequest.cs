namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Represents a request to retrieve password policy and options.
/// </summary>
public class GetPasswordOptionsRequest : BaseRequestGet
{
    /// <summary>
    /// Initializes a new instance of <see cref="GetPasswordOptionsRequest"/> with action set.
    /// </summary>
    public GetPasswordOptionsRequest()
    {
        this.Action = "password/options";
    }
}