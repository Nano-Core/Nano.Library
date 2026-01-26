using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <summary>
/// Represents a request to check if an email address is already taken.
/// </summary>
public class IsEmailAddressTakenRequest : BaseRequestGet
{
    /// <summary>
    /// The email address to check.
    /// </summary>
    [Required]
    [Query]
    public virtual string EmailAddress { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of <see cref="IsEmailAddressTakenRequest"/> with action set.
    /// </summary>
    public IsEmailAddressTakenRequest()
    {
        this.Action = "email/is-taken";
    }
}