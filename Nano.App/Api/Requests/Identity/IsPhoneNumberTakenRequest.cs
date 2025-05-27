using Nano.App.Api.Requests.Attributes;

namespace Nano.App.Api.Requests.Identity;

/// <inheritdoc />
public class IsPhoneNumberTakenRequest : BaseRequestGet
{
    /// <summary>
    /// Phone Number.
    /// </summary>
    [Query]
    public virtual string PhoneNumber { get; set; }

    /// <inheritdoc />
    public IsPhoneNumberTakenRequest()
    {
        this.Action = "phone/is-taken";
    }
}