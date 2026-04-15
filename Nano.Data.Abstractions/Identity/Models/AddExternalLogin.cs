using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Identity.Authentication.Models;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Base type for requests that add an external login to an existing user.
/// </summary>
/// <typeparam name="TFlow">The type of authentication flow, e.g. auth-code, implicit, etc.</typeparam>
public class AddExternalLogin<TFlow>
    where TFlow : BaseAuthFlow
{
    /// <summary>
    /// The external authentication flow.
    /// </summary>
    [Required]
    public virtual required TFlow Flow { get; set; }
}