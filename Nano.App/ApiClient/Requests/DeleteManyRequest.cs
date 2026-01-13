using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DeleteManyRequest : DeleteManyRequest<Guid>;

/// <summary>
/// Delete Many Request.
/// </summary>
public class DeleteManyRequest<TIdentity> : BaseRequestDelete
{
    /// <summary>
    /// Ids.
    /// </summary>
    [Required]
    public virtual IEnumerable<TIdentity> Ids { get; set; } = [];

    /// <summary>
    /// Constructor.
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