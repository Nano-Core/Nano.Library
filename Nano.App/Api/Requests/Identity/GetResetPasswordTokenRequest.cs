using Nano.App.Api.Requests.Attributes;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GetResetPasswordTokenRequest : BaseRequestGet
{
    /// <summary>
    /// Email Address.
    /// </summary>
    [Query]
    public virtual string EmailAddress { get; set; }

    /// <inheritdoc />
    public GetResetPasswordTokenRequest()
    {
        this.Action = "password/reset/token";
    }
}