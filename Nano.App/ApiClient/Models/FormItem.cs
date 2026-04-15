using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Models;

/// <summary>
/// Represents a single item in a form submission.
/// </summary>
public class FormItem
{
    /// <summary>
    /// The name of the form field.
    /// </summary>
    [Required]
    public virtual required string Name { get; set; }

    /// <summary>
    /// The value of the form field.
    /// </summary>
    public virtual object? Value { get; set; }
}