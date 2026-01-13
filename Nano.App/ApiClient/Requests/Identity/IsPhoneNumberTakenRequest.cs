using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class IsPhoneNumberTakenRequest : BaseRequestGet
{
    /// <summary>
    /// Phone Number.
    /// </summary>
    [Required]
    [Query]
    public virtual string PhoneNumber { get; set; } = null!;

    /// <inheritdoc />
    public IsPhoneNumberTakenRequest()
    {
        this.Action = "phone/is-taken";
    }
}