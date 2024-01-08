using Nano.Models.Interfaces;

namespace Nano.App.Api.Requests;

/// <summary>
/// Update Request.
/// </summary>
public class EditRequest : BaseRequestPut
{
    /// <summary>
    /// Entity.
    /// </summary>
    public virtual IEntityUpdatable Entity { get; set; }

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