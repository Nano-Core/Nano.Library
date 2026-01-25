using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Entities.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Create Request.
/// </summary>
public class CreateRequest : BaseRequestPost
{
    /// <summary>
    /// Entity.
    /// </summary>
    [Required]
    public virtual IEntityCreatable Entity { get; set; } = null!;

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