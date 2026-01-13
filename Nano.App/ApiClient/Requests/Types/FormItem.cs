using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests.Types;

/// <summary>
/// Form Item.
/// </summary>
public class FormItem
{
    /// <summary>
    /// Name.
    /// </summary>
    [Required]
    public virtual string Name { get; set; } = null!;

    /// <summary>
    /// Value.
    /// </summary>
    public virtual object? Value { get; set; }
}