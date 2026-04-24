using Nano.Common.Annotations;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Represents a request to check if a phone number is already taken.
/// </summary>
[GetAction(ActionRoutes.IDENTITY_PHONE_IS_TAKEN)]
public class IsPhoneNumberTakenRequest : BaseRequest
{
    /// <summary>
    /// The phone number to check.
    /// </summary>
    [Required]
    [Query]
    [InternationalPhone]
    public virtual required string PhoneNumber { get; set; }
}