using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to update multiple entities at once.
/// </summary>
public class EditManyRequest : BaseRequestPut
{
    /// <summary>
    /// The entities to update.
    /// </summary>
    [Required]
    public virtual IEnumerable<IEntityUpdatable> Entities { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of <see cref="EditManyRequest"/>.
    /// </summary>
    public EditManyRequest()
    {
        this.Action = "edit/many";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Entities;
    }
}