using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Create And Get Request.
/// </summary>
public class CreateAndGetRequest : BaseRequestPost
{
    /// <summary>
    /// Entity.
    /// </summary>
    [Required]
    public virtual IEntityCreatable Entity { get; set; } = null!;

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