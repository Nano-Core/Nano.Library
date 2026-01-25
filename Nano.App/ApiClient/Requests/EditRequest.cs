using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Entities.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Edit Request.
/// </summary>
public class EditRequest : BaseRequestPut
{
    /// <summary>
    /// Entity.
    /// </summary>
    [Required]
    public virtual IEntityUpdatable Entity { get; set; } = null!;

    /// <summary>
    /// Constructor.
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