using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class IsEmailAddressTakenRequest : BaseRequestGet
{
    /// <summary>
    /// Change Email Token.
    /// </summary>
    [Query]
    public virtual string EmailAddress { get; set; }

    /// <inheritdoc />
    public IsEmailAddressTakenRequest()
    {
        this.Action = "email/is-taken";
    }
}