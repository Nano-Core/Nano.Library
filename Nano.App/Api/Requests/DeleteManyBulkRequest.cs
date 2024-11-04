using System;

namespace Nano.App.Api.Requests;

/// <inheritdoc />
public class DeleteManyBulkRequest : DeleteManyBulkRequest<Guid>;

/// <summary>
/// Delete Many Bulk Request.
/// </summary>
public class DeleteManyBulkRequest<TIdentity> : DeleteManyRequest<TIdentity>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public DeleteManyBulkRequest()
    {
        this.Action = "delete/many/bulk";
    }
}