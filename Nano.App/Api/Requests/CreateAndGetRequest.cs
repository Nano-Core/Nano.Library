using Nano.Models.Interfaces;

namespace Nano.App.Api.Requests;

/// <summary>
/// Create And Get Request.
/// </summary>
public class CreateAndGetRequest : BaseRequestPost
{
    /// <summary>
    /// Entity.
    /// </summary>
    public virtual IEntityCreatable Entity { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public CreateAndGetRequest()
    {
        this.Action = "create/get";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Entity;
    }
}