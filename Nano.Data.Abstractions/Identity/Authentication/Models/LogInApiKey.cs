using System.ComponentModel.DataAnnotations;

namespace Nano.Data.Abstractions.Identity.Authentication.Models;

/// <summary>
/// Represents a api-key login request.
/// </summary>
public class LogInApiKey : BaseLogIn
{
    /// <summary>
    /// The api-key to use for logging.
    /// </summary>
    [Required]
    public virtual required string ApiKey { get; set; }
}