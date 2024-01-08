using Nano.Models.Interfaces;

namespace Nano.App.Api.Requests;

/// <summary>
/// Create Request.
/// </summary>
public class CreateRequest : BaseRequestPost
{
    /// <summary>
    /// Entity.
    /// </summary>
    public virtual IEntityCreatable Entity { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public CreateRequest()
    {
        this.Action = "create";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Entity;
    }
}