using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests;

/// <inheritdoc />
public class DeleteRequest : DeleteRequest<Guid>;

/// <summary>
/// Delete Request.
/// </summary>
public class DeleteRequest<TIdentity> : BaseRequestDelete
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// Id.
    /// </summary>
    [Route]
    [Required]
    public virtual TIdentity Id { get; set; } = default!;

    /// <summary>
    /// Constructor.
    /// </summary>
    public DeleteRequest()
    {
        this.Action = "delete";
    }

    /// <inheritdoc />
    public override object? GetBody()
    {
        return null;
    }
}