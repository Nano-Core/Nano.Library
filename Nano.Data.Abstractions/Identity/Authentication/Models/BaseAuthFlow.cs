using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Identity.Authentication.Models.Enums;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents the base class for all authentication flows used with external providers.
/// This class can be extended to implement specific flows, such as authorization code or implicit flows.
/// </summary>
public abstract class BaseAuthFlow(AuthFlowType type)
{
    /// <summary>
    /// The type of flow. "AuthCode" or "Implicit".
    /// </summary>
    [Required]
    public virtual AuthFlowType Type { get; set; } = type;
}