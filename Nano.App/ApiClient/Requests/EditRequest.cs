using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to edit a single entity.
/// </summary>
public class EditRequest : BaseRequestPut
{
    /// <summary>
    /// The entity to update.
    /// </summary>
    [Required]
    public virtual IEntityUpdatable Entity { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of <see cref="EditRequest"/>.
    /// </summary>
    public EditRequest()
    {
        this.Action = "edit";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Entity;
    }
}