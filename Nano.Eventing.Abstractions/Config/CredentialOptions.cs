using System.ComponentModel.DataAnnotations;

namespace Nano.Eventing.Abstractions.Config;

/// <summary>
/// Configuration options for eventing credentials in Nano applications.
/// </summary>
public class CredentialOptions
{
    /// <summary>
    /// Username or account id for authenticating with the broker.
    /// </summary>
    [Required]
    public virtual required string Id { get; set; }

    /// <summary>
    /// Passeword, secret or key for authenticating with the broker.
    /// </summary>
    [Required]
    public virtual required string Secret { get; set; }
}