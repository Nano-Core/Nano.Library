using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to edit an entity and retrieve it.
/// </summary>
public class EditAndGetRequest : BaseRequestPut
{
    /// <summary>
    /// The entity to update.
    /// </summary>
    [Required]
    public virtual IEntityUpdatable Entity { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of <see cref="EditAndGetRequest"/>.
    /// </summary>
    public EditAndGetRequest()
    {
        this.Action = "edit/get";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Entity;
    }
}