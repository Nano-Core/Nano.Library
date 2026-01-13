using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class IsEmailAddressTakenRequest : BaseRequestGet
{
    /// <summary>
    /// Change Email Token.
    /// </summary>
    [Query]
    [Required]
    public virtual string EmailAddress { get; set; } = null!;

    /// <inheritdoc />
    public IsEmailAddressTakenRequest()
    {
        this.Action = "email/is-taken";
    }
}