using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Edit And Get Request.
/// </summary>
public class EditAndGetRequest : BaseRequestPut
{
    /// <summary>
    /// Entity.
    /// </summary>
    [Required]
    public virtual IEntityUpdatable Entity { get; set; } = null!;

    /// <summary>
    /// Constructor.
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