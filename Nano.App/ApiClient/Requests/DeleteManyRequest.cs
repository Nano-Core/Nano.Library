using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DeleteManyRequest : DeleteManyRequest<Guid>;

/// <summary>
/// Represents a request to delete multiple entities by their IDs.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifiers.</typeparam>
public class DeleteManyRequest<TIdentity> : BaseRequestDelete
{
    /// <summary>
    /// The IDs of the entities to delete.
    /// </summary>
    [Required]
    public virtual IEnumerable<TIdentity> Ids { get; set; } = [];

    /// <summary>
    /// Initializes a new instance of <see cref="DeleteManyRequest{TIdentity}"/>.
    /// </summary>
    public DeleteManyRequest()
    {
        this.Action = "delete/many";
    }

    /// <inheritdoc />
    public override object GetBody()
    {
        return this.Ids;
    }
}