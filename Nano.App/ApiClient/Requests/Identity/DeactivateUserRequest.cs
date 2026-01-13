using System;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class DeactivateUserRequest : DeactivateUserRequest<Guid>;

/// <summary>
/// Deactivate User Request.
/// </summary>
public class DeactivateUserRequest<TIdentity> : BaseRequestDelete
{
    /// <summary>
    /// Id.
    /// </summary>
    [Route(Order = 0)]
    public virtual TIdentity Id { get; set; } = default!;

    /// <summary>
    /// Constructor.
    /// </summary>
    public DeactivateUserRequest()
    {
        this.Action = "activate";
    }

    /// <inheritdoc />
    public override object? GetBody()
    {
        return null;
    }
}