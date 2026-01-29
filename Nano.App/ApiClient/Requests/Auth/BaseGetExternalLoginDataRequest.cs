using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.App.ApiClient.Requests.Auth;

/// <summary>
/// Base class for requests to retrieve external login provider data.
/// </summary>
/// <typeparam name="TProvider">The type of external login provider.</typeparam>
public abstract class BaseGetExternalLoginDataRequest<TProvider> : BaseAuthRequest
    where TProvider : BaseLogInExternalProvider, new()
{
    /// <summary>
    /// The external login provider.
    /// </summary>
    [Required]
    [Body]
    public virtual TProvider Provider { get; set; } = new();
}