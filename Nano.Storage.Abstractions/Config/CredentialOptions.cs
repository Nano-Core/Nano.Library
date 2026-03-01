using System.ComponentModel.DataAnnotations;

namespace Nano.Storage.Abstractions.Config;

/// <summary>
/// Represents configuration options for the credentials of a storage account.
/// </summary>
public class CredentialOptions
{
    /// <summary>
    /// Gets or sets the username, account or tenant identifier used to authenticate with the storage provider.
    /// </summary>
    /// <remarks>
    ///     The exact meaning of this value depends on the storage provider
    ///     (for example, it may represent an account name, tenant ID, or service identifier).
    /// </remarks>
    [Required]
    public virtual string Id { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password, secret, key, or credential used to authenticate with the storage provider.
    /// </summary>
    /// <remarks>
    ///     This value is treated as sensitive data and should be stored securely.
    /// </remarks>
    [Required]
    public virtual string Secret { get; set; } = null!;
}