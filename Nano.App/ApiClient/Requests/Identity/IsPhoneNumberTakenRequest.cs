using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Requests.Annotations;
using Nano.Common.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

// BUG: can we remove GetBody? and make annotation

/// <summary>
/// Represents a request to check if a phone number is already taken.
/// </summary>
public class IsPhoneNumberTakenRequest : BaseRequestGet
{
    /// <summary>
    /// The phone number to check.
    /// </summary>
    [Required]
    [Query]
    [InternationalPhone]
    public virtual string PhoneNumber { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of <see cref="IsPhoneNumberTakenRequest"/> with action set.
    /// </summary>
    public IsPhoneNumberTakenRequest()
    {
        this.Action = "phone/is-taken"; // BUG: Move this to annotation? We might also be able to handle route parameters inline in route, if we have string template in annotation
    }
}