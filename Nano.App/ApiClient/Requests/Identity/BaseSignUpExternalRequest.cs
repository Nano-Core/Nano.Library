using Nano.Data.Abstractions.Identity.Models;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public abstract class BaseSignUpExternalRequest : BaseRequest;

/// <summary>
/// Base request for signing up a user via an external provider.
/// </summary>
/// <typeparam name="TSignUp">Type of the sign-up information.</typeparam>
public abstract class BaseSignUpExternalRequest<TSignUp> : BaseSignUpExternalRequest
    where TSignUp : BaseSignUpExternal, new()
{
    /// <summary>
    /// The external sign-up information.
    /// </summary>
    [Required]
    [Body]
    public virtual TSignUp SignUpExternal { get; set; } = new();
}