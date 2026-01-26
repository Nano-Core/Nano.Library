using System;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DeleteManyBulkRequest : DeleteManyBulkRequest<Guid>;

/// <summary>
/// Represents a bulk request to delete many entities by their IDs.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifiers.</typeparam>
public class DeleteManyBulkRequest<TIdentity> : DeleteManyRequest<TIdentity>
{
    /// <summary>
    /// Initializes a new instance of <see cref="DeleteManyBulkRequest{TIdentity}"/>.
    /// </summary>
    public DeleteManyBulkRequest()
    {
        this.Action = "delete/many/bulk";
    }
}