namespace Nano.Eventing.Abstractions.Config;

/// <summary>
/// Configuration options for eventing credentials in Nano applications.
/// </summary>
public class CredentialOptions
{
    /// <summary>
    /// Username or account id for authenticating with the broker.
    /// </summary>
    public virtual string Id { get; set; } = null!;

    /// <summary>
    /// Passeword, secret or key for authenticating with the broker.
    /// </summary>
    public virtual string Secret { get; set; } = null!;
}