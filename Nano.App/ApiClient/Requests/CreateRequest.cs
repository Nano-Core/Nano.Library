using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to create a single entity.
/// </summary>
public class CreateRequest : BaseRequestPost
{
    /// <summary>
    /// The entity to create.
    /// </summary>
    [Required]
    public virtual IEntityCreatable Entity { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of <see cref="CreateRequest"/>.
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