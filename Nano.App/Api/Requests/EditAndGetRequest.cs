using Nano.Models.Interfaces;

namespace Nano.App.Api.Requests;

/// <summary>
/// Edit And Get Request.
/// </summary>
public class EditAndGetRequest : BaseRequestPut
{
    /// <summary>
    /// Entity.
    /// </summary>
    public virtual IEntityUpdatable Entity { get; set; }

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