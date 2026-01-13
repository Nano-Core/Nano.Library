using System;
using System.ComponentModel.DataAnnotations;
using Nano.App.ApiClient.Requests.Attributes;

namespace Nano.App.ApiClient.Requests.Identity;

/// <inheritdoc />
public class GetApiKeysRequest : GetApiKeysRequest<Guid>;

/// <inheritdoc />
public class GetApiKeysRequest<TIdentity> : BaseRequestGet
    where TIdentity : IEquatable<TIdentity>
{
    /// <summary>
    /// User Id.
    /// </summary>
    //[Required]
    [Required]
    [Route(Order = 0)]
    public virtual TIdentity UserId { get; set; } = default!;

    /// <inheritdoc />
    public GetApiKeysRequest()
    {
        this.Action = "api-keys";
    }
}