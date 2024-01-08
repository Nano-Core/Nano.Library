using Nano.App.Api.Requests.Attributes;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class GetConfirmEmailTokenRequest : BaseRequestGet
{
    /// <summary>
    /// Email Address.
    /// </summary>
    [Query]
    public virtual string EmailAddress { get; set; }

    /// <inheritdoc />
    public GetConfirmEmailTokenRequest()
    {
        this.Action = "email/confirm/token";
    }
}