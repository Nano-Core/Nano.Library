using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Base type for requests that add an external login to an existing user.
/// </summary>
/// <typeparam name="TProvider">The external authentication provider type.</typeparam>
public abstract class BaseAddExternalLogin<TProvider>
    where TProvider : BaseLogInExternalProvider, new()
{
    /// <summary>
    /// The external authentication provider configuration.
    /// </summary>
    [Required]
    public TProvider Provider { get; set; } = new();
}