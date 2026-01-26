using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to create an entity and retrieve it.
/// </summary>
public class CreateAndGetRequest : BaseRequestPost
{
    /// <summary>
    /// The entity to create.
    /// </summary>
    [Required]
    public virtual IEntityCreatable Entity { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of <see cref="CreateAndGetRequest"/>.
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