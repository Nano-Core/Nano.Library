using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Annotations;
using Nano.App.ApiClient.Annotations.Actions;
using Nano.App.Consts;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DetailsManyRequest : DetailsManyRequest<Guid>;

/// <summary>
/// Represents a request to get details of multiple entities.
/// </summary>
/// <typeparam name="TIdentity">The type of the entity identifiers.</typeparam>
[PostAction(ActionRoutes.DETAILS_MANY)]
public class DetailsManyRequest<TIdentity> : BaseRequest
{
    /// <summary>
    /// The IDs of the entities.
    /// </summary>
    [Required]
    [Body]
    public virtual ICollection<TIdentity> Ids { get; set; } = [];

    /// <summary>
    /// Optional depth for including related entities.
    /// </summary>
    [Query]
    public virtual int? IncludeDepth { get; set; }
}