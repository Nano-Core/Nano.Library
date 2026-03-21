using System.ComponentModel.DataAnnotations;
using Nano.Common.Annotations;

namespace Nano.Data.Abstractions.Identity.Models;

/// <summary>
/// Represents a request to generate a change phone number token for a user.
/// </summary>
public class GenerateChangePhoneToken
{
    /// <summary>
    /// The new phone number to be associated with the user.
    /// </summary>
    [Required]
    [MaxLength(20)]
    [InternationalPhone]
    public virtual string NewPhoneNumber { get; set; } = null!;
}