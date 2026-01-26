using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Requests.Annotations;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetApiKeysRequest : GetApiKeysRequest<Guid>;

/// <summary>
/// Request to retrieve API keys for a user.
/// </summary>
/// <typeparam name="TIdentity">Type of the user identifier.</typeparam>
public class GetApiKeysRequest<TIdentity> : BaseRequestGet
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// The identifier of the user.
    /// </summary>
    [Required]
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of <see cref="GetApiKeysRequest{TIdentity}"/>.
    /// Sets the action to "api-keys".
    /// </summary>
    public GetApiKeysRequest()
    {
        this.Action = "api-keys";
    }
}