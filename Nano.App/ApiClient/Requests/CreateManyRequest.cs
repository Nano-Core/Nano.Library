using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nano.Data.Abstractions.Models.Abstractions;

namespace Nano.App.ApiClient.Requests;

/// <summary>
/// Represents a request to create multiple entities at once.
/// </summary>
public class CreateManyRequest : BaseRequestPost
{
    /// <summary>
    /// The entities to create.
    /// </summary>
    [Required]
    public virtual IEnumerable<IEntityCreatable> Entities { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of <see cref="CreateManyRequest"/>.
    /// </summary>
    public CreateManyRequest()
    {
        this.Action = "create/many";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Entities;
    }
}