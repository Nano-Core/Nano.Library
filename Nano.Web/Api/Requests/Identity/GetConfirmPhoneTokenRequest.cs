using Nano.Web.Api.Requests.Attributes;

namespace Nano.Web.Api.Requests.Identity;

/// <inheritdoc />
public class GetConfirmPhoneTokenRequest : BaseRequestGet
{
    /// <summary>
    /// Phone Number.
    /// </summary>
    [Query]
    public virtual string PhoneNumber { get; set; }

    /// <inheritdoc />
    public GetConfirmPhoneTokenRequest()
    {
        this.Action = "phone/confirm/token";
    }
}