using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.Common.Consts;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Represents a request to check if an email address is already taken.
/// </summary>
[GetAction(ActionRoutes.IDENTITY_EMAIL_IS_TAKEN)]
public class IsEmailAddressTakenRequest : BaseRequest
{
    /// <summary>
    /// The email address to check.
    /// </summary>
    [Required]
    [Query]
    public virtual required string EmailAddress { get; set; }
}