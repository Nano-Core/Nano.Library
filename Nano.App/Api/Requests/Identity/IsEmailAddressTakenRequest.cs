using Nano.App.Api.Requests.Attributes;

namespace Nano.App.Api.Requests.Identity;

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