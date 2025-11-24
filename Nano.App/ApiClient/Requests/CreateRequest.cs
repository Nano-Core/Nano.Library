using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

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